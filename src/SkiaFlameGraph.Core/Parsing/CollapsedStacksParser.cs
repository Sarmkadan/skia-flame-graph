using System.Globalization;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Parsing;

/// <summary>
/// Parser for Brendan Gregg's collapsed stack format.
/// Each line is of the form: frame1;frame2;frame3 123
/// where 123 is the count/value for that stack.
/// </summary>
public static class CollapsedStacksParser
{
    /// <summary>
    /// Parses a collapsed stacks file into a flame graph tree.
    /// </summary>
    /// <param name="path">Path to the collapsed stacks file.</param>
    /// <returns>A FlameNode tree with "root" as the root node.</returns>
    public static FlameNode ParseFile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Collapsed stacks file not found.", path);
        }

        var lines = File.ReadAllLines(path);
        return Parse(lines);
    }

    /// <summary>
    /// Parses collapsed stacks from an enumerable of lines.
    /// </summary>
    /// <param name="lines">Lines of text in Brendan Gregg collapsed stack format.</param>
    /// <returns>A FlameNode tree with "root" as the root node.</returns>
    public static FlameNode Parse(IEnumerable<string> lines)
    {
        var root = new FlameNode("root");

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue; // Skip blank lines
            }

            try
            {
                var node = ParseLine(line);
                if (node != null)
                {
                    root.Value += node.Value;
                    MergeIntoTree(root, node);
                }
            }
            catch
            {
                // Skip malformed lines
                continue;
            }
        }

        return root;
    }

    private static void MergeIntoTree(FlameNode parent, FlameNode source)
    {
        // Recursively merge the source tree into the parent tree
        // This handles the case where the same stack appears multiple times
        foreach (var child in source.Children)
        {
            var existing = parent.Children.FirstOrDefault(c =>
                c.Name == child.Name &&
                c.File == child.File &&
                c.Line == child.Line);

            if (existing != null)
            {
                existing.Value += child.Value;
                MergeIntoTree(existing, child);
            }
            else
            {
                // Clone the child to avoid modifying the source tree
                var newChild = new FlameNode(child.Name)
                {
                    File = child.File,
                    Line = child.Line,
                    Value = child.Value,
                    Depth = child.Depth,
                    Parent = parent
                };

                foreach (var grandChild in child.Children)
                {
                    newChild.Children.Add(grandChild);
                    grandChild.Parent = newChild;
                }

                parent.Children.Add(newChild);
            }
        }
    }

    private static FlameNode? ParseLine(string line)
    {
        // Split on the last space to separate frames from the count
        var lastSpaceIndex = line.LastIndexOf(' ');
        if (lastSpaceIndex <= 0)
        {
            return null; // No count value
        }

        var framesPart = line.AsSpan(0, lastSpaceIndex);
        var countPart = line.AsSpan(lastSpaceIndex + 1);

        // Parse the count value
        if (!double.TryParse(countPart, NumberStyles.Float, CultureInfo.InvariantCulture, out var count))
        {
            return null; // Invalid count
        }

        if (count <= 0)
        {
            return null; // Zero or negative count
        }

        // Split frames by semicolon
        var frameStrings = framesPart.ToString().Split(';');
        if (frameStrings.Length == 0)
        {
            return null; // No frames
        }

        // Build the tree from bottom to top
        FlameNode? current = null;

        // Process frames in reverse order (bottom-up)
        for (int i = frameStrings.Length - 1; i >= 0; i--)
        {
            var frame = frameStrings[i].Trim();
            if (string.IsNullOrEmpty(frame))
            {
                continue;
            }

            if (current == null)
            {
                // Create leaf node with the count
                current = new FlameNode(frame)
                {
                    Value = count
                };
            }
            else
            {
                // Create parent node and add current as child
                var parentNode = new FlameNode(frame);
                parentNode.AddChild(current.Name, current.File, current.Line).Value += current.Value;
                current = parentNode;
            }
        }

        return current;
    }
}
