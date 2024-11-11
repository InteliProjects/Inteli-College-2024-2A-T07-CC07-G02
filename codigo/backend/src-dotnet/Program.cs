using DotNetEnv;
using Backend.Data;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.OpenApi.Models;

Env.Load("../.env");
var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8000);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var connectionString = $"Server={Env.GetString("DB_HOST")};Database={Env.GetString("DB_NAME")};User={Env.GetString("DB_USERNAME")};Password={Env.GetString("DB_PASSWORD")};Port={Env.GetString("DB_PORT")};SslMode=Preferred;";


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mysqlOptions =>
    {
        mysqlOptions.MaxBatchSize(100);
        mysqlOptions.CommandTimeout(30);
        mysqlOptions.EnableRetryOnFailure(5);
        mysqlOptions.MinBatchSize(5);
    }));

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();
app.UseCors("AllowAllOrigins");

app.Run();
