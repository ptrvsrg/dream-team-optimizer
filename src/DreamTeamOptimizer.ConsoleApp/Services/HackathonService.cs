using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Persistence;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.ConsoleApp.Services.Mappers;
using Serilog;

namespace DreamTeamOptimizer.ConsoleApp.Services;

public class HackathonService(
    AppDbContext dbContext,
    IHackathonRepository hackathonRepository,
    IEmployeeService employeeService,
    IWishListService wishListService,
    IHrManagerService hrManagerService,
    IHrDirectorService hrDirectorService
) : IHackathonService
{
    public Core.Models.Hackathon Conduct()
    {
        Log.Information("conduct hackathon");

        // Find employees
        var juniors = employeeService.FindAllJuniors();
        var teamLeads = employeeService.FindAllTeamLeads();

        // Save participants
        var employees = new List<Employee>()
            .Concat(EmployeeMapper.ToEntities(juniors))
            .Concat(EmployeeMapper.ToEntities(teamLeads))
            .ToList();

        // Create hackathon
        var hackathon = new Hackathon
        {
            Status = HackathonStatus.IN_PROCESSING,
            Employees = employees,
            Result = 0.0
        };
        hackathonRepository.Create(hackathon);

        using (var tx = dbContext.Database.BeginTransaction())
        {
            try
            {
                // Generate wish lists
                var juniorWishLists = wishListService.GenerateWishlists(juniors, teamLeads, hackathon.Id);
                var teamLeadWishLists = wishListService.GenerateWishlists(teamLeads, juniors, hackathon.Id);

                // Build teams
                var teams = hrManagerService.BuildTeams(teamLeads, juniors, teamLeadWishLists, juniorWishLists,
                    hackathon.Id);

                // Calculate satisfactions
                var satisfactions =
                    hrDirectorService.CalculateSatisfactions(teams, teamLeadWishLists, juniorWishLists, hackathon.Id);

                // Calculate harmonic mean
                var satisfactionRanks = satisfactions.Select(s => s.Rank).ToList();
                var harmonicMean = Helpers.Math.CalculateHarmonicMean(satisfactionRanks);
                hackathon.Result = harmonicMean;

                // Complete hackathon
                hackathon.Status = HackathonStatus.COMPLETED;
                hackathonRepository.Update(hackathon);
            }
            catch (Exception)
            {
                tx.Rollback();

                // Fail hackathon
                hackathon.Status = HackathonStatus.FAILED;
                hackathonRepository.Update(hackathon);

                throw;
            }
            
            tx.Commit();
            return HackathonMapper.ToModel(hackathonRepository.FindById(hackathon.Id)!);
        }
    }

    public double CalculateAverageHarmonicity()
    {
        return hackathonRepository.FindAverageResult();
    }

    public Core.Models.Hackathon? FindById(int id)
    {
        var hackathon = hackathonRepository.FindById(id);
        if (hackathon == null)
        {
            return null;
        }
        
        return HackathonMapper.ToModel(hackathon);
    }
}