using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Persistence;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;
using DreamTeamOptimizer.ConsoleApp.Services;
using DreamTeamOptimizer.ConsoleApp.Services.Mappers;
using DreamTeamOptimizer.Strategies;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Integration.Services;

[Collection("IntegrationTests")]
public class HackathonServiceTest : IClassFixture<IntegrationTestFactory>
{
    private readonly AppDbContext _dbContext;
    private readonly IHackathonService _service;

    public HackathonServiceTest(IntegrationTestFactory factory)
    {
        _dbContext = factory.DbContext;

        var strategy = new WeightedPreferenceStrategy();

        var employeeRepository = new EmployeeRepository(_dbContext);
        var wishListRepository = new WishListRepository(_dbContext);
        var teamRepository = new TeamRepository(_dbContext);
        var satisfactionRepository = new SatisfactionRepository(_dbContext);
        var hackathonRepository = new HackathonRepository(_dbContext);

        var employeeService = new EmployeeService(employeeRepository);
        var wishListService = new WishListService(wishListRepository);
        var hrManagerService = new HrManagerService(strategy, teamRepository);
        var hrDirectorService = new HrDirectorService(satisfactionRepository);

        _service = new HackathonService(_dbContext, hackathonRepository, employeeService, wishListService,
            hrManagerService, hrDirectorService);
    }

    [Fact]
    public void Conduct()
    {
        // Act
        Core.Models.Hackathon? hackathon = null;
        var exception = Record.Exception(() => { hackathon = _service.Conduct(); });
        Assert.Null(exception);
        Assert.NotNull(hackathon);

        // Check
        var dbHackathon = _dbContext.Set<Hackathon>().Find(hackathon.Id)!;

        Assert.Equal(hackathon.Result, dbHackathon.Result);
        Assert.Equal(hackathon.Status.ToString(), dbHackathon.Status.ToString());
        hackathon.Employees
            .Should()
            .BeEquivalentTo(
                EmployeeMapper.ToModels(dbHackathon.Employees.ToList())
            );
        hackathon.WishLists
            .Should()
            .BeEquivalentTo(
                WishListMapper.ToModels(dbHackathon.WishLists.ToList())
            );
        hackathon.Teams
            .Should()
            .BeEquivalentTo(
                TeamMapper.ToModels(dbHackathon.Teams.ToList())
            );

        // Clean
        _dbContext.Set<Hackathon>().Remove(dbHackathon);
        _dbContext.ChangeTracker.Clear();
    }

    [Fact]
    public void FindById()
    {
        // Prepare
        Core.Models.Hackathon? expectedHackathon = null;
        var exception = Record.Exception(() => { expectedHackathon = _service.Conduct(); });
        Assert.Null(exception);
        Assert.NotNull(expectedHackathon);

        // Act
        var actualHackathon = _service.FindById(expectedHackathon.Id);

        // Assert
        actualHackathon.Should().BeEquivalentTo(expectedHackathon);

        // Clean
        var dbHackathon = _dbContext.Set<Hackathon>().Find(expectedHackathon.Id)!;
        _dbContext.Set<Hackathon>().Remove(dbHackathon);
        _dbContext.ChangeTracker.Clear();
    }
}