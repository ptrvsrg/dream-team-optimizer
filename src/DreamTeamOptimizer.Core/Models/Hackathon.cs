using System.Runtime.Serialization;

namespace DreamTeamOptimizer.Core.Models;

public enum HackathonStatus
{
    [EnumMember(Value = "IN_PROCESSING")] IN_PROCESSING,

    [EnumMember(Value = "COMPLETED")] COMPLETED,

    [EnumMember(Value = "FAILED")] FAILED
}

public record Hackathon(
    int Id,
    HackathonStatus Status,
    double Result,
    List<Employee> Employees,
    List<WishList> WishLists,
    List<Team> Teams,
    List<Satisfaction> Satisfactions);