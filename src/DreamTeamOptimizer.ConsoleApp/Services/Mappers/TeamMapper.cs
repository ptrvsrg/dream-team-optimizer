using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Services.Mappers;

public class TeamMapper
{
    public static Team ToEntity(Core.Models.Team team)
    {
        return new Team
        {
            TeamLeadId = team.TeamLead.Id,
            JuniorId = team.Junior.Id
        };
    }

    public static List<Team> ToEntities(List<Core.Models.Team> teams)
    {
        return teams.Select(ToEntity).ToList();
    }
}