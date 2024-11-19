using DreamTeamOptimizer.ConsoleApp.Helpers;
using CoreHelpers = DreamTeamOptimizer.ConsoleApp.Helpers;
using Xunit;
using Math = DreamTeamOptimizer.ConsoleApp.Helpers.Math;

namespace DreamTeamOptimizer.ConsoleApp.Tests.Helpers;

public class MathTests
{
    [Fact]
    public void CalculateHarmonicMean_ListWithoutZeros_ShouldHaveCorrect()
    {
        // Prepare
        var numbers = new List<double>
        {
            3.0, 3.0, 3.0, 3.0, 3.0
        };

        // Act
        var harmonicMean = ConsoleApp.Helpers.Math.CalculateHarmonicMean(numbers);

        // Check
        Assert.NotStrictEqual(3.0, harmonicMean);
    }
    
    [Fact]
    public void GetWishlists_ListWithZero_ThrowException()
    {
        // Prepare
        var numbers = new List<double>
        {
            3.0, 3.0, 3.0, 3.0, 0.0
        };

        // Act and check
        Assert.Throws<DivideByZeroException>(() =>
            ConsoleApp.Helpers.Math.CalculateHarmonicMean(numbers));
    }
    
    [Fact]
    public void GetWishlists_EmptyList_ThrowException()
    {
        // Prepare
        var numbers = new List<double>();

        // Act and check
        Assert.Throws<DivideByZeroException>(() =>
            ConsoleApp.Helpers.Math.CalculateHarmonicMean(numbers));
    }
}