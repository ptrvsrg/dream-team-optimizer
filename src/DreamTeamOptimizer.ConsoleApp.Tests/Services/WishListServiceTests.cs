using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Interfaces.IServices;
using DreamTeamOptimizer.Core.Services;
using Xunit;
using FluentAssertions;

namespace DreamTeamOptimizer.Core.Tests.Services;

public class WishListServiceTests : IClassFixture<WishListServiceFixture>
{
    private readonly IWishListService _service;
    private readonly WishListServiceFixture _fixture;

    public WishListServiceTests(WishListServiceFixture fixture)
    {
        _fixture = fixture;
        _service = fixture.Service;
    }

    [Fact]
    public void GetWishlists_ShouldHaveCorrectList()
    {
        // Act
        var wishlists = _service.GetWishlists(_fixture.Employees, _fixture.DesiredEmployees);

        // Assert
        wishlists.Should().HaveCount(_fixture.Employees.Count);

        for (int i = 0; i < wishlists.Count; i++)
        {
            wishlists[i].EmployeeId.Should().Be(_fixture.Employees[i].Id);
            wishlists[i].DesiredEmployees.Should()
                .HaveCount(_fixture.DesiredEmployees.Count)
                .And.BeEquivalentTo(_fixture.DesiredEmployeeIds);
        }
    }
}

public class WishListServiceFixture
{
    public IWishListService Service { get; }
    public List<Employee> Employees { get; }
    public List<Employee> DesiredEmployees { get; }
    public List<int> DesiredEmployeeIds { get; }

    public WishListServiceFixture()
    {
        Service = new WishListService();
        
        Employees = new List<Employee>
        {
            new(1, "employee_1"),
            new(2, "employee_2"),
        };

        DesiredEmployees = new List<Employee>
        {
            new(3, "employee_3"),
            new(4, "employee_4")
        };

        DesiredEmployeeIds = DesiredEmployees.Select(e => e.Id).ToList();
    }
}