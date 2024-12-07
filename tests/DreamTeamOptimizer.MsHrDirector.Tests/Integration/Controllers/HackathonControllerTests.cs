using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using HackathonEntity = DreamTeamOptimizer.Core.Persistence.Entities.Hackathon;
using HackathonStatusEntity = DreamTeamOptimizer.Core.Persistence.Entities.HackathonStatus;

namespace DreamTeamOptimizer.MsHrDirector.Tests.Integration.Controllers;

public class HackathonControllerTests : IClassFixture<WebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;
    private readonly IMemoryCache _memoryCache;
    private readonly IHackathonRepository _hackathonRepository;

    public HackathonControllerTests(WebAppFactory factory)
    {
        var clientOptions = new WebApplicationFactoryClientOptions();
        clientOptions.AllowAutoRedirect = false;
        _httpClient = factory.CreateClient(clientOptions);

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        _hackathonRepository = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
    }

    public Task InitializeAsync()
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions();
        _memoryCache.Set(1, true, cacheEntryOptions);

        var hackathon = new HackathonEntity
        {
            Id = 1,
            Status = HackathonStatusEntity.IN_PROCESSING,
            Result = 0
        };
        _hackathonRepository.Create(hackathon);
        _dbContext.Entry(hackathon).State = EntityState.Detached;

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _dbContext.Set<HackathonEntity>().RemoveRange(_dbContext.Set<HackathonEntity>());
        _dbContext.SaveChanges();
        _dbContext.ChangeTracker.Clear();

        return Task.CompletedTask;
    }

    [Theory]
    [InlineData("/api/v1/hackathons/webhook")]
    public async Task SaveResult_ShouldReturn200_WhenResultIsSaved(string url)
    {
        // Arrange
        var hackathonId = 1;
        var result = new HackathonResult(
            new List<WishList>
            {
                new(1, [2])
            },
            new List<WishList>
            {
                new(2, [1])
            },
            new List<Team>
            {
                new(2, 1)
            });

        // Act
        var response =
            await _httpClient.PostAsJsonAsync($"{url}?hackathonId={hackathonId}", result);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("/api/v1/hackathons/webhook")]
    public async Task SaveResult_ShouldReturn404_WhenHackathonNotFound(string url)
    {
        // Arrange
        var hackathonId = 999;
        var result = new HackathonResult(new List<WishList>(), new List<WishList>(), new List<Team>());

        // Act
        var response =
            await _httpClient.PostAsJsonAsync($"{url}?hackathonId={hackathonId}", result);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("/api/v1/hackathons")]
    public async Task GetById_ShouldReturnHackathon_WhenFound(string url)
    {
        // Arrange
        var hackathonId = 1;

        // Act
        var response = await _httpClient.GetAsync($"{url}/{hackathonId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<Hackathon>(new JsonSerializerOptions()
        {
            Converters = { new JsonStringEnumConverter() }
        });
        result.Should().NotBeNull();
    }

    [Theory]
    [InlineData("/api/v1/hackathons")]
    public async Task GetById_ShouldReturn404_WhenHackathonNotFound(string url)
    {
        // Arrange
        var hackathonId = 999;

        // Act
        var response = await _httpClient.GetAsync($"{url}/{hackathonId}");

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
}