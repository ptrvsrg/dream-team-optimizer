namespace DreamTeamOptimizer.Core;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; }

    public WishList GetWishlist(IEnumerable<Employee> employees)
    {
        var employeesIds = employees
            .Select(employee => employee.Id)
            .OrderBy(_ => Random.Shared.Next())
            .ToArray();
        return new WishList(Id, employeesIds);
    }
}