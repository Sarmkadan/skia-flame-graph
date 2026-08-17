namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Extension methods for <see cref="FlameNode"/> that combine or compare call trees.
/// </summary>
public static class FlameNodeMergeExtensions
{
    /// <summary>
    /// Combines two call trees into a new tree, matching nodes by frame name and
    /// summing their weights. Nodes present in only one tree are carried over with
    /// their original weight. The receiver (<paramref name="a"/>) is not mutated.
    /// </summary>
    /// <param name="a">The first tree to merge.</param>
    /// <param name="b">The second tree to merge.</param>
    /// <returns>A new tree whose weights are the sum of the two inputs.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="a"/> or <paramref name="b"/> is null.</exception>
    public static FlameNode Merge(this FlameNode a, FlameNode b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);

        var merged = new FlameNode(a.Name)
        {
            File = a.File,
            Line = a.Line,
            Depth = a.Depth,
            Value = a.Value + b.Value,
        };

        var bChildrenByName = new Dictionary<string, FlameNode>();
        foreach (var child in b.Children)
        {
            bChildrenByName[child.Name] = child;
        }

        foreach (var aChild in a.Children)
        {
            var mergedChild = bChildrenByName.TryGetValue(aChild.Name, out var bChild)
                ? Merge(aChild, bChild)
                : CloneSubtree(aChild);
            merged.Children.Add(mergedChild);
            mergedChild.Parent = merged;
        }

        foreach (var bChild in b.Children)
        {
            if (bChildrenByName.ContainsKey(bChild.Name) && a.Children.Any(c => c.Name == bChild.Name))
            {
                continue;
            }

            var mergedChild = CloneSubtree(bChild);
            merged.Children.Add(mergedChild);
            mergedChild.Parent = merged;
        }

        return merged;
    }

    /// <summary>
    /// Computes the per-node weight delta between two call trees, matching nodes by
    /// frame name. Each node in the result carries <c>current - baseline</c> for its
    /// weight. Nodes present in only one tree are included with their full (or
    /// negated) weight. Neither input is mutated.
    /// </summary>
    /// <param name="baseline">The baseline tree (typically the older profile).</param>
    /// <param name="current">The current tree (typically the newer profile).</param>
    /// <returns>A new tree whose weights are the per-node delta.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="baseline"/> or <paramref name="current"/> is null.</exception>
    public static FlameNode Diff(this FlameNode baseline, FlameNode current)
    {
        ArgumentNullException.ThrowIfNull(baseline);
        ArgumentNullException.ThrowIfNull(current);

        var delta = new FlameNode(current.Name)
        {
            File = current.File,
            Line = current.Line,
            Depth = current.Depth,
            Value = current.Value - baseline.Value,
        };

        var baselineChildrenByName = new Dictionary<string, FlameNode>();
        foreach (var child in baseline.Children)
        {
            baselineChildrenByName[child.Name] = child;
        }

        foreach (var currentChild in current.Children)
        {
            var deltaChild = baselineChildrenByName.TryGetValue(currentChild.Name, out var baselineChild)
                ? Diff(baselineChild, currentChild)
                : CloneSubtree(currentChild);
            delta.Children.Add(deltaChild);
            deltaChild.Parent = delta;
        }

        foreach (var baselineChild in baseline.Children)
        {
            if (baselineChildrenByName.ContainsKey(baselineChild.Name) && current.Children.Any(c => c.Name == baselineChild.Name))
            {
                continue;
            }

            var deltaChild = NegateSubtree(baselineChild);
            delta.Children.Add(deltaChild);
            deltaChild.Parent = delta;
        }

        return delta;
    }

    private static FlameNode CloneSubtree(FlameNode source)
    {
        var copy = new FlameNode(source.Name)
        {
            File = source.File,
            Line = source.Line,
            Depth = source.Depth,
            Value = source.Value,
        };

        foreach (var child in source.Children)
        {
            var childCopy = CloneSubtree(child);
            copy.Children.Add(childCopy);
            childCopy.Parent = copy;
        }

        return copy;
    }

    private static FlameNode NegateSubtree(FlameNode source)
    {
        var copy = new FlameNode(source.Name)
        {
            File = source.File,
            Line = source.Line,
            Depth = source.Depth,
            Value = -source.Value,
        };

        foreach (var child in source.Children)
        {
            var childCopy = NegateSubtree(child);
            copy.Children.Add(childCopy);
            childCopy.Parent = copy;
        }

        return copy;
    }
}