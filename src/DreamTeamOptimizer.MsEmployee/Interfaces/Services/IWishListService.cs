using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsEmployee.Interfaces.Services;

public interface IWishListService
{
    WishList GetWishlist(List<int> employeeIds);
}