using DreamTeamOptimizer.Core.Interfaces.IServices;

namespace DreamTeamOptimizer.Core.Services;

public class HackathonService(
    IEmployeeService employeeService,
    IWishListService wishListService,
    IHrManagerService hrManagerService,
    IHrDirectorService hrDirectorService
): IHackathonService
{
    public double Conduct()
    {
        var juniors = employeeService.FindAllJuniors();
        var teamLeads = employeeService.FindAllTeamLeads();

        var juniorWishLists = wishListService.GetWishlists(juniors, teamLeads);
        var teamLeadWishLists = wishListService.GetWishlists(teamLeads, juniors);

        var teams = hrManagerService.BuildTeams(teamLeads, juniors, teamLeadWishLists, juniorWishLists);

        return hrDirectorService.CalculateDistributionHarmony(teams, teamLeadWishLists, juniorWishLists);
    }
}