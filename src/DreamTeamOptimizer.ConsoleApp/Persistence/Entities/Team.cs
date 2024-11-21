using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.ConsoleApp.Persistence.Entities;

[Table("teams")]
[Index(nameof(JuniorId), nameof(TeamLeadId), nameof(HackathonId), IsUnique = true)]
public class Team
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; }

    [Required]
    [Column("junior_id")]
    public int JuniorId { get; set; }
    public Employee Junior { get; set; }
    
    [Required]
    [Column("team_lead_id")]
    public int TeamLeadId { get; set; }
    public Employee TeamLead { get; set; }
    
    [Required]
    [Column("hackathon_id")]
    public int HackathonId { get; set; }
    public Hackathon Hackathon { get; set; }
}