using System.Linq;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Reporting;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Tests for <see cref="HotFunctionsReport"/> aggregation logic and edge cases.
/// </summary>
public sealed class HotFunctionsReportTests
{
    /// <summary>
    /// Verifies that an empty root node produces an empty report without throwing.
    /// </summary>
    [Fact]
    public void Ctor_EmptyRoot_ProducesEmptyReport()
    {
        // Arrange
        var root = new FlameNode("root");
        
        // Act
        var report = new HotFunctionsReport(root);
        
        // Assert
        Assert.Empty(report.Functions);
        Assert.Equal(0.0, report.TotalSelfTime);
    }

    /// <summary>
    /// Verifies that a function appearing at multiple stack depths is aggregated once
    /// with correctly summed self-time and total-time.
    /// </summary>
    [Fact]
    public void Ctor_SingleFunctionRecursing_AggregatesCorrectly()
    {
        // Arrange
        // Stack: root -> A(10) -> B(5) -> A(2)
        var root = new FlameNode("root");
        var a1 = root.AddChild("A", file: "mod1.dll");
        a1.Value = 15;
        
        var b = a1.AddChild("B", file: "mod1.dll");
        b.Value = 5;
        
        var a2 = b.AddChild("A", file: "mod1.dll");
        a2.Value = 2;

        // Act
        var report = new HotFunctionsReport(root);
        
        // Assert
        Assert.Equal(2, report.Functions.Count);
        var func = report.Functions.Single(f => f.Name == "A");
        Assert.Equal(12.0, func.Self);
        Assert.Equal(17.0, func.Total);
    }

    /// <summary>
    /// Verifies that functions with identical names but different frame identities
    /// (file/line) are kept separate and not merged.
    /// </summary>
    [Fact]
    public void Ctor_IdenticalNamesDifferentFrames_NotMerged()
    {
        // Arrange
        var root = new FlameNode("root");
        var a1 = root.AddChild("Process", file: "app.exe", line: 10);
        a1.Value = 10;
        
        var a2 = root.AddChild("Process", file: "lib.dll", line: 20);
        a2.Value = 20;

        // Act
        var report = new HotFunctionsReport(root);
        
        // Assert
        Assert.Equal(2, report.Functions.Count);
        Assert.Contains(report.Functions, f => f.File == "app.exe" && f.Self == 10);
        Assert.Contains(report.Functions, f => f.File == "lib.dll" && f.Self == 20);
    }

    /// <summary>
    /// Verifies that ordering and ranking are correct when multiple functions tie on self-time.
    /// </summary>
    [Fact]
    public void Ctor_MultipleFunctionsTieOnSelfTime_RanksCorrectly()
    {
        // Arrange
        var root = new FlameNode("root");
        var a = root.AddChild("Alpha");
        a.Value = 10;
        
        var b = root.AddChild("Beta");
        b.Value = 10;
        
        var c = root.AddChild("Gamma");
        c.Value = 5;

        // Act
        var report = new HotFunctionsReport(root);
        
        // Assert
        Assert.Equal(3, report.Functions.Count);
        Assert.Equal(10.0, report.Functions[0].Self);
        Assert.Equal(10.0, report.Functions[1].Self);
        Assert.Equal(5.0, report.Functions[2].Self);
        
        var topNames = report.Functions.Take(2).Select(f => f.Name).ToHashSet();
        Assert.Contains("Alpha", topNames);
        Assert.Contains("Beta", topNames);
    }

    /// <summary>
    /// Verifies that a child whose total exceeds its parent's total clamps the parent's
    /// self-time to zero, so the parent is excluded from the report deterministically.
    /// </summary>
    [Fact]
    public void Ctor_ChildExceedsParentTotal_ClampsParentSelfToZero()
    {
        // Arrange
        var root = new FlameNode("root");
        var a = root.AddChild("A");
        a.Value = 5;

        var b = a.AddChild("B");
        b.Value = 10;

        // Act
        var report = new HotFunctionsReport(root);

        // Assert
        Assert.Single(report.Functions);
        Assert.Equal("B", report.Functions[0].Name);
        Assert.Equal(10.0, report.Functions[0].Self);
        Assert.Equal(10.0, report.Functions[0].Total);
    }
}
