using System.Reflection;
using System.Text.Json.Serialization;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.Core.Persistence.Repositories;
using DreamTeamOptimizer.MsEmployee.Config;
using DreamTeamOptimizer.MsEmployee.ExceptionHandlers;
using DreamTeamOptimizer.MsEmployee.Interfaces.Services;
using DreamTeamOptimizer.MsEmployee.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;

namespace DreamTeamOptimizer.MsEmployee;

public class Startup(IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureConfiguration(services);
        ConfigureRepositoryLayer(services);
        ConfigureServiceLayer(services);
        ConfigureControllerLayer(services);
    }

    public void Configure(IApplicationBuilder app, IEmployeeRepository employeeRepository)
    {
        CheckEmployee(employeeRepository);
        
        app.UseExceptionHandler();
        app.UseSerilogRequestLogging();
        app.UseCors();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseRouting();
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
    }

    private void ConfigureServiceLayer(IServiceCollection services)
    {
        services.AddServiceDiscovery(builder => builder.UseConsul());
        services.AddDiscoveryClient(configuration);

        services.AddScoped<IWishListService, WishListService>();
    }

    private void ConfigureControllerLayer(IServiceCollection services)
    {
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
                Title = "Employee API",
                Description = "API documentation for Employee microservice",
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    private void CheckEmployee(IEmployeeRepository employeeRepository)
    {
        var employeeId = configuration.GetValue<int>("Application:EmployeeID");
        
        var employee = employeeRepository.FindById(employeeId);
        if (employee == null)
        {
            throw new KeyNotFoundException($"Employee #{employeeId} not found");
        }
    }
}