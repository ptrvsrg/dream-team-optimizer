using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.Core.Models.Events;
using DreamTeamOptimizer.MsEmployee.Interfaces.Brokers.Publishers;
using MassTransit;

namespace DreamTeamOptimizer.MsEmployee.Brokers.Publishers;

public class WishListPublisher(ILogger<WishListPublisher> logger, IBus bus) : IWishListPublisher
{
    public void SendWishList(WishList wishList, int hackathonId)
    {
        logger.LogInformation("publish wish list event");
        bus.Publish<WishListEvent>(new (hackathonId, wishList));
    }
}