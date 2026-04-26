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
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
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
