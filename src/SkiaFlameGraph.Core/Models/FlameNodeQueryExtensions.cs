namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Extension methods for <see cref="FlameNode"/> that provide query operations on the call tree.
/// </summary>
public static class FlameNodeQueryExtensions
{
    /// <summary>
    /// Gets all nodes in the subtree, including the root node, in a depth-first traversal order.
    /// </summary>
    /// <param name="node">The root node to start traversal from.</param>
    /// <returns>An enumerable of all nodes in the subtree in depth-first order.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    public static IEnumerable<FlameNode> DescendantsDepthFirst(this FlameNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        yield return node;
        foreach (var child in node.Children)
        {
            foreach (var descendant in DescendantsDepthFirst(child))
            {
                yield return descendant;
            }
        }
    }

    /// <summary>
    /// Calculates the total number of nodes in the subtree rooted at the specified node.
    /// </summary>
    /// <param name="node">The root node to count from.</param>
    /// <returns>The total count of nodes in the subtree.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    public static int TotalNodeCount(this FlameNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        return 1 + node.Children.Sum(c => TotalNodeCount(c));
    }

    /// <summary>
    /// Gets the path from the root node to the leaf node following the highest-value children.
    /// </summary>
    /// <param name="node">The root node to start searching from.</param>
    /// <returns>An enumerable of nodes representing the hottest path.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    public static IEnumerable<FlameNode> HottestPath(this FlameNode node)
    {
        ArgumentNullException.ThrowIfNull(node);
        var current = node;
        yield return current;
        while (current.Children.Count > 0)
        {
            current = current.Children.MaxBy(c => c.Value)!;
            yield return current;
        }
    }
}
