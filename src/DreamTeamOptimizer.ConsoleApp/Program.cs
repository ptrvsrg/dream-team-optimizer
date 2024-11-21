using DreamTeamOptimizer.ConsoleApp.Config;
using DreamTeamOptimizer.ConsoleApp.DI;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Persistence;
using DreamTeamOptimizer.ConsoleApp.Services;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Strategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;

var builder = Host.CreateApplicationBuilder();

// Setup logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Logging.ClearProviders().AddSerilog();
builder.Logging.ClearProviders().AddSerilog();

// Setup configuration
var configPath = Environment.GetEnvironmentVariable("HACKATHON_CONFIG_PATH") ?? "appsettings.json";
builder.Configuration
    .AddJsonFile(configPath, optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddOptions<Config>()
    .Bind(builder.Configuration.GetSection(nameof(Config)))
    .ValidateDataAnnotations();

builder.Services.ConfigureRepositories(builder.Configuration);
builder.Services.ConfigureServices();
builder.Services.ConfigureStrategy(builder.Configuration);
builder.Services.AddHostedService<HackathonWorker>();

using var host = builder.Build();
using (var scope = host.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();
}

host.Run();