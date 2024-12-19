using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.Core.Persistence;
using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using DreamTeamOptimizer.MsHrManager.Services.Mappers;
using WishList = DreamTeamOptimizer.Core.Models.WishList;
using EmployeeModel = DreamTeamOptimizer.Core.Models.Employee;

namespace DreamTeamOptimizer.MsHrManager.Services;

public class HackathonService(
    ILogger<HackathonService> logger,
    AppDbContext dbContext,
    IWishListRepository wishListRepository,
    IHackathonEmployeeRepository hackathonEmployeeRepository,
    IHackathonResultPublisher hackathonResultPublisher,
    IEmployeeService employeeService,
    IWishListService wishListService,
    IStrategyService strategyService) : IHackathonService
{
    public void StartHackathon(int hackathonId)
    {
        logger.LogInformation($"conduct hackathon #{hackathonId}");

        var tx = dbContext.Database.BeginTransaction();
        try
        {
            logger.LogDebug("find juniors and team leads");
            var juniors = employeeService.FindAllJuniors();
            var teamLeads = employeeService.FindAllTeamLeads();

            logger.LogDebug("save participants");
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

            logger.LogDebug("start voting");
            wishListService.StartVoting(teamLeads, juniors, hackathonId);

            tx.Commit();
        }
        catch (Exception e)
        {
            tx.Rollback();
            logger.LogWarning(e, e.Message);
        }
    }

    public void CompleteHackathon(List<EmployeeModel> teamLeads, List<EmployeeModel> juniors,
        List<WishList> teamLeadWishLists, List<WishList> juniorWishLists, int hackathonId)
    {
        logger.LogInformation($"complete hackathon #{hackathonId}");
        
        logger.LogDebug("save wishlists");
        var allWishLists = new List<WishList>()
            .Concat(teamLeadWishLists)
            .Concat(juniorWishLists)
            .Select(WishListMapper.ToEntity)
            .Select(w =>
            {
                w.HackathonId = hackathonId;
                return w;
            })
            .ToList();
        wishListRepository.CreateAll(allWishLists);
        
        var teams = strategyService.BuildTeams(teamLeads, juniors, teamLeadWishLists, juniorWishLists, hackathonId);

        var result = new HackathonResultEvent(hackathonId, juniorWishLists, teamLeadWishLists, teams);
        hackathonResultPublisher.SaveResult(result);
    }
}