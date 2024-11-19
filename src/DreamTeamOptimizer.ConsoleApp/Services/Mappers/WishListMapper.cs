using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.ConsoleApp.Services.Mappers;

public class WishListMapper
{
    public static List<Preference> ToEntities(WishList wishList)
    {
        return wishList.DesiredEmployees.Select(de => new Preference
        {
            EmployeeId = wishList.EmployeeId,
            DesiredEmployeeId = de
        }).ToList();
    }

    public static List<Preference> ToEntities(List<WishList> wishLists)
    {
        var preferences = new List<Preference>();
        wishLists.ForEach(w => preferences.AddRange(ToEntities(w)));
        return preferences;
    }

    public static List<WishList> ToModels(List<Preference> preferences)
    {
        var dict = new Dictionary<int, List<int>>();

        foreach (var preference in preferences)
        {
            if (!dict.TryGetValue(preference.EmployeeId, out var desiredEmployees))
            {
                desiredEmployees = new List<int>();
            }
            
            desiredEmployees.Add(preference.DesiredEmployeeId);
            dict.Add(preference.EmployeeId, desiredEmployees);
        }

        return dict.Select(e => new WishList(e.Key, e.Value.ToArray())).ToList();
    }
}