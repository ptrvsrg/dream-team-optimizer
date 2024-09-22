using DreamTeamOptimizer.Core;

namespace DreamTeamOptimizer.Strategies;

public class StrategyFactory
{
    public static IStrategy NewStrategy(StrategyType type)
    {
        switch (type)
        {
            case StrategyType.GaleShapley:
                return new GaleShapleyStrategy();
            case StrategyType.BipartiteGraphWithRating:
                return new BipartiteGraphWithRatingStrategy();
            default:
                throw new NotSupportedException("Unsupported strategy");
        }
    }
}