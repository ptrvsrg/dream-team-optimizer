using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Interfaces;
using Moq;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Entities;

public class HackathonTests
{
    private readonly Mock<IStrategy> _strategyMock;
    private readonly TeamLead _teamLead;
    private readonly Junior _junior;
    private readonly HrManager _hrManager;
    private readonly HrDirector _hrDirector;
    private readonly Hackathon _hackathon;

    public HackathonTests()
    {
        _strategyMock = new Mock<IStrategy>();

        _teamLead = new TeamLead(1, "Lead1");
        _junior = new Junior(2, "Junior1");
        var teamLeads = new List<TeamLead> { _teamLead };
        var juniors = new List<Junior> { _junior };

        _hrManager = new HrManager(_strategyMock.Object);
        _hrDirector = new HrDirector();
        _hackathon = new Hackathon(teamLeads, juniors, _hrManager, _hrDirector);
    }

    [Fact]
    public void Conduct_ShouldReturnDistributionHarmony()
    {
        // Arrange
        var teams = new List<Team>
        {
            new(_teamLead, _junior)
        };

        // Mock
        _strategyMock
            .Setup(s => s.BuildTeams(It.IsAny<IEnumerable<Employee>>(), It.IsAny<IEnumerable<Employee>>(),
                It.IsAny<IEnumerable<WishList>>(), It.IsAny<IEnumerable<WishList>>()))
            .Returns(teams);

        // Act
        var result = _hackathon.Conduct();

        // Assert
        Assert.Equal(1.0, result);
        _strategyMock.Verify(
            s => s.BuildTeams(It.IsAny<IEnumerable<Employee>>(), It.IsAny<IEnumerable<Employee>>(),
                It.IsAny<IEnumerable<WishList>>(), It.IsAny<IEnumerable<WishList>>()), Times.Once);
    }
}