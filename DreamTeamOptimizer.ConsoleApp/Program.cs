using CommandLine;
using DreamTeamOptimizer.Core;
using DreamTeamOptimizer.Strategies;
using log4net;
using log4net.Config;

namespace DreamTeamOptimizer.ConsoleApp;

class Program
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
    
    private class Options
    {
        [Option('j', "juniors", Required = true, HelpText = "Path to the juniors CSV file.")]
        public string JuniorsFilePath { get; set; }

        [Option('t', "teamleads", Required = true, HelpText = "Path to the team leads CSV file.")]
        public string TeamLeadsFilePath { get; set; }

        [Option('s', "strategy", Default = StrategyType.GaleShapley,
            HelpText = "Strategy to use for team building (GaleShapley, BipartiteGraphWithRating).")]
        public StrategyType Strategy { get; set; }

        [Option('n', "hackathons", Default = 1000, HelpText = "Number of hackathons to conduct.")]
        public int HackathonsCount { get; set; }
    }

    private static void Main(string[] args)
    {
        BasicConfigurator.Configure();
        Parser.Default.ParseArguments<Options>(args)
            .WithParsed(Run);
    }

    private static void Run(Options options)
    {
        var juniors = CsvLoader.Load<Employee>(options.JuniorsFilePath);
        var teamLeads = CsvLoader.Load<Employee>(options.TeamLeadsFilePath);
        var strategy = StrategyFactory.NewStrategy(options.Strategy);
        var hrManager = new HRManager(strategy);
        var hrDirector = new HRDirector();

        int hackathonNum = 1;
        double totalHarmonicity = 0;
        object lockObj = new object();

        Parallel.For(0, options.HackathonsCount, new ParallelOptions(), _ =>
        {
            Hackathon hackathon = new Hackathon(juniors, teamLeads, hrManager, hrDirector);
            double harmonicity = hackathon.Start();

            lock (lockObj)
            {
                totalHarmonicity += harmonicity;
                Logger.Info($"Hackathon {hackathonNum} harmonicity: {harmonicity}");
                hackathonNum++;
            }
        });

        double averageHarmonicity = totalHarmonicity / options.HackathonsCount;
        Logger.Info($"Average harmonicity across all hackathons: {averageHarmonicity}");
    }
}