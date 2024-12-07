using DreamTeamOptimizer.ConsoleApp.Exceptions;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.ConsoleApp.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Unit.Services;

public class HrDirectorServiceTests : IClassFixture<HrDirectorServiceFixture>
{
    private readonly HrDirectorServiceFixture _fixture;
    private readonly Mock<ISatisfactionRepository> _satisfactionRepositoryMock;
    private readonly IHrDirectorService _service;

    public HrDirectorServiceTests(HrDirectorServiceFixture fixture)
    {
        _fixture = fixture;
        _satisfactionRepositoryMock = new Mock<ISatisfactionRepository>();
        _service = new HrDirectorService(_satisfactionRepositoryMock.Object);
    }

    [Fact]
    public void CalculateDistributionHarmony_ValidInputs_ReturnsCorrectDistributionHarmony()
    {
        // Prepare
        _satisfactionRepositoryMock.Setup(r => r.CreateAll(It.IsAny<List<Core.Persistence.Entities.Satisfaction>>()));

        // Act
        var result = _service.CalculateSatisfactions(_fixture.Teams, _fixture.TeamLeadsWishlists,
            _fixture.JuniorsWishlists, _fixture.HackathonId);

        // Assert
        Assert.NotNull(result);
        result.Should()
            .HaveCount(_fixture.Satisfactions.Count)
            .And.BeEquivalentTo(_fixture.Satisfactions);

        _satisfactionRepositoryMock.Verify(r => r.CreateAll(It.IsAny<List<Core.Persistence.Entities.Satisfaction>>()),
            Times.Once);
    }

    [Fact]
    public void CalculateDistributionHarmony_EmployeeNotInWishlist_ThrowsException()
    {
        // Arrange
        var teamLeadsWishlists = new List<WishList>
        {
            new(1, [3, 4]),
            new(2, [4])
        };

        // Act & Assert
        var exception = Assert.Throws<WishListNotFoundException>(() =>
            _service.CalculateSatisfactions(_fixture.Teams, teamLeadsWishlists, _fixture.JuniorsWishlists,
                _fixture.HackathonId));

        Assert.Equal(3, exception.EmployeeId);
    }

    [Fact]
    public void CalculateDistributionHarmony_WishlistNotFound_ThrowsException()
    {
        // Arrange
        var teamLeadsWishlists = new List<WishList>();

        // Act & Assert
        var exception = Assert.Throws<WishListNotFoundException>(() =>
            _service.CalculateSatisfactions(_fixture.Teams, teamLeadsWishlists, _fixture.JuniorsWishlists,
                _fixture.HackathonId));

        Assert.Equal(3, exception.EmployeeId);
    }
}

public class HrDirectorServiceFixture
{
    public List<Employee> Juniors { get; set; }
    public List<Employee> TeamLeads { get; set; }
    public List<WishList> JuniorsWishlists { get; set; }
    public List<WishList> TeamLeadsWishlists { get; set; }
    public List<Team> Teams { get; set; }
    public List<Satisfaction> Satisfactions { get; set; }
    public int HackathonId { get; set; }

    public HrDirectorServiceFixture()
    {
        Juniors = new List<Employee> { new(1, "Junior1", Grade.JUNIOR), new(2, "Junior2", Grade.JUNIOR) };
        TeamLeads = new List<Employee> { new(3, "TeamLead1", Grade.TEAM_LEAD), new(4, "TeamLead2", Grade.TEAM_LEAD) };
        JuniorsWishlists = new List<WishList> { new(1, [3, 4]), new(2, [4, 3]) };
        TeamLeadsWishlists = new List<WishList> { new(3, [1, 2]), new(4, [2, 1]) };
        Teams = new List<Team> { new(TeamLeads[0], Juniors[0]), new(TeamLeads[1], Juniors[1]) };
        Satisfactions = new List<Satisfaction>
        {
            new(1, 2.0),
            new(2, 2.0),
            new(3, 2.0),
            new(4, 2.0)
        };
        HackathonId = 1;
    }
}