using DreamTeamOptimizer.Core.Models;

namespace DreamTeamOptimizer.MsHrManager.Interfaces.Services;

public interface IEmployeeService
{
    List<Employee> FindAllJuniors();
    List<Employee> FindAllTeamLeads();
}