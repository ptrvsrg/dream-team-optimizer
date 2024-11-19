using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Services.Mappers;

public class SatisfactionMapper
{
    public static Satisfaction ToEntity(Core.Models.Satisfaction satisfaction)
    {
        return new Satisfaction
        {
            EmployeeId = satisfaction.EmployeeId,
            Rank = satisfaction.Rank
        };
    }

    public static List<Satisfaction> ToEntities(List<Core.Models.Satisfaction> satisfactions)
    {
        return satisfactions.Select(ToEntity).ToList();
    }
}