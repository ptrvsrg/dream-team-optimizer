using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Brokers.Publishers;
using MassTransit;

namespace DreamTeamOptimizer.MsHrDirector.Brokers.Publishers;

public class HackathonStartedPublisher(ILogger<HackathonStartedPublisher> logger, IBus bus) : IHackathonStartedPublisher
{
    public void StartHackathon(int hackathonId)
    {
        logger.LogInformation("publish hackathon #{hackathonId} started", hackathonId);
        bus.Publish<HackathonStartedEvent>(new(hackathonId));
    }
}