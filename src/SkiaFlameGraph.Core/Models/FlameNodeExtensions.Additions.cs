namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Additional extension methods for <see cref="FlameNode"/> that provide common operations on the call tree.
/// </summary>
public static partial class FlameNodeExtensions
{
    /// <summary>
    /// Finds the largest <see cref="FlameNode.SelfValue"/> in the subtree rooted at this node,
    /// including the node itself.
    /// </summary>
    /// <param name="node">The root node to search from.</param>
    /// <returns>The maximum self value among the node and all of its descendants.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    public static double MaxSelfValue(this FlameNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var max = node.SelfValue;
        foreach (var child in node.Children)
        {
            max = Math.Max(max, child.MaxSelfValue());
        }

        return max;
    }
}
