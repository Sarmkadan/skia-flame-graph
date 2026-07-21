using System.Collections.Generic;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Parsing;
using Xunit;

namespace SkiaFlameGraph.Tests;

public class CollapsedStacksParserTests
{
    [Fact]
    public void Parse_NormalInput_BuildsCorrectTree()
    {
        // Arrange
        var lines = new[]
        {
            "funcA;funcB 5",
            "funcA;funcC 3"
        };

        // Act
        var root = CollapsedStacksParser.Parse(lines);

        // Assert
        Assert.Equal(8, root.Value);
        Assert.Single(root.Children);
        var a = Assert.Single(root.Children);
        Assert.Equal("funcA", a.Name);
        Assert.Equal(8, a.Value);
        Assert.Equal(2, a.Children.Count);

        var b = a.Children.Find(n => n.Name == "funcB")!;
        var c = a.Children.Find(n => n.Name == "funcC")!;
        Assert.Equal(5, b.Value);
        Assert.Equal(3, c.Value);
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsEmptyRoot()
    {
        // Arrange
        var lines = new string[0];

        // Act
        var root = CollapsedStacksParser.Parse(lines);

        // Assert
        Assert.Equal(0, root.Value);
        Assert.Empty(root.Children);
    }

    [Fact]
    public void Parse_MalformedLines_AreSkipped()
    {
        // Arrange
        var lines = new[]
        {
            "funcA 5",          // valid
            "badline",          // no count
            "funcB -2",         // negative count
            "funcC 0",          // zero count
            "   ",              // whitespace only
            "funcD 2"           // valid
        };

        // Act
        var root = CollapsedStacksParser.Parse(lines);

        // Assert
        // Only the two valid lines should contribute.
        Assert.Equal(7, root.Value);
        Assert.Equal(2, root.Children.Count);

        var a = root.Children.Find(n => n.Name == "funcA")!;
        var d = root.Children.Find(n => n.Name == "funcD")!;
        Assert.Equal(5, a.Value);
        Assert.Equal(2, d.Value);
    }

    [Fact]
    public void Parse_DuplicateStacks_MergedCorrectly()
    {
        // Arrange
        var lines = new[]
        {
            "a;b 2",
            "a;b 3",
            "a 1"
        };

        // Act
        var root = CollapsedStacksParser.Parse(lines);

        // Assert
        // Total value = 2 + 3 + 1 = 6
        Assert.Equal(6, root.Value);
        Assert.Single(root.Children);
        var a = Assert.Single(root.Children);
        Assert.Equal("a", a.Name);
        Assert.Equal(6, a.Value);
        Assert.Single(a.Children);
        var b = Assert.Single(a.Children);
        Assert.Equal("b", b.Name);
        Assert.Equal(5, b.Value); // 2 + 3
    }

    [Fact]
    public void Parse_WhitespaceHandling_IgnoresExtraSpaces()
    {
        // Arrange
        var lines = new[]
        {
            "  funcX ; funcY 4",
            "\tfuncX;funcZ\t2"
        };

        // Act
        var root = CollapsedStacksParser.Parse(lines);

        // Assert
        Assert.Equal(6, root.Value);
        Assert.Single(root.Children);
        var x = Assert.Single(root.Children);
        Assert.Equal("funcX", x.Name);
        Assert.Equal(6, x.Value);
        Assert.Equal(2, x.Children.Count);

        var y = x.Children.Find(n => n.Name == "funcY")!;
        var z = x.Children.Find(n => n.Name == "funcZ")!;
        Assert.Equal(4, y.Value);
        Assert.Equal(2, z.Value);
    }
}
