using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.MsHrManager.Services.Mappers;

namespace DreamTeamOptimizer.MsHrDirector.Services.Mappers;

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
            TeamMapper.ToModels(hackathon.Teams.ToList()));
    }

    public static Core.Models.HackathonSimple ToModelSimple(Hackathon hackathon)
    {
        return new Core.Models.HackathonSimple(
            hackathon.Id,
            ToModelStatus(hackathon.Status),
            hackathon.Result);
    }

    public static Core.Models.HackathonStatus ToModelStatus(HackathonStatus status)
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

    public static HackathonStatus ToEntityStatus(Core.Models.HackathonStatus status)
    {
        switch (status)
        {
            case Core.Models.HackathonStatus.IN_PROCESSING:
                return HackathonStatus.IN_PROCESSING;
            case Core.Models.HackathonStatus.COMPLETED:
                return HackathonStatus.COMPLETED;
            default:
                return HackathonStatus.FAILED;
        }
    }
}