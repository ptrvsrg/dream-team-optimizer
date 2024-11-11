using System.Runtime.Serialization;

namespace DreamTeamOptimizer.Strategies;

public enum StrategyType
{
    [EnumMember(Value = "GaleShapley")]
    GaleShapley,
    
    [EnumMember(Value = "BipartiteGraph")]
    BipartiteGraph,
    
    [EnumMember(Value = "WeightedPreference")]
    WeightedPreference
}