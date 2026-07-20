using SkiaSharp;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests;

public class FramePaletteTests
{
    [Fact]
    public void ForFrame_SameName_ReturnsSameColor()
    {
        // Arrange
        var name = "TestFunction";

        // Act
        var color1 = FramePalette.ForFrame(name);
        var color2 = FramePalette.ForFrame(name);

        // Assert
        Assert.Equal(color1, color2);
    }

    [Fact]
    public void ForFrame_DifferentNames_ReturnsDifferentColors()
    {
        // Arrange
        var name1 = "FunctionA";
        var name2 = "FunctionB";

        // Act
        var color1 = FramePalette.ForFrame(name1);
        var color2 = FramePalette.ForFrame(name2);

        // Assert
        // Note: While hash collisions are theoretically possible, they are extremely 
        // unlikely for distinct short strings in a standard implementation.
        Assert.NotEqual(color1, color2);
    }

    [Fact]
    public void ForFrame_ReturnsValidColorRange()
    {
        // Arrange
        var name = "FunctionC";

        // Act
        var color = FramePalette.ForFrame(name);

        // Assert
        Assert.InRange(color.Red, (byte)0, (byte)255);
        Assert.InRange(color.Green, (byte)0, (byte)255);
        Assert.InRange(color.Blue, (byte)0, (byte)255);
        Assert.InRange(color.Alpha, (byte)0, (byte)255);
    }

    [Fact]
    public void ForFrame_WithHighlightPattern_Match_ReturnsValidColor()
    {
        // Arrange
        var name = "HotFunction";
        var pattern = "HotFunction";

        // Act
        var color = FramePalette.ForFrame(name, pattern);

        // Assert
        Assert.InRange(color.Red, (byte)0, (byte)255);
        Assert.InRange(color.Green, (byte)0, (byte)255);
        Assert.InRange(color.Blue, (byte)0, (byte)255);
        Assert.InRange(color.Alpha, (byte)0, (byte)255);
    }

    [Fact]
    public void ForFrame_WithHighlightPattern_NoMatch_ReturnsSameAsStandard()
    {
        // Arrange
        var name = "ColdFunction";
        var pattern = "HotFunction";

        // Act
        var colorStandard = FramePalette.ForFrame(name);
        var colorHighlighted = FramePalette.ForFrame(name, pattern);

        // Assert
        Assert.Equal(colorStandard, colorHighlighted);
    }

    [Fact]
    public void ForFrame_WithHighlightPattern_Match_ReturnsDifferentColorThanStandard()
    {
        // Arrange
        var name = "TargetFunction";
        var pattern = "TargetFunction";

        // Act
        var colorStandard = FramePalette.ForFrame(name);
        var colorHighlighted = FramePalette.ForFrame(name, pattern);

        // Assert
        // Assuming the highlight color is distinct from the hash-based color.
        // If the hash happens to match the highlight color exactly, this test might flake,
        // but it is a reasonable assumption for a highlight feature.
        Assert.NotEqual(colorStandard, colorHighlighted);
    }
}
