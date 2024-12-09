using Nsu.HackathonProblem.Contracts;

namespace DreamTeamOptimizer.Strategies;

public class StrategyFactory
{
    public static ITeamBuildingStrategy NewStrategy(StrategyType type)
    {
        return type switch
        {
            StrategyType.GaleShapley => new GaleShapleyStrategy(),
            StrategyType.WeightedPreference => new WeightedPreferenceStrategy(),
            StrategyType.BipartiteGraph => new BipartiteGraphStrategy(),
            _ => throw new NotSupportedException("Unsupported strategy")
        };
    }
}