using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.MsHrManager.Interfaces.Clients;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using DreamTeamOptimizer.MsHrManager.Services.Mappers;
using Employee = DreamTeamOptimizer.Core.Persistence.Entities.Employee;

namespace DreamTeamOptimizer.MsHrManager.Services;

public class HackathonService(
    ILogger<HackathonService> logger,
    IServiceScopeFactory serviceScopeFactory,
    IHackathonRepository hackathonRepository) : IHackathonService
{
    public bool ExistsById(int id)
    {
        logger.LogInformation($"exists hackathon #{id}");

        var hackathon = hackathonRepository.FindById(id);
        return hackathon != null;
    }
    
    public void Conduct(int hackathonId)
    {
        logger.LogInformation($"conduct hackathon #{hackathonId}");
        
        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var employeeService = scope.ServiceProvider.GetRequiredService<IEmployeeService>();
        var wishListService = scope.ServiceProvider.GetRequiredService<IWishListService>();
        var strategyService = scope.ServiceProvider.GetRequiredService<IStrategyService>();
        var hrDirectorClient = scope.ServiceProvider.GetRequiredService<IHrDirectorClient>();
        var hackathonEmployeeRepository = scope.ServiceProvider.GetRequiredService<IHackathonEmployeeRepository>();

        var tx = dbContext.Database.BeginTransaction();
        try
        {
            var juniors = employeeService.FindAllJuniors();
            var teamLeads = employeeService.FindAllTeamLeads();

            var employees = new List<Employee>()
                .Concat(EmployeeMapper.ToEntities(juniors))
                .Concat(EmployeeMapper.ToEntities(teamLeads))
                .ToList();

            var participants = employees.Select(e => new HackathonEmployee
            {
                HackathonId = hackathonId,
                EmployeeId = e.Id
            }).ToList();

            hackathonEmployeeRepository.CreateAll(participants);

            var juniorWishLists = wishListService.Vote(juniors, teamLeads, hackathonId);
            var teamLeadWishLists = wishListService.Vote(teamLeads, juniors, hackathonId);

            var teams = strategyService.BuildTeams(teamLeads, juniors, teamLeadWishLists, juniorWishLists, hackathonId);

            var result = new HackathonResult(juniorWishLists, teamLeadWishLists, teams);
            hrDirectorClient.SaveResult(result, hackathonId);

            tx.Commit();
        }
        catch (Exception e)
        {
            tx.Rollback();
            logger.LogWarning(e, e.Message);
        }
    }
}
