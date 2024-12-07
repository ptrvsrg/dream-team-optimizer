using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.ConsoleApp.Interfaces.Services;

public interface IEmployeeService
{
    List<Employee> FindAllJuniors();
    List<Employee> FindAllTeamLeads();
}