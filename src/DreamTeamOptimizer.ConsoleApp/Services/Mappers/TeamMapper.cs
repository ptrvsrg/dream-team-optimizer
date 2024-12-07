using DreamTeamOptimizer.Core.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Services.Mappers;

public class TeamMapper
{
    public static Team ToEntity(Core.Models.Team team)
    {
        return new Team
        {
            TeamLeadId = team.TeamLeadId,
            JuniorId = team.JuniorId
        };
    }

    public static List<Team> ToEntities(List<Core.Models.Team> teams)
    {
        return teams.Select(ToEntity).ToList();
    }

    public static Core.Models.Team ToModel(Team team)
    {
        return new Core.Models.Team(team.TeamLead.Id, team.Junior.Id);
    }
    
    public static List<Core.Models.Team> ToModels(List<Team> teams)
    {
        return teams.Select(ToModel).ToList();
    }
}