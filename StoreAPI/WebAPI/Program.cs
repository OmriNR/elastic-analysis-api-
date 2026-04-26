using System.Text;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Repositories;
using Repositories.Interfaces;
using Repositories.Repositories;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using Services;
using Services.Interfaces;
using Services.Rabbit;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
    {
        AutoRegisterTemplate = true,
        IndexFormat = "store-api-logs-{0:yyyy.MM.dd}",
        NumberOfReplicas = 1,
        NumberOfShards = 2
    }).CreateLogger();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });
});

var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.UTF8.GetBytes(jwtSection["Secret"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var connectionString = builder.Configuration.GetConnectionString("PostgresDb");
builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(connectionString));

var minioConfig = builder.Configuration.GetSection("Minio");

builder.Services.AddSingleton<IAmazonS3>(sp =>
{
    var config = new AmazonS3Config
    {
        ServiceURL = minioConfig["ServiceUrl"],
        ForcePathStyle = true
    };
    return new AmazonS3Client(minioConfig["AccessKey"], minioConfig["SecretKey"], config);
});

builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
builder.Services.AddScoped<IProductsService, ProductsService>();

builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersService, UsersService>();

builder.Services.AddScoped<IDiscountsService, DiscountService>();
builder.Services.AddScoped<IDiscountsRepository, DiscountsRepository>();

builder.Services.AddScoped<IOrdersService, OrdersService>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();

builder.Services.AddScoped<IImagesRepository, ImagesRepository>();
builder.Services.AddScoped<IImagesService, ImagesService>();

builder.Services.AddSingleton<IMessageProducer, MessageProducer>();

builder.Services.AddScoped<TokenService>();

var app = builder.Build();

//app.Services.GetRequiredService<IMessageProducer>();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    db.Database.EnsureCreated();

    if (!db.Users.Any(u => u.IsAdmin))
    {
        db.Users.Add(new Domain.User
        {
            UserId = Guid.NewGuid().ToString(),
            Email = "admin@store.com",
            Password = "Admin123!",
            IsActive = true,
            IsAdmin = true,
            CreatedAt = DateTime.UtcNow,
            Properties = new Domain.UserProperties
            {
                UserName = "Admin",
                Age = 0,
                Gender = "other",
                Location = new Domain.GeoProperties
                {
                    City = "",
                    Country = "",
                    Address = "",
                    ZipCode = ""
                }
            }
        });
        db.SaveChanges();
    }

    var adminUser = db.Users.FirstOrDefault(u => u.IsAdmin);

    var seedProducts = new[]
    {
        new Domain.Product
        {
            ProductId = "11111111-0000-0000-0000-000000000001",
            OwnerId = adminUser!.UserId,
            Name = "Wireless Noise-Cancelling Headphones",
            Description = "Premium over-ear headphones with active noise cancellation, 30-hour battery life, and crystal-clear audio for an immersive listening experience.",
            Category = "Electronics",
            SubCategory = "Audio",
            Price = 149.99,
            Quantity = 50
        },
        new Domain.Product
        {
            ProductId = "11111111-0000-0000-0000-000000000002",
            OwnerId = adminUser!.UserId,
            Name = "Classic Leather Jacket",
            Description = "Timeless genuine leather jacket with a slim fit design, zip pockets, and a durable inner lining. Available in multiple sizes.",
            Category = "Clothing",
            SubCategory = "Outerwear",
            Price = 199.99,
            Quantity = 30
        },
        new Domain.Product
        {
            ProductId = "11111111-0000-0000-0000-000000000003",
            OwnerId = adminUser!.UserId,
            Name = "Ergonomic Office Chair",
            Description = "Adjustable lumbar support, breathable mesh back, and 360-degree swivel. Designed for all-day comfort during long work sessions.",
            Category = "Home",
            SubCategory = "Furniture",
            Price = 349.99,
            Quantity = 15
        }
    };

    if (adminUser != null && !db.Products.Any())
    {
        db.Products.AddRange(seedProducts);
        db.SaveChanges();
    }

    // Seed product images into MinIO
    var s3Client = scope.ServiceProvider.GetRequiredService<IAmazonS3>();
    var imagesRepo = scope.ServiceProvider.GetRequiredService<IImagesRepository>();

    try { await s3Client.PutBucketAsync("store-images"); }
    catch { /* bucket already exists */ }

    var seedImages = new Dictionary<string, string>
    {
        { "11111111-0000-0000-0000-000000000001", "https://picsum.photos/seed/electronics-headphones/600/400.jpg" },
        { "11111111-0000-0000-0000-000000000002", "https://picsum.photos/seed/fashion-leather-jacket/600/400.jpg" },
        { "11111111-0000-0000-0000-000000000003", "https://picsum.photos/seed/office-ergonomic-chair/600/400.jpg" },
    };

    using var httpClient = new HttpClient();
    httpClient.Timeout = TimeSpan.FromSeconds(10);

    foreach (var (productId, imageUrl) in seedImages)
    {
        try
        {
            var existing = await imagesRepo.GetImage(productId);
            if (existing != null) { existing.Dispose(); continue; }

            var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
            await imagesRepo.UploadImage(productId, new MemoryStream(imageBytes), "image/jpeg");
        }
        catch (Exception ex)
        {
            Log.Logger.Warning("Could not seed image for product {Id}: {Error}", productId, ex.Message);
        }
    }
}

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
