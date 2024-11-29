using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.ConsoleApp.Services;
using DreamTeamOptimizer.Core.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Unit.Services;

public class HrManagerServiceTests : IClassFixture<HrManagerServiceFixture>
{
    private readonly HrManagerServiceFixture _fixture;
    private readonly Mock<IStrategy> _strategyMock;
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly IHrManagerService _service;

    public HrManagerServiceTests(HrManagerServiceFixture fixture)
    {
        _fixture = fixture;
        _strategyMock = new Mock<IStrategy>();
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _service = new HrManagerService(_strategyMock.Object, _teamRepositoryMock.Object);
    }

    [Fact]
    public void BuildTeams_ShouldCallStrategyBuildTeams()
    {
        // Prepare
        _strategyMock
            .Setup(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists,
                _fixture.JuniorsWishlists))
            .Returns(_fixture.Teams);

        // Act
        var result = _service.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists,
            _fixture.JuniorsWishlists, _fixture.HackathonId);

        // Assert
        Assert.NotNull(result);
        result.Should()
            .HaveCount(_fixture.Teams.Count)
            .And.BeEquivalentTo(_fixture.Teams);
        _strategyMock.Verify(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists,
            _fixture.JuniorsWishlists), Times.Once);
    }
}

public class HrManagerServiceFixture
{
    public List<Employee> TeamLeads { get; set; }
    public List<Employee> Juniors { get; set; }
    public List<WishList> TeamLeadsWishlists { get; set; }
    public List<WishList> JuniorsWishlists { get; set; }
    public List<Team> Teams { get; set; }
    public int HackathonId { get; set; }

    public HrManagerServiceFixture()
    {
        TeamLeads = new List<Employee> { new(1, "TeamLead1", Grade.TEAM_LEAD) };
        Juniors = new List<Employee> { new(2, "Junior1", Grade.JUNIOR) };
        TeamLeadsWishlists = new List<WishList> { new(1, [2]) };
        JuniorsWishlists = new List<WishList> { new(2, [1]) };
        Teams = new List<Team> { new(TeamLeads[0], Juniors[0]) };
        HackathonId = 1;
    }
}