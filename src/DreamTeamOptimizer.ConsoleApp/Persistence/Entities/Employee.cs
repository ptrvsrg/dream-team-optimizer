using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

public enum Grade
{
    JUNIOR,
    TEAM_LEAD
}

[Table("employees")]
public class Employee
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; }
    
    [Required]
    [Column("name", TypeName = "text")]
    public string Name { get; set; }
    
    [Required]
    [Column("grade", TypeName = "text")]
    public Grade Grade { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("created_at")]
    public DateTime CreatedAt { get; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("updated_at")]
    public DateTime UpdatedAt { get; }

    public ICollection<Hackathon> Hackathons { get; } = new List<Hackathon>();
}