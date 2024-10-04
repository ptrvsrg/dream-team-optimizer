namespace DreamTeamOptimizer.Core.Entities;

public class Hackathon
{
    private readonly IEnumerable<Employee> _juniors;
    private readonly IEnumerable<Employee> _teamLeads;
    private readonly HrManager _hrManager;
    private readonly HrDirector _hrDirector;

    public Hackathon(List<TeamLead> teamLeads, List<Junior> juniors, HrManager hrManager, HrDirector hrDirector)
    {
        _juniors = juniors;
        _teamLeads = teamLeads;
        _hrManager = hrManager;
        _hrDirector = hrDirector;
    }

    public double Conduct()
    {
        var teamLeadsWishLists = _hrManager.VoteEmployees(_teamLeads, _juniors);
        var juniorsWishLists = _hrManager.VoteEmployees(_juniors, _teamLeads);
        var teams = _hrManager.BuildTeams(_teamLeads, _juniors, teamLeadsWishLists, juniorsWishLists);

        return _hrDirector.CalculateDistributionHarmony(teams, teamLeadsWishLists, juniorsWishLists);
    }
}