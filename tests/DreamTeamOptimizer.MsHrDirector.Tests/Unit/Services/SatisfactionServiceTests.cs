using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrDirector.Services;
using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace DreamTeamOptimizer.MsHrDirector.Tests.Unit.Services;

public class SatisfactionServiceTests(SatisfactionServiceFixture fixture) : IClassFixture<SatisfactionServiceFixture>
{
    [Theory]
    [MemberData(nameof(SatisfactionServiceFixture.ValidTeamsData), MemberType = typeof(SatisfactionServiceFixture))]
    public void Evaluate_ShouldReturnSatisfaction_WhenValidDataProvided(List<Team> teams,
        List<WishList> teamLeadsWishlists, List<WishList> juniorsWishlists)
    {
        // Arrange
        var service = new SatisfactionService(fixture.Logger);

        // Act
        var result = service.Evaluate(teams, teamLeadsWishlists, juniorsWishlists);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(teams.Count * 2);
    }
}

public class SatisfactionServiceFixture
{
    public static IEnumerable<object[]> ValidTeamsData =>
        new List<object[]>
        {
            new object[]
            {
                new List<Team>
                {
                    new(1, 2),
                    new(3, 4)
                },
                new List<WishList>
                {
                    new(1, [2, 4]),
                    new(3, [4, 2])
                },
                new List<WishList>
                {
                    new(2, [1, 3]),
                    new(4, [3, 1])
                }
            }
        };

    public ILogger<SatisfactionService> Logger { get; }

    public SatisfactionServiceFixture()
    {
        var loggerMock = new Mock<ILogger<SatisfactionService>>();
        Logger = loggerMock.Object;
    }
}