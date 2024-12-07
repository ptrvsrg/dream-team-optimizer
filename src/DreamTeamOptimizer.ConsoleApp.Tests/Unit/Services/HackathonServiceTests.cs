using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.ConsoleApp.Services;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Xunit;
using Hackathon = DreamTeamOptimizer.Core.Persistence.Entities.Hackathon;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Unit.Services;

public class HackathonServiceTests : IClassFixture<HackathonServiceFixture>
{
    private readonly HackathonServiceFixture _fixture;
    private readonly Mock<IDbContextTransaction> _txMock;
    private readonly Mock<IHackathonRepository> _hackathonRepositoryMock;
    private readonly Mock<IEmployeeService> _employeeServiceMock;
    private readonly Mock<IWishListService> _wishListServiceMock;
    private readonly Mock<IHrManagerService> _hrManagerServiceMock;
    private readonly Mock<IHrDirectorService> _hrDirectorServiceMock;
    private readonly IHackathonService _hackathonService;

    public HackathonServiceTests(HackathonServiceFixture fixture)
    {
        _fixture = fixture;
        _txMock = new Mock<IDbContextTransaction>();
        _hackathonRepositoryMock = new Mock<IHackathonRepository>();
        _employeeServiceMock = new Mock<IEmployeeService>();
        _wishListServiceMock = new Mock<IWishListService>();
        _hrManagerServiceMock = new Mock<IHrManagerService>();
        _hrDirectorServiceMock = new Mock<IHrDirectorService>();
        
        var dbContextMock = new Mock<AppDbContext>();
        var databaseFacadeMock = new Mock<DatabaseFacade>(dbContextMock.Object);
        databaseFacadeMock
            .Setup(db => db.BeginTransaction())
            .Returns(_txMock.Object);
        dbContextMock
            .Setup(s => s.Database)
            .Returns(databaseFacadeMock.Object);

        _hackathonService = new HackathonService(dbContextMock.Object, _hackathonRepositoryMock.Object,
            _employeeServiceMock.Object, _wishListServiceMock.Object, _hrManagerServiceMock.Object,
            _hrDirectorServiceMock.Object);
    }

    [Fact]
    public void Conduct_ThrowsException_WhenSaveNewHackathonInDB()
    {
        // Prepare
        _employeeServiceMock
            .Setup(s => s.FindAllJuniors())
            .Returns(new List<Employee>());
        _employeeServiceMock
            .Setup(s => s.FindAllTeamLeads())
            .Returns(new List<Employee>());
        _hackathonRepositoryMock
            .Setup(s => s.Create(It.IsAny<Hackathon>()))
            .Throws(new Exception("Test exception"));

        // Act & Assert
        Assert.Throws<Exception>(() => _hackathonService.Conduct());
        
        _employeeServiceMock.Verify(s => s.FindAllJuniors(), Times.Once);
        _employeeServiceMock.Verify(s => s.FindAllTeamLeads(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Create(It.IsAny<Hackathon>()), Times.Once);
    }

    [Fact]
    public void Conduct_ThrowsException_WhenGenerateJuniorsWishLists()
    {
        // Prepare
        _employeeServiceMock
            .Setup(s => s.FindAllJuniors())
            .Returns(_fixture.Juniors);
        _employeeServiceMock
            .Setup(s => s.FindAllTeamLeads())
            .Returns(_fixture.TeamLeads);
        _hackathonRepositoryMock
            .Setup(s => s.Create(It.IsAny<Hackathon>()));
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()))
            .Throws(new Exception("Test exception"));
        _txMock
            .Setup(s => s.Rollback());
        _hackathonRepositoryMock
            .Setup(s => s.Update(It.IsAny<Hackathon>()));

        // Act & Assert
        Assert.Throws<Exception>(() => _hackathonService.Conduct());
        
