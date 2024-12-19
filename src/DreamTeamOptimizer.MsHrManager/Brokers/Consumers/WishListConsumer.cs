using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using MassTransit;

namespace DreamTeamOptimizer.MsHrManager.Brokers.Consumers;

public class WishListConsumer(ILogger<WishListConsumer> logger, IWishListService wishListService)
    : IConsumer<Batch<WishListEvent>>
{
    public Task Consume(ConsumeContext<Batch<WishListEvent>> context)
    {
        logger.LogInformation("consume wishlist event");

        foreach (var ctx in context.Message)
        {
            wishListService.SaveWishListToSession(ctx.Message.WishList, ctx.Message.HackathonId);
        }
        
        return Task.CompletedTask;
    }
}