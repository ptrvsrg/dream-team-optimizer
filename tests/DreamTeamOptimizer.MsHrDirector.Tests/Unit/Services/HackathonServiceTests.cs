using System.Net;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;
using DreamTeamOptimizer.MsHrDirector.Services;
using DreamTeamOptimizer.MsHrDirector.Services.Mappers;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Grade = DreamTeamOptimizer.Core.Persistence.Entities.Grade;
using HackathonStatusEntity = DreamTeamOptimizer.Core.Persistence.Entities.HackathonStatus;
using HackathonEntity = DreamTeamOptimizer.Core.Persistence.Entities.Hackathon;

namespace DreamTeamOptimizer.MsHrDirector.Tests.Unit.Services;

public class HackathonServiceTests(HackathonServiceFixture fixture) : IClassFixture<HackathonServiceFixture>, IDisposable
{
    public void Dispose()
    {
        fixture.ResetMocks();
    }

    [Fact]
    public void Create_ShouldReturnHackathon_WhenSuccessful()
    {
        // Arrange
        var hackathon = new HackathonEntity
        {
            Id = 1,
            Status = HackathonStatusEntity.IN_PROCESSING,
            Result = 0
        };

        fixture.HackathonRepositoryMock.Setup(r => r.Create(It.IsAny<HackathonEntity>())).Verifiable();
        fixture.HackathonRepositoryMock.Setup(r => r.FindById(It.IsAny<int>())).Returns(hackathon);
        fixture.HackathonStartedPublisherMock.Setup(c => c.StartHackathon(It.IsAny<int>())).Verifiable();

        // Act
        var result = fixture.HackathonService.Create();

        // Assert
        result.Should().NotBeNull();
        fixture.HackathonRepositoryMock.Verify(r => r.Create(It.IsAny<HackathonEntity>()), Times.Once);
        fixture.HackathonStartedPublisherMock.Verify(c => c.StartHackathon(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public void Create_ShouldThrowException_WhenConductHackathonFails()
    {
        // Arrange
        fixture.HackathonRepositoryMock.Setup(r => r.Create(It.IsAny<HackathonEntity>())).Verifiable();
        fixture.HackathonStartedPublisherMock.Setup(c => c.StartHackathon(It.IsAny<int>())).Throws<Exception>();

        // Act
        var act = () => fixture.HackathonService.Create();

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void SaveResult_ShouldSaveResult_WhenHackathonIsFoundAndSessionExists()
    {
        // Arrange
        var hackathon = new HackathonEntity { Id = 1, Status = HackathonStatusEntity.IN_PROCESSING, Result = 0 };
        var result = new HackathonResultEvent(
            1, new List<WishList>(), new List<WishList>(), new List<Team>()
        );
        var satisfactions = new List<Satisfaction> { new(1, 1) };

        fixture.HackathonRepositoryMock.Setup(r => r.FindById(It.IsAny<int>())).Returns(hackathon);
        fixture.HackathonRepositoryMock.Setup(r => r.Update(It.IsAny<HackathonEntity>())).Verifiable();
        fixture.SatisfactionServiceMock
            .Setup(s => s.Evaluate(It.IsAny<List<Team>>(), It.IsAny<List<WishList>>(), It.IsAny<List<WishList>>()))
            .Returns(satisfactions);
    }

    [Fact]
    public void CalculateAverageHarmonicity_ShouldReturnAverage_WhenDataExists()
    {
        // Arrange
        var averageResult = 5.5; // пример среднего результата
        fixture.HackathonRepositoryMock.Setup(r => r.FindAverageResult()).Returns(averageResult);

        // Act
        var result = fixture.HackathonService.CalculateAverageHarmonicity();

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().Be(averageResult);
    }

    [Fact]
    public void CalculateAverageHarmonicity_ShouldThrowException_WhenNoDataExists()
    {
        // Arrange
        fixture.HackathonRepositoryMock.Setup(r => r.FindAverageResult()).Throws<Exception>();

        // Act
        var act = () => fixture.HackathonService.CalculateAverageHarmonicity();

        // Assert
        act.Should().Throw<Exception>();
    }

    [Fact]
    public void GetById_ShouldReturnHackathon_WhenFoundCompleted()
    {
        // Arrange
        var hackathonId = 1;
        var hackathonEntity = new HackathonEntity
        {
            Id = hackathonId,
            Status = HackathonStatusEntity.COMPLETED,
            Employees = new List<Core.Persistence.Entities.Employee>
            {
                new() { Id = 1, Name = "Junior", Grade = Grade.JUNIOR },
                new() { Id = 2, Name = "Teamlead", Grade = Grade.TEAM_LEAD }
            },
            WishLists = new List<Core.Persistence.Entities.WishList>
            {
                new() { EmployeeId = 1, DesiredEmployeeIds = [2] },
                new() { EmployeeId = 2, DesiredEmployeeIds = [1] }
            },
            Teams = new List<Core.Persistence.Entities.Team>
            {
                new() { TeamLead = new() { Id = 1 }, Junior = new() { Id = 2 } }
            },
            Satisfactions = new List<Core.Persistence.Entities.Satisfaction>
            {
                new() { Employee = new() { Id = 1 }, Rank = 1 },
                new() { Employee = new() { Id = 2 }, Rank = 1 }
            },
            Result = 1.0
        };

        fixture.HackathonRepositoryMock.Setup(r => r.FindById(hackathonId)).Returns(hackathonEntity);

        // Act
        var result = fixture.HackathonService.GetById(hackathonId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(HackathonMapper.ToModel(hackathonEntity));
        
        fixture.HackathonRepositoryMock.Verify(r => r.FindById(hackathonId), Times.Once);
    }

    [Theory]
    [InlineData(HackathonStatusEntity.IN_PROCESSING)]
    [InlineData(HackathonStatusEntity.FAILED)]
    public void GetById_ShouldReturnHackathon_WhenFoundNotCompleted(HackathonStatusEntity status)
    {
        // Arrange
        var hackathonId = 1;
        var hackathonEntity = new HackathonEntity
        {
            Id = hackathonId,
            Status = status,
            Employees = new List<Core.Persistence.Entities.Employee>
            {
                new() { Id = 1, Name = "Junior", Grade = Grade.JUNIOR },
                new() { Id = 2, Name = "Teamlead", Grade = Grade.TEAM_LEAD }
            },
            WishLists = new List<Core.Persistence.Entities.WishList>
            {
                new() { EmployeeId = 1, DesiredEmployeeIds = [2] },
                new() { EmployeeId = 2, DesiredEmployeeIds = [1] }
            },
            Teams = new List<Core.Persistence.Entities.Team>
            {
                new() { TeamLead = new() { Id = 1 }, Junior = new() { Id = 2 } }
            },
            Satisfactions = new List<Core.Persistence.Entities.Satisfaction>
            {
                new() { Employee = new() { Id = 1 }, Rank = 1 },
                new() { Employee = new() { Id = 2 }, Rank = 1 }
            },
            Result = 1.0
        };

        fixture.HackathonRepositoryMock.Setup(r => r.FindById(hackathonId)).Returns(hackathonEntity);

        // Act
        var result = fixture.HackathonService.GetById(hackathonId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(hackathonEntity.Id);
        result.Status.Should().Be(HackathonMapper.ToModelStatus(status));
        result.Employees.Should().BeEquivalentTo(EmployeeMapper.ToModels(hackathonEntity.Employees.ToList()));
        result.WishLists.Should().BeEquivalentTo(new List<WishList>());
        result.Teams.Should().BeEquivalentTo(new List<Team>());
        result.Satisfactions.Should().BeEquivalentTo(new List<Satisfaction>());
        result.Result.Should().Be(0.0);
        
        fixture.HackathonRepositoryMock.Verify(r => r.FindById(hackathonId), Times.Once);
    }

    [Fact]
    public void GetById_ShouldThrowNotFoundException_WhenHackathonNotFound()
    {
        // Arrange
        var hackathonId = 999; // Пример несуществующего ID
        fixture.HackathonRepositoryMock.Setup(r => r.FindById(hackathonId)).Returns((HackathonEntity)null);

        // Act
        var act = () => fixture.HackathonService.GetById(hackathonId);

        // Assert
        act.Should().Throw<HttpStatusException>()
            .WithMessage($"No hackathon #{hackathonId} found")
            .And.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

public class HackathonServiceFixture
{
    public Mock<ILogger<HackathonService>> LoggerMock { get; }
    public Mock<IServiceScopeFactory> ServiceScopeFactoryMock { get; }
    public IMemoryCache MemoryCache { get; }
    public Mock<IHackathonRepository> HackathonRepositoryMock { get; }
    public Mock<IHackathonStartedPublisher> HackathonStartedPublisherMock { get; }
    public Mock<ISatisfactionService> SatisfactionServiceMock { get; }
    public HackathonService HackathonService { get; }

    public HackathonServiceFixture()
    {
        LoggerMock = new Mock<ILogger<HackathonService>>();
        ServiceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        MemoryCache = new MemoryCache(new MemoryCacheOptions());
        HackathonRepositoryMock = new Mock<IHackathonRepository>();
        HackathonStartedPublisherMock = new Mock<IHackathonStartedPublisher>();
        SatisfactionServiceMock = new Mock<ISatisfactionService>();

        HackathonService = new HackathonService(
            LoggerMock.Object,
            ServiceScopeFactoryMock.Object,
            MemoryCache,
            HackathonRepositoryMock.Object,
            HackathonStartedPublisherMock.Object,
            SatisfactionServiceMock.Object
        );
    }

    public void ResetMocks()
    {
        LoggerMock.Reset();
        ServiceScopeFactoryMock.Reset();
        HackathonRepositoryMock.Reset();
        HackathonStartedPublisherMock.Reset();
        SatisfactionServiceMock.Reset();
    }
}