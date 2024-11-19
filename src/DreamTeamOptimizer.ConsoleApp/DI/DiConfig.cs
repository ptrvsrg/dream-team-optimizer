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

namespace DreamTeamOptimizer.ConsoleApp.DI;

public static class DiConfig
{
    public static void ConfigureStrategy(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IStrategy>(_ =>
            {
                if (StrategyType.TryParse(config["HACKATHON_STRATEGY_TYPE"], out StrategyType strategyType))
                {
                    return StrategyFactory.NewStrategy(strategyType);
                }

                return new GaleShapleyStrategy();
            }
        );
    }

    public static void ConfigureRepositories(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(
            opt =>
            {
                var host = config["HACKATHON_DB_HOST"]!;
                var port = int.Parse(config["HACKATHON_DB_PORT"]!);
                var db = config["HACKATHON_DB_NAME"]!;
                var user = config["HACKATHON_DB_USER"]!;
                var password = config["HACKATHON_DB_PASSWORD"]!;
                var connectionString = $"Host={host};Port={port};Database={db};Username={user};Password={password}";
                opt.UseNpgsql(connectionString);
            }
        );

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IPreferenceRepository, PreferenceRepository>();
        services.AddScoped<ITeamRepositroy, TeamRepository>();
        services.AddScoped<ISatisfactionRepository, SatisfactionRepository>();
        services.AddScoped<IHackathonRepository, HackathonRepository>();
    }

    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IWishListService, WishListService>();
        services.AddScoped<IHrManagerService, HrManagerService>();
        services.AddScoped<IHrDirectorService, HrDirectorService>();
        services.AddScoped<IHackathonService, HackathonService>();
    }
}