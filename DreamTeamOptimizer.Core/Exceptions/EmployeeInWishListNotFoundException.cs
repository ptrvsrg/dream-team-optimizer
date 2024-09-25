namespace DreamTeamOptimizer.Core.Exceptions;

public class EmployeeInWishListNotFoundException : Exception
{
    private int? EmployeeId { get; }
    private int? SelectedEmployeeId { get; }

    public EmployeeInWishListNotFoundException() : base(
        "Employee not found in wish list")
    {}

    public EmployeeInWishListNotFoundException(int employeeId, int selectedEmployeeId) : base(
        $"Employee with ID {selectedEmployeeId} not found in wish list of employee with ID {employeeId}")
    {
        EmployeeId = employeeId;
        SelectedEmployeeId = selectedEmployeeId;
    }

    public EmployeeInWishListNotFoundException(string? message, int employeeId, int selectedEmployeeId) : base(message)
    {
        EmployeeId = employeeId;
        SelectedEmployeeId = selectedEmployeeId;
    }
}