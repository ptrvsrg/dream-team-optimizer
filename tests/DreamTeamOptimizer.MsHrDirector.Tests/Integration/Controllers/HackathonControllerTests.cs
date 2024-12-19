using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.Core.Persistence;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HackathonEntity = DreamTeamOptimizer.Core.Persistence.Entities.Hackathon;
using HackathonStatusEntity = DreamTeamOptimizer.Core.Persistence.Entities.HackathonStatus;
using WishListEntity = DreamTeamOptimizer.Core.Persistence.Entities.WishList;
using TeamEntity = DreamTeamOptimizer.Core.Persistence.Entities.Team;

namespace DreamTeamOptimizer.MsHrDirector.Tests.Integration.Controllers;

public class HackathonControllerTests : IClassFixture<WebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly IHackathonRepository _hackathonRepository;
    private readonly IBus _bus;

    private int _hackathonId;

    public HackathonControllerTests(WebAppFactory factory)
    {
        var clientOptions = new WebApplicationFactoryClientOptions();
        clientOptions.AllowAutoRedirect = false;
        _httpClient = factory.CreateClient(clientOptions);

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        _hackathonRepository = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
        _bus = scope.ServiceProvider.GetRequiredService<IBus>();
    }

    public Task InitializeAsync()
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions();
        _memoryCache.Set(1, true, cacheEntryOptions);

        // Hackathon
        var hackathon = new HackathonEntity
        {
            Status = HackathonStatusEntity.IN_PROCESSING,
            Result = 0
        };
        _hackathonRepository.Create(hackathon);
        _dbContext.Entry(hackathon).State = EntityState.Detached;

        // WishList
        var wishLists = new List<WishListEntity>
        {
            new()
            {
                HackathonId = hackathon.Id,
                EmployeeId = 1,
                DesiredEmployeeIds = [21]
            },
            new()
            {
                HackathonId = hackathon.Id,
                EmployeeId = 21,
                DesiredEmployeeIds = [1]
            }
        };
        _dbContext.Set<WishListEntity>().AddRange(wishLists);
        wishLists.ForEach(w => _dbContext.Entry(w).State = EntityState.Detached);

        // Team
        var team = new TeamEntity
        {
            HackathonId = hackathon.Id,
            JuniorId = 1,
            TeamLeadId = 21
        };
        _dbContext.Set<TeamEntity>().Add(team);
        _dbContext.Entry(team).State = EntityState.Detached;

        // Save
        _dbContext.SaveChanges();
        _dbContext.ChangeTracker.Clear();

        _hackathonId = hackathon.Id;

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        // Hackathon
        _dbContext.Set<HackathonEntity>().RemoveRange(_dbContext.Set<HackathonEntity>());
        
        // WishList
        _dbContext.Set<WishListEntity>().RemoveRange(_dbContext.Set<WishListEntity>());
        
        // Team
        _dbContext.Set<TeamEntity>().RemoveRange(_dbContext.Set<TeamEntity>());
        
        // Clear context
        _dbContext.SaveChanges();
        _dbContext.ChangeTracker.Clear();

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData("/api/v1/hackathons")]
    public async Task Create_ShouldReturn201_WhenSuccess(string url)
    {
        // Act
        var response = await _httpClient.PostAsync(url, null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };
        var result = await response.Content.ReadFromJsonAsync<HackathonSimple>(options);
        result.Should().NotBeNull();
        result!.Status.Should().Be(HackathonStatus.IN_PROCESSING);
        result.Result.Should().Be(0);
    }

    [Theory]
    [InlineData("/api/v1/hackathons")]
    public async Task GetById_ShouldReturnHackathon_WhenFound(string url)
    {
        // Act
        var response = await _httpClient.GetAsync($"{url}/{_hackathonId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };
        var result = await response.Content.ReadFromJsonAsync<Hackathon>(options);
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("/api/v1/hackathons")]
    public async Task GetById_ShouldReturn404_WhenHackathonNotFound(string url)
    {
        // Act
        var response = await _httpClient.GetAsync($"{url}/{_hackathonId + 1000}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("/api/v1/hackathons/average-harmonic")]
    public async Task CalculateAverageHarmonicity_ShouldReturn200_WhenSuccess(string url)
    {
        // Act
        var response = await _httpClient.GetAsync(url);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<AverageHarmonicity>();
        result.Should().NotBeNull();
        result?.Result.Should().BeGreaterOrEqualTo(0.0);
    }

    [Fact]
    public async Task ConductHackathon()
    {
        // Start hackathon
        var response = await _httpClient.PostAsync("/api/v1/hackathons", null);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true
        };
        var hackathonSimple = await response.Content.ReadFromJsonAsync<HackathonSimple>(options);
        hackathonSimple.Should().NotBeNull();
        hackathonSimple!.Status.Should().Be(HackathonStatus.IN_PROCESSING);

        // Publish event
        var hackathonResult = new HackathonResultEvent(
            hackathonSimple.Id,
            new List<WishList> { new(1, [21]) },
            new List<WishList> { new(21, [1]) },
            new List<Team> { new(21, 1) });
        await _bus.Publish(hackathonResult);

        // Wait for result
        await Task.Delay(1000);

        // Get result
        response = await _httpClient.GetAsync($"/api/v1/hackathons/{hackathonSimple.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var hackathon = await response.Content.ReadFromJsonAsync<Hackathon>(options);
        hackathon.Should().NotBeNull();
        hackathon!.Status.Should().Be(HackathonStatus.COMPLETED);
    }
}