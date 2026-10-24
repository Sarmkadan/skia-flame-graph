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
    private const char FrameSeparator = ';';
    private static readonly char[] CountSeparators = { ' ', '\t' };
    /// <summary>
    /// Parses a collapsed stacks file into a flame graph tree.
    /// </summary>
    /// <param name="path">Path to the collapsed stacks file.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A FlameNode tree with "root" as the root node.</returns>
    public static FlameNode ParseFile(string path, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(path);
        cancellationToken.ThrowIfCancellationRequested();

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Collapsed stacks file not found.", path);
        }

        var lines = File.ReadAllLines(path);
        return Parse(lines, cancellationToken);
    }

    /// <summary>
    /// Parses collapsed stacks from an enumerable of lines.
    /// </summary>
    /// <param name="lines">Lines of text in Brendan Gregg collapsed stack format.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A FlameNode tree with "root" as the root node.</returns>
    public static FlameNode Parse(IEnumerable<string> lines, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(lines);

        var root = new FlameNode("root");

        foreach (var line in lines)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(line))
            {
                continue; // Skip blank lines
            }

            try
            {
                if (TryParseLine(line, out var frames, out var count))
                {
                    AddStack(root, frames, count);
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

    /// <summary>
    /// Adds a stack to the tree, creating or reusing one node per frame and adding
    /// the count to every node on the path so each node's value is its subtree total.
    /// </summary>
    private static void AddStack(FlameNode root, List<string> frames, double count)
    {
        root.Value += count;

        var current = root;
        foreach (var frame in frames)
        {
            current = current.AddChild(frame);
            current.Value += count;
        }
    }

    /// <summary>
    /// Splits a line into its frames (outermost first) and its count.
    /// </summary>
    /// <returns><see langword="false"/> when the line has no count, a non-positive count, or no frames.</returns>
    private static bool TryParseLine(string line, out List<string> frames, out double count)
    {
        frames = new List<string>();
        count = 0;

        // Split on the last whitespace to separate frames from the count
        var trimmed = line.TrimEnd();
        var separatorIndex = trimmed.LastIndexOfAny(CountSeparators);
        if (separatorIndex <= 0)
        {
            return false; // No count value
        }

        if (!double.TryParse(trimmed.AsSpan(separatorIndex + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out count) ||
            count <= 0)
        {
            return false; // Invalid or zero/negative count
        }

        foreach (var part in trimmed.AsSpan(0, separatorIndex).ToString().Split(FrameSeparator))
        {
            var frame = part.Trim();
            if (frame.Length > 0)
            {
                frames.Add(frame);
            }
        }

        return frames.Count > 0;
    }
}
