using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence.Entities;

namespace DreamTeamOptimizer.Core.Persistence.Repositories;

public class WishListRepository : GenericRepository<WishList>, IWishListRepository
{
    public WishListRepository(AppDbContext dbContext) : base(dbContext)
    {
    }
}