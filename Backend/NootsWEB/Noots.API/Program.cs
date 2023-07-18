using Common.Azure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Noots.Mapper;
using Noots.MapperLocked;
using Noots.SignalrUpdater;
using Noots.SignalrUpdater.Impl;
using Noots.Storage;
using Serilog;
using System;
using System.IO;
using Noots.DatabaseContext;
using Common.Redis;
using Common.ConstraintsUploadFiles;
using Common.Filters;
using Noots.API.ConfigureAPP;
using Noots.API.Hosted;
using Noots.API.Middlewares;
using Common.App;
using Common.SignalR;
using Noots.Auth.Entities;
using Common.Google;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel((context, options) =>
{
    options.Limits.MaxRequestBodySize = FileSizeConstraints.MaxRequestFileSize; // TODO MAYBE MOVE
});


Console.WriteLine("builder.Environment.EnvironmentName: " + builder.Environment.EnvironmentName);

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", reloadOnChange: true, optional: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();


builder.Host.UseSerilog();

builder.Configuration.AddConfiguration(configBuilder.Build());

// Add services to the container.

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = FileSizeConstraints.MaxRequestFileSize;
});


var jwtTokenConfig = builder.Configuration.GetSection("JWTTokenConfig").Get<JwtTokenConfig>();
builder.Services.AddSingleton(jwtTokenConfig);

var googleConfig = builder.Configuration.GetSection("GoogleAuth").Get<GoogleAuth>();
builder.Services.AddSingleton(googleConfig);

var dbConn = builder.Configuration.GetSection("WriteDB").Value;
var azureConfig = builder.Configuration.GetSection("Azure").Get<AzureConfig>();
var redisConfig = builder.Configuration.GetSection("Redis").Get<RedisConfig>();

var controllersConfig = builder.Configuration.GetSection("Controllers").Get<ControllersActiveConfig>();
builder.Services.AddSingleton(x => controllersConfig);

Console.WriteLine("REDIS ACTIVE: " + redisConfig.Active);
Console.WriteLine("REDIS STR: "+ redisConfig.Connection);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

builder.Services.SetupLogger(builder.Configuration, environment);

builder.Services.ApplyAzureConfig(azureConfig);
builder.Services.TimersConfig(builder.Configuration);
builder.Services.JWT(jwtTokenConfig);


builder.Services.ApplyMapperDI();
builder.Services.ApplyMapperLockedDI();

builder.Services.AddControllers(opt => opt.Filters.Add(new ValidationFilter()))
                .AddNewtonsoftJson();
builder.Services.AddScoped<DisableInProductionFilter>();

builder.Services.AddHealthChecks();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // check

builder.Services.SetupSignalR(redisConfig);

builder.Services.AddSingleton<IUserIdProvider, IdProvider>();

builder.Services.Mediatr();

builder.Services.ApplyDataBaseDI(dbConn);
builder.Services.DapperDI(dbConn);

builder.Services.BI();
builder.Services.ApplySignalRDI();

builder.Services.AddMemoryCache();


builder.Services.AddHostedService<SetupServicesHosted>();
builder.Services.AddHostedService<HistoryProcessingHosted>();

builder.Services.AddHttpClient();

builder.Services.AddSwaggerGen();

// APP

var app = builder.Build();

// Configure the HTTP request pipeline.
Console.WriteLine("is DockerDev: " + app.Environment.IsEnvironment("DockerDev"));

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("DockerDev"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var isHostASP = false;
if (isHostASP && !string.IsNullOrEmpty(builder.Environment.WebRootPath))
{
    var path = Path.Combine(builder.Environment.WebRootPath);
    app.UseStaticFiles(new StaticFileOptions
    {
        ServeUnknownFileTypes = true,
        FileProvider = new PhysicalFileProvider(path)
    });
}

app.UseRouting();

if (isHostASP && !string.IsNullOrEmpty(builder.Environment.WebRootPath))
{
    var path = Path.Combine(builder.Environment.WebRootPath);
    app.MapFallbackToFile("index.html", new StaticFileOptions()
    {
        ServeUnknownFileTypes = true,
        FileProvider = new PhysicalFileProvider(path),
    });
}


app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<AppSignalRHub>(HubSettings.endPoint);

app.MapHealthChecks("/health");

await app.RunAsync();