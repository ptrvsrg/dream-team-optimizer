using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.Core.Persistence.Entities;

[Table("teams")]
[Index(nameof(JuniorId), nameof(TeamLeadId), nameof(HackathonId), IsUnique = true)]
public class Team
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public virtual int Id { get; }

    [Required] [Column("junior_id")] public virtual int JuniorId { get; set; }
    public virtual Employee Junior { get; set; }

    [Required] [Column("team_lead_id")] public virtual int TeamLeadId { get; set; }
    public virtual Employee TeamLead { get; set; }

    [Required] [Column("hackathon_id")] public virtual int HackathonId { get; set; }
    public virtual Hackathon Hackathon { get; set; }
}