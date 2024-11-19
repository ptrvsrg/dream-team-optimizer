using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

[Table("satisfactions")]
[Index(nameof(EmployeeId), nameof(HackathonId), IsUnique = true)]
public class Satisfaction
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
    [Column("hackathon_id")]
    public int HackathonId { get; set;  }
    public Hackathon Hackathon { get; set;  }
    
    [Column("rank", TypeName = "double precision")]
    public double Rank { get; set;  }
}