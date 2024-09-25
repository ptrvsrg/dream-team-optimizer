using DreamTeamOptimizer.Core.Interfaces;

namespace DreamTeamOptimizer.Strategies;

public class StrategyFactory
{
    public static IStrategy NewStrategy(StrategyType type)
    {
        return type switch
        {
            StrategyType.GaleShapley => new GaleShapleyStrategy(),
            StrategyType.BipartiteGraphWithRating => new BipartiteGraphWithRatingStrategy(),
            _ => throw new NotSupportedException("Unsupported strategy")
        };
    }
}