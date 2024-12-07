using Testcontainers.Consul;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Fixtures;

public class ConsulFixture : IAsyncLifetime
{
    private readonly ConsulContainer _consulContainer = new ConsulBuilder()
        .WithImage("consul:1.15.4")
        .WithPortBinding(8500, true)
        .WithCleanUp(true)
        .Build();

    public Task InitializeAsync()
    {
        return _consulContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _consulContainer.DisposeAsync().AsTask();
    }

    public string Host => _consulContainer.Hostname;
    public int Port => _consulContainer.GetMappedPublicPort(8500);
}