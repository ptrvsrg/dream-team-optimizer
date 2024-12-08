using System.ComponentModel.DataAnnotations;

namespace DreamTeamOptimizer.MsEmployee.Config;

public class AppConfig
{
    public const string Name = "Application";

    [Required]  
    public int EmployeeID { get; set; }

    [Required]  
    public string EmployeeName { get; set; }
}