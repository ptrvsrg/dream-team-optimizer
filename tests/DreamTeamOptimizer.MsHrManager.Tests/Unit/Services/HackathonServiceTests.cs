using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using DreamTeamOptimizer.MsHrManager.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;

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
        _fixture.SetupMocks();
    }

    [Fact]
    public void StartHackathon_ShouldStartHackathonTask_WhenCalled()
    {
        // Act
        _fixture.HackathonService.StartHackathon(_fixture.HackathonId);

        // Assert
        _fixture.LoggerMock.Verify(
            log => log.Log(
                It.Is<LogLevel>(level => level == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((msg, t) => msg.ToString().Contains($"conduct hackathon #{_fixture.HackathonId}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _fixture.EmployeeServiceMock.Verify(es => es.FindAllJuniors(), Times.Once);
        _fixture.EmployeeServiceMock.Verify(es => es.FindAllTeamLeads(), Times.Once);
        _fixture.HackathonEmployeeRepositoryMock.Verify(her => her.CreateAll(It.IsAny<List<HackathonEmployee>>()),
            Times.Once);
        _fixture.WishListServiceMock.Verify(
            ws => ws.StartVoting(It.IsAny<List<Core.Models.Employee>>(), It.IsAny<List<Core.Models.Employee>>(),
                It.IsAny<int>()),
            Times.Once);
        _fixture.TxMock.Verify(ws => ws.Commit(), Times.Once);
        _fixture.TxMock.Verify(ws => ws.Rollback(), Times.Never);
    }

    [Fact]
    public void StartHackathon_ShouldStartHackathonTask_WhenDBThrowsException()
    {
        // Arrange
        _fixture.HackathonEmployeeRepositoryMock
            .Setup(r => r.CreateAll(It.IsAny<List<HackathonEmployee>>()))
            .Throws(new DbUpdateException());

        // Act
        _fixture.HackathonService.StartHackathon(_fixture.HackathonId);

        // Assert
        _fixture.LoggerMock.Verify(
            log => log.Log(
                It.Is<LogLevel>(level => level == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((msg, t) => msg.ToString().Contains($"conduct hackathon #{_fixture.HackathonId}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _fixture.EmployeeServiceMock.Verify(es => es.FindAllJuniors(), Times.Once);
        _fixture.EmployeeServiceMock.Verify(es => es.FindAllTeamLeads(), Times.Once);
        _fixture.HackathonEmployeeRepositoryMock.Verify(her => her.CreateAll(It.IsAny<List<HackathonEmployee>>()),
            Times.Once);
        _fixture.TxMock.Verify(ws => ws.Commit(), Times.Never);
        _fixture.TxMock.Verify(ws => ws.Rollback(), Times.Once);
    }

    [Fact]
    public void CompleteHackathon_ShouldCompleteHackathonTask_WhenCalled()
    {
        // Act
        _fixture.HackathonService.CompleteHackathon(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists,
            _fixture.JuniorsWishlists, _fixture.HackathonId);

        // Assert
        _fixture.LoggerMock.Verify(
            log => log.Log(
                It.Is<LogLevel>(level => level == LogLevel.Information),
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((msg, t) => msg.ToString().Contains($"complete hackathon #{_fixture.HackathonId}")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
        _fixture.WishListRepositoryMock.Verify(wr => wr.CreateAll(It.IsAny<List<WishList>>()), Times.Once);
        _fixture.StrategyServiceMock.Verify(ss => ss.BuildTeams(_fixture.TeamLeads, _fixture.Juniors,
            _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, _fixture.HackathonId), Times.Once);
        _fixture.HackathonResultPublisherMock.Verify(hrp => hrp.SaveResult(It.IsAny<HackathonResultEvent>()),
            Times.Once);
    }
}

public class HackathonServiceFixture : IDisposable
{
    public Mock<ILogger<HackathonService>> LoggerMock { get; }
    public Mock<IWishListRepository> WishListRepositoryMock { get; }
    public Mock<IHackathonEmployeeRepository> HackathonEmployeeRepositoryMock { get; }
    public Mock<IHackathonResultPublisher> HackathonResultPublisherMock { get; }
    public Mock<IServiceProvider> ScopeServiceProviderMock { get; }
    public Mock<IEmployeeService> EmployeeServiceMock { get; }
    public Mock<IWishListService> WishListServiceMock { get; }
    public Mock<IStrategyService> StrategyServiceMock { get; }
    public Mock<AppDbContext> DbContextMock { get; }
    public Mock<IDbContextTransaction> TxMock { get; }
    public HackathonService HackathonService { get; }

    public int HackathonId { get; }
    public List<Core.Models.Employee> TeamLeads { get; }
    public List<Core.Models.Employee> Juniors { get; }
    public List<Core.Models.WishList> TeamLeadsWishlists { get; }
    public List<Core.Models.WishList> JuniorsWishlists { get; }

    public HackathonServiceFixture()
    {
        LoggerMock = new Mock<ILogger<HackathonService>>();
        ScopeServiceProviderMock = new Mock<IServiceProvider>();

        EmployeeServiceMock = new Mock<IEmployeeService>();
        WishListServiceMock = new Mock<IWishListService>();
        StrategyServiceMock = new Mock<IStrategyService>();
        HackathonResultPublisherMock = new Mock<IHackathonResultPublisher>();
        WishListRepositoryMock = new Mock<IWishListRepository>();
        HackathonEmployeeRepositoryMock = new Mock<IHackathonEmployeeRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>().Options;
        DbContextMock = new Mock<AppDbContext>(options);
        TxMock = new Mock<IDbContextTransaction>();

        HackathonService = new HackathonService(
            LoggerMock.Object,
            DbContextMock.Object,
            WishListRepositoryMock.Object,
            HackathonEmployeeRepositoryMock.Object,
            HackathonResultPublisherMock.Object,
            EmployeeServiceMock.Object,
            WishListServiceMock.Object,
            StrategyServiceMock.Object);

        Juniors = new List<Core.Models.Employee>
        {
            new(1, "Junior1", Core.Models.Grade.JUNIOR),
            new(2, "Junior2", Core.Models.Grade.JUNIOR)
        };

        TeamLeads = new List<Core.Models.Employee>
        {
            new(3, "TeamLead1", Core.Models.Grade.TEAM_LEAD),
            new(4, "TeamLead2", Core.Models.Grade.TEAM_LEAD)
        };

        JuniorsWishlists = new List<Core.Models.WishList>
        {
            new(1, [3, 4]),
            new(2, [3, 4])
        };

        TeamLeadsWishlists = new List<Core.Models.WishList>
        {
            new(3, [1, 2]),
            new(4, [1, 2])
        };

        SetupMocks();
    }

    public void SetupMocks()
    {
        var databaseFacadeMock = new Mock<DatabaseFacade>(DbContextMock.Object);
        databaseFacadeMock
            .Setup(db => db.BeginTransaction())
            .Returns(TxMock.Object);

        DbContextMock.Setup(s => s.Database).Returns(databaseFacadeMock.Object);

        // Список "команд"
        var teams = new List<Core.Models.Team>
        {
            new(Juniors[0].Id, TeamLeads[0].Id),
            new(Juniors[1].Id, TeamLeads[1].Id)
        };

        EmployeeServiceMock
            .Setup(service => service.FindAllJuniors())
            .Returns(Juniors);

        EmployeeServiceMock
            .Setup(service => service.FindAllTeamLeads())
            .Returns(TeamLeads);

        WishListServiceMock
            .Setup(service =>
                service.StartVoting(It.IsAny<List<Core.Models.Employee>>(), It.IsAny<List<Core.Models.Employee>>(),
                    It.IsAny<int>()))
            .Verifiable();

        StrategyServiceMock
            .Setup(service => service.BuildTeams(
                It.IsAny<List<Core.Models.Employee>>(),
                It.IsAny<List<Core.Models.Employee>>(),
                It.IsAny<List<Core.Models.WishList>>(),
                It.IsAny<List<Core.Models.WishList>>(),
                It.IsAny<int>()))
            .Returns(teams);

        HackathonResultPublisherMock
            .Setup(client => client.SaveResult(It.IsAny<HackathonResultEvent>()))
            .Verifiable();

        HackathonEmployeeRepositoryMock
            .Setup(repo => repo.CreateAll(It.IsAny<List<HackathonEmployee>>()))
            .Verifiable();
    }


    public void ResetMocks()
    {
        LoggerMock.Reset();
        DbContextMock.Reset();
        TxMock.Reset();
        WishListRepositoryMock.Reset();
        HackathonEmployeeRepositoryMock.Reset();
        HackathonResultPublisherMock.Reset();
        EmployeeServiceMock.Reset();
        WishListServiceMock.Reset();
        StrategyServiceMock.Reset();
    }

    public void Dispose()
    {
    }
}