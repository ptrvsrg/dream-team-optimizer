namespace DreamTeamOptimizer.Core.Exceptions;

public class WishListNotFoundException : Exception
{
    private int _employeeId;

    public WishListNotFoundException(int employeeId) : base($"Wish list by employee ID {employeeId} not found")
    {
        _employeeId = employeeId;
    }

    public WishListNotFoundException(string? message, int employeeId) : base(message)
    {
        _employeeId = employeeId;
    }
    
    public int EmployeeId => _employeeId;
}