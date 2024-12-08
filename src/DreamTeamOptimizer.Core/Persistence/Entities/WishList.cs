using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DreamTeamOptimizer.Core.Persistence.Entities;

[Table("wish_lists")]
[Index(nameof(EmployeeId), nameof(HackathonId), IsUnique = true)]
public class WishList
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public virtual int Id { get; set; }

    [Required] [Column("employee_id")] public virtual int EmployeeId { get; set; }
    public virtual Employee Employee { get; set; }

    [Required]
    [Column("desired_employees")]
    public virtual int[] DesiredEmployeeIds { get; set; }

    [Required] [Column("hackathon_id")] public virtual int HackathonId { get; set; }
    public virtual Hackathon Hackathon { get; set; }
}