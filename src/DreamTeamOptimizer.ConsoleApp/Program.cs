using DreamTeamOptimizer.ConsoleApp.DI;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Core.Interfaces.IServices;
using DreamTeamOptimizer.Core.Services;
using DreamTeamOptimizer.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

HostApplicationBuilderSettings settings = new()
{
    Args = args,
    Configuration = new ConfigurationManager(),
    ContentRootPath = Directory.GetCurrentDirectory(),
};
settings.Configuration.AddEnvironmentVariables(prefix: "HACKATHON_");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(settings);
builder.Logging.ClearProviders().AddSerilog();
builder.Services.AddSingleton<IEmployeeService, EmployeeService>(_ =>
    new EmployeeService(
        builder.Configuration["HACKATHON_JUNIORS_FILE_PATH"]!,
        builder.Configuration["HACKATHON_TEAM_LEADS_FILE_PATH"]!
    ));
builder.Services.AddSingleton<IWishListService, WishListService>();
builder.Services.AddSingleton<IStrategy>(_ =>
    {
        if (StrategyType.TryParse(builder.Configuration["HACKATHON_STRATEGY_TYPE"], out StrategyType strategyType))
        {
            return StrategyFactory.NewStrategy(strategyType);
        }
        return new GaleShapleyStrategy();
    }
);
builder.Services.AddSingleton<IHrManagerService, HrManagerService>();
builder.Services.AddSingleton<IHrDirectorService, HrDirectorService>();
builder.Services.AddSingleton<IHackathonService, HackathonService>();
builder.Services.AddHostedService<HackathonWorker>();

using var host = builder.Build();
host.Run();