namespace SkiaFlameGraph.Core.Models;

public interface IFlameNode
{
    /// <summary>Gets the name of the frame represented by this node.</summary>
    string Name { get; }

    /// <summary>Gets or sets the source file for the frame, when available.</summary>
    string? File { get; set; }

    /// <summary>Gets or sets the source line for the frame, when available.</summary>
    int? Line { get; set; }

    /// <summary>Gets or sets the total weight of this node and its descendants.</summary>
    double Value { get; set; }

    /// <summary>Gets or sets the depth of this node from the root.</summary>
    int Depth { get; set; }

    /// <summary>Gets the child frames called by this frame.</summary>
    List<FlameNode> Children { get; }

    /// <summary>Gets or sets the parent node, or <see langword="null"/> for the root.</summary>
    FlameNode? Parent { get; set; }

    /// <summary>Adds a child frame or returns an existing child with the same identity.</summary>
    FlameNode AddChild(string name, string? file = null, int? line = null);

    /// <summary>Gets the maximum depth of the subtree rooted at this node.</summary>
    int MaxDepth();
}
