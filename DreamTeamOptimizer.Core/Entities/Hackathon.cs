namespace DreamTeamOptimizer.Core.Entities;

public class Hackathon
{
    private IEnumerable<Employee> Juniors { get; }
    private IEnumerable<Employee> TeamLeads { get; }
    private HRManager HrManager { get; }
    private HRDirector HrDirector { get; }

    public Hackathon(IEnumerable<Employee> juniors, IEnumerable<Employee> teamLeads, HRManager hrManager,
        HRDirector hrDirector)
    {
        Juniors = juniors;
        TeamLeads = teamLeads;
        HrManager = hrManager;
        HrDirector = hrDirector;
    }

    public double Start()
    {
        var teamLeadsWishLists = HrManager.VoteEmployees(TeamLeads, Juniors);
        var juniorsWishLists = HrManager.VoteEmployees(Juniors, TeamLeads);
        var teams = HrManager.BuildTeams(TeamLeads, Juniors, teamLeadsWishLists, juniorsWishLists);

        return HrDirector.CalculateDistributionHarmony(teams, teamLeadsWishLists, juniorsWishLists);
    }
}