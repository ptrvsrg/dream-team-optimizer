using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using DreamTeamOptimizer.MsHrManager.Models.Events;
using MassTransit;

namespace DreamTeamOptimizer.MsHrManager.Brokers.Consumers;

public class VotingCompletedConsumer(ILogger<VotingCompletedConsumer> logger, IHackathonService hackathonService)
    : IConsumer<Batch<VotingCompletedEvent>>
{
    public Task Consume(ConsumeContext<Batch<VotingCompletedEvent>> context)
    {
        logger.LogInformation("consume voting completed event");

        foreach (var ctx in context.Message)
        {
            hackathonService.CompleteHackathon(
                ctx.Message.TeamLeads, 
                ctx.Message.Juniors,
                ctx.Message.TeamLeadsWishlists, 
                ctx.Message.JuniorsWishlists, 
                ctx.Message.HackathonId);
        }

        return Task.CompletedTask;
    }
}