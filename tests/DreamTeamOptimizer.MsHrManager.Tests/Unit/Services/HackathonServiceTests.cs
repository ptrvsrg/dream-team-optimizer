using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.MsHrManager.Interfaces.Clients;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using DreamTeamOptimizer.MsHrManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Employee = DreamTeamOptimizer.Core.Models.Employee;
using Grade = DreamTeamOptimizer.Core.Models.Grade;
using Team = DreamTeamOptimizer.Core.Models.Team;
using WishList = DreamTeamOptimizer.Core.Models.WishList;

namespace DreamTeamOptimizer.MsHrManager.Tests.Unit.Services;

public class HackathonServiceTests : IClassFixture<HackathonServiceFixture>, IDisposable
{
    private readonly HackathonServiceFixture _fixture;

    public HackathonServiceTests(HackathonServiceFixture fixture)
    {
        _fixture = fixture;
    }

    public void Dispose()
    {
        _fixture.ResetMocks();
    }

    [Fact]
    public void Conduct_ShouldStartHackathonTask_WhenCalled()
    {
        // Arrange
        const int hackathonId = 123;

        // Act
        _fixture.HackathonService.Conduct(hackathonId);

        // Assert
        _fixture.LoggerMock.Verify(
            log => log.Log(
                It.Is<LogLevel>(level => level == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((msg, t) => msg.ToString().Contains($"conduct hackathon #{hackathonId}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _fixture.EmployeeServiceMock.Verify(es => es.FindAllJuniors(), Times.Once);
        _fixture.EmployeeServiceMock.Verify(es => es.FindAllTeamLeads(), Times.Once);
        _fixture.HackathonEmployeeRepositoryMock.Verify(her => her.CreateAll(It.IsAny<List<HackathonEmployee>>()),
            Times.Once);
        _fixture.WishListServiceMock.Verify(
            ws => ws.Vote(It.IsAny<List<Employee>>(), It.IsAny<List<Employee>>(), It.IsAny<int>()), Times.Exactly(2));
        _fixture.StrategyServiceMock.Verify(ss => ss.BuildTeams(It.IsAny<List<Employee>>(), It.IsAny<List<Employee>>(),
            It.IsAny<List<WishList>>(), It.IsAny<List<WishList>>(), It.IsAny<int>()), Times.Once);
        _fixture.HrDirectorClientMock.Verify(hr => hr.SaveResult(It.IsAny<HackathonResult>(), It.IsAny<int>()),
            Times.Once);
    }
}

public class HackathonServiceFixture : IDisposable
{
    public Mock<ILogger<HackathonService>> LoggerMock { get; }
    public Mock<IServiceScopeFactory> ServiceScopeFactoryMock { get; }
    public Mock<IServiceScope> ServiceScopeMock { get; }
    public Mock<IServiceProvider> ScopeServiceProviderMock { get; }
    public Mock<IEmployeeService> EmployeeServiceMock { get; }
    public Mock<IWishListService> WishListServiceMock { get; }
    public Mock<IStrategyService> StrategyServiceMock { get; }
    public Mock<IHrDirectorClient> HrDirectorClientMock { get; }
    public Mock<IHackathonRepository> HackathonRepositoryMock { get; }
    public Mock<IHackathonEmployeeRepository> HackathonEmployeeRepositoryMock { get; }
    public Mock<AppDbContext> DbContextMock { get; }
    public Mock<IDbContextTransaction> TxMock { get; }
    public HackathonService HackathonService { get; }

    public HackathonServiceFixture()
    {
        LoggerMock = new Mock<ILogger<HackathonService>>();
        ServiceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        ServiceScopeMock = new Mock<IServiceScope>();
        ScopeServiceProviderMock = new Mock<IServiceProvider>();

        EmployeeServiceMock = new Mock<IEmployeeService>();
        WishListServiceMock = new Mock<IWishListService>();
        StrategyServiceMock = new Mock<IStrategyService>();
        HrDirectorClientMock = new Mock<IHrDirectorClient>();
        HackathonRepositoryMock = new Mock<IHackathonRepository>();
        HackathonEmployeeRepositoryMock = new Mock<IHackathonEmployeeRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>().Options;
        DbContextMock = new Mock<AppDbContext>(options);
        TxMock = new Mock<IDbContextTransaction>();

        HackathonService = new HackathonService(LoggerMock.Object, ServiceScopeFactoryMock.Object,
            HackathonRepositoryMock.Object);

        SetupScopedServices();
    }

    public void SetupScopedServices()
    {
        var databaseFacadeMock = new Mock<DatabaseFacade>(DbContextMock.Object);
        databaseFacadeMock
            .Setup(db => db.BeginTransaction())
            .Returns(TxMock.Object);

        DbContextMock.Setup(s => s.Database).Returns(databaseFacadeMock.Object);

        // Настройка возвращаемых значений для методов репозиториев и сервисов

        // Список "младших сотрудников"
        var juniors = new List<Employee>
        {
            new(1, "Junior1", Grade.JUNIOR),
            new(2, "Junior2", Grade.JUNIOR)
        };

        // Список "тимлидов"
        var teamLeads = new List<Employee>
        {
            new(3, "TeamLead1", Grade.TEAM_LEAD),
            new(4, "TeamLead2", Grade.TEAM_LEAD)
        };

        // Список "желаний"
        var wishLists = new List<WishList>
        {
            new(1, [3, 4]),
            new(2, [3, 4]),
            new(3, [1, 2]),
            new(4, [1, 2])
        };

        // Список "команд"
        var teams = new List<Team>
        {
            new(juniors[0].Id, teamLeads[0].Id),
            new(juniors[1].Id, teamLeads[1].Id)
        };

        EmployeeServiceMock
            .Setup(service => service.FindAllJuniors())
            .Returns(juniors);

        EmployeeServiceMock
            .Setup(service => service.FindAllTeamLeads())
            .Returns(teamLeads);

        WishListServiceMock
            .Setup(service => service.Vote(It.IsAny<List<Employee>>(), It.IsAny<List<Employee>>(), It.IsAny<int>()))
            .Returns(wishLists);

        StrategyServiceMock
            .Setup(service => service.BuildTeams(
                It.IsAny<List<Employee>>(),
                It.IsAny<List<Employee>>(),
                It.IsAny<List<WishList>>(),
                It.IsAny<List<WishList>>(),
                It.IsAny<int>()))
            .Returns(teams);

        HrDirectorClientMock
            .Setup(client => client.SaveResult(It.IsAny<HackathonResult>(), It.IsAny<int>()))
            .Verifiable();

        HackathonEmployeeRepositoryMock
            .Setup(repo => repo.CreateAll(It.IsAny<List<HackathonEmployee>>()))
            .Verifiable();

        // Настройка ServiceProvider
        ScopeServiceProviderMock.Setup(s => s.GetService(typeof(AppDbContext))).Returns(DbContextMock.Object);
        ScopeServiceProviderMock.Setup(s => s.GetService(typeof(IEmployeeService))).Returns(EmployeeServiceMock.Object);
        ScopeServiceProviderMock.Setup(s => s.GetService(typeof(IWishListService))).Returns(WishListServiceMock.Object);
        ScopeServiceProviderMock.Setup(s => s.GetService(typeof(IStrategyService))).Returns(StrategyServiceMock.Object);
        ScopeServiceProviderMock.Setup(s => s.GetService(typeof(IHrDirectorClient)))
            .Returns(HrDirectorClientMock.Object);
        ScopeServiceProviderMock.Setup(s => s.GetService(typeof(IHackathonEmployeeRepository)))
            .Returns(HackathonEmployeeRepositoryMock.Object);

        // Настройка ServiceScope
        ServiceScopeMock.Setup(s => s.ServiceProvider).Returns(ScopeServiceProviderMock.Object);
        ServiceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(ServiceScopeMock.Object);
    }


    public void ResetMocks()
    {
        LoggerMock.Reset();
        ServiceScopeFactoryMock.Reset();
        ServiceScopeMock.Reset();
        ScopeServiceProviderMock.Reset();
        EmployeeServiceMock.Reset();
        WishListServiceMock.Reset();
        StrategyServiceMock.Reset();
        HrDirectorClientMock.Reset();
        HackathonEmployeeRepositoryMock.Reset();
        DbContextMock.Reset();
    }

    public void Dispose()
    {
    }
}