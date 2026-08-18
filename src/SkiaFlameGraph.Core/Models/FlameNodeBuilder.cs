namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Fluent builder for <see cref="FlameNode"/>. Use it to construct a node with
/// only the properties you care about, then call <see cref="Build"/>.
/// </summary>
public class FlameNodeBuilder
{
    private string? _name;
    private string? _file;
    private int? _line;
    private double _value;
    private int _depth;
    private FlameNode? _parent;

    /// <summary>Sets the node name (required).</summary>
    /// <param name="name">The frame name.</param>
    /// <returns>This builder, for chaining.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="name"/> is empty or whitespace.</exception>
    public FlameNodeBuilder WithName(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        _name = name;
        return this;
    }

    /// <summary>Sets the source file for the frame.</summary>
    /// <param name="file">The source file, or null.</param>
    /// <returns>This builder, for chaining.</returns>
    public FlameNodeBuilder WithFile(string? file)
    {
        _file = file;
        return this;
    }

    /// <summary>Sets the source line for the frame.</summary>
    /// <param name="line">The source line, or null.</param>
    /// <returns>This builder, for chaining.</returns>
    public FlameNodeBuilder WithLine(int? line)
    {
        _line = line;
        return this;
    }

    /// <summary>Sets the total weight of this subtree (self + descendants).</summary>
    /// <param name="value">The subtree weight.</param>
    /// <returns>This builder, for chaining.</returns>
    public FlameNodeBuilder WithValue(double value)
    {
        _value = value;
        return this;
    }

    /// <summary>Sets the depth from the synthetic root (root == 0).</summary>
    /// <param name="depth">The node depth.</param>
    /// <returns>This builder, for chaining.</returns>
    public FlameNodeBuilder WithDepth(int depth)
    {
        _depth = depth;
        return this;
    }

    /// <summary>Sets the parent node.</summary>
    /// <param name="parent">The parent node, or null.</param>
    /// <returns>This builder, for chaining.</returns>
    public FlameNodeBuilder WithParent(FlameNode? parent)
    {
        _parent = parent;
        return this;
    }

    /// <summary>Builds a <see cref="FlameNode"/> from the configured values.</summary>
    /// <returns>The configured node.</returns>
    /// <exception cref="InvalidOperationException">The name was not set before building.</exception>
    public FlameNode Build()
    {
        if (_name is null)
            throw new InvalidOperationException("FlameNodeBuilder requires a name; call WithName before Build.");

        return new FlameNode(_name)
        {
            File = _file,
            Line = _line,
            Value = _value,
            Depth = _depth,
            Parent = _parent,
        };
    }

    /// <summary>Creates a builder pre-filled from an existing node.</summary>
    /// <param name="template">The node to copy property values from.</param>
    /// <returns>A builder seeded with <paramref name="template"/>'s values.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="template"/> is null.</exception>
    public static FlameNodeBuilder From(FlameNode template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return new FlameNodeBuilder
        {
            _name = template.Name,
            _file = template.File,
            _line = template.Line,
            _value = template.Value,
            _depth = template.Depth,
            _parent = template.Parent,
        };
    }
}