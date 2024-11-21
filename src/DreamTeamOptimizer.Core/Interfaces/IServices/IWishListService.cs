using DreamTeamOptimizer.Core.Entities;

namespace DreamTeamOptimizer.Core.Interfaces.IServices;

public interface IWishListService
{
    List<WishList> GetWishlists(List<Employee> employees, List<Employee> desiredEmployees);
}