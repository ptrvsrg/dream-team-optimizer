using DreamTeamOptimizer.ConsoleApp.Config;
using DreamTeamOptimizer.ConsoleApp.DI;
using DreamTeamOptimizer.Core.Interfaces.IServices;
using DreamTeamOptimizer.Core.Services;
using DreamTeamOptimizer.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nsu.HackathonProblem.Contracts;
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

// Setup configuration
var configPath = Environment.GetEnvironmentVariable("HACKATHON_CONFIG_PATH") ?? "appsettings.json";
builder.Configuration
    .AddJsonFile(configPath, optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
builder.Services.AddOptions<Config>()
    .Bind(builder.Configuration.GetSection(nameof(Config)))
    .ValidateDataAnnotations();

builder.Services.AddSingleton<ITeamBuildingStrategy>(provider =>
    {
        var config = provider.GetService<IOptions<Config>>()!;
        Log.Information($"Used strategy: {config.Value.Strategy.ToString()}");
        return StrategyFactory.NewStrategy(config.Value.Strategy);
    }
);
builder.Services.AddSingleton<IEmployeeService, EmployeeService>(provider =>
    {
        var config = provider.GetService<IOptions<Config>>()!;
        return new EmployeeService(config.Value.JuniorsFilePath, config.Value.TeamLeadsFilePath);
    }
);
builder.Services.AddSingleton<IWishListService, WishListService>();
builder.Services.AddSingleton<IHrManagerService, HrManagerService>();
builder.Services.AddSingleton<IHrDirectorService, HrDirectorService>();
builder.Services.AddSingleton<IHackathonService, HackathonService>();
builder.Services.AddHostedService<HackathonWorker>();

using var host = builder.Build();
host.Run();