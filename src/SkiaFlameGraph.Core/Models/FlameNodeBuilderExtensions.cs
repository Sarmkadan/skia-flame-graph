namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Extension methods for <see cref="FlameNodeBuilder"/> that build a node and attach children in one call.
/// </summary>
public static class FlameNodeBuilderExtensions
{
    /// <summary>
    /// Builds a <see cref="FlameNode"/> from the builder and attaches the given children to it.
    /// Each child's <see cref="FlameNode.Parent"/> is set to the built node, and the depth of each
    /// child and its descendants is adjusted to sit one level below the built node.
    /// </summary>
    /// <param name="builder">The builder that supplies the node's name and properties.</param>
    /// <param name="children">The child nodes to attach, in order. May be empty.</param>
    /// <returns>The configured node with <paramref name="children"/> attached.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="builder"/>, <paramref name="children"/>, or one of its elements is null.</exception>
    /// <exception cref="InvalidOperationException">The name was not set before building.</exception>
    public static FlameNode BuildWithChildren(this FlameNodeBuilder builder, params FlameNode[] children)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(children);

        var node = builder.Build();
        foreach (var child in children)
        {
            ArgumentNullException.ThrowIfNull(child, nameof(children));
            SetSubtreeDepth(child, node.Depth + 1);
            child.Parent = node;
            node.Children.Add(child);
        }

        return node;
    }

    private static void SetSubtreeDepth(FlameNode node, int depth)
    {
        node.Depth = depth;
        foreach (var child in node.Children)
            SetSubtreeDepth(child, depth + 1);
    }
}
