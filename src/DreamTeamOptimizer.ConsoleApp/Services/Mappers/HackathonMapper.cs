using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Services.Mappers;

public class HackathonMapper
{
    public static Core.Models.Hackathon ToModel(Hackathon hackathon)
    {
        return new Core.Models.Hackathon(hackathon.Id, hackathon.Result);
    }
}