using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Services;
using DreamTeamOptimizer.Core.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Unit.Services;

public class WishListServiceTests : IClassFixture<WishListServiceFixture>
{
    private readonly Mock<IWishListRepository> _repositoryMock;
    private readonly IWishListService _service;
    private readonly WishListServiceFixture _fixture;

    public WishListServiceTests(WishListServiceFixture fixture)
    {
        _fixture = fixture;
        _repositoryMock = new Mock<IWishListRepository>();
        _service = new WishListService(_repositoryMock.Object);
    }

    [Fact]
    public void GetWishlists_ShouldHaveCorrectList()
    {
        // Prepare
        _repositoryMock.Setup(s => s.CreateAll(It.IsAny<List<Persistence.Entities.WishList>>()));
        
        // Act
        var wishlists = _service.GenerateWishlists(_fixture.Employees, _fixture.DesiredEmployees, _fixture.HackathonId);

        // Assert
        wishlists.Should().HaveCount(_fixture.Employees.Count);

        for (int i = 0; i < wishlists.Count; i++)
        {
            wishlists[i].EmployeeId.Should().Be(_fixture.Employees[i].Id);
            wishlists[i].DesiredEmployees.Should()
                .HaveCount(_fixture.DesiredEmployees.Count)
                .And.BeEquivalentTo(_fixture.DesiredEmployeeIds);
        }
        
        _repositoryMock.Verify(r => r.CreateAll(It.IsAny<List<Persistence.Entities.WishList>>()), Times.Once);
    }
}

public class WishListServiceFixture
{
    public int HackathonId { get; }
    public List<Employee> Employees { get; }
    public List<Employee> DesiredEmployees { get; }
    public List<int> DesiredEmployeeIds { get; }

    public WishListServiceFixture()
    {
        HackathonId = 1;

        Employees = new List<Employee>
        {
            new(1, "employee_1", Grade.TEAM_LEAD),
            new(2, "employee_2", Grade.TEAM_LEAD),
        };

        DesiredEmployees = new List<Employee>
        {
            new(3, "employee_3", Grade.TEAM_LEAD),
            new(4, "employee_4", Grade.TEAM_LEAD)
        };

        DesiredEmployeeIds = DesiredEmployees.Select(e => e.Id).ToList();
    }
}