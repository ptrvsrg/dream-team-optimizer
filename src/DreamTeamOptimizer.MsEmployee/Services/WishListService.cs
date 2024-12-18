using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsEmployee.Interfaces.Services;

namespace DreamTeamOptimizer.MsEmployee.Services;

public class WishListService(ILogger<WishListService> logger, IConfiguration config) : IWishListService
{
    public WishList GetWishlist(List<int> desiredEmployeeIds)
    {
        logger.LogInformation("get wish list");
        
        var employeeId = config.GetValue<int>("Application:EmployeeID");

        var employeesIds = desiredEmployeeIds
            .OrderBy(_ => Random.Shared.Next())
            .ToArray();
        var wishlist = new WishList(employeeId, employeesIds);
        
        return wishlist;
    }
}