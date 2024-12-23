using System.Net;
using System.Web;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.Core.Persistence;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace DreamTeamOptimizer.MsEmployee.Tests.Integration.Controller;

public class WishListControllerTests : IClassFixture<WebAppFactory>
{
    private readonly IBus _bus;

    public WishListControllerTests(WebAppFactory factory)
    {
        var scope = factory.Services.CreateScope();
        _bus = scope.ServiceProvider.GetRequiredService<IBus>();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(0)]
    public async Task GetWishlist_ShouldReturnWishlist_WhenDesiredEmployeeIdsAreProvided(int countJuniors)
    {
        // Arrange
        var juniorIds = Enumerable.Range(10, countJuniors).ToList();
        var teamLeadIds = Enumerable.Range(20, countJuniors).ToList();

        var hackathonId = 1;
        var juniors = juniorIds.Select(id => new Employee(id, $"Junior {id}")).ToList();
        var teamLeads = teamLeadIds.Select(id => new Employee(id, $"Team Lead {id}")).ToList();

        // Act
        var votingStartedEvent = new VotingStartedEvent(hackathonId, teamLeads, juniors);
        await _bus.Publish(votingStartedEvent);

        _bus.ConnectPublishObserver(new PublishObserver(juniorIds, teamLeadIds));
    }
}

public class PublishObserver(List<int> juniorIds, List<int> teamLeadIds) : IPublishObserver
{
    public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
    {
        return Task.CompletedTask;
    }

    public Task PostPublish<T>(PublishContext<T> context) where T : class
    {
        if (context.Message is not WishListEvent wishListEvent)
        {
            return Task.CompletedTask;
        }

        var wishList = wishListEvent.WishList;

        wishList.Should().NotBeNull();

        if (wishList.EmployeeId >= 20)
        {
            teamLeadIds.Should().Contain(wishList.EmployeeId);
            wishList.DesiredEmployees.Should().BeEquivalentTo(juniorIds);
        }
        else
        {
            juniorIds.Should().Contain(wishList.EmployeeId);
            wishList.DesiredEmployees.Should().BeEquivalentTo(teamLeadIds);
        }

        return Task.CompletedTask;
    }

    public Task PrePublish<T>(PublishContext<T> context) where T : class
    {
        return Task.CompletedTask;
    }
}