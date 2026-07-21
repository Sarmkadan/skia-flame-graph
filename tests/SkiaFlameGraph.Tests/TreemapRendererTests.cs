using System.Collections.Generic;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;
using SkiaSharp;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Tests for <see cref="TreemapRenderer"/> ensuring that rendering completes without
/// throwing exceptions for various tree shapes and that the resulting image contains
/// pixel data.
/// </summary>
public class TreemapRendererTests
{
    private static SKImage RenderTree(IEnumerable<string> collapsedLines)
    {
        // Build a FlameNode tree from collapsed stack lines using the existing parser.
        var root = CollapsedStacksParser.Parse(collapsedLines);
        var renderer = new TreemapRenderer();
        return renderer.Render(root);
    }

    private static void AssertImageIsValid(SKImage image)
    {
        Assert.NotNull(image);
        Assert.True(image.Width > 0, "Image width should be greater than zero.");
        Assert.True(image.Height > 0, "Image height should be greater than zero.");

        // Encode to PNG and ensure we get some bytes back.
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        Assert.NotNull(data);
        Assert.True(data.Size > 0, "Encoded image data should not be empty.");
    }

    [Fact]
    public void Render_SingleNode_DoesNotThrow()
    {
        // A single node with a weight of 1.
        var lines = new[] { "Root 1" };
        var image = RenderTree(lines);
        AssertImageIsValid(image);
    }

    [Fact]
    public void Render_TreeWithZeroWeightNode_DoesNotThrow()
    {
        // Include a node with zero weight; it should be ignored or rendered harmlessly.
        var lines = new[]
        {
            "Root;ZeroWeight 0",
            "Root;Child 5"
        };
        var image = RenderTree(lines);
        AssertImageIsValid(image);
    }

    [Fact]
    public void Render_SmallTree_DoesNotThrow()
    {
        // A small tree with a few branches.
        var lines = new[]
        {
            "Root;A 3",
            "Root;B 2",
            "Root;C 1"
        };
        var image = RenderTree(lines);
        AssertImageIsValid(image);
    }

    [Fact]
    public void Render_LargeTree_DoesNotThrow()
    {
        // Generate a larger tree (e.g., 100 leaf nodes) to ensure the renderer can handle it.
        var lines = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            lines.Add($"Root;Node{i} {i + 1}");
        }

        var image = RenderTree(lines);
        AssertImageIsValid(image);
    }

    [Fact]
    public void Render_DeepTree_DoesNotThrow()
    {
        // A deep tree (depth > typical recursion limit) to test the depth guard (depth >= 12).
        var lines = new List<string>();
        var path = new List<string>();
        for (int depth = 0; depth < 20; depth++)
        {
            path.Add($"Level{depth}");
            var joined = string.Join(";", path);
            lines.Add($"{joined} 1");
        }

        var image = RenderTree(lines);
        AssertImageIsValid(image);
    }
}
