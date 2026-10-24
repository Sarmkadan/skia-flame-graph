using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Rendering;
using System;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Soak tests for rendering stability and memory usage.
/// </summary>
public class SoakTests
{
    /// <summary>
    /// Creates a sample flame node tree for testing.
    /// </summary>
    private static FlameNode CreateSampleFlameNode()
    {
        // Create a simple tree: root -> child1 -> grandchild1, child2
        var root = new FlameNode("root");
        root.Value = 100;
        // AddChild returns the attached node, so the children are set on the nodes that are in the tree
        var child1 = root.AddChild("child1");
        child1.Value = 50;
        var child2 = root.AddChild("child2");
        child2.Value = 30;
        var grandchild1 = child1.AddChild("grandchild1");
        grandchild1.Value = 20;
        var grandchild2 = child1.AddChild("grandchild2");
        grandchild2.Value = 10;

        return root;
    }

    /// <summary>
    /// Soak test for FlameGraphRenderer: renders 1000 times and asserts memory stability.
    /// </summary>
    [Fact]
    public void FlameGraphRenderer_SoakTest_Renders1000TimesWithoutMemoryLeak()
    {
        // Arrange
        var root = CreateSampleFlameNode();
        var options = new RenderOptions { Width = 800 };
        var renderer = new FlameGraphRenderer(options);

        // Warm-up
        using var warmup = renderer.Render(root);

        // Measure memory before the test loop
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long before = GC.GetTotalMemory(true);

        // Act: render 1000 times
        for (int i = 0; i < 1000; i++)
        {
            using var image = renderer.Render(root);
            // Image is disposed at the end of the using block
        }

        // Measure memory after the test loop
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long after = GC.GetTotalMemory(true);

        // Assert: memory increase should be reasonable (less than 5MB). Only the upper bound matters for a leak;
        // a negative delta is possible when the GC reclaims more than was allocated during the warm-up.
        long increase = after - before;
        const long maxIncreaseBytes = 5L * 1024 * 1024; // 5 MB
        Assert.True(increase < maxIncreaseBytes, $"Memory grew by {increase} bytes over 1000 renders, expected less than {maxIncreaseBytes}.");
    }

    /// <summary>
    /// Soak test for TreemapRenderer: renders 1000 times and asserts memory stability.
    /// </summary>
    [Fact]
    public void TreemapRenderer_SoakTest_Renders1000TimesWithoutMemoryLeak()
    {
        // Arrange
        var root = CreateSampleFlameNode();
        var options = new RenderOptions { Width = 800 };
        var renderer = new TreemapRenderer(options);

        // Warm-up
        using var warmup = renderer.Render(root);

        // Measure memory before the test loop
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long before = GC.GetTotalMemory(true);

        // Act: render 1000 times
        for (int i = 0; i < 1000; i++)
        {
            using var image = renderer.Render(root);
            // Image is disposed at the end of the using block
        }

        // Measure memory after the test loop
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long after = GC.GetTotalMemory(true);

        // Assert: memory increase should be reasonable (less than 5MB). Only the upper bound matters for a leak;
        // a negative delta is possible when the GC reclaims more than was allocated during the warm-up.
        long increase = after - before;
        const long maxIncreaseBytes = 5L * 1024 * 1024; // 5 MB
        Assert.True(increase < maxIncreaseBytes, $"Memory grew by {increase} bytes over 1000 renders, expected less than {maxIncreaseBytes}.");
    }

    /// <summary>
    /// Soak test for DifferentialFlameGraphRenderer: renders 1000 times and asserts memory stability.
    /// </summary>
    [Fact]
    public void DifferentialFlameGraphRenderer_SoakTest_Renders1000TimesWithoutMemoryLeak()
    {
        // Arrange
        var baseline = CreateSampleFlameNode();
        var current = CreateSampleFlameNode();
        // Make a change in the current tree to ensure non-zero delta
        current.Children[0].Children[0].Value = 25; // grandchild1 was 20, now 25

        var options = new RenderOptions { Width = 800 };
        var renderer = new DifferentialFlameGraphRenderer(options);

        // Warm-up
        using var warmup = renderer.RenderDifferential(baseline, current);

        // Measure memory before the test loop
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long before = GC.GetTotalMemory(true);

        // Act: render 1000 times
        for (int i = 0; i < 1000; i++)
        {
            using var image = renderer.RenderDifferential(baseline, current);
            // Image is disposed at the end of the using block
        }

        // Measure memory after the test loop
        GC.Collect();
        GC.WaitForPendingFinalizers();
        long after = GC.GetTotalMemory(true);

        // Assert: memory increase should be reasonable (less than 5MB). Only the upper bound matters for a leak;
        // a negative delta is possible when the GC reclaims more than was allocated during the warm-up.
        long increase = after - before;
        const long maxIncreaseBytes = 5L * 1024 * 1024; // 5 MB
        Assert.True(increase < maxIncreaseBytes, $"Memory grew by {increase} bytes over 1000 renders, expected less than {maxIncreaseBytes}.");
    }
}