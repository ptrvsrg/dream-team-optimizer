using DreamTeamOptimizer.Core.Persistence.Entities;

namespace DreamTeamOptimizer.MsHrDirector.Services.Mappers;

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
    
    public static Core.Models.Satisfaction ToModel(Satisfaction satisfaction)
    {
        return new Core.Models.Satisfaction(satisfaction.EmployeeId, satisfaction.Rank);
    }
    
    public static List<Core.Models.Satisfaction> ToModels(List<Satisfaction> satisfactions)
    {
        return satisfactions.Select(ToModel).ToList();
    }
}