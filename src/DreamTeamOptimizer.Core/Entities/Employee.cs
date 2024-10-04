namespace DreamTeamOptimizer.Core.Entities;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Employee(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public WishList GetWishlist(IEnumerable<Employee> employees)
    {
        var employeesIds = employees
            .Select(employee => employee.Id)
            .OrderBy(_ => Random.Shared.Next())
            .ToArray();
        return new WishList(Id, employeesIds);
    }
}