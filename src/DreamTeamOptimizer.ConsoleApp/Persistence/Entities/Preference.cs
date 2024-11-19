using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

[Table("preferences")]
[Index(nameof(EmployeeId), nameof(DesiredEmployeeId), nameof(HackathonId), IsUnique = true)]
public class Preference
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; }
    
    [Required]
    [Column("employee_id")]
    public int EmployeeId { get; set;  }
    public Employee Employee { get; set;  }
    
    [Required]
    [Column("desired_employee_id")]
    public int DesiredEmployeeId { get; set;  }
    public Employee DesiredEmployee { get; set;  }
    
    [Required]
    [Column("hackathon_id")]
    public int HackathonId { get; set;  }
    public Hackathon Hackathon { get; set;  }
}