using System.Globalization;
using SkiaFlameGraph.Core.Models;
using static SkiaFlameGraph.IntegrationTests.CollapsedStacksParserIntegrationTestsConstants;

namespace SkiaFlameGraph.IntegrationTests;

/// <summary>
/// One generated stack: its frames, outermost first, and its sample count.
/// </summary>
/// <param name="Frames">Frame names, outermost first.</param>
/// <param name="Count">Samples attributed to this exact stack.</param>
internal readonly record struct StackSample(IReadOnlyList<string> Frames, int Count);

/// <summary>
/// Helpers that generate realistic collapsed-stack input, compute an independent oracle for expected totals,
/// and walk the parsed tree so tests can compare every node.
/// </summary>
internal static class CollapsedStacksParserIntegrationTestsExtensions
{
    /// <summary>
    /// Frame names per depth level. Levels mix plain names, spaces, generics, module paths, and Unicode,
    /// including a surrogate pair (emoji) and a name containing a comma inside generic arguments.
    /// </summary>
    private static readonly string[][] FramePools =
    {
        new[] { "main", "ThreadPool Worker", "Ünïcode.Thread", "🔥 hot loop", "kernel_entry" },
        new[] { "List<T>.Add", "Dictionary<TKey, TValue>.TryGetValue", "render frame", "エンコード", "Span<byte>.CopyTo" },
        new[] { "parse (src/Lexer.cs)", "Array.Resize<T>", "flame graph::layout", "ß Straße", "Math.Max" },
        new[] { "deflate", "memcpy", "Ω≈ç√∫ kernel", "IEnumerable<KeyValuePair<string, int>>.GetEnumerator" },
        new[] { "[unknown]", "__libc_start_main", "Ωmega sort" },
    };

    /// <summary>
    /// Builds a deterministic profile of <paramref name="lineCount"/> stacks. The same index always yields the same
    /// stack, so the output is reproducible without a random seed. Stacks share prefixes, so the tree has merged nodes.
    /// </summary>
    public static List<StackSample> GenerateProfile(int lineCount)
    {
        var samples = new List<StackSample>(lineCount);
        for (var i = 0; i < lineCount; i++)
        {
            var depth = 1 + i % MaxGeneratedDepth;
            var frames = new string[depth];
            for (var level = 0; level < depth; level++)
            {
                var pool = FramePools[level];
                frames[level] = pool[(i * (2 * level + 3) + level) % pool.Length];
            }

            var count = 1 + (i * CountStride) % CountModulo;
            samples.Add(new StackSample(frames, count));
        }

        return samples;
    }

    /// <summary>
    /// Formats samples as collapsed-stack lines: "frame1;frame2;... count".
    /// </summary>
    public static List<string> ToCollapsedLines(this IEnumerable<StackSample> samples) =>
        samples
            .Select(s => $"{string.Join(FrameSeparator, s.Frames)} {s.Count.ToString(CultureInfo.InvariantCulture)}")
            .ToList();

    /// <summary>
    /// Computes the expected subtree total for every path prefix, without using the parser. Each stack adds its
    /// count to every prefix of its frames, which is the definition of a flame graph node's value.
    /// </summary>
    public static Dictionary<string, double> ExpectedSubtreeTotals(IEnumerable<StackSample> samples)
    {
        var totals = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (var sample in samples)
        {
            for (var length = 1; length <= sample.Frames.Count; length++)
            {
                var key = PathKey(sample.Frames.Take(length));
                totals[key] = totals.GetValueOrDefault(key) + sample.Count;
            }
        }

        return totals;
    }

    /// <summary>
    /// Maps every non-root node's path (frame names joined by the separator) to its node. Throws if two siblings
    /// share a name, because the parser must merge identical frames under one parent.
    /// </summary>
    public static Dictionary<string, FlameNode> IndexByPath(FlameNode root)
    {
        var index = new Dictionary<string, FlameNode>(StringComparer.Ordinal);
        CollectPaths(root, Array.Empty<string>(), index);
        return index;
    }

    /// <summary>
    /// Produces a deterministic text form of a whole tree (path, depth, value per node, root included),
    /// so two trees can be compared with a single assertion.
    /// </summary>
    public static string TreeSignature(FlameNode root)
    {
        var lines = new List<string> { FormatNode(string.Empty, root) };
        lines.AddRange(
            IndexByPath(root)
                .OrderBy(kv => kv.Key, StringComparer.Ordinal)
                .Select(kv => FormatNode(kv.Key, kv.Value)));
        return string.Join('\n', lines);
    }

    /// <summary>
    /// Joins frame names into a path key using the collapsed-stack separator.
    /// </summary>
    public static string PathKey(IEnumerable<string> frames) => string.Join(FrameSeparator, frames);

    /// <summary>
    /// Lazily yields the lines of a reader. Used to feed the parser from a stream without loading it into memory.
    /// </summary>
    public static IEnumerable<string> ReadLines(TextReader reader)
    {
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            yield return line;
        }
    }

    private static void CollectPaths(FlameNode node, IReadOnlyList<string> path, IDictionary<string, FlameNode> index)
    {
        foreach (var child in node.Children)
        {
            var childPath = path.Append(child.Name).ToList();
            index.Add(PathKey(childPath), child);
            CollectPaths(child, childPath, index);
        }
    }

    private static string FormatNode(string key, FlameNode node) =>
        $"{key}|{node.Depth.ToString(CultureInfo.InvariantCulture)}|{node.Value.ToString("R", CultureInfo.InvariantCulture)}";
}
