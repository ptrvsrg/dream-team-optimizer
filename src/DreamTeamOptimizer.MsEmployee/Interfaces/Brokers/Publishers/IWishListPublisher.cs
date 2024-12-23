using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsEmployee.Interfaces.Brokers.Publishers;

public interface IWishListPublisher
{
    void SendWishList(WishList wishList, int hackathonId);
}