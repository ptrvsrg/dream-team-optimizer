namespace DreamTeamOptimizer.ConsoleApp.Helpers;

public class Math
{
    public static double CalculateHarmonicMean(List<double> values)
    {
        var sum = 0.0;
        foreach (var value in values)
        {
            if (value == 0) throw new DivideByZeroException();
            sum += 1.0 / value;
        }

        if (sum == 0.0) throw new DivideByZeroException();
        return values.Count / sum;
    }
}