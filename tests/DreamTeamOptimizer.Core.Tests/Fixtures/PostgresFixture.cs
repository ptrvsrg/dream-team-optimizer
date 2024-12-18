using Testcontainers.PostgreSql;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:17-alpine3.20")
        .WithPortBinding(5432, true)
        .WithDatabase("test_db")
        .WithUsername("test_user")
        .WithPassword("test_password")
        .WithCleanUp(true)
        .Build();

    public Task InitializeAsync()
    {
        return _postgresContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgresContainer.DisposeAsync().AsTask();
    }

    public string ConnectionString => _postgresContainer.GetConnectionString();
}