using System.Net;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Persistence;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Hackathon = DreamTeamOptimizer.Core.Persistence.Entities.Hackathon;
using HackathonStatus = DreamTeamOptimizer.Core.Persistence.Entities.HackathonStatus;

namespace DreamTeamOptimizer.MsHrManager.Tests.Integration.Controller;

public class HackathonControllerTests : IClassFixture<WebAppFactory>, IAsyncLifetime
{
    private readonly HttpClient _httpClient;
    private readonly AppDbContext _dbContext;
    private readonly IHackathonRepository _hackathonRepository;

    public HackathonControllerTests(WebAppFactory factory)
    {
        var clientOptions = new WebApplicationFactoryClientOptions();
        clientOptions.AllowAutoRedirect = false;
        _httpClient = factory.CreateClient(clientOptions);

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _hackathonRepository = scope.ServiceProvider.GetRequiredService<IHackathonRepository>();
    }

    public Task InitializeAsync()
    {
        var hackathon = new Hackathon
        {
            Id = 1,
            Status = HackathonStatus.IN_PROCESSING,
            Result = 0
        };
        _hackathonRepository.Create(hackathon);
    
        _dbContext.Entry(hackathon).State = EntityState.Detached;

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        _dbContext.Set<Hackathon>().RemoveRange(_dbContext.Set<Hackathon>());
        _dbContext.SaveChanges();
        _dbContext.ChangeTracker.Clear();
        
        return Task.CompletedTask;
    }

    [Theory]
    [InlineData("/api/v1/hackathons", 1)]
    public async Task GetWishlist_ShouldReturnWishlist_WhenDesiredEmployeeIdsAreProvided(string url,
        int hackathonId)
    {
        // Act
        var response = await _httpClient.GetAsync($"{url}/{hackathonId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Accepted);
    }

    [Theory]
    [InlineData("/api/v1/hackathons", 2)]
    public async Task GetWishlist_ShouldReturnNotFound_WhenNonExistentHackathon(string url,
        int hackathonId)
    {
        // Act
        var response = await _httpClient.GetAsync($"{url}/{hackathonId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Content.Headers.ContentType!.ToString().Should().Contain("application/json");

        var content = await response.Content.ReadAsStringAsync();
        var error = JsonConvert.DeserializeObject<Error>(content);

        error.Should().NotBeNull();
        error?.Message.Should().Be($"No hackathon #{hackathonId} found");
    }
}