        _employeeServiceMock.Verify(s => s.FindAllJuniors(), Times.Once);
        _employeeServiceMock.Verify(s => s.FindAllTeamLeads(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Create(It.IsAny<Hackathon>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()), Times.Once);
        _txMock.Verify(s => s.Rollback(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Update(It.IsAny<Hackathon>()), Times.Once);
    }

    [Fact]
    public void Conduct_ThrowsException_WhenGenerateTeamLeadsWishLists()
    {
        // Prepare
        _employeeServiceMock
            .Setup(s => s.FindAllJuniors())
            .Returns(_fixture.Juniors);
        _employeeServiceMock
            .Setup(s => s.FindAllTeamLeads())
            .Returns(_fixture.TeamLeads);
        _hackathonRepositoryMock
            .Setup(s => s.Create(It.IsAny<Hackathon>()));
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()))
            .Returns(new List<WishList>());
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()))
            .Throws(new Exception("Test exception"));
        _txMock
            .Setup(s => s.Rollback());
        _hackathonRepositoryMock
            .Setup(s => s.Update(It.IsAny<Hackathon>()));

        // Act & Assert
        Assert.Throws<Exception>(() => _hackathonService.Conduct());
        
        _employeeServiceMock.Verify(s => s.FindAllJuniors(), Times.Once);
        _employeeServiceMock.Verify(s => s.FindAllTeamLeads(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Create(It.IsAny<Hackathon>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()), Times.Once);
        _txMock.Verify(s => s.Rollback(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Update(It.IsAny<Hackathon>()), Times.Once);
    }

    [Fact]
    public void Conduct_ThrowsException_WhenBuildTeams()
    {
        // Prepare
        _employeeServiceMock
            .Setup(s => s.FindAllJuniors())
            .Returns(_fixture.Juniors);
        _employeeServiceMock
            .Setup(s => s.FindAllTeamLeads())
            .Returns(_fixture.TeamLeads);
        _hackathonRepositoryMock
            .Setup(s => s.Create(It.IsAny<Hackathon>()));
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()))
            .Returns(_fixture.JuniorsWishlists);
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()))
            .Returns(_fixture.TeamLeadsWishlists);
        _hrManagerServiceMock
            .Setup(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()))
            .Throws(new Exception("Test exception"));
        _txMock
            .Setup(s => s.Rollback());
        _hackathonRepositoryMock
            .Setup(s => s.Update(It.IsAny<Hackathon>()));

        // Act & Assert
        Assert.Throws<Exception>(() => _hackathonService.Conduct());
        
        _employeeServiceMock.Verify(s => s.FindAllJuniors(), Times.Once);
        _employeeServiceMock.Verify(s => s.FindAllTeamLeads(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Create(It.IsAny<Hackathon>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()), Times.Once);
        _hrManagerServiceMock.Verify(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()), Times.Once);
        _txMock.Verify(s => s.Rollback(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Update(It.IsAny<Hackathon>()), Times.Once);
    }

    [Fact]
    public void Conduct_ThrowsException_WhenCalculateSatisfactions()
    {
        // Prepare
        _employeeServiceMock
            .Setup(s => s.FindAllJuniors())
            .Returns(_fixture.Juniors);
        _employeeServiceMock
            .Setup(s => s.FindAllTeamLeads())
            .Returns(_fixture.TeamLeads);
        _hackathonRepositoryMock
            .Setup(s => s.Create(It.IsAny<Hackathon>()));
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()))
            .Returns(_fixture.JuniorsWishlists);
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()))
            .Returns(_fixture.TeamLeadsWishlists);
        _hrManagerServiceMock
            .Setup(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()))
            .Returns(_fixture.Teams);
        _hrDirectorServiceMock
            .Setup(s => s.CalculateSatisfactions(_fixture.Teams, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()))
            .Throws(new Exception("Test exception"));
        _txMock
            .Setup(s => s.Rollback());
        _hackathonRepositoryMock
            .Setup(s => s.Update(It.IsAny<Hackathon>()));

        // Act & Assert
        Assert.Throws<Exception>(() => _hackathonService.Conduct());
        
        _employeeServiceMock.Verify(s => s.FindAllJuniors(), Times.Once);
        _employeeServiceMock.Verify(s => s.FindAllTeamLeads(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Create(It.IsAny<Hackathon>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()), Times.Once);
        _hrManagerServiceMock.Verify(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()), Times.Once);
        _hrDirectorServiceMock.Verify(s => s.CalculateSatisfactions(_fixture.Teams, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()), Times.Once);
        _txMock.Verify(s => s.Rollback(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Update(It.IsAny<Hackathon>()), Times.Once);
    }

    [Fact]
    public void Conduct_ThrowsException_WhenUpdateHackathon()
    {
        // Prepare
        _employeeServiceMock
            .Setup(s => s.FindAllJuniors())
            .Returns(_fixture.Juniors);
        _employeeServiceMock
            .Setup(s => s.FindAllTeamLeads())
            .Returns(_fixture.TeamLeads);
        _hackathonRepositoryMock
            .Setup(s => s.Create(It.IsAny<Hackathon>()));
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()))
            .Returns(_fixture.JuniorsWishlists);
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()))
            .Returns(_fixture.TeamLeadsWishlists);
        _hrManagerServiceMock
            .Setup(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()))
            .Returns(_fixture.Teams);
        _hrDirectorServiceMock
            .Setup(s => s.CalculateSatisfactions(_fixture.Teams, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()))
            .Returns(_fixture.Satisfactions);
        _hackathonRepositoryMock
            .Setup(s => s.Update(It.IsAny<Hackathon>()))
            .Throws(new Exception("Test exception"));
        _txMock
            .Setup(s => s.Rollback());

        // Act & Assert
        Assert.Throws<Exception>(() => _hackathonService.Conduct());
        
        _employeeServiceMock.Verify(s => s.FindAllJuniors(), Times.Once);
        _employeeServiceMock.Verify(s => s.FindAllTeamLeads(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Create(It.IsAny<Hackathon>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()), Times.Once);
        _hrManagerServiceMock.Verify(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()), Times.Once);
        _hrDirectorServiceMock.Verify(s => s.CalculateSatisfactions(_fixture.Teams, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Update(It.IsAny<Hackathon>()), Times.Exactly(2));
        _txMock.Verify(s => s.Rollback(), Times.Once);
    }

    [Fact]
    public void Conduct_ThrowsException_WhenCommitTransaction()
    {
        // Prepare
        _employeeServiceMock
            .Setup(s => s.FindAllJuniors())
            .Returns(_fixture.Juniors);
        _employeeServiceMock
            .Setup(s => s.FindAllTeamLeads())
            .Returns(_fixture.TeamLeads);
        _hackathonRepositoryMock
            .Setup(s => s.Create(It.IsAny<Hackathon>()));
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()))
            .Returns(_fixture.JuniorsWishlists);
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()))
            .Returns(_fixture.TeamLeadsWishlists);
        _hrManagerServiceMock
            .Setup(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()))
            .Returns(_fixture.Teams);
        _hrDirectorServiceMock
            .Setup(s => s.CalculateSatisfactions(_fixture.Teams, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()))
            .Returns(_fixture.Satisfactions);
        _hackathonRepositoryMock
            .Setup(s => s.Update(It.IsAny<Hackathon>()));
        _txMock
            .Setup(s => s.Commit())
            .Throws(new Exception("Test exception"));

        // Act & Assert
        Assert.Throws<Exception>(() => _hackathonService.Conduct());
        
        _employeeServiceMock.Verify(s => s.FindAllJuniors(), Times.Once);
        _employeeServiceMock.Verify(s => s.FindAllTeamLeads(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Create(It.IsAny<Hackathon>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()), Times.Once);
        _hrManagerServiceMock.Verify(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()), Times.Once);
        _hrDirectorServiceMock.Verify(s => s.CalculateSatisfactions(_fixture.Teams, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Update(It.IsAny<Hackathon>()), Times.Once);
        _txMock.Verify(s => s.Commit(), Times.Once);
    }
    
    [Fact]
    public void Conduct_ReturnCompletedHackathon()
    {
        // Prepare
        var hackathonEntity = new Hackathon();
        
        _employeeServiceMock
            .Setup(s => s.FindAllJuniors())
            .Returns(_fixture.Juniors);
        _employeeServiceMock
            .Setup(s => s.FindAllTeamLeads())
            .Returns(_fixture.TeamLeads);
        _hackathonRepositoryMock
            .Setup(s => s.Create(It.IsAny<Hackathon>()))
            .Callback((Hackathon h) =>
            {
                hackathonEntity = h;
            });
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()))
            .Returns(_fixture.JuniorsWishlists);
        _wishListServiceMock
            .Setup(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()))
            .Returns(_fixture.TeamLeadsWishlists);
        _hrManagerServiceMock
            .Setup(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()))
            .Returns(_fixture.Teams);
        _hrDirectorServiceMock
            .Setup(s => s.CalculateSatisfactions(_fixture.Teams, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()))
            .Returns(_fixture.Satisfactions);
        _hackathonRepositoryMock
            .Setup(s => s.Update(It.IsAny<Hackathon>()))
            .Callback((Hackathon h) =>
            {
                hackathonEntity = h;
            });
        _txMock
            .Setup(s => s.Commit());
        _hackathonRepositoryMock
            .Setup(s => s.FindById(It.IsAny<int>()))
            .Returns(() => hackathonEntity);

        // Act
        var hackathon = _hackathonService.Conduct();

        // Assert
        Assert.NotNull(hackathon);
        Assert.Equal(_fixture.Result, hackathon.Result);
        
        _employeeServiceMock.Verify(s => s.FindAllJuniors(), Times.Once);
        _employeeServiceMock.Verify(s => s.FindAllTeamLeads(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Create(It.IsAny<Hackathon>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.Juniors, _fixture.TeamLeads, It.IsAny<int>()), Times.Once);
        _wishListServiceMock.Verify(s => s.GenerateWishlists(_fixture.TeamLeads, _fixture.Juniors, It.IsAny<int>()), Times.Once);
        _hrManagerServiceMock.Verify(s => s.BuildTeams(_fixture.TeamLeads, _fixture.Juniors, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()), Times.Once);
        _hrDirectorServiceMock.Verify(s => s.CalculateSatisfactions(_fixture.Teams, _fixture.TeamLeadsWishlists, _fixture.JuniorsWishlists, It.IsAny<int>()), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.Update(It.IsAny<Hackathon>()), Times.Once);
        _txMock.Verify(s => s.Commit(), Times.Once);
        _hackathonRepositoryMock.Verify(s => s.FindById(It.IsAny<int>()), Times.Once());
    }

    [Fact]
    public void CalculateAverageHarmonicity_ReturnsCorrectValue()
    {
        // Prepare
        var expectedAverage = 4.5;
        _hackathonRepositoryMock
            .Setup(s => s.FindAverageResult())
            .Returns(expectedAverage);

        // Act
        var result = _hackathonService.CalculateAverageHarmonicity();

        // Assert
        Assert.Equal(expectedAverage, result);
        _hackathonRepositoryMock.Verify(s => s.FindAverageResult(), Times.Once);
    }

    [Fact]
    public void FindById_ReturnsHackathon_WhenHackathonExists()
    {
        // Prepare
        var hackathonId = 1;
        var hackathonEntity = new Hackathon
        {
            Id = hackathonId,
            Status = Core.Persistence.Entities.HackathonStatus.COMPLETED,
            Result = 5.0
        };
        _hackathonRepositoryMock
            .Setup(s => s.FindById(hackathonId))
            .Returns(hackathonEntity);

        // Act
        var result = _hackathonService.FindById(hackathonId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(hackathonId, result.Id);
        Assert.Equal(HackathonStatus.COMPLETED, result.Status);
        Assert.Equal(5.0, result.Result);
        _hackathonRepositoryMock.Verify(s => s.FindById(hackathonId), Times.Once);
    }

    [Fact]
    public void FindById_ReturnsNull_WhenHackathonDoesNotExist()
    {
        // Prepare
        var hackathonId = 1;
        _hackathonRepositoryMock
            .Setup(s => s.FindById(hackathonId))
            .Returns((Hackathon?)null);

        // Act
        var result = _hackathonService.FindById(hackathonId);

        // Assert
        Assert.Null(result);
        _hackathonRepositoryMock.Verify(s => s.FindById(hackathonId), Times.Once);
    }
}

public class HackathonServiceFixture
{
    public List<Employee> Juniors { get; set; }
    public List<Employee> TeamLeads { get; set; }
    public List<WishList> JuniorsWishlists { get; set; }
    public List<WishList> TeamLeadsWishlists { get; set; }
    public List<Team> Teams { get; set; }
    public List<Satisfaction> Satisfactions { get; set; }
    public double Result { get; set; }
    
    public HackathonServiceFixture()
    {
        Juniors = new List<Employee> { new(1, "Junior1", Grade.JUNIOR) };
        TeamLeads = new List<Employee> { new(2, "TeamLead1", Grade.TEAM_LEAD) };
        JuniorsWishlists = new List<WishList> { new(1, [2]) };
        TeamLeadsWishlists = new List<WishList> { new(2, [1]) };
        Teams = new List<Team> { new(TeamLeads[0], Juniors[0]) };
        Satisfactions = new List<Satisfaction>
        {
            new(1, 3.0),
            new(2, 3.0)
        };
        Result = 3.0;
    }
}