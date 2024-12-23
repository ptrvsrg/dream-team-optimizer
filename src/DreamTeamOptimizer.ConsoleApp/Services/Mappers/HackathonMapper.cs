using DreamTeamOptimizer.Core.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Services.Mappers;

public class HackathonMapper
{
    public static Core.Models.Hackathon ToModel(Hackathon hackathon)
    {
        return new Core.Models.Hackathon(
            hackathon.Id, 
            ToModelStatus(hackathon.Status), 
            hackathon.Result,
            EmployeeMapper.ToModels(hackathon.Employees.ToList()),
            WishListMapper.ToModels(hackathon.WishLists.ToList()),
            TeamMapper.ToModels(hackathon.Teams.ToList()),
            SatisfactionMapper.ToModels(hackathon.Satisfactions.ToList()));
    }

    private static Core.Models.HackathonStatus ToModelStatus(HackathonStatus status)
    {
        switch (status)
        {
            case HackathonStatus.IN_PROCESSING:
                return Core.Models.HackathonStatus.IN_PROCESSING;
            case HackathonStatus.COMPLETED:
                return Core.Models.HackathonStatus.COMPLETED;
            default:
                return Core.Models.HackathonStatus.FAILED;
        }
    }
}