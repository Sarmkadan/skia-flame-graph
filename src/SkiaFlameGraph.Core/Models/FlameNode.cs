namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// A node in the aggregated call tree. This is the format-independent shape both
/// the flame graph and treemap renderers draw from. <see cref="Value"/> is the
/// total time (in the profile's unit) spent in this frame and all its children.
/// </summary>
public sealed class FlameNode : IFlameNode, IEquatable<FlameNode>
{
    public FlameNode(string name)
    {
        Name = name;
    }

    public string Name { get; }

    /// <summary>Source file for the frame, when the profiler recorded one.</summary>
    public string? File { get; set; }

    public int? Line { get; set; }

    /// <summary>Total weight of this subtree (self + descendants).</summary>
    public double Value { get; set; }

    /// <summary>Depth from the synthetic root (root == 0).</summary>
    public int Depth { get; set; }

    public List<FlameNode> Children { get; } = new();

    public FlameNode? Parent { get; set; }

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

    public override bool Equals(object? obj)
    {
        return Equals(obj as FlameNode);
    }

    public override string ToString() => $"FlameNode {{ File = {File}, Line = {Line}, Value = {Value}, Depth = {Depth}, Parent = {Parent?.Name ?? "null"} }}";

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, File, Line, Value, Depth, Parent);
    }

    public static bool operator ==(FlameNode? left, FlameNode? right)
    {
        return Equals(left, right);
    }

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
