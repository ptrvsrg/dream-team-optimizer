using Nsu.HackathonProblem.Contracts;

namespace DreamTeamOptimizer.Core.Interfaces.IServices;

public interface IWishListService
{
    List<Wishlist> GetWishlists(List<Employee> employees, List<Employee> desiredEmployees);
}