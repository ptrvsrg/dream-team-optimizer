using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Services;

public interface IWishListService
{
    void StartVoting(List<Employee> teamLeads, List<Employee> juniors, int hackathonId);
    void SaveWishListToSession(WishList wishList, int hackathonId);
}