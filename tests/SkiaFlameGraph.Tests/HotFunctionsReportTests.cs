using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Reporting;
using Xunit;

namespace SkiaFlameGraph.Tests;

public class HotFunctionsReportTests
{
    [Fact]
    public void Constructor_WithValidRootNode_DoesNotThrow()
    {
        // Arrange
        var root = new FlameNode("root");

        // Act and Assert
        _ = new HotFunctionsReport(root);
    }

    [Fact]
    public void Constructor_WithNullRootNode_ThrowsArgumentNullException()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new HotFunctionsReport(null));
    }

    [Fact]
    public void ToText_WithValidReport_ReturnsExpectedString()
    {
        // Arrange
        var root = new FlameNode("root");
        root.AddChild("child1", "file1", 1);
        root.AddChild("child2", "file2", 2);
        var report = new HotFunctionsReport(root);

        // Act
        var result = report.ToText();

        // Assert
        Assert.NotEmpty(result);
    }

    [Fact]
    public void HotFunction_Name_ReturnsExpectedValue()
    {
        // Arrange
        var func = new HotFunctionsReport.HotFunction("name");

        // Act and Assert
        Assert.Equal("name", func.Name);
    }

    [Fact]
    public void HotFunction_Self_ReturnsExpectedValue()
    {
        // Arrange
        var func = new HotFunctionsReport.HotFunction("name");
        func.Self = 10.5;

        // Act and Assert
        Assert.Equal(10.5, func.Self);
    }

    [Fact]
    public void HotFunction_Total_ReturnsExpectedValue()
    {
        // Arrange
        var func = new HotFunctionsReport.HotFunction("name");
        func.Total = 20.5;

        // Act and Assert
        Assert.Equal(20.5, func.Total);
    }
}
