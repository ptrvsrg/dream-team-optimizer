using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrManager.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

using TeamEntity = DreamTeamOptimizer.Core.Persistence.Entities.Team;

namespace DreamTeamOptimizer.MsHrManager.Tests.Unit.Services;

public class StrategyServiceTests : IClassFixture<StrategyServiceFixture>, IDisposable
{
    private readonly StrategyServiceFixture _fixture;

    public StrategyServiceTests(StrategyServiceFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.ResetMocks();
    }

    [Fact]
    public void BuildTeams_ShouldReturnTeams_WhenValidDataProvided()
    {
        // Arrange
        var teamLeads = _fixture.TeamLeads;
        var juniors = _fixture.Juniors;
        var teamLeadsWishlists = _fixture.TeamLeadsWishlists;
        var juniorsWishlists = _fixture.JuniorsWishlists;
        var expectedTeams = _fixture.ExpectedTeamModels;

        _fixture.StrategyMock
            .Setup(s => s.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists))
            .Returns(expectedTeams);

        _fixture.TeamRepositoryMock
            .Setup(repo => repo.CreateAll(It.IsAny<List<TeamEntity>>()))
            .Verifiable();

        // Act
        var result = _fixture.StrategyService.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists, hackathonId: 123);

        // Assert
        result.Should().BeEquivalentTo(expectedTeams);
        _fixture.TeamRepositoryMock.Verify(repo => repo.CreateAll(It.IsAny<List<TeamEntity>>()), Times.Once);
    }

    [Fact]
    public void BuildTeams_ShouldLogInformation_WhenCalled()
    {
        // Arrange
        var teamLeads = _fixture.TeamLeads;
        var juniors = _fixture.Juniors;
        var teamLeadsWishlists = _fixture.TeamLeadsWishlists;
        var juniorsWishlists = _fixture.JuniorsWishlists;

        _fixture.StrategyMock
            .Setup(s => s.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists))
            .Returns(_fixture.ExpectedTeamModels);

        // Act
        _fixture.StrategyService.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists, hackathonId: 123);

        // Assert
        _fixture.LoggerMock.Verify(
            log => log.Log(
                It.Is<LogLevel>(level => level == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((obj, t) => obj.ToString() == "build teams"),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}

public class StrategyServiceFixture : IDisposable
{
    public Mock<ILogger<StrategyService>> LoggerMock { get; }
    public Mock<IStrategy> StrategyMock { get; }
    public Mock<ITeamRepository> TeamRepositoryMock { get; }
    public StrategyService StrategyService { get; }

    public List<Employee> TeamLeads { get; }
    public List<Employee> Juniors { get; }
    public List<WishList> TeamLeadsWishlists { get; }
    public List<WishList> JuniorsWishlists { get; }
    public List<Team> ExpectedTeamModels { get; }

    public StrategyServiceFixture()
    {
        LoggerMock = new Mock<ILogger<StrategyService>>();
        StrategyMock = new Mock<IStrategy>();
        TeamRepositoryMock = new Mock<ITeamRepository>();

        StrategyService = new StrategyService(LoggerMock.Object, StrategyMock.Object, TeamRepositoryMock.Object);

        TeamLeads = new List<Employee>
        {
            new(1, "TeamLead 1"),
            new(2, "TeamLead 2")
        };

        Juniors = new List<Employee>
        {
            new(3, "Junior 1"),
            new(4, "Junior 2")
        };

        TeamLeadsWishlists = new List<WishList>
        {
            new(1, new[] { 3, 4 }),
            new(2, new[] { 4, 3 })
        };

        JuniorsWishlists = new List<WishList>
        {
            new(3, new[] { 1 }),
            new(4, new[] { 2 })
        };

        ExpectedTeamModels = new List<Team>
        {
            new(1, 3),
            new(2, 4)
        };
    }

    public void ResetMocks()
    {
        LoggerMock.Reset();
        StrategyMock.Reset();
        TeamRepositoryMock.Reset();
    }

    public void Dispose()
    {
    }
}
