using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.ConsoleApp.Interfaces.Services;

public interface IWishListService
{
    List<WishList> GenerateWishlists(List<Employee> employees, List<Employee> desiredEmployees, int hackathonId);
}