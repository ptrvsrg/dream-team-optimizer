using System.ComponentModel.DataAnnotations;
using DreamTeamOptimizer.Strategies;

namespace DreamTeamOptimizer.ConsoleApp.Config;

public class Config
{
    [Required] 
    public string JuniorsFilePath { get; set; } = "";
    
    [Required]
    public string TeamLeadsFilePath { get; set; } = "";

    [Required, EnumDataType(typeof(StrategyType))]  
    public StrategyType Strategy { get; set; }

    [Range(0, Int32.MaxValue)]
    public int HackathonCount { get; set; } = 1;
}