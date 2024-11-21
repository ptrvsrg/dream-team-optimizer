using DreamTeamOptimizer.Core.Entities;

namespace DreamTeamOptimizer.Core.Interfaces.IServices;

public interface IEmployeeService
{
    List<Employee> FindAllJuniors();
    List<Employee> FindAllTeamLeads();
}