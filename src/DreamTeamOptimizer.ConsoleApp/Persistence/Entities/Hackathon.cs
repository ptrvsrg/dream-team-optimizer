using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

public enum HackathonStatus
{
    IN_PROCESSING,
    COMPLETED,
    FAILED
}

[Table("hackathons")]
public class Hackathon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; }
    
    [Required]
    [Column("status", TypeName = "text")]
    public HackathonStatus Status { get; set; }
    
    [Required]
    [Column("result", TypeName = "double precision")]
    public double Result { get; set; }
    
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Team> Teams { get; set; } = new List<Team>();
    public ICollection<Satisfaction> Satisfactions { get; set; } = new List<Satisfaction>();
    public ICollection<Preference> Preferences { get; set; } = new List<Preference>();
}