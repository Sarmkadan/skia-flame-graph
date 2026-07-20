using System.Text;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Exports a FlameNode tree to Brendan Gregg's collapsed-stacks format.
/// Each line is of the form: frame1;frame2;frame3 123
/// where 123 is the self value for that stack.
/// </summary>
public class CollapsedStacksExporter
{
    /// <summary>
    /// Exports the flame graph tree to collapsed stacks format and writes to a TextWriter.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree.</param>
    /// <param name="writer">The TextWriter to write the output to.</param>
    public void Export(FlameNode root, TextWriter writer)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));
        if (writer == null)
            throw new ArgumentNullException(nameof(writer));

        WriteNode(root, new Stack<string>(), writer);
    }

    /// <summary>
    /// Exports the flame graph tree to collapsed stacks format and returns as a string.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree.</param>
    /// <returns>A string containing the collapsed stacks format.</returns>
    public string ExportToString(FlameNode root)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));

        using var writer = new StringWriter();
        Export(root, writer);
        return writer.ToString();
    }

    private void WriteNode(FlameNode node, Stack<string> stack, TextWriter writer)
    {
        if (node == null || node.SelfValue <= 0)
            return;

        // Push current frame onto stack
        stack.Push(node.Name);

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
        var frames = new List<string>();
        foreach (var frame in stack.Reverse())
        {
            frames.Add(frame);
        }

        // Join frames with semicolons and write with self value
        var stackString = string.Join(";", frames);
        writer.WriteLine($"{stackString} {selfValue}");
    }
}
