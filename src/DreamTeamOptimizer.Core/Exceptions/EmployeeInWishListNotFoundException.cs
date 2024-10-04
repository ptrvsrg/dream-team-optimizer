namespace DreamTeamOptimizer.Core.Exceptions;

public class EmployeeInWishListNotFoundException : Exception
{
    private int? _employeeId;
    private int? _selectedEmployeeId;

    public EmployeeInWishListNotFoundException() : base("Employee not found in wish list")
    {
    }

    public EmployeeInWishListNotFoundException(int employeeId, int selectedEmployeeId) : base(
        $"Employee with ID {selectedEmployeeId} not found in wish list of employee with ID {employeeId}")
    {
        _employeeId = employeeId;
        _selectedEmployeeId = selectedEmployeeId;
    }

    public EmployeeInWishListNotFoundException(string? message, int employeeId, int selectedEmployeeId) : base(message)
    {
        _employeeId = employeeId;
        _selectedEmployeeId = selectedEmployeeId;
    }

    public int? EmployeeId => _employeeId;
    
    public int? SelectedEmployeeId => _selectedEmployeeId;
}