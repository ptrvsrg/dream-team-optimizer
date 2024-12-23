using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using MassTransit;

namespace DreamTeamOptimizer.MsHrManager.Brokers.Consumers;

public class HackathonStartedConsumer(ILogger<HackathonStartedConsumer> logger, IHackathonService hackathonService)
    : IConsumer<Batch<HackathonStartedEvent>>
{
    public Task Consume(ConsumeContext<Batch<HackathonStartedEvent>> context)
    {
        logger.LogInformation("consume hackathon started event");

        foreach (var ctx in context.Message)
        {
            hackathonService.StartHackathon(ctx.Message.Id);
        }
        
        return Task.CompletedTask;
    }
}