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
    public virtual int Id { get; set; }

    [Required]
    [Column("name", TypeName = "text")]
    public virtual string Name { get; set; }

    [Required]
    [Column("grade", TypeName = "text")]
    public virtual Grade Grade { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("created_at")]
    public virtual DateTime CreatedAt { get; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("updated_at")]
    public virtual DateTime UpdatedAt { get; }

    public virtual ICollection<Hackathon> Hackathons { get; } = new List<Hackathon>();
}