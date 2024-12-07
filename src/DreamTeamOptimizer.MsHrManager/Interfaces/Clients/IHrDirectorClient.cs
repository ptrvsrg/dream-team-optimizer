using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Clients;

public interface IHrDirectorClient
{
    void SaveResult(HackathonResult result, int hackathonId);
}