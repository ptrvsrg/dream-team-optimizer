using DreamTeamOptimizer.Core.Entities;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Entities;

public class EmployeeTests
{
    [Fact]
    public void GetWishlist_ShouldHaveCorrectList()
    {
        // Prepare
        var employee = new Employee(1, "employee_1");
        var employees = new List<Employee>
        {
            new(2, "employee_2"),
            new(3, "employee_3"),
            new(4, "employee_4")
        };

        // Act
        var wishlist = employee.GetWishlist(employees);

        // Check
        Assert.Equal(employee.Id, wishlist.EmployeeId);
        Assert.Equal(employees.Count, wishlist.DesiredEmployees.Length);

        var expectedEmployeeIds = employees.Select(e => e.Id).ToArray();
        Array.Sort(expectedEmployeeIds);

        var actualEmployeeIds = (int[])wishlist.DesiredEmployees.Clone();
        Array.Sort(actualEmployeeIds);

        Assert.Equal(expectedEmployeeIds, actualEmployeeIds);
    }
}