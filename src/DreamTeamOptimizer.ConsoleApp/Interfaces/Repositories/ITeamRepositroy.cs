using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;

public interface ITeamRepositroy
{
    void Insert(Team team);
    void InsertAll(List<Team> teams);
    void Save();
}