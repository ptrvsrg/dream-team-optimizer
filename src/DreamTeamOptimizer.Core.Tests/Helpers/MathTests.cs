using CoreHelpers = DreamTeamOptimizer.Core.Helpers;
using Xunit;

namespace DreamTeamOptimizer.Core.Tests.Helpers;

public class MathTests
{
    [Theory]
    [InlineData(new double[] { 1.0, 2.0, 4.0 }, 1.7142857142857142)]
    [InlineData(new double[] { 3.0, 3.0, 3.0, 3.0, 3.0 }, 3.0)]
    [InlineData(new double[] { 10.0, 5.0 }, 6.666666666666667)]
    public void CalculateHarmonicMean_ValidList_ShouldReturnCorrect(double[] numbers, double expected)
    {
        // Act
        var harmonicMean = CoreHelpers.Math.CalculateHarmonicMean(numbers.ToList());

        // Assert
        Assert.Equal(expected, harmonicMean, precision: 9); // Точность до 9 знаков после запятой
    }

    [Fact]
    public void CalculateHarmonicMean_ListWithZero_ThrowException()
    {
        // Prepare
        var numbers = new List<double> { 3.0, 3.0, 3.0, 3.0, 0.0 };

        // Act and Assert
        Assert.Throws<DivideByZeroException>(() =>
            CoreHelpers.Math.CalculateHarmonicMean(numbers));
    }

    [Fact]
    public void CalculateHarmonicMean_EmptyList_ThrowException()
    {
        // Prepare
        var numbers = new List<double>();

        // Act and Assert
        Assert.Throws<DivideByZeroException>(() =>
            CoreHelpers.Math.CalculateHarmonicMean(numbers));
    }

    [Theory]
    [InlineData(new double[] { 1e-10, 1e-10, 1e-10 }, 1e-10)]
    [InlineData(new double[] { 1e10, 1e10, 1e10 }, 1e10)]
    public void CalculateHarmonicMean_ExtremeValues_ShouldHandleCorrectly(double[] numbers, double expected)
    {
        // Act
        var harmonicMean = CoreHelpers.Math.CalculateHarmonicMean(numbers.ToList());

        // Assert
        Assert.Equal(expected, harmonicMean, precision: 9);
    }
}