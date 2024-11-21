using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Interfaces;
using DreamTeamOptimizer.Core.Interfaces.IServices;
using DreamTeamOptimizer.Core.Services;
using Moq;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Services;

public class HackathonServiceTests
{
    private readonly Mock<IStrategy> _strategyMock;
    private readonly Mock<IEmployeeService> _employeeServiceMock;
    private readonly IHackathonService _hackathonService;

    public HackathonServiceTests()
    {
        _strategyMock = new Mock<IStrategy>();
        _employeeServiceMock = new Mock<IEmployeeService>();
        var wishListService = new WishListService();
        var hrManagerService = new HrManagerService(_strategyMock.Object);
        var hrDirectorService = new HrDirectorService();
        _hackathonService = new HackathonService(
            _employeeServiceMock.Object, wishListService, hrManagerService, hrDirectorService);
    }

    [Fact]
    public void Conduct_ShouldReturnDistributionHarmony()
    {
        // Arrange
        var teamLead = new Employee(1, "Lead1");
        var junior = new Employee(2, "Junior1");
        
        var teamLeads = new List<Employee> { teamLead };
        var juniors = new List<Employee> { junior };
        
        var teams = new List<Team>
        {
            new(teamLead, junior)
        };

        // Mock
        _employeeServiceMock
            .Setup(s => s.FindAllJuniors())
            .Returns(juniors);
        _employeeServiceMock
            .Setup(s => s.FindAllTeamLeads())
            .Returns(teamLeads);
        _strategyMock
            .Setup(s => s.BuildTeams(It.IsAny<IEnumerable<Employee>>(), It.IsAny<IEnumerable<Employee>>(),
                It.IsAny<IEnumerable<WishList>>(), It.IsAny<IEnumerable<WishList>>()))
            .Returns(teams);

        // Act
        var result = _hackathonService.Conduct();

        // Assert
        Assert.Equal(1.0, result);
        _strategyMock.Verify(
            s => s.BuildTeams(It.IsAny<IEnumerable<Employee>>(), It.IsAny<IEnumerable<Employee>>(),
                It.IsAny<IEnumerable<WishList>>(), It.IsAny<IEnumerable<WishList>>()), Times.Once);
    }
}