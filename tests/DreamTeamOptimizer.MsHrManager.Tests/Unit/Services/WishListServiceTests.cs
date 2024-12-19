using Moq;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsHrManager.Models.Sessions;
using DreamTeamOptimizer.MsHrManager.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DreamTeamOptimizer.MsHrManager.Tests.Unit.Services;

public class WishListServiceTests(WishListServiceFixture fixture) : IClassFixture<WishListServiceFixture>, IDisposable
{
    public void Dispose()
    {
        fixture.ResetMocks();
    }

    [Theory]
    [MemberData(nameof(StartVotingData))]
    public void StartVoting_ShouldSaveWishLists_WhenCalled(
        List<Employee> juniors,
        List<Employee> teamLeads,
        int hackathonId)
    {
        // Arrange
        fixture.VotingStartedPublisherMock.Setup(vsp => vsp.StartVoting(It.IsAny<VotingStartedEvent>())).Verifiable();

        // Act
        fixture.WishListService.StartVoting(teamLeads, juniors, hackathonId);

        // Assert
        fixture.VotingStartedPublisherMock.Verify(vsp => vsp.StartVoting(It.IsAny<VotingStartedEvent>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(SaveWishListToSessionData))]
    public void SaveWishListToSession_ShouldSaveWishList_WhenCalled(
        List<Employee> juniors,
        List<Employee> teamLeads,
        List<WishList> juniorWishLists,
        List<WishList> teamLeadWishLists,
        WishList wishList,
        int hackathonId)
    {
        // Arrange
        var session = new VotingSession(
            teamLeadWishLists.Count + juniorWishLists.Count,
            teamLeads.Count + juniors.Count,
            teamLeads,
            juniors,
            teamLeadWishLists,
            juniorWishLists);
        fixture.Cache.Set(hackathonId, session);
        fixture.VotingCompletedPublisherMock.Setup(vsp =>
                vsp.CompleteVoting(teamLeads, juniors, It.IsAny<List<WishList>>(), It.IsAny<List<WishList>>(),
                    hackathonId))
            .Verifiable();

        // Act
        fixture.WishListService.SaveWishListToSession(wishList, hackathonId);

        // Assert
        fixture.VotingCompletedPublisherMock.Verify(vsp =>
                vsp.CompleteVoting(teamLeads, juniors, It.IsAny<List<WishList>>(), It.IsAny<List<WishList>>(),
                    hackathonId),
            Times.Once);
    }

    public static IEnumerable<object[]> StartVotingData => new List<object[]>
    {
        new object[]
        {
            new List<Employee>
            {
                new(1, "Employee 1"),
                new(2, "Employee 2")
            },
            new List<Employee>
            {
                new(3, "Employee 3"),
                new(4, "Employee 4")
            },
            1
        },
        new object[] { new List<Employee>(), new List<Employee>(), 1 }
    };

    public static IEnumerable<object[]> SaveWishListToSessionData => new List<object[]>
    {
        new object[]
        {
            new List<Employee>
            {
                new(1, "Employee 1"),
                new(2, "Employee 2")
            },
            new List<Employee>
            {
                new(3, "Employee 3"),
                new(4, "Employee 4")
            },
            new List<WishList>
            {
                new(1, [4, 3]),
                new(2, [3, 4])
            },
            new List<WishList>
            {
                new(3, [1, 2]),
            },
            new WishList(4, [2, 1]),
            1
        },
    };
}

public class WishListServiceFixture : IDisposable
{
    public IMemoryCache Cache { get; }
    public Mock<IVotingStartedPublisher> VotingStartedPublisherMock { get; }
    public Mock<IVotingCompletedPublisher> VotingCompletedPublisherMock { get; }
    public WishListService WishListService { get; }

    public WishListServiceFixture()
    {
        Cache = new MemoryCache(new MemoryCacheOptions());
        VotingStartedPublisherMock = new Mock<IVotingStartedPublisher>();
        VotingCompletedPublisherMock = new Mock<IVotingCompletedPublisher>();

        WishListService = new WishListService(
            new Mock<ILogger<WishListService>>().Object,
            Cache,
            VotingStartedPublisherMock.Object,
            VotingCompletedPublisherMock.Object
        );
    }

    public void ResetMocks()
    {
        VotingStartedPublisherMock.Reset();
        VotingCompletedPublisherMock.Reset();
    }

    public void Dispose()
    {
    }
}