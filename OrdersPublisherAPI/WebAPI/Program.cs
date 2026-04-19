using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Repositories;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
var connectionString = builder.Configuration.GetConnectionString("PostgresDb");
builder.Services.AddDbContext<AppDBContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddSingleton<OrdersRepository>();
builder.Services.AddSingleton<WorkerControlService>();
builder.Services.AddHostedService<DailyService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
