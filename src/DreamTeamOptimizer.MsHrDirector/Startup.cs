using System.Reflection;
using System.Text.Json.Serialization;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.Core.Persistence.Repositories;
using DreamTeamOptimizer.MsHrDirector.Brokers.Consumers;
using DreamTeamOptimizer.MsHrDirector.Brokers.Publishers;
using DreamTeamOptimizer.MsHrDirector.ExceptionHandlers;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;
using DreamTeamOptimizer.MsHrDirector.Services;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;

namespace DreamTeamOptimizer.MsHrDirector;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
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

    private void ConfigureCaching(IServiceCollection services)
    {
        services.AddMemoryCache();
    }

    private void ConfigureBrokerLayer(IServiceCollection services)
    {
        services.AddMassTransit(registerConfig =>
        {
            registerConfig.AddConsumer<HackathonResultConsumer>();
            registerConfig.UsingRabbitMq((context, factoryConfig) =>
            {
                var connectionString = configuration.GetConnectionString("RabbitMQ");
                factoryConfig.Host(connectionString);
                factoryConfig.ConfigureEndpoints(context);
            });
        });
        services.AddScoped<IHackathonStartedPublisher, HackathonStartedPublisher>();
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
        services.AddScoped<ISatisfactionRepository, SatisfactionRepository>();
        services.AddScoped<IHackathonRepository, HackathonRepository>();
    }

    private void ConfigureServiceLayer(IServiceCollection services)
    {
        services.AddScoped<ISatisfactionService, SatisfactionService>();
        services.AddScoped<IHackathonService, HackathonService>();
    }

    private void ConfigureControllerLayer(IServiceCollection services)
    {
        services.AddServiceDiscovery(builder => builder.UseConsul());   
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
                Title = "HR Director API",
                Description = "API documentation for HR Director microservice"
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }
}