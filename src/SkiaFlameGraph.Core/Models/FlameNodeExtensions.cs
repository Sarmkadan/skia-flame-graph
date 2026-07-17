namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Extension methods for <see cref="FlameNode"/> that provide common operations on the call tree.
/// </summary>
public static class FlameNodeExtensions
{
    /// <summary>
    /// Finds the first descendant with the specified name (case-sensitive).
    /// </summary>
    /// <param name="node">The root node to search from.</param>
    /// <param name="name">The exact name to match.</param>
    /// <returns>The first matching descendant, or null if not found.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    public static FlameNode? FindByName(this FlameNode? node, string name)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (node.Name == name)
        {
            return node;
        }

        foreach (var child in node.Children)
        {
            var found = FindByName(child, name);
            if (found is not null)
            {
                return found;
            }
        }

        return null;
    }

    /// <summary>
    /// Gets all descendants of the specified node at the given depth level.
    /// </summary>
    /// <param name="node">The root node to search from.</param>
    /// <param name="depth">The depth level to find (0 = direct children, 1 = grandchildren, etc.).</param>
    /// <returns>An enumerable of all nodes at the specified depth.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="depth"/> is negative.</exception>
    public static IEnumerable<FlameNode> GetNodesAtDepth(this FlameNode node, int depth)
    {
        ArgumentNullException.ThrowIfNull(node);
        if (depth < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(depth), "Depth cannot be negative.");
        }

        if (depth == 0)
        {
            return node.Children;
        }

        return node.Children.SelectMany(child => GetNodesAtDepth(child, depth - 1));
    }

    /// <summary>
    /// Calculates the total weight of the subtree rooted at this node, including only nodes
    /// that match the specified predicate.
    /// </summary>
    /// <param name="node">The root node to calculate from.</param>
    /// <param name="predicate">The filter predicate to apply to each node.</param>
    /// <returns>The sum of values for all matching nodes in the subtree.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="predicate"/> is null.</exception>
    public static double SumValuesWhere(this FlameNode node, Func<FlameNode, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(predicate);

        var sum = 0.0;
        if (predicate(node))
        {
            sum += node.Value;
        }

        foreach (var child in node.Children)
        {
            sum += SumValuesWhere(child, predicate);
        }

        return sum;
    }

    /// <summary>
    /// Gets all leaf nodes in the subtree (nodes with no children).
    /// </summary>
    /// <param name="node">The root node to search from.</param>
    /// <returns>An enumerable of all leaf nodes in the subtree.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    public static IEnumerable<FlameNode> GetLeafNodes(this FlameNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        if (node.Children.Count == 0)
        {
            yield return node;
        }
        else
        {
            foreach (var child in node.Children)
            {
                foreach (var leaf in GetLeafNodes(child))
                {
                    yield return leaf;
                }
            }
        }
    }

    /// <summary>
    /// Calculates the percentage of the total tree weight that this node represents.
    /// </summary>
    /// <param name="node">The node to calculate percentage for.</param>
    /// <param name="totalRoot">The root node of the entire tree for normalization.</param>
    /// <returns>The percentage (0-100) that this node represents of the total tree weight.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> or <paramref name="totalRoot"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="totalRoot"/> has no value (all children are zero).</exception>
    public static double CalculatePercentageOfTotal(this FlameNode node, FlameNode totalRoot)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(totalRoot);

        var total = totalRoot.Value;
        if (Math.Abs(total) < double.Epsilon)
        {
            throw new ArgumentException("Total root has zero value, cannot calculate percentage.", nameof(totalRoot));
        }

        return (node.Value / total) * 100.0;
    }

    /// <summary>
    /// Gets the path from the root to this node as a sequence of node names.
    /// </summary>
    /// <param name="node">The node to get the path for.</param>
    /// <returns>An enumerable of node names representing the path from root to this node.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    public static IEnumerable<string> GetPathToNode(this FlameNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var path = new Stack<string>();
        var current = node;
        while (current is not null)
        {
            path.Push(current.Name);
            current = current.Parent;
        }

        return path;
    }

    /// <summary>
    /// Finds the deepest node in the subtree (the node with the maximum depth).
    /// </summary>
    /// <param name="node">The root node to search from.</param>
    /// <returns>The deepest node in the subtree, or this node if it's a leaf.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    public static FlameNode GetDeepestNode(this FlameNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        FlameNode deepest = node;
        var maxDepth = node.Depth;

        foreach (var child in node.Children)
        {
            var childDeepest = GetDeepestNode(child);
            if (childDeepest.Depth > maxDepth)
            {
                deepest = childDeepest;
                maxDepth = childDeepest.Depth;
            }
        }

        return deepest;
    }

    /// <summary>
    /// Gets the cumulative value from this node up to the root (sum of self values along the path).
    /// </summary>
    /// <param name="node">The node to calculate cumulative value for.</param>
    /// <returns>The sum of self values from this node to the root.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="node"/> is null.</exception>
    public static double GetCumulativeValueToRoot(this FlameNode node)
    {
        ArgumentNullException.ThrowIfNull(node);

        var cumulative = node.SelfValue;
        var current = node.Parent;
        while (current is not null)
        {
            cumulative += current.SelfValue;
            current = current.Parent;
        }

        return cumulative;
    }
}