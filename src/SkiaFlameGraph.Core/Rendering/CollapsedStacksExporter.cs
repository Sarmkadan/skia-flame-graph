using System.Globalization;
using System.Text;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Exports a FlameNode tree to Brendan Gregg's collapsed-stacks format.
/// Each line is of the form: frame1;frame2;frame3 123
/// where 123 is the self value for that stack.
/// </summary>
public sealed class CollapsedStacksExporter
{
    /// <summary>
    /// Exports the flame graph tree to collapsed stacks format and writes to a TextWriter.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree.</param>
    /// <param name="writer">The TextWriter to write the output to.</param>
    /// <exception cref="ArgumentNullException">Thrown if root or writer is null.</exception>
    public void Export(FlameNode root, TextWriter writer)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(writer);

        WriteNode(root, new Stack<string>(), writer);
    }

    /// <summary>
    /// Exports the flame graph tree to collapsed stacks format and returns as a string.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree.</param>
    /// <returns>A string containing the collapsed stacks format.</returns>
    /// <exception cref="ArgumentNullException">Thrown if root is null.</exception>
    public string ExportToString(FlameNode root)
    {
        ArgumentNullException.ThrowIfNull(root);

        using var writer = new StringWriter();
        Export(root, writer);
        return writer.ToString();
    }

    private void WriteNode(FlameNode node, Stack<string> stack, TextWriter writer)
    {
        // Preserving original logic: skip nodes with no self value.
        if (node == null || node.SelfValue <= 0)
            return;

        // Push current frame onto stack (Sanitized)
        stack.Push(SanitizeFrameName(node.Name));

        // If this node has children, recursively process them first
        if (node.Children != null && node.Children.Count > 0)
        {
            foreach (var child in node.Children)
            {
                WriteNode(child, stack, writer);
            }
        }
        else
        {
            // Leaf node - write the collapsed stack line
            WriteStackLine(stack, node.SelfValue, writer);
        }

        // Pop current frame from stack
        stack.Pop();
    }

    private void WriteStackLine(Stack<string> stack, double selfValue, TextWriter writer)
    {
        // Build the stack string from top to bottom (root to leaf)
        var frames = stack.Reverse();
        var stackString = string.Join(";", frames);
        
        // Format weight using invariant culture to ensure '.' is used as decimal separator
        var valueString = selfValue.ToString(CultureInfo.InvariantCulture);
        
        writer.WriteLine($"{stackString} {valueString}");
    }

    /// <summary>
    /// Sanitizes a frame name by replacing characters that would corrupt the
    /// collapsed stacks format (semicolons, newlines, carriage returns, and spaces).
    /// </summary>
    /// <param name="name">The original frame name.</param>
    /// <returns>The sanitized frame name.</returns>
    private static string SanitizeFrameName(string name) =>
        name.Replace(';', '_')
            .Replace('\n', '_')
            .Replace('\r', '_')
            .Replace(' ', '_');
}
