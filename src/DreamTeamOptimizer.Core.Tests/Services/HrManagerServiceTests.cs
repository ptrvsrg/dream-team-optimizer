using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Core.Interfaces.IServices;
using DreamTeamOptimizer.Core.Services;
using Moq;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Services;

public class HrManagerServiceTests
{
    private readonly Mock<IStrategy> _strategyMock;
    private readonly IHrManagerService _service;
    
    public HrManagerServiceTests()
    {
        _strategyMock = new Mock<IStrategy>();
        _service = new HrManagerService(_strategyMock.Object);
    }
    
    [Fact]
    public void BuildTeams_ShouldCallStrategyBuildTeams()
    {
        // Arrange
        var teamLead1 = new Employee(1, "TeamLead1");
        var junior1 = new Employee(2, "Junior1");
        var teamLeads = new List<Employee> { teamLead1 };
        var juniors = new List<Employee> { junior1 };
        var teamLeadsWishlists = new List<WishList>();
        var juniorsWishlists = new List<WishList>();
    
        var expectedTeams = new List<Team> { new(teamLead1, junior1) };
        _strategyMock.Setup(s => s.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists))
            .Returns(expectedTeams);
    
        // Act
        var result = _service.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);
    
        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTeams.Count, result.Count);
        _strategyMock.Verify(s => s.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists), Times.Once);
    }
}