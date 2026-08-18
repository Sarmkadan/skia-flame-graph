namespace SkiaFlameGraph.Core.Models;

public interface IFlameNode
{
    string Name { get; }

    string? File { get; set; }

    int? Line { get; set; }

    double Value { get; set; }

    int Depth { get; set; }

    List<FlameNode> Children { get; }

    FlameNode? Parent { get; set; }

    FlameNode AddChild(string name, string? file = null, int? line = null);

    int MaxDepth();
}