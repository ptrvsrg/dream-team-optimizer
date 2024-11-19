using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.ConsoleApp.Interfaces.Services;

public interface IEmployeeService
{
    void LoadJuniorsFromFile(string path);
    void LoadTeamLeadsFromFile(string path);
    List<Employee> FindAllJuniors();
    List<Employee> FindAllTeamLeads();
}