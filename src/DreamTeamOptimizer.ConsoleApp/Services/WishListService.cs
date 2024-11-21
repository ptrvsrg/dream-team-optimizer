using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Persistence;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.ConsoleApp.Services.Mappers;
using DreamTeamOptimizer.Core.Models;
using Serilog;
using Employee = DreamTeamOptimizer.Core.Models.Employee;

namespace DreamTeamOptimizer.ConsoleApp.Services;

public class WishListService(IPreferenceRepository preferenceRepository) : IWishListService
{
    public List<WishList> GenerateWishlists(List<Employee> employees, List<Employee> desiredEmployees, int hackathonId)
    {
        Log.Information("generate wish lists");
        
        var wishlists = new List<WishList>();
        foreach (var employee in employees)
        {
            var employeesIds = desiredEmployees
                .Select(e => e.Id)
                .OrderBy(_ => Random.Shared.Next())
                .ToArray();
            wishlists.Add(new WishList(employee.Id, employeesIds));
        }

        var preferences = WishListMapper.ToEntities(wishlists);
        preferences.ForEach(p => p.HackathonId = hackathonId);
        
        preferenceRepository.CreateAll(preferences);
        
        return wishlists;
    }
}