using SkiaFlameGraph.Core.Models;
using Xunit;

namespace SkiaFlameGraph.Tests;

public class FlameNodeQueryExtensionsTests
{
    [Fact]
    public void DescendantsDepthFirst_ReturnsAllNodesInOrder()
    {
        var root = new FlameNode("root");
        var child1 = root.AddChild("child1");
        var child2 = root.AddChild("child2");
        var grandchild1 = child1.AddChild("grandchild1");

        var nodes = root.DescendantsDepthFirst().ToList();

        Assert.Equal(4, nodes.Count);
        Assert.Equal("root", nodes[0].Name);
        Assert.Equal("child1", nodes[1].Name);
        Assert.Equal("grandchild1", nodes[2].Name);
        Assert.Equal("child2", nodes[3].Name);
    }

    [Fact]
    public void TotalNodeCount_ReturnsCorrectCount()
    {
        var root = new FlameNode("root");
        root.AddChild("child1");
        root.AddChild("child2").AddChild("grandchild");

        Assert.Equal(4, root.TotalNodeCount());
    }

    [Fact]
    public void HottestPath_ReturnsCorrectPath()
    {
        var root = new FlameNode("root") { Value = 100 };
        var child1 = root.AddChild("child1");
        child1.Value = 50;
        var child2 = root.AddChild("child2");
        child2.Value = 30;

        var grandchild = child1.AddChild("grandchild");
        grandchild.Value = 40;

        var path = root.HottestPath().ToList();

        Assert.Equal(3, path.Count);
        Assert.Equal("root", path[0].Name);
        Assert.Equal("child1", path[1].Name);
        Assert.Equal("grandchild", path[2].Name);
    }
}
