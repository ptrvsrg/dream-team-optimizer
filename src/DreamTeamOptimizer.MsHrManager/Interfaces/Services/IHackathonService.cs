using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Services;

public interface IHackathonService
{
    void StartHackathon(int hackathonId);
    void CompleteHackathon(List<Employee> teamLeads, List<Employee> juniors,
        List<WishList> teamLeadWishLists, List<WishList> juniorWishLists, int hackathonId);
}