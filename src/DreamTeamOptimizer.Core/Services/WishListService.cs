using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Interfaces.IServices;

namespace DreamTeamOptimizer.Core.Services;

public class WishListService : IWishListService
{
    public List<WishList> GetWishlists(List<Employee> employees, List<Employee> desiredEmployees)
    {
        var wishlists = new List<WishList>();
        foreach (var employee in employees)
        {
            var employeesIds = desiredEmployees
                .Select(e => e.Id)
                .OrderBy(_ => Random.Shared.Next())
                .ToArray();
            wishlists.Add(new WishList(employee.Id, employeesIds));
        }
        return wishlists;
    }
}