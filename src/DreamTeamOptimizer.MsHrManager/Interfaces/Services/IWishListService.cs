using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Services;

public interface IWishListService
{
    List<WishList> Vote(List<Employee> teamLeads, List<Employee> juniors, int hackathonId);
}