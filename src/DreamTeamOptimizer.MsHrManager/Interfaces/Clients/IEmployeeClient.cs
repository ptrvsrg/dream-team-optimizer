using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Clients;

public interface IEmployeeClient
{
    WishList Vote(int employeeId, List<int> desiredEmployeeIds);
}