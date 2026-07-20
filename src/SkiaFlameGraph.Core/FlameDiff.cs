using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core;

/// <summary>
/// Provides diffing functionality for FlameNode trees, computing deltas between baseline and current profiles.
/// </summary>
public static class FlameDiff
{
    /// <summary>
    /// Computes the delta between two flame graphs by matching nodes by name and calculating (current - baseline).
    /// Nodes present in only one tree are included with their full value.
    /// </summary>
    /// <param name="baseline">The baseline flame graph node (typically the older/previous profile).</param>
    /// <param name="current">The current flame graph node (typically the newer/current profile).</param>
    /// <returns>A new FlameNode tree representing the delta values.</returns>
    public static FlameNode Diff(FlameNode baseline, FlameNode current)
    {
        if (baseline == null)
        {
            throw new ArgumentNullException(nameof(baseline));
        }

        if (current == null)
        {
            throw new ArgumentNullException(nameof(current));
        }

        // Create the root delta node
        var deltaRoot = new FlameNode(current.Name)
        {
            File = current.File,
            Line = current.Line,
            Depth = current.Depth
        };

        // Compute delta for this node
        deltaRoot.Value = current.Value - baseline.Value;

        // Recursively diff children
        DiffChildren(baseline.Children, current.Children, deltaRoot);

        return deltaRoot;
    }

    private static void DiffChildren(IReadOnlyList<FlameNode> baselineChildren, IReadOnlyList<FlameNode> currentChildren, FlameNode deltaParent)
    {
        // Use a dictionary for efficient lookup by name
        var baselineDict = new Dictionary<string, FlameNode>();
        foreach (var child in baselineChildren)
        {
            baselineDict[child.Name] = child;
        }

        // Process current children - match by name
        foreach (var currentChild in currentChildren)
        {
            if (baselineDict.TryGetValue(currentChild.Name, out var baselineChild))
            {
                // Both trees have this child - compute delta
                var deltaChild = new FlameNode(currentChild.Name)
                {
                    File = currentChild.File,
                    Line = currentChild.Line,
                    Depth = currentChild.Depth,
                    Value = currentChild.Value - baselineChild.Value
                };

                deltaParent.Children.Add(deltaChild);

                // Recursively process grandchildren
                DiffChildren(baselineChild.Children, currentChild.Children, deltaChild);
            }
            else
            {
                // Child only exists in current tree - include with full value
                var deltaChild = new FlameNode(currentChild.Name)
                {
                    File = currentChild.File,
                    Line = currentChild.Line,
                    Depth = currentChild.Depth,
                    Value = currentChild.Value
                };

                deltaParent.Children.Add(deltaChild);

                // Include its children as-is
                CopySubtree(currentChild, deltaChild);
            }
        }

        // Process children that only exist in baseline tree
        foreach (var baselineChild in baselineChildren)
        {
            if (!baselineDict.ContainsKey(baselineChild.Name))
            {
                // Child only exists in baseline tree - include with negative value
                var deltaChild = new FlameNode(baselineChild.Name)
                {
                    File = baselineChild.File,
                    Line = baselineChild.Line,
                    Depth = baselineChild.Depth,
                    Value = -baselineChild.Value
                };

                deltaParent.Children.Add(deltaChild);

                // Include its children as-is
                CopySubtree(baselineChild, deltaChild);
            }
        }
    }

    private static void CopySubtree(FlameNode source, FlameNode target)
    {
        foreach (var child in source.Children)
        {
            var childCopy = new FlameNode(child.Name)
            {
                File = child.File,
                Line = child.Line,
                Depth = child.Depth,
                Value = child.Value,
                Parent = target
            };
            target.Children.Add(childCopy);

            CopySubtree(child, childCopy);
        }
    }
}