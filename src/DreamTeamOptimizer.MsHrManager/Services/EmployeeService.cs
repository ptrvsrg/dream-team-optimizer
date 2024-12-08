using System.Text.RegularExpressions;
using DreamTeamOptimizer.Core.Interfaces.Repositories;
using DreamTeamOptimizer.Core.Models;
using DreamTeamOptimizer.MsHrManager.Interfaces.Services;
using DreamTeamOptimizer.MsHrManager.Services.Mappers;
using Steeltoe.Discovery;
using Grade = DreamTeamOptimizer.Core.Persistence.Entities.Grade;

namespace DreamTeamOptimizer.MsHrManager.Services;

public class EmployeeService(
    ILogger<EmployeeService> logger,
    IDiscoveryClient discoveryClient,
    IEmployeeRepository employeeRepository) : IEmployeeService
{
    private const string Pattern = @"ms-employee-(\d+)";

    public List<Employee> FindAllJuniors()
    {
        logger.LogInformation("find all juniors");

        var ids = GetActiveEmployeesIds();
        var employees = employeeRepository.Find(e => e.Grade == Grade.JUNIOR && ids.Contains(e.Id));
        return EmployeeMapper.ToModels(employees);
    }

    public List<Employee> FindAllTeamLeads()
    {
        logger.LogInformation("find all team leads");

        var ids = GetActiveEmployeesIds();
        var employees = employeeRepository.Find(e => e.Grade == Grade.TEAM_LEAD && ids.Contains(e.Id));
        return EmployeeMapper.ToModels(employees);
    }

    private List<int> GetActiveEmployeesIds()
    {
        logger.LogDebug("get active employees ids");
        
        return discoveryClient.Services
            .Where(s => Regex.Match(s, Pattern).Success)
            .Select(s => Regex.Match(s, Pattern).Groups[1].Value)
            .Select(int.Parse)
            .ToList();
    }
}