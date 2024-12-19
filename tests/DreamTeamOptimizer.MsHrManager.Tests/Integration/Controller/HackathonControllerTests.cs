using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.Core.Persistence;
using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Hackathon = DreamTeamOptimizer.Core.Persistence.Entities.Hackathon;
using HackathonStatus = DreamTeamOptimizer.Core.Persistence.Entities.HackathonStatus;

namespace DreamTeamOptimizer.MsHrManager.Tests.Integration.Controller;

public class HackathonControllerTests : IClassFixture<WebAppFactory>, IAsyncLifetime
{
    private readonly AppDbContext _dbContext;
    private readonly IHackathonRepository _hackathonRepository;
    private readonly IBus _bus;

    public HackathonControllerTests(WebAppFactory factory)
    {
        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _hackathonRepository = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
        _bus = scope.ServiceProvider.GetRequiredService<IBus>();
    }

    public Task InitializeAsync()
    {
        var hackathon = new Hackathon
        {
            Id = 1,
            Status = HackathonStatus.IN_PROCESSING,
            Result = 0
        };
        _hackathonRepository.Create(hackathon);
    
        _dbContext.Entry(hackathon).State = EntityState.Detached;

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _dbContext.Set<Hackathon>().RemoveRange(_dbContext.Set<Hackathon>());
        _dbContext.SaveChanges();
        _dbContext.ChangeTracker.Clear();
        
        return Task.CompletedTask;
    }

    [Fact]
    public async Task ConudctVoting_ShouldPublishHackathonResult_WhenCalled()
    {
        // Arrange
        var hackathonId = 1;
        _bus.ConnectPublishObserver(new VotingStartedPublisherObserver(hackathonId, _bus));
        _bus.ConnectPublishObserver(new HackathonResultPublisherObserver(hackathonId));
        
        // Act
        await _bus.Publish(new HackathonStartedEvent(hackathonId));
    }
}

class VotingStartedPublisherObserver(int hackathonId, IBus bus) : IPublishObserver
{
    public Task PrePublish<T>(PublishContext<T> context) where T : class
    {
        return Task.CompletedTask;
    }

    public async Task PostPublish<T>(PublishContext<T> context) where T : class
    {
        if (context.Message is VotingStartedEvent votingStartedEvent)
        {
            var juniorIds = votingStartedEvent.Juniors.Select(j => j.Id).ToList();
            var teamLeadIds = votingStartedEvent.TeamLeads.Select(j => j.Id).ToList();
            
            foreach (var j in juniorIds)
            {
                var wishListEvent = new WishListEvent(hackathonId, new (j, teamLeadIds.ToArray()));
                await bus.Publish(wishListEvent);

            }
            foreach (var t in teamLeadIds)
            {
                var wishListEvent = new WishListEvent(hackathonId, new (t, juniorIds.ToArray()));
                await bus.Publish(wishListEvent);
            }
        }
    }

    public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
    {
        return Task.CompletedTask;
    }
}

class HackathonResultPublisherObserver(int hackathonId) : IPublishObserver
{
    public Task PrePublish<T>(PublishContext<T> context) where T : class
    {
        return Task.CompletedTask;
    }

    public Task PostPublish<T>(PublishContext<T> context) where T : class
    {
        if (context.Message is HackathonResultEvent hackathonResultEvent)
        {
            hackathonResultEvent.Id.Should().Be(hackathonId);
        }
        
        return Task.CompletedTask;
    }

    public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
    {
        return Task.CompletedTask;
    }
}
