using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsHrDirector.Interfaces.Services;
using MassTransit;

namespace DreamTeamOptimizer.MsHrDirector.Brokers.Consumers;

public class HackathonResultConsumer(
    ILogger<HackathonResultConsumer> logger,
    IHackathonService hackathonService
) : IConsumer<Batch<HackathonResultEvent>>
{
    public Task Consume(ConsumeContext<Batch<HackathonResultEvent>> context)
    {
        logger.LogInformation("consume hackathon result events");

        foreach (var ctx in context.Message)
        {
            hackathonService.SaveResult(ctx.Message.Teams, ctx.Message.TeamLeadsWishlists, ctx.Message.JuniorsWishlists,
                ctx.Message.Id);
        }

        return Task.CompletedTask;
    }
}