using DreamTeamOptimizer.Core.Entities;
using DreamTeamOptimizer.Core.Helpers;
using DreamTeamOptimizer.Strategies;
using log4net;

namespace DreamTeamOptimizer.ConsoleApp;

public class HackathonWorker
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

    public static void Run(string juniorsFilePath, string teamLeadsFilePath,
        StrategyType strategyType = StrategyType.GaleShapley, int hackathonsCount = 1000, int threadCount = 1)
    {
        var juniors = CsvLoader.Load<Employee>(juniorsFilePath);
        var teamLeads = CsvLoader.Load<Employee>(teamLeadsFilePath);
        var strategy = StrategyFactory.NewStrategy(strategyType);
        var hrManager = new HRManager(strategy);
        var hrDirector = new HRDirector();

        var hackathonNum = 1;
        var totalHarmonicity = 0.0;
        var lockObj = new object();

        Parallel.For(0, hackathonsCount, new ParallelOptions{ MaxDegreeOfParallelism = threadCount }, _ =>
        {
            var hackathon = new Hackathon(juniors, teamLeads, hrManager, hrDirector);
            var harmonicity = hackathon.Start();

            lock (lockObj)
            {
                totalHarmonicity += harmonicity;
                var averageHarmonicity = totalHarmonicity / hackathonNum;
                Logger.Info(
                    $"Hackathon {hackathonNum}: harmonicity={harmonicity}, average_harmonicity={averageHarmonicity}");
                hackathonNum++;
            }
        });
    }
}