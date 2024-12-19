using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Models.Sessions;

public class VotingSession
{
    public int Current { get; set; }
    
    public int Total { get; set; }

    public List<Employee> TeamLeads { get; set; }

    public List<Employee> Juniors { get; set; }

    public List<WishList> TeamLeadsWishlists { get; set; }

    public List<WishList> JuniorsWishlists { get; set; }
    
    public VotingSession(int current, int total, List<Employee> teamLeads, List<Employee> juniors, List<WishList> teamLeadsWishlists,
        List<WishList> juniorsWishlists)
    {
        Current = current;
        Total = total;
        TeamLeads = teamLeads;
        Juniors = juniors;
        TeamLeadsWishlists = teamLeadsWishlists;
        JuniorsWishlists = juniorsWishlists;
    }
}