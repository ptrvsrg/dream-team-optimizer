using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.MsHrManager.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Steeltoe.Discovery;

namespace DreamTeamOptimizer.MsHrManager.Tests.Unit.Services;

public class EmployeeServiceTests(EmployeeServiceFixture fixture) : IClassFixture<EmployeeServiceFixture>, IDisposable
{
    public static IEnumerable<object[]> JuniorModels => new List<object[]>
    {
        new object[]
        {
            new List<string>
            {
                "ms-employee-1",
                "ms-employee-2",
                "ms-employee-3",
                "ms-employee-4"
            },
            new List<Employee>
            {
                new()
                {
                    Id = 1,
                    Name = "Employee 1",
                    Grade = Grade.JUNIOR
                },
                new()
                {
                    Id = 2,
                    Name = "Employee 2",
                    Grade = Grade.JUNIOR
                }
            }
        },
        new object[]
        {
            new List<string>(),
            new List<Employee>()
        }
    };

    public static IEnumerable<object[]> TeamLeadModels => new List<object[]>
    {
        new object[]
        {
            new List<string>
            {
                "ms-employee-1",
                "ms-employee-2",
                "ms-employee-3",
                "ms-employee-4"
            },
            new List<Employee>
            {
                new()
                {
                    Id = 1,
                    Name = "Employee 1",
                    Grade = Grade.TEAM_LEAD
                },
                new()
                {
                    Id = 2,
                    Name = "Employee 2",
                    Grade = Grade.TEAM_LEAD
                }
            }
        },
        new object[]
        {
            new List<string>(),
            new List<Employee>()
        }
    };

    public void Dispose()
    {
        fixture.ResetMocks();
    }

    [Theory]
    [MemberData(nameof(JuniorModels))]
    public void FindAllJuniors_ShouldReturnEmployees_WhenActiveJuniorsExist(List<string> services,
        List<Employee> juniors)
    {
        // Arrange
        fixture.DiscoveryClientMock
            .Setup(dc => dc.Services)
            .Returns(services);

        fixture.EmployeeRepositoryMock
            .Setup(er => er.Find(It.IsAny<Func<Employee, bool>>()))
            .Returns(juniors);

        // Act
        var result = fixture.EmployeeService.FindAllJuniors();

        // Assert
        result.Should().HaveCount(juniors.Count);
        if (juniors.Count > 0)
        {
            result.Should().OnlyContain(e => e.Grade == Core.Models.Grade.JUNIOR);
        }

        fixture.DiscoveryClientMock.Verify(dc => dc.Services, Times.Once);
        fixture.EmployeeRepositoryMock.Verify(er => er.Find(It.IsAny<Func<Employee, bool>>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(TeamLeadModels))]
    public void FindAllTeamLeads_ShouldReturnEmployees_WhenActiveTeamLeadsExist(List<string> services,
        List<Employee> teamLeads)
    {
        // Arrange
        fixture.DiscoveryClientMock
            .Setup(dc => dc.Services)
            .Returns(services);

        fixture.EmployeeRepositoryMock
            .Setup(er => er.Find(It.IsAny<Func<Employee, bool>>()))
            .Returns(teamLeads);

        // Act
        var result = fixture.EmployeeService.FindAllTeamLeads();

        // Assert
        result.Should().HaveCount(teamLeads.Count);
        if (teamLeads.Count > 0)
        {
            result.Should().OnlyContain(e => e.Grade == Core.Models.Grade.TEAM_LEAD);
        }

        fixture.DiscoveryClientMock.Verify(dc => dc.Services, Times.Once);
        fixture.EmployeeRepositoryMock.Verify(er => er.Find(It.IsAny<Func<Employee, bool>>()), Times.Once);
    }
}

public class EmployeeServiceFixture : IDisposable
{
    public Mock<ILogger<EmployeeService>> LoggerMock { get; }
    public Mock<IDiscoveryClient> DiscoveryClientMock { get; }
    public Mock<IEmployeeRepository> EmployeeRepositoryMock { get; }
    public EmployeeService EmployeeService { get; }

    public EmployeeServiceFixture()
    {
        LoggerMock = new Mock<ILogger<EmployeeService>>();
        DiscoveryClientMock = new Mock<IDiscoveryClient>();
        EmployeeRepositoryMock = new Mock<IEmployeeRepository>();

        EmployeeService = new EmployeeService(
            LoggerMock.Object,
            DiscoveryClientMock.Object,
            EmployeeRepositoryMock.Object
        );
    }

    public void ResetMocks()
    {
        LoggerMock.Reset();
        DiscoveryClientMock.Reset();
        EmployeeRepositoryMock.Reset();
    }

    public void Dispose()
    {
    }
}