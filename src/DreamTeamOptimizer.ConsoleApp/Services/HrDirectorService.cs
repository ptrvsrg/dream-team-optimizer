using DreamTeamOptimizer.ConsoleApp.Exceptions;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Services.Mappers;
using DreamTeamOptimizer.Core.Models;
using Serilog;
using SatisfactionModel = DreamTeamOptimizer.Core.Models.Satisfaction;

namespace DreamTeamOptimizer.ConsoleApp.Services;

public class HrDirectorService(ISatisfactionRepository satisfactionRepository) : IHrDirectorService
{
    public List<SatisfactionModel> CalculateSatisfactions(List<Team> teams, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists, int hackathonId)
    {
        Log.Information("calculate satisfactions");
     
        if (teams.Count == 0) throw new NoTeamsException();

        var teamLeadsWishlistsDict =
            teamLeadsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());
        var juniorsWishlistsDict = juniorsWishlists.ToDictionary(w => w.EmployeeId, w => w.DesiredEmployees.ToList());

        var satisfactionModels = new List<SatisfactionModel>();
        foreach (var team in teams)
        {
            var teamLeadSatisfaction = CalculateSatisfaction(team.TeamLeadId, team.JuniorId, teamLeadsWishlistsDict);
            satisfactionModels.Add(teamLeadSatisfaction);

            var juniorSatisfaction = CalculateSatisfaction(team.JuniorId, team.TeamLeadId, juniorsWishlistsDict);
            satisfactionModels.Add(juniorSatisfaction);
        }

        var satisfactions = SatisfactionMapper.ToEntities(satisfactionModels);
        satisfactions.ForEach(s => s.HackathonId = hackathonId);
        
        satisfactionRepository.CreateAll(satisfactions);
        
        return satisfactionModels;
    }

    private static SatisfactionModel CalculateSatisfaction(int employeeId, int selectedEmployeeId,
        Dictionary<int, List<int>> wishlists)
    {
        if (!wishlists.TryGetValue(employeeId, out var wishlist)) throw new WishListNotFoundException(employeeId);

        var index = wishlist.IndexOf(selectedEmployeeId);
        if (index == -1) throw new EmployeeInWishListNotFoundException(employeeId, selectedEmployeeId);
        return new SatisfactionModel(employeeId, wishlist.Count - index);
    }
}