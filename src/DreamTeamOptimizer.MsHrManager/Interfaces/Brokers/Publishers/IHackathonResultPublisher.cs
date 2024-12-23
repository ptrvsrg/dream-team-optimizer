using DreamTeamOptimizer.Core.Models.Events;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;

public interface IHackathonResultPublisher
{
    void SaveResult(HackathonResultEvent hackathonResultEvent);
}