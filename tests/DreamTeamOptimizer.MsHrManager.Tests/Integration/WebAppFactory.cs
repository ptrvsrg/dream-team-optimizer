using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.Core.Tests.Fixtures;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DreamTeamOptimizer.MsHrManager.Tests.Integration;

public class WebAppFactory : WebApplicationFactory<Startup>
{
    public WebAppFactory()
    {
        var postgresFixture = new PostgresFixture();
        var consulFixture = new ConsulFixture();
        var rabbitMqFixture = new RabbitMQFixture();

        Task.WhenAll(
            postgresFixture.InitializeAsync(),
            consulFixture.InitializeAsync(),
            rabbitMqFixture.InitializeAsync()
        ).Wait();

        Environment.SetEnvironmentVariable("Logging__Serilog__MinimumLevel__Default", "Debug");
        Environment.SetEnvironmentVariable("Logging__Serilog__MinimumLevel__Override__Microsoft", "Debug");
        Environment.SetEnvironmentVariable("Logging__Serilog__MinimumLevel__Override__System", "Debug");
        Environment.SetEnvironmentVariable("ConnectionStrings__Postgres", postgresFixture.ConnectionString);
        Environment.SetEnvironmentVariable("ConnectionStrings__RabbitMQ", rabbitMqFixture.ConnectionString);
        Environment.SetEnvironmentVariable("Consul__Host", consulFixture.Host);
        Environment.SetEnvironmentVariable("Consul__Port", consulFixture.Port.ToString());
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = builder.Build();
        using (var scope = host.Services.CreateScope())
        {
            using (var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>())
            {
                ctx.Database.Migrate();
            }
        }

        host.Start();
        return host;
    }
}