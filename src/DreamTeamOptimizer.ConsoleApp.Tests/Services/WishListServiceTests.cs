using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.ConsoleApp.Services;
using Xunit;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Services;

public class WishListServiceTests
{
    private readonly IWishListService _service;

    public WishListServiceTests()
    {
        _service = new WishListService();
    }

    [Fact]
    public void GetWishlists_ShouldHaveCorrectList()
    {
        // Prepare
        var employees = new List<Employee>
        {
            new(1, "employee_1"),
            new(2, "employee_2"),
        };
        var desiredEmployees = new List<Employee>
        {
            new(3, "employee_3"),
            new(4, "employee_4")
        };

        // Act
        var wishlists = _service.GenerateWishlists(employees, desiredEmployees);

        // Check
        Assert.Equal(employees.Count, wishlists.Count);
        
        var expectedEmployeeIds = desiredEmployees.Select(e => e.Id).ToArray();
        Array.Sort(expectedEmployeeIds);
        
        for (int i = 0; i < wishlists.Count; i++)
        {
            Assert.Equal(employees[i].Id, wishlists[i].EmployeeId);
            Assert.Equal(desiredEmployees.Count, wishlists[i].DesiredEmployees.Length);
            
            var actualEmployeeIds = (int[])wishlists[i].DesiredEmployees.Clone();
            Array.Sort(actualEmployeeIds);

            Assert.Equal(expectedEmployeeIds, actualEmployeeIds);
        }
    }
}