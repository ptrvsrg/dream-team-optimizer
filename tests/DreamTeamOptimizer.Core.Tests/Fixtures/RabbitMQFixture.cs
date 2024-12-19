using Testcontainers.RabbitMq;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Fixtures;

public class RabbitMQFixture : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder()
        .WithImage("rabbitmq:4.0.4-alpine")
        .WithPortBinding(5672, true)
        .WithUsername("test_user")
        .WithPassword("test_password")
        .WithCleanUp(true)
        .Build();

    public Task InitializeAsync()
    {
        return _rabbitMqContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _rabbitMqContainer.DisposeAsync().AsTask();
    }

    public string ConnectionString => _rabbitMqContainer.GetConnectionString();
}