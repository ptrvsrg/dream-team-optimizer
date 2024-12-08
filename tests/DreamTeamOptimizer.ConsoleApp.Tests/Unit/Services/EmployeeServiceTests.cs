using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.ConsoleApp.Services;
using DreamTeamOptimizer.ConsoleApp.Services.Mappers;
using FluentAssertions;
using Moq;
using Xunit;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Unit.Services;

public class EmployeeServiceTests : IClassFixture<EmployeeServiceFixture>
{
    private readonly EmployeeServiceFixture _fixture;
    private readonly Mock<IEmployeeRepository> _employeeRepositoryMock;
    private readonly IEmployeeService _service;

    public EmployeeServiceTests(EmployeeServiceFixture fixture)
    {
        _fixture = fixture;
        _employeeRepositoryMock = new Mock<IEmployeeRepository>();
        _service = new EmployeeService(_employeeRepositoryMock.Object);
    }


    [Fact]
    public void FindAllJuniors_WhenFetchesJuniors_ThenReturnsJuniors()
    {
        // Prepare
        _employeeRepositoryMock.Setup(r => r.Find(It.IsAny<Func<Employee, bool>>())).Returns(_fixture.Juniors);

        // Act
        var juniors = _service.FindAllJuniors();

        // Assert
        juniors.Should().BeEquivalentTo(EmployeeMapper.ToModels(_fixture.Juniors));

        _employeeRepositoryMock.Verify(r => r.Find(It.IsAny<Func<Employee, bool>>()), Times.Once);
    }

    [Fact]
    public void FindAllJuniors_WhenThrowsRepositoryException_ThenReturnsEmptyList()
    {
        // Prepare
        _employeeRepositoryMock.Setup(r => r.Find(It.IsAny<Func<Employee, bool>>())).Throws(new Exception());

        // Act
        var juniors = _service.FindAllJuniors();

        // Assert
        juniors.Should().BeEmpty();

        _employeeRepositoryMock.Verify(r => r.Find(It.IsAny<Func<Employee, bool>>()), Times.Once);
    }

    [Fact]
    public void FindAllTeamLeads_WhenFetchesTeamLeads_ThenReturnsTeamLeads()
    {
        // Prepare
        _employeeRepositoryMock.Setup(r => r.Find(It.IsAny<Func<Employee, bool>>())).Returns(_fixture.TeamLeads);

        // Act
        var teamLeads = _service.FindAllTeamLeads();

        // Assert
        teamLeads.Should().BeEquivalentTo(EmployeeMapper.ToModels(_fixture.TeamLeads));

        _employeeRepositoryMock.Verify(r => r.Find(It.IsAny<Func<Employee, bool>>()), Times.Once);
    }

    [Fact]
    public void FindAllTeamLeads_WhenThrowsRepositoryException_ThenReturnsEmptyList()
    {
        // Prepare
        _employeeRepositoryMock.Setup(r => r.Find(It.IsAny<Func<Employee, bool>>())).Throws(new Exception());

        // Act
        var teamLeads = _service.FindAllTeamLeads();

        // Assert
        teamLeads.Should().BeEmpty();

        _employeeRepositoryMock.Verify(r => r.Find(It.IsAny<Func<Employee, bool>>()), Times.Once);
    }
}

public class EmployeeServiceFixture
{
    public List<Employee> Juniors { get; set; }
    public List<Employee> TeamLeads { get; set; }

    public EmployeeServiceFixture()
    {
        Juniors = new List<Employee>
            { new Employee { Id = 1, Name = "Junior1" }, new Employee { Id = 2, Name = "Junior2" } };
        TeamLeads = new List<Employee>
            { new Employee { Id = 3, Name = "TeamLead1" }, new Employee { Id = 4, Name = "TeamLead2" } };
    }

    private string CreateCsvFile(string fileName, List<Employee> employees)
    {
        var lines = new List<string> { "Id;Name" };
        employees.Select(e => $"{e.Id};{e.Name}").ToList().ForEach(lines.Add);
        
        var path = Path.Combine(Path.GetTempPath(), fileName);
        File.WriteAllLines(path, lines);
        
        return path;
    }
}