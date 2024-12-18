namespace DreamTeamOptimizer.MsHrManager.Interfaces.Services;

public interface IHackathonService
{
    bool ExistsById(int id);
    void Conduct(int hackathonId);
}