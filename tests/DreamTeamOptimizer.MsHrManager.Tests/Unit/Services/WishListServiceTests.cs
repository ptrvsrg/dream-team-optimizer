using System.Net;
using DreamTeamOptimizer.Core.Exceptions;
using FluentAssertions;
using Moq;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.MsHrManager.Interfaces.Clients;
using DreamTeamOptimizer.MsHrManager.Services;
using Microsoft.Extensions.Logging;
using WishListEntity = DreamTeamOptimizer.Core.Persistence.Entities.WishList;

namespace DreamTeamOptimizer.MsHrManager.Tests.Unit.Services;

public class WishListServiceTests(WishListServiceFixture fixture) : IClassFixture<WishListServiceFixture>, IDisposable
{
    public void Dispose()
    {
        fixture.ResetMocks();
    }

    [Theory]
    [MemberData(nameof(ValidVoteModels))]
    public void Vote_ShouldSaveWishLists_WhenValidEmployeesAndDesiredEmployeesExist(
        List<Employee> employees,
        List<Employee> desiredEmployees,
        List<WishList> expectedWishLists)
    {
        // Arrange
        foreach (var employee in employees)
        {
            fixture.EmployeeClientMock
                .Setup(client => client.Vote(employee.Id, It.IsAny<List<int>>()))
                .Returns(new WishList(employee.Id, [3, 4]));
        }

        fixture.WishListRepositoryMock
            .Setup(r => r.CreateAll(It.IsAny<List<WishListEntity>>()))
            .Verifiable();

        // Act
        var result = fixture.WishListService.Vote(employees, desiredEmployees, hackathonId: 123);

        // Assert
        result.Should().BeEquivalentTo(expectedWishLists);
        fixture.WishListRepositoryMock.Verify(r => r.CreateAll(It.IsAny<List<WishListEntity>>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(EmptyEmployeesModels))]
    public void Vote_ShouldReturnEmpty_WhenNoEmployeesProvided(List<Employee> employees,
        List<Employee> desiredEmployees)
    {
        // Arrange
        fixture.EmployeeClientMock
            .Setup(client => client.Vote(It.IsAny<int>(), It.IsAny<List<int>>()))
            .Returns(new WishList(1, []));

        // Act
        var result = fixture.WishListService.Vote(employees, desiredEmployees, hackathonId: 123);

        // Assert
        result.Should().BeEmpty();
        fixture.EmployeeClientMock.Verify(client => client.Vote(It.IsAny<int>(), It.IsAny<List<int>>()), Times.Never);
    }

    [Fact]
    public void Vote_ShouldThrowException_WhenEmployeeClientFails()
    {
        // Arrange
        var employees = new List<Employee> { new(1, "Employee 1") };
        var desiredEmployees = new List<Employee> { new(2, "Employee 2") };

        fixture.EmployeeClientMock
            .Setup(client => client.Vote(It.IsAny<int>(), It.IsAny<List<int>>()))
            .Throws(new HttpStatusException(HttpStatusCode.InternalServerError, "Error"));

        // Act
        var act = () => fixture.WishListService.Vote(employees, desiredEmployees, hackathonId: 123);

        // Assert
        act.Should().Throw<HttpStatusException>().WithMessage("Error");
    }

    public static IEnumerable<object[]> ValidVoteModels => new List<object[]>
    {
        new object[]
        {
            new List<Employee>
            {
                new(1, "Employee 1"),
                new(2, "Employee 2")
            },
            new List<Employee>
            {
                new(3, "Employee 3"),
                new(4, "Employee 4")
            },
            new List<WishList>
            {
                new(1, [3, 4]),
                new(2, [3, 4])
            }
        }
    };

    public static IEnumerable<object[]> EmptyEmployeesModels => new List<object[]>
    {
        new object[] { new List<Employee>(), new List<Employee>() }
    };
}

public class WishListServiceFixture : IDisposable
{
    public Mock<IWishListRepository> WishListRepositoryMock { get; }
    public Mock<IEmployeeClient> EmployeeClientMock { get; }
    public WishListService WishListService { get; }

    public WishListServiceFixture()
    {
        WishListRepositoryMock = new Mock<IWishListRepository>();
        EmployeeClientMock = new Mock<IEmployeeClient>();

        WishListService = new WishListService(
            new Mock<ILogger<WishListService>>().Object,
            WishListRepositoryMock.Object,
            EmployeeClientMock.Object
        );
    }

    public void ResetMocks()
    {
        WishListRepositoryMock.Reset();
        EmployeeClientMock.Reset();
    }

    public void Dispose()
    {
    }
}