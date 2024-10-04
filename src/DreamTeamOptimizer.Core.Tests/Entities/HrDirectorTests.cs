using System;
using System.Collections.Generic;
using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Exceptions;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Entities;

public class HrDirectorTests
{
    private readonly HrDirector _hrDirector;

    public HrDirectorTests()
    {
        _hrDirector = new HrDirector();
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
            new(1, new[] { 2, 4 }),
            new(3, new[] { 4, 2 })
        };

        var juniorsWishlists = new List<WishList>
        {
            new(2, new[] { 1, 3 }),
            new(4, new[] { 3, 1 })
        };

        // Act
        var result = _hrDirector.CalculateDistributionHarmony(teams, teamLeadsWishlists, juniorsWishlists);

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
            new(1, new[] { 4 }), // miss Employee with ID 2
            new(3, new[] { 4, 2 })
        };

        var juniorsWishlists = new List<WishList>
        {
            new(2, new[] { 1, 3 }),
            new(4, new[] { 3, 1 })
        };

        // Act & Assert
        var exception = Assert.Throws<EmployeeInWishListNotFoundException>(() =>
            _hrDirector.CalculateDistributionHarmony(teams, teamLeadsWishlists, juniorsWishlists));

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
            new(2, new[] { 1, 3 }),
            new(4, new[] { 3, 1 })
        };

        // Act & Assert
        var exception = Assert.Throws<WishListNotFoundException>(() =>
            _hrDirector.CalculateDistributionHarmony(teams, teamLeadsWishlists, juniorsWishlists));

        Assert.Equal(1, exception.EmployeeId);
    }
}