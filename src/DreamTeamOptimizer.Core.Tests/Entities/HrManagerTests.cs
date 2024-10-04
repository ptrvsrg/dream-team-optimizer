using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Interfaces;
using Moq;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Entities;

public class HrManagerTests
{
    private readonly Mock<IStrategy> _strategyMock;
    private readonly HrManager _hrManager;

    public HrManagerTests()
    {
        _strategyMock = new Mock<IStrategy>();
        _hrManager = new HrManager(_strategyMock.Object);
    }

    [Fact]
    public void VoteEmployees_ShouldReturnCorrectWishLists()
    {
        // Prepare
        var employees = new List<Employee>
        {
            new(1, "employee_1"),
            new(2, "employee_2"),
            new(3, "employee_3")
        };
        var variants = new List<Employee>
        {
            new(4, "employee_4"),
            new(5, "employee_5"),
            new(6, "employee_6")
        };

        // Act
        var result = _hrManager.VoteEmployees(employees, variants);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employees.Count, result.Count);
        Assert.All(result, wishlist => Assert.Equal(variants.Count, wishlist.DesiredEmployees.Length));
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
        var result = _hrManager.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedTeams.Count, result.Count);
        _strategyMock.Verify(s => s.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists), Times.Once);
    }
}