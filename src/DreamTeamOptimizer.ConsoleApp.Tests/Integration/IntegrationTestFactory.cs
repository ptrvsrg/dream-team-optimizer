using DreamTeamOptimizer.ConsoleApp.Persistence;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Integration;

public class IntegrationTestFactory : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine3.20")
        .WithName("hackathon_test_postgres")
        .WithPortBinding(5432, 35432)
        .WithDatabase("test_db")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .WithCleanUp(true)
        .WithReuse(true)
        .Build();

    public AppDbContext DbContext;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        
        var connectionString = _postgresContainer.GetConnectionString();
        var dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseLazyLoadingProxies()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseNpgsql(connectionString)
            .Options;
        DbContext = new AppDbContext(dbContextOptions);

        await DbContext.Database.MigrateAsync();
        await DbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await _postgresContainer.DisposeAsync();
    }
}