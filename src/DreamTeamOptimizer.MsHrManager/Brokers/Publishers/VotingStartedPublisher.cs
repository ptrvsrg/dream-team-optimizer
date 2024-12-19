using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;
using MassTransit;

namespace DreamTeamOptimizer.MsHrManager.Brokers.Publishers;

public class VotingStartedPublisher(ILogger<VotingStartedPublisher> logger, IBus bus) : IVotingStartedPublisher
{
    public void StartVoting(VotingStartedEvent votingStartedEvent)
    {
        logger.LogInformation("publish voting started event");
        bus.Publish(votingStartedEvent);
    }
}