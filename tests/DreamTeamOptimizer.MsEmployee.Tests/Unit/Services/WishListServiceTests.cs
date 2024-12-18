using DreamTeamOptimizer.MsEmployee.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace DreamTeamOptimizer.MsEmployee.Tests.Unit.Services;

public class WishListServiceTests(WishListServiceTestsFixture fixture) : IClassFixture<WishListServiceTestsFixture>
{
    [Fact]
    public void GetWishlist_ShouldReturnWishlist_WithEmployeeIdFromConfig()
    {
        // Arrange
        var desiredEmployeeIds = fixture.DesiredEmployeeIds;

        // Act
        var result = fixture.WishListService.GetWishlist(desiredEmployeeIds);

        // Assert
        result.Should().NotBeNull();
        result.EmployeeId.Should().Be(fixture.EmployeeId);
        result.DesiredEmployees.Should().HaveCount(desiredEmployeeIds.Count);
    }

    [Fact]
    public void GetWishlist_ShouldReturnCorrectOrderOfEmployees_WhenDesiredEmployeeIdsAreProvided()
    {
        // Arrange
        var desiredEmployeeIds = fixture.DesiredEmployeeIds;

        // Act
        var result = fixture.WishListService.GetWishlist(desiredEmployeeIds);

        // Assert
        result.DesiredEmployees.Should().NotEqual(desiredEmployeeIds);
    }

    [Fact]
    public void GetWishlist_ShouldHandleEmptyDesiredEmployeeIds()
    {
        // Arrange
        var desiredEmployeeIds = new List<int>();

        // Act
        var result = fixture.WishListService.GetWishlist(desiredEmployeeIds);

        // Assert
        result.DesiredEmployees.Should().BeEmpty();
    }
}

public class WishListServiceTestsFixture
{
    public Mock<ILogger<WishListService>> LoggerMock { get; }
    public WishListService WishListService { get; }
    public List<int> DesiredEmployeeIds { get; }
    public int EmployeeId { get; }

    public WishListServiceTestsFixture()
    {
        LoggerMock = new Mock<ILogger<WishListService>>();

        // Настроим реальную конфигурацию с использованием ConfigurationBuilder
        var inMemorySettings = new Dictionary<string, string>
        {
            { "Application:EmployeeID", "1" }
        };

        var configurationBuilder = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings);
        var config = configurationBuilder.Build();

        // Данные для тестов
        EmployeeId = 1;
        DesiredEmployeeIds = new List<int> { 2, 3, 4, 5 };

        WishListService = new WishListService(
            LoggerMock.Object,
            config
        );
    }

    public void ResetMocks()
    {
        LoggerMock.Reset();
    }
}