using System.Net;
using DreamTeamOptimizer.Core.Exceptions;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;

namespace DreamTeamOptimizer.MsHrDirector.Services;

public class SatisfactionService(ILogger<SatisfactionService> logger) : ISatisfactionService
{
    public List<Satisfaction> Evaluate(List<Team> teams, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists)
    {
        logger.LogInformation("calculate satisfactions");

        var teamLeadsWishlistsDict =
            teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());

        var satisfactionModels = new List<Satisfaction>();
        foreach (var team in teams)
        {
            var teamLeadSatisfaction = CalculateSatisfaction(team.TeamLeadId, team.JuniorId, teamLeadsWishlistsDict);
            satisfactionModels.Add(teamLeadSatisfaction);

            var juniorSatisfaction = CalculateSatisfaction(team.JuniorId, team.TeamLeadId, juniorsWishlistsDict);
            satisfactionModels.Add(juniorSatisfaction);
        }

        return satisfactionModels;
    }

    private Satisfaction CalculateSatisfaction(int employeeId, int selectedEmployeeId,
        Dictionary<int, List<int>> wishlists)
    {
        logger.LogDebug(
            "calculate satisfaction for employee with ID {employeeId} and selected employee with ID {selectedEmployeeId}",
            employeeId, selectedEmployeeId);

        if (!wishlists.TryGetValue(employeeId, out var wishlist))
            throw new HttpStatusException(HttpStatusCode.BadRequest,
                $"No wish list for employee with ID {employeeId} found");

        var index = wishlist.IndexOf(selectedEmployeeId);
        if (index == -1)
            throw new HttpStatusException(HttpStatusCode.BadRequest,
                $"No employee with ID {selectedEmployeeId} found in wish list of employee with ID {employeeId}");

        return new Satisfaction(employeeId, wishlist.Count - index);
    }
}