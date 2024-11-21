using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Services.Mappers;

public class EmployeeMapper
{
    public static Employee ToEntity(Core.Models.Employee employeeModel, Grade grade)
    {
        return new Employee
        {
            Name = employeeModel.Name,
            Grade = grade
        };
    }
    
    public static List<Employee> ToEntities(List<Core.Models.Employee> employeeModels, Grade grade)
    {
        return employeeModels.Select(e => ToEntity(e, grade)).ToList();
    }
    
    public static Core.Models.Employee ToModel(Employee employee)
    {
        return new Core.Models.Employee(employee.Id, employee.Name);
    }
    
    public static List<Core.Models.Employee> ToModels(List<Employee> employees)
    {
        return employees.Select(ToModel).ToList();
    }
}