using SkiaFlameGraph.Core.Models;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Unit tests for the <see cref="FlameNode"/> model.
/// These tests exercise the node construction logic, child linkage,
/// depth calculation, and metadata handling.
/// </summary>
public class FlameNodeTests
{
    /// <summary>
    /// Verifies that <see cref="FlameNode.AddChild"/> creates a child node
    /// that is correctly linked to its parent and has the expected depth.
    /// </summary>
    [Fact]
    public void AddChild_CreatesChildWithCorrectDepth()
    {
        var root = new FlameNode("root");
        var child = root.AddChild("child");

        Assert.NotNull(child);
        Assert.Equal("child", child.Name);
        Assert.Equal(root.Depth + 1, child.Depth);
        Assert.Same(root, child.Parent);
        Assert.Contains(child, root.Children);
    }

    /// <summary>
    /// Verifies that adding multiple children with the same name results
    /// in distinct nodes rather than overwriting or merging them.
    /// </summary>
    [Fact]
    public void AddChild_WithSameName_CreatesDistinctNodes()
    {
        var root = new FlameNode("root");
        var first = root.AddChild("dup");
        var second = root.AddChild("dup");

        Assert.NotSame(first, second);
        Assert.Equal(2, root.Children.Count);
        Assert.Contains(first, root.Children);
        Assert.Contains(second, root.Children);
    }

    /// <summary>
    /// MaxDepth on a leaf node should return 0, indicating no deeper levels.
    /// </summary>
    [Fact]
    public void MaxDepth_LeafNode_ReturnsZero()
    {
        var leaf = new FlameNode("leaf");
        Assert.Equal(0, leaf.MaxDepth());
    }

    /// <summary>
    /// MaxDepth on a three‑level tree should return 2 (root → child → grandchild).
    /// </summary>
    [Fact]
    public void MaxDepth_ThreeLevelTree_ReturnsTwo()
    {
        var root = new FlameNode("root");
        var child = root.AddChild("child");
        var grandchild = child.AddChild("grandchild");

        Assert.Equal(2, root.MaxDepth());
        Assert.Equal(1, child.MaxDepth());
        Assert.Equal(0, grandchild.MaxDepth());
    }

    /// <summary>
    /// Adding a child with file and line metadata should store those values
    /// on the child node.
    /// </summary>
    [Fact]
    public void AddChild_WithFileAndLine_SetsMetadata()
    {
        var root = new FlameNode("root");
        var child = root.AddChild("child", file: "Test.cs", line: 42);

        Assert.Equal("Test.cs", child.File);
        Assert.Equal(42, child.Line);
    }
}
