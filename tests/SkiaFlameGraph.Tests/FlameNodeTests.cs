using SkiaFlameGraph.Core.Models;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Unit tests for the <see cref="FlameNode"/> model.
/// These tests exercise the node construction logic, child linkage,
/// depth calculation, and metadata handling.
/// </summary>
public class FlameNodeTests : IFlameNodeTests
{
    /// <summary>
    /// Verifies that <see cref="FlameNode.AddChild"/> creates a child node
    /// that is correctly linked to its parent and has the expected depth.
    /// </summary>
    [Fact]
    public void AddChild_CreatesChildWithCorrectDepth()
    {
        var root = new FlameNode(FlameNodeTestsConstants.RootNodeName);
        var child = root.AddChild(FlameNodeTestsConstants.ChildNodeName);

        Assert.NotNull(child);
        Assert.Equal(FlameNodeTestsConstants.ChildNodeName, child.Name);
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
        var root = new FlameNode(FlameNodeTestsConstants.RootNodeName);
        var first = root.AddChild(FlameNodeTestsConstants.DuplicateNodeName);
        var second = root.AddChild(FlameNodeTestsConstants.DuplicateNodeName);

        Assert.NotSame(first, second);
        Assert.Equal(FlameNodeTestsConstants.DuplicateChildCount, root.Children.Count);
        Assert.Contains(first, root.Children);
        Assert.Contains(second, root.Children);
    }

    /// <summary>
    /// MaxDepth on a leaf node should return 0, indicating no deeper levels.
    /// </summary>
    [Fact]
    public void MaxDepth_LeafNode_ReturnsZero()
    {
        var leaf = new FlameNode(FlameNodeTestsConstants.LeafNodeName);
        Assert.Equal(FlameNodeTestsConstants.LeafMaxDepth, leaf.MaxDepth());
    }

    /// <summary>
    /// MaxDepth on a three‑level tree should return 2 (root → child → grandchild).
    /// </summary>
    [Fact]
    public void MaxDepth_ThreeLevelTree_ReturnsTwo()
    {
        var root = new FlameNode(FlameNodeTestsConstants.RootNodeName);
        var child = root.AddChild(FlameNodeTestsConstants.ChildNodeName);
        var grandchild = child.AddChild(FlameNodeTestsConstants.GrandchildNodeName);

        Assert.Equal(FlameNodeTestsConstants.ThreeLevelTreeMaxDepth, root.MaxDepth());
        Assert.Equal(FlameNodeTestsConstants.ChildMaxDepth, child.MaxDepth());
        Assert.Equal(FlameNodeTestsConstants.LeafMaxDepth, grandchild.MaxDepth());
    }

    /// <summary>
    /// Adding a child with file and line metadata should store those values
    /// on the child node.
    /// </summary>
    [Fact]
    public void AddChild_WithFileAndLine_SetsMetadata()
    {
        var root = new FlameNode(FlameNodeTestsConstants.RootNodeName);
        var child = root.AddChild(FlameNodeTestsConstants.ChildNodeName, file: FlameNodeTestsConstants.TestFileName, line: FlameNodeTestsConstants.TestFileLine);

        Assert.Equal(FlameNodeTestsConstants.TestFileName, child.File);
        Assert.Equal(FlameNodeTestsConstants.TestFileLine, child.Line);
    }
}
