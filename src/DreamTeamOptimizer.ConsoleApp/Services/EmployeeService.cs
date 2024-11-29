using DreamTeamOptimizer.ConsoleApp.Helpers;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Repositories;
using DreamTeamOptimizer.ConsoleApp.Interfaces.Services;
using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;
using DreamTeamOptimizer.ConsoleApp.Services.Mappers;
using Serilog;
using Employee = DreamTeamOptimizer.Core.Models.Employee;

namespace DreamTeamOptimizer.ConsoleApp.Services;

public class EmployeeService(IEmployeeRepository employeeRepository) : IEmployeeService
{
    public List<Employee> FindAllJuniors()
    {
        Log.Information("find all juniors");
        try
        {
            var employees = employeeRepository.Find(e => e.Grade == Grade.JUNIOR);
            return EmployeeMapper.ToModels(employees);
        }
        catch (Exception e)
        {
            Log.Warning("find all juniors failed: " + e.Message);
            return new List<Employee>();
        }
    }

    public List<Employee> FindAllTeamLeads()
    {
        Log.Information("find all team leads");
        try
        {
            var employees = employeeRepository.Find(e => e.Grade == Grade.TEAM_LEAD);
            return EmployeeMapper.ToModels(employees);
        }
        catch (Exception e)
        {
            Log.Warning("find all team leads failed: " + e.Message);
            return new List<Employee>();
        }
    }
}