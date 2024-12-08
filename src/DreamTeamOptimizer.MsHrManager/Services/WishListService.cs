using System.Collections.Concurrent;
using System.Net;
using System.Web;
using DreamTeamOptimizer.MsHrManager.Services.Mappers;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.MsHrManager.Interfaces.Clients;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using Serilog;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;
using Employee = DreamTeamOptimizer.Core.Models.Employee;
using WishList = DreamTeamOptimizer.Core.Models.WishList;

namespace DreamTeamOptimizer.MsHrManager.Services;

public class WishListService(
    ILogger<WishListService> logger,
    IWishListRepository wishlistRepository,
    IEmployeeClient employeeClient) : IWishListService
{
    public List<WishList> Vote(List<Employee> employees, List<Employee> desiredEmployees, int hackathonId)
    {
        logger.LogInformation("vote employees");

        var desiredEmployeeIds = desiredEmployees.Select(e => e.Id).ToList();

        logger.LogDebug("send vote requests to {count} employees", employees.Count);
        var wishlists = new ConcurrentBag<WishList>();
        Parallel.ForEach(employees, employee => wishlists.Add(employeeClient.Vote(employee.Id, desiredEmployeeIds)));

        logger.LogDebug("save wishlists to database");
        var wishlistList = wishlists.ToList();
        var wishlistEntities = WishListMapper.ToEntities(wishlistList);
        wishlistEntities.ForEach(p => p.HackathonId = hackathonId);

        wishlistRepository.CreateAll(wishlistEntities);

        return wishlistList;
    }
}