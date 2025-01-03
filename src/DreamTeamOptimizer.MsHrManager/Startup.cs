using System.Reflection;
using System.Text.Json.Serialization;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.Core.Persistence.Repositories;
using DreamTeamOptimizer.MsHrManager.Brokers.Consumers;
using DreamTeamOptimizer.MsHrManager.Brokers.Publishers;
using DreamTeamOptimizer.MsHrManager.Config;
using DreamTeamOptimizer.MsHrManager.ExceptionHandlers;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using DreamTeamOptimizer.MsHrManager.Services;
using DreamTeamOptimizer.Strategies;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Serilog;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;

namespace DreamTeamOptimizer.MsHrManager;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureConfiguration(services);
        ConfigureStrategy(services);
        ConfigureCaching(services);
        ConfigureBrokerLayer(services);
        ConfigureRepositoryLayer(services);
        ConfigureServiceLayer(services);
        ConfigureControllerLayer(services);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();
        app.UseCors();
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapHealthChecks("/health");
            endpoints.MapHealthChecks("/health/readiness", new HealthCheckOptions
            {
                Predicate = _ => true
            });
            endpoints.MapHealthChecks("/health/liveness", new HealthCheckOptions
            {
                Predicate = _ => true
            });
        });
    }

    private void ConfigureConfiguration(IServiceCollection services)
    {
        var section = configuration.GetSection(AppConfig.Name);
        services.AddOptions<AppConfig>()
            .Bind(section)
            .ValidateDataAnnotations();
    }

    private void ConfigureStrategy(IServiceCollection services)
    {
        services.AddSingleton<IStrategy>(provider =>
            {
                var config = provider.GetService<IOptions<AppConfig>>()!;
                return StrategyFactory.NewStrategy(config.Value.Strategy);
            }
        );
    }
    
    private void ConfigureCaching(IServiceCollection services)
    {
        services.AddMemoryCache();
    }
    
    private void ConfigureBrokerLayer(IServiceCollection services)
    {
        // RabbitMQ
        services.AddMassTransit(registerConfig =>
        {
            registerConfig.AddConsumer<WishListConsumer>();
            registerConfig.AddConsumer<HackathonStartedConsumer>();
            registerConfig.UsingRabbitMq((context, factoryConfig) =>
            {
                var connectionString = configuration.GetConnectionString("RabbitMQ")!;
                factoryConfig.Host(connectionString);
                factoryConfig.ConfigureEndpoints(context);
            });
        });
        services.AddScoped<IVotingStartedPublisher, VotingStartedPublisher>();
        services.AddScoped<IHackathonResultPublisher, HackathonResultPublisher>();
        
        // In memory
        services.AddMassTransit<IMemoryBus>(registerConfig =>
        {
            registerConfig.AddConsumer<VotingCompletedConsumer>();
            registerConfig.UsingInMemory((context, factoryConfig) =>
            {
                factoryConfig.ConfigureEndpoints(context);
            });
        });
        services.AddScoped<IVotingCompletedPublisher, VotingCompletedPublisher>();
    }

    private void ConfigureRepositoryLayer(IServiceCollection services)
    {
        var connectionString = configuration.GetConnectionString("Postgres")!;
        services.AddDbContext<AppDbContext>(dbBuilder =>
        {
            dbBuilder
                .EnableSensitiveDataLogging()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseLazyLoadingProxies()
                .UseNpgsql(connectionString);
        });

        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IWishListRepository, WishListRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IHackathonRepository, HackathonRepository>();
        services.AddScoped<IHackathonEmployeeRepository, HackathonEmployeeRepository>();
    }

    private void ConfigureServiceLayer(IServiceCollection services)
    {
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IWishListService, WishListService>();
        services.AddScoped<IStrategyService, StrategyService>();
        services.AddScoped<IHackathonService, HackathonService>();
    }

    private void ConfigureControllerLayer(IServiceCollection services)
    {
        services.AddServiceDiscovery(builder => builder.UseConsul());
        services.AddDiscoveryClient(configuration);
        
        services.AddProblemDetails();
        services.AddExceptionHandler<HttpStatusExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddControllers()
            .AddJsonOptions(x => { x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
        services.AddHealthChecks();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "HR Manager API",
                Description = "API documentation for HR Manager microservice",
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }
}