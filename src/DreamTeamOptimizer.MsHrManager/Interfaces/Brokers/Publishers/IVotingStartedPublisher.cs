using DreamTeamOptimizer.Core.Models.Events;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Brokers.Publishers;

public interface IVotingStartedPublisher
{
    void StartVoting(VotingStartedEvent votingStartedEvent);
}