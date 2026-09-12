namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// A node in the aggregated call tree. This is the format-independent shape both
/// the flame graph and treemap renderers draw from. <see cref="Value"/> is the
/// total time (in the profile's unit) spent in this frame and all its children.
/// </summary>
public sealed class FlameNode : IFlameNode, IEquatable<FlameNode>
{
    /// <summary>Initializes a new flame graph node with the specified frame name.</summary>
    /// <param name="name">The name of the frame represented by the node.</param>
    public FlameNode(string name)
    {
        Name = name;
    }

    /// <summary>Gets the name of the frame represented by this node.</summary>
    public string Name { get; }

    /// <summary>Source file for the frame, when the profiler recorded one.</summary>
    public string? File { get; set; }

    /// <summary>Gets or sets the source line for the frame, when available.</summary>
    public int? Line { get; set; }

    /// <summary>Total weight of this subtree (self + descendants).</summary>
    public double Value { get; set; }

    /// <summary>Depth from the synthetic root (root == 0).</summary>
    public int Depth { get; set; }

    /// <summary>Gets the child frames called by this frame.</summary>
    public List<FlameNode> Children { get; } = new();

    /// <summary>Gets or sets the parent node, or <see langword="null"/> for the root.</summary>
    public FlameNode? Parent { get; set; }

    /// <summary>Determines whether this node is equal to another node.</summary>
    /// <param name="other">The node to compare with this node.</param>
    /// <returns><see langword="true"/> when the nodes are equal; otherwise, <see langword="false"/>.</returns>
    public bool Equals(FlameNode? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name == other.Name &&
               File == other.File &&
               Line == other.Line &&
               Value.Equals(other.Value) &&
               Depth == other.Depth &&
               (Parent == null ? other.Parent == null : Parent.Equals(other.Parent));
    }

    /// <summary>Determines whether this node is equal to the specified object.</summary>
    /// <param name="obj">The object to compare with this node.</param>
    /// <returns><see langword="true"/> when the object is an equal node; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object? obj)
    {
        return Equals(obj as FlameNode);
    }

    /// <summary>Returns a string that describes this node.</summary>
    /// <returns>A string containing the node's source and hierarchy information.</returns>
    public override string ToString() => $"FlameNode {{ File = {File}, Line = {Line}, Value = {Value}, Depth = {Depth}, Parent = {Parent?.Name ?? "null"} }}";

    /// <summary>Returns a hash code for this node.</summary>
    /// <returns>A hash code based on the node's identifying values.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Name, File, Line, Value, Depth, Parent);
    }

    /// <summary>Determines whether two nodes are equal.</summary>
    /// <param name="left">The first node to compare.</param>
    /// <param name="right">The second node to compare.</param>
    /// <returns><see langword="true"/> when the nodes are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(FlameNode? left, FlameNode? right)
    {
        return Equals(left, right);
    }

    /// <summary>Determines whether two nodes are not equal.</summary>
    /// <param name="left">The first node to compare.</param>
    /// <param name="right">The second node to compare.</param>
    /// <returns><see langword="true"/> when the nodes are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(FlameNode? left, FlameNode? right)
    {
        return !Equals(left, right);
    }

    /// <summary>Weight attributed to this frame alone, excluding children.</summary>
    public double SelfValue
    {
        get
        {
            var childSum = 0.0;
            foreach (var c in Children)
                childSum += c.Value;
            var self = Value - childSum;
            return self < 0 ? 0 : self;
        }
    }

    /// <summary>Adds a child frame or returns an existing child with the same identity.</summary>
    /// <param name="name">The name of the child frame.</param>
    /// <param name="file">The source file for the child frame, when available.</param>
    /// <param name="line">The source line for the child frame, when available.</param>
    /// <returns>The matching existing child or the newly created child.</returns>
    public FlameNode AddChild(string name, string? file = null, int? line = null)
    {
        // Merge into an existing child with the same identity so recursive calls
        // collapse into a single, wider box instead of a stack of slivers.
        foreach (var existing in Children)
        {
            if (existing.Name == name && existing.File == file && existing.Line == line)
                return existing;
        }

        var node = new FlameNode(name)
        {
            File = file,
            Line = line,
            Depth = Depth + 1,
            Parent = this,
        };
        Children.Add(node);
        return node;
    }

    /// <summary>Max depth of the subtree rooted here, including this node.</summary>
    public int MaxDepth()
    {
        var max = Depth;
        foreach (var c in Children)
            max = Math.Max(max, c.MaxDepth());
        return max;
    }
}
