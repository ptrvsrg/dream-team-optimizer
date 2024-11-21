using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

[Table("hackathons_employees")]
[PrimaryKey(nameof(EmployeeId), nameof(HackathonId))]
public class HackathonEmployee
{
    [Required]
    [Column("employee_id")]
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;
    
    [Required]
    [Column("hackathon_id")]
    public int HackathonId { get; set; }
    public Hackathon Hackathon { get; set; } = null!;
}