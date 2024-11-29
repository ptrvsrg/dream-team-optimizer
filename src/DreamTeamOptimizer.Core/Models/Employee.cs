using System.Runtime.Serialization;

namespace DreamTeamOptimizer.Core.Models;

public enum Grade
{
    [EnumMember(Value = "UNKNOWN")] UNKNOWN,
    
    [EnumMember(Value = "JUNIOR")] JUNIOR,

    [EnumMember(Value = "TEAM_LEAD")] TEAM_LEAD,
}

public record Employee(int Id, string Name, Grade Grade = Grade.UNKNOWN);