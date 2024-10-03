using CommandLine;
using DreamTeamOptimizer.Strategies;
using log4net.Config;

namespace DreamTeamOptimizer.ConsoleApp;

public class Program
{
    private class Options
    {
        [Option('j', "juniors", Required = true, HelpText = "Path to the juniors CSV file.")]
        public string JuniorsFilePath { get; set; } = null!;

        [Option('t', "teamleads", Required = true, HelpText = "Path to the team leads CSV file.")]
        public string TeamLeadsFilePath { get; set; } = null!;

        [Option('s', "strategy", Default = StrategyType.GaleShapley,
            HelpText =
                "Strategy to use for team building (GaleShapley, BipartiteGraph, WeightedPreference).")]
        public StrategyType StrategyType { get; set; }

        [Option('n', "hackathons", Default = 1000, HelpText = "Number of hackathons to conduct.")]
        public int HackathonsCount { get; set; }

        [Option('c', "concurrency", Default = 1, HelpText = "Number of threads.")]
        public int ThreadCount { get; set; }
    }

    private static void Main(string[] args)
    {
        BasicConfigurator.Configure();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(options => HackathonWorker.Run(options.JuniorsFilePath, options.TeamLeadsFilePath,
                options.StrategyType, options.HackathonsCount, options.ThreadCount));
    }
}