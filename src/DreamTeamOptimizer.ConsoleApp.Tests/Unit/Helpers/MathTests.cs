using CoreHelpers = DreamTeamOptimizer.ConsoleApp.Helpers;
using Xunit;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Unit.Helpers;

public class MathTests
{
    [Theory]
    [InlineData(new [] { 1.0, 2.0, 4.0 }, 1.7142857142857142)]
    [InlineData(new [] { 3.0, 3.0, 3.0, 3.0, 3.0 }, 3.0)]
    [InlineData(new [] { 10.0, 5.0 }, 6.666666666666667)]
    public void CalculateHarmonicMean_WhenValidList_ThenReturnCorrect(double[] numbers, double expected)
    {
        // Act
        var harmonicMean = CoreHelpers.Math.CalculateHarmonicMean(numbers.ToList());

        // Assert
        Assert.Equal(expected, harmonicMean, precision: 9); // Точность до 9 знаков после запятой
    }

    [Fact]
    public void CalculateHarmonicMean_WhenListWithZero_ThenThrowException()
    {
        // Prepare
        var numbers = new List<double> { 3.0, 3.0, 3.0, 3.0, 0.0 };

        // Act and Assert
        Assert.Throws<DivideByZeroException>(() =>
            CoreHelpers.Math.CalculateHarmonicMean(numbers));
    }

    [Fact]
    public void CalculateHarmonicMean_WhenEmptyList_ThenThrowException()
    {
        // Prepare
        var numbers = new List<double>();

        // Act and Assert
        Assert.Throws<DivideByZeroException>(() =>
            CoreHelpers.Math.CalculateHarmonicMean(numbers));
    }

    [Theory]
    [InlineData(new [] { 1e-10, 1e-10, 1e-10 }, 1e-10)]
    [InlineData(new [] { 1e10, 1e10, 1e10 }, 1e10)]
    public void CalculateHarmonicMean_WhenExtremeValues_ThenHandleCorrectly(double[] numbers, double expected)
    {
        // Act
        var harmonicMean = CoreHelpers.Math.CalculateHarmonicMean(numbers.ToList());

        // Assert
        Assert.Equal(expected, harmonicMean, precision: 9);
    }
}