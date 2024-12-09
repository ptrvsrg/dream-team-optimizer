using DreamTeamOptimizer.Core.Interfaces.IServices;
using Nsu.HackathonProblem.Contracts;

namespace DreamTeamOptimizer.Core.Services;

public class WishListService : IWishListService
{
    public List<Wishlist> GetWishlists(List<Employee> employees, List<Employee> desiredEmployees)
    {
        var wishlists = new List<Wishlist>();
        foreach (var employee in employees)
        {
            var employeesIds = desiredEmployees
                .Select(e => e.Id)
                .OrderBy(_ => Random.Shared.Next())
                .ToArray();
            wishlists.Add(new Wishlist(employee.Id, employeesIds));
        }
        return wishlists;
    }
}