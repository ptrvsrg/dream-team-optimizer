using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;
using MassTransit;

namespace DreamTeamOptimizer.MsHrManager.Brokers.Publishers;

public class HackathonResultPublisher(ILogger<HackathonResultPublisher> logger, IBus bus) : IHackathonResultPublisher
{
    public void SaveResult(HackathonResultEvent hackathonResultEvent)
    {
        logger.LogInformation("publish hackathon result event");
        bus.Publish(hackathonResultEvent);
    }
}