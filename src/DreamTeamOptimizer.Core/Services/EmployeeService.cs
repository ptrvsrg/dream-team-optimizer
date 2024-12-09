using DreamTeamOptimizer.Core.Helpers;
using DreamTeamOptimizer.Core.Interfaces.IServices;
using Nsu.HackathonProblem.Contracts;

namespace DreamTeamOptimizer.Core.Services;

public class EmployeeService(string juniorPath, string teamLeadPath): IEmployeeService
{
    public List<Employee> FindAllJuniors()
    {
        return CsvLoader.Load<Employee>(juniorPath);
    }

    public List<Employee> FindAllTeamLeads()
    {
        return CsvLoader.Load<Employee>(teamLeadPath);
    }
}