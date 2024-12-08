using System.Net;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;
using DreamTeamOptimizer.MsHrDirector.Services;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using HackathonStatusEntity = DreamTeamOptimizer.Core.Persistence.Entities.HackathonStatus;
using HackathonEntity = DreamTeamOptimizer.Core.Persistence.Entities.Hackathon;

namespace DreamTeamOptimizer.MsHrDirector.Tests.Unit.Services;

public class HackathonServiceTests(HackathonServiceFixture fixture) : IClassFixture<HackathonServiceFixture>
{
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
        fixture.HrManagerClientMock.Setup(c => c.ConductHackathon(It.IsAny<int>())).Verifiable();

        // Act
        var result = fixture.HackathonService.Create();

        // Assert
        result.Should().NotBeNull();
        fixture.HackathonRepositoryMock.Verify(r => r.Create(It.IsAny<HackathonEntity>()), Times.Once);
        fixture.HrManagerClientMock.Verify(c => c.ConductHackathon(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public void Create_ShouldThrowException_WhenConductHackathonFails()
    {
        // Arrange
        fixture.HackathonRepositoryMock.Setup(r => r.Create(It.IsAny<HackathonEntity>())).Verifiable();
        fixture.HrManagerClientMock.Setup(c => c.ConductHackathon(It.IsAny<int>())).Throws<Exception>();

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
        var result = new HackathonResult(
            new List<WishList>(), new List<WishList>(), new List<Team>()
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
    public void GetById_ShouldReturnHackathon_WhenFound()
    {
        // Arrange
        var hackathonId = 1;
        var hackathonEntity = new HackathonEntity
        {
            Id = hackathonId,
            Status = HackathonStatusEntity.IN_PROCESSING,
            Result = 0
        };

        fixture.HackathonRepositoryMock.Setup(r => r.FindById(hackathonId)).Returns(hackathonEntity);

        // Act
        var result = fixture.HackathonService.GetById(hackathonId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(hackathonId);
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
    public Mock<IServiceProvider> ServiceProviderMock { get; }
    public IMemoryCache MemoryCache { get; }
    public Mock<IHackathonRepository> HackathonRepositoryMock { get; }
    public Mock<IHrManagerClient> HrManagerClientMock { get; }
    public Mock<ISatisfactionService> SatisfactionServiceMock { get; }
    public HackathonService HackathonService { get; }

    public HackathonServiceFixture()
    {
        LoggerMock = new Mock<ILogger<HackathonService>>();
        ServiceProviderMock = new Mock<IServiceProvider>();
        MemoryCache = new MemoryCache(new MemoryCacheOptions());
        HackathonRepositoryMock = new Mock<IHackathonRepository>();
        HrManagerClientMock = new Mock<IHrManagerClient>();
        SatisfactionServiceMock = new Mock<ISatisfactionService>();

        HackathonService = new HackathonService(
            LoggerMock.Object,
            ServiceProviderMock.Object,
            MemoryCache,
            HackathonRepositoryMock.Object,
            HrManagerClientMock.Object,
            SatisfactionServiceMock.Object
        );
    }
}