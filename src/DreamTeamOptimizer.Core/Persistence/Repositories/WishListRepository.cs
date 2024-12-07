using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Persistence.Entities;
using DreamTeamOptimizer.Core.Persistence.Repositories;

namespace DreamTeamOptimizer.Core.Persistence.Repositories;

public class WishListRepository: GenericRepository<WishList>, IWishListRepository
{
    public WishListRepository(AppDbContext dbContext): base(dbContext)
    {
    }
}