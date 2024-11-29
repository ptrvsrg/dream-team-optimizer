using DreamTeamOptimizer.ConsoleApp.Config;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence;
using DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;
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

namespace DreamTeamOptimizer.ConsoleApp.DI;

public static class DiConfig
{

    public static void ConfigureConfiguration(this HostApplicationBuilder builder)
    {
        var configPath = Environment.GetEnvironmentVariable("HACKATHON_CONFIG_PATH") ?? "appsettings.json";
        builder.Configuration
            .AddJsonFile(configPath, optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var section = builder.Configuration.GetSection(ApplicationOptions.Name);
        builder.Services.AddOptions<ApplicationOptions>()
            .Bind(section)
            .ValidateDataAnnotations();
    }
    public static void ConfigureLogger(this HostApplicationBuilder builder)
    {
        var config = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Configuration.GetSection("Logging"));
        Log.Logger =  config.CreateLogger();
        builder.Logging.ClearProviders().AddSerilog();
    }

    public static void ConfigureStrategy(this HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IStrategy>(provider =>
            {
                var config = provider.GetService<IOptions<ApplicationOptions>>()!;
                return StrategyFactory.NewStrategy(config.Value.Strategy);
            }
        );
    }

    public static void ConfigureRepositories(this HostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("Postgres")!;
        builder.Services.AddDbContext<AppDbContext>(dbBuilder =>
        {
            dbBuilder
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseLazyLoadingProxies()
                .UseNpgsql(connectionString);
        });
        builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        builder.Services.AddScoped<IWishListRepository, WishListRepository>();
        builder.Services.AddScoped<ITeamRepository, TeamRepository>();
        builder.Services.AddScoped<ISatisfactionRepository, SatisfactionRepository>();
        builder.Services.AddScoped<IHackathonRepository, HackathonRepository>();
    }

    public static void ConfigureServices(this HostApplicationBuilder builder)
    {
        builder.Services.AddScoped<IEmployeeService, EmployeeService>();
        builder.Services.AddScoped<IWishListService, WishListService>();
        builder.Services.AddScoped<IHrManagerService, HrManagerService>();
        builder.Services.AddScoped<IHrDirectorService, HrDirectorService>();
        builder.Services.AddScoped<IHackathonService, HackathonService>();
    }

    public static IHost MigrateDatabases(this IHost host)
    {
        using (var scope = host.Services.CreateScope())
        {
            using (var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                ctx.Database.Migrate();
            }
        }

        return host;
    }
}