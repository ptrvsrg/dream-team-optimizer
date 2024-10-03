namespace DreamTeamOptimizer.Core.Exceptions;

public class WishListNotFoundException : Exception
{
    private int EmployeeId { get; }

    public WishListNotFoundException(int employeeId) : base($"Wish list by employee ID {employeeId} not found")
    {
        EmployeeId = employeeId;
    }

    public WishListNotFoundException(string? message, int employeeId) : base(message)
    {
        EmployeeId = employeeId;
    }
}