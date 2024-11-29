using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

[Table("hackathons_employees")]
[PrimaryKey(nameof(EmployeeId), nameof(HackathonId))]
public class HackathonEmployee
{
    [Required] [Column("employee_id")] public virtual int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; } = null!;

    [Required] [Column("hackathon_id")] public virtual int HackathonId { get; set; }
    public virtual Hackathon Hackathon { get; set; } = null!;
}