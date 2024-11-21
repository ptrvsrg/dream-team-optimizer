using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Interfaces.IServices;
using DreamTeamOptimizer.Core.Services;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Services;

public class HrDirectorServiceTests
{
    private readonly IHrDirectorService _service;

    public HrDirectorServiceTests()
    {
        _service = new HrDirectorService();
    }

    [Fact]
    public void CalculateDistributionHarmony_ValidInputs_ReturnsCorrectDistributionHarmony()
    {
        // Arrange
        var teams = new List<Team>
        {
            new(new Employee(1, "employee_1"), new Employee(2, "employee_2")),
            new(new Employee(3, "employee_3"), new Employee(4, "employee_4"))
        };

        var teamLeadsWishlists = new List<WishList>
        {
            new(1, [2, 4]),
            new(3, [4, 2])
        };

        var juniorsWishlists = new List<WishList>
        {
            new(2, [1, 3]),
            new(4, [3, 1])
        };

        // Act
        var result = _service.CalculateDistributionHarmony(teams, teamLeadsWishlists, juniorsWishlists);

        // Assert
        Assert.Equal(2.0, result, 4); // Пример ожидаемого результата
    }

    [Fact]
    public void CalculateDistributionHarmony_EmployeeNotInWishlist_ThrowsException()
    {
        // Arrange
        var teams = new List<Team>
        {
            new(new Employee(1, "employee_1"), new Employee(2, "employee_2")),
            new(new Employee(3, "employee_3"), new Employee(4, "employee_4"))
        };

        var teamLeadsWishlists = new List<WishList>
        {
            new(1, [4]), // miss Employee with ID 2
            new(3,  [4, 2])
        };

        var juniorsWishlists = new List<WishList>
        {
            new(2, [1, 3]),
            new(4, [3, 1])
        };

        // Act & Assert
        var exception = Assert.Throws<EmployeeInWishListNotFoundException>(() =>
            _service.CalculateDistributionHarmony(teams, teamLeadsWishlists, juniorsWishlists));

        Assert.Equal(2, exception.SelectedEmployeeId);
    }

    [Fact]
    public void CalculateDistributionHarmony_WishlistNotFound_ThrowsException()
    {
        // Arrange
        var teams = new List<Team>
        {
            new(new Employee(1, "employee_1"), new Employee(2, "employee_2")),
            new(new Employee(3, "employee_3"), new Employee(4, "employee_4"))
        };

        var teamLeadsWishlists = new List<WishList>(); // Empty wish list
        var juniorsWishlists = new List<WishList>
        {
            new(2, [1, 3]),
            new(4, [3, 1])
        };

        // Act & Assert
        var exception = Assert.Throws<WishListNotFoundException>(() =>
            _service.CalculateDistributionHarmony(teams, teamLeadsWishlists, juniorsWishlists));

        Assert.Equal(1, exception.EmployeeId);
    }
}