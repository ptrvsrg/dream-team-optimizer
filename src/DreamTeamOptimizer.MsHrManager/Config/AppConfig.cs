using System.ComponentModel.DataAnnotations;
using DreamTeamOptimizer.Strategies;

namespace DreamTeamOptimizer.MsHrManager.Config;

public class AppConfig
{
    public const string Name = "Application";

    [Required, EnumDataType(typeof(StrategyType))]  
    public StrategyType Strategy { get; set; }
}