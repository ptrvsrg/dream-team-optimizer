using DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

namespace DreamTeamOptimizer.ConsoleApp.Services.Mappers;

public class EmployeeMapper
{
    public static Employee ToEntity(Core.Models.Employee employeeModel)
    {
        return new Employee
        {
            Id = employeeModel.Id,
            Name = employeeModel.Name,
            Grade = ToEntityGrade(employeeModel.Grade)
        };
    }

    public static List<Employee> ToEntities(List<Core.Models.Employee> employeeModels)
    {
        return employeeModels.Select(ToEntity).ToList();
    }

    public static Core.Models.Employee ToModel(Employee employee)
    {
        return new Core.Models.Employee(employee.Id, employee.Name, ToModelGrade(employee.Grade));
    }

    public static List<Core.Models.Employee> ToModels(List<Employee> employees)
    {
        return employees.Select(ToModel).ToList();
    }

    private static Core.Models.Grade ToModelGrade(Grade grade)
    {
        switch (grade)
        {
            case Grade.JUNIOR:
                return Core.Models.Grade.JUNIOR;
            case Grade.TEAM_LEAD:
                return Core.Models.Grade.TEAM_LEAD;
            default:
                throw new ArgumentOutOfRangeException(nameof(grade), grade, null);
        }
    }

    private static Grade ToEntityGrade(Core.Models.Grade grade)
    {
        switch (grade)
        {
            case Core.Models.Grade.JUNIOR:
                return Grade.JUNIOR;
            case Core.Models.Grade.TEAM_LEAD:
                return Grade.TEAM_LEAD;
            default:
                throw new ArgumentOutOfRangeException(nameof(grade), grade, null);
        }
    }
}