using DreamTeamOptimizer.Core.Persistence.Entities;

namespace DreamTeamOptimizer.MsHrManager.Services.Mappers;

public class WishListMapper
{
    public static WishList ToEntity(Core.Models.WishList wishList)
    {
        return new WishList
        {
            EmployeeId = wishList.EmployeeId,
            DesiredEmployeeIds = wishList.DesiredEmployees
        };
    }

    public static List<WishList> ToEntities(List<Core.Models.WishList> wishLists)
    {
        return wishLists.Select(ToEntity).ToList();
    }

    public static Core.Models.WishList ToModel(WishList wishList)
    {
        return new Core.Models.WishList(wishList.EmployeeId, wishList.DesiredEmployeeIds);
    }

    public static List<Core.Models.WishList> ToModels(List<WishList> wishLists)
    {
        return wishLists.Select(ToModel).ToList();
    }
}