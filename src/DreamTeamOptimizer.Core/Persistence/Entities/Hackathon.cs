using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamTeamOptimizer.Core.Persistence.Entities;

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
    public virtual int Id { get; set; }

    [Required]
    [Column("status", TypeName = "text")]
    public virtual HackathonStatus Status { get; set; }

    [Required]
    [Column("result", TypeName = "double precision")]
    public virtual double Result { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("created_at")]
    public virtual DateTime CreatedAt { get; }

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column("updated_at")]
    public virtual DateTime UpdatedAt { get; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
    public virtual ICollection<Satisfaction> Satisfactions { get; set; } = new List<Satisfaction>();
    public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();
}