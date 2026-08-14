using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Exports a <see cref="FlameNode"/> tree to Brendan Gregg's collapsed‑stacks format.
/// Each line is of the form: <c>frame1;frame2;frame3 123</c> where <c>123</c> is the self value for that stack.
/// </summary>
public sealed class CollapsedStacksExporter
{
    /// <summary>
    /// Exports the flame graph tree to collapsed‑stacks format and writes to a <see cref="TextWriter"/>.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree.</param>
    /// <param name="writer">The <see cref="TextWriter"/> to write the output to.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="root"/> or <paramref name="writer"/> is <c>null</c>.</exception>
    public void Export(FlameNode root, TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(writer);

        WriteNode(root, new Stack<string>(), writer);
    }

    /// <summary>
    /// Exports the flame graph tree to collapsed‑stacks format and returns the result as a <see cref="string"/>.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree.</param>
    /// <returns>A string containing the collapsed‑stacks representation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="root"/> is <c>null</c>.</exception>
    public string ExportToString(FlameNode root)
    {
        ArgumentNullException.ThrowIfNull(root);

        using var writer = new StringWriter();
        Export(root, writer);
        return writer.ToString();
    }

    /// <summary>
    /// Exports the flame graph tree to collapsed‑stacks format and writes the result to a file.
    /// The <paramref name="filePath"/> is resolved against <paramref name="baseDirectory"/>; any attempt to
    /// traverse outside <paramref name="baseDirectory"/> results in an <see cref="ArgumentException"/>.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree.</param>
    /// <param name="filePath">The target file path (may be relative or absolute).</param>
    /// <param name="baseDirectory">The directory that all output files must reside in.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="root"/>, <paramref name="filePath"/> or <paramref name="baseDirectory"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="filePath"/> is empty, <paramref name="baseDirectory"/> is empty,
    /// or if the resolved <paramref name="filePath"/> escapes <paramref name="baseDirectory"/>.</exception>
    public void ExportToFile(FlameNode root, string filePath, string baseDirectory)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(filePath);
        ArgumentNullException.ThrowIfNull(baseDirectory);
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path must not be empty.", nameof(filePath));
        if (string.IsNullOrWhiteSpace(baseDirectory))
            throw new ArgumentException("Base directory must not be empty.", nameof(baseDirectory));

        // Resolve both paths to absolute form.
        var baseFull = Path.GetFullPath(baseDirectory);
        var targetFull = Path.GetFullPath(Path.Combine(baseFull, filePath));

        // Ensure the target path starts with the base directory path (case‑insensitive on Windows).
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        if (!targetFull.StartsWith(baseFull, comparison))
            throw new ArgumentException($"The resolved path '{targetFull}' escapes the base directory '{baseFull}'.", nameof(filePath));

        // Ensure the directory exists.
        var dir = Path.GetDirectoryName(targetFull) ?? baseFull;
        Directory.CreateDirectory(dir);

        using var writer = new StreamWriter(targetFull, append: false, Encoding.UTF8);
        Export(root, writer);
    }

    private static void WriteNode(FlameNode node, Stack<string> stack, TextWriter writer)
    {
        // Preserve original logic: skip nodes with no self value.
        if (node == null || node.SelfValue <= 0)
            return;

        // Push current frame onto stack (sanitized).
        stack.Push(SanitizeFrameName(node.Name));

        // Recurse into children first, if any.
        if (node.Children != null && node.Children.Count > 0)
        {
            foreach (var child in node.Children)
                WriteNode(child, stack, writer);
        }
        else
        {
            // Leaf node – write the collapsed‑stack line.
            WriteStackLine(stack, node.SelfValue, writer);
        }

        // Pop current frame from stack.
        stack.Pop();
    }

    private static void WriteStackLine(Stack<string> stack, double selfValue, TextWriter writer)
    {
        // Build the stack string from root to leaf.
        var frames = stack.Reverse();
        var stackString = string.Join(";", frames);

        // Use invariant culture to guarantee '.' as decimal separator.
        var valueString = selfValue.ToString(CultureInfo.InvariantCulture);

        writer.WriteLine($"{stackString} {valueString}");
    }

    /// <summary>
    /// Sanitizes a frame name by replacing characters that would corrupt the collapsed‑stacks format
    /// (semicolon, newline, carriage‑return, and space) with an underscore.
    /// </summary>
    /// <param name="name">The original frame name.</param>
    /// <returns>The sanitized frame name.</returns>
    private static string SanitizeFrameName(string name) =>
        name.Replace(';', '_')
            .Replace('\n', '_')
            .Replace('\r', '_')
            .Replace(' ', '_');
}
