using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Repositories;

public class TeamRepository(AppDbContext dbContext): ITeamRepositroy
{
    public void Insert(Team team)
    {
        dbContext.Teams.Add(team);
    }

    public void InsertAll(List<Team> teams)
    {
        teams.ForEach(t => dbContext.Teams.Add(t));
    }

    public void Save()
    {
        dbContext.SaveChanges();
    }
}