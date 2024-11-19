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
    public void LoadJuniorsFromFile(string path)
    {
        Log.Information($"load juniors from file {path}");

        var employeeModels = CsvLoader.Load<Employee>(path);
        var employees = EmployeeMapper.ToEntities(employeeModels, Grade.JUNIOR);

        employeeRepository.CreateAll(employees);
    }

    public void LoadTeamLeadsFromFile(string path)
    {
        Log.Information($"load team leads from file {path}");
        
        var employeeModels = CsvLoader.Load<Employee>(path);
        var employees = EmployeeMapper.ToEntities(employeeModels, Grade.TEAM_LEAD);

        employeeRepository.CreateAll(employees);
    }

    public List<Employee> FindAllJuniors()
    {
        Log.Information("find all juniors");
        var employees = employeeRepository.Find(e => e.Grade == Grade.JUNIOR);
        return EmployeeMapper.ToModels(employees);
    }

    public List<Employee> FindAllTeamLeads()
    {
        Log.Information("find all team leads");
        var employees = employeeRepository.Find(e => e.Grade == Grade.TEAM_LEAD);
        return EmployeeMapper.ToModels(employees);
    }
}