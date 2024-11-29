using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.Core.Persistence.Repositories;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class WishListRepository: GenericRepository<WishList>, IWishListRepository
{
    public WishListRepository(AppDbContext dbContext): base(dbContext)
    {
    }
}