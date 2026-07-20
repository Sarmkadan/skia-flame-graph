using System.Text;
using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Exports flame graphs to SVG format using hand-written SVG generation.
/// Uses the same layout math as FlameGraphRenderer for consistency.
/// </summary>
public class FlameGraphSvgExporter
{
    private readonly RenderOptions _options;

    public FlameGraphSvgExporter(RenderOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public void RenderToSvg(FlameNode root, string path)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));
        if (string.IsNullOrWhiteSpace(path))
            throw new ArgumentException("Path cannot be null or empty", nameof(path));

        var svg = GenerateSvg(root);
        File.WriteAllText(path, svg);
    }

    private string GenerateSvg(FlameNode root)
    {
        var sb = new StringBuilder();
        sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        sb.AppendLine("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");

        // Calculate total value for scaling
        var totalValue = CalculateTotalValue(root);
        var width = _options.Width;
        var height = CalculateTotalHeight(root);

        sb.AppendLine($"<svg width=\"{width}\" height=\"{height}\" version=\"1.1\" xmlns=\"http://www.w3.org/2000/svg\">");
        sb.AppendLine("  <style type=\"text/css\"><![CDATA[");
        sb.AppendLine("    .frame { stroke: #333; stroke-width: 0.5; }");
        sb.AppendLine("    .frame:hover { stroke-width: 1; stroke: #000; }");
        sb.AppendLine("    .frame-label { font-family: Arial, sans-serif; font-size: 11px; fill: #fff; text-shadow: 0 0 2px #000; }");
        sb.AppendLine("  ]]></style>");

        // Draw frames recursively
        var y = 0f;
        DrawFramesRecursive(root, 0, 0, y, (float)totalValue, width, sb);

        sb.AppendLine("</svg>");
        return sb.ToString();
    }

    private double CalculateTotalValue(FlameNode node)
    {
        if (node.Children == null || node.Children.Count == 0)
            return node.Value;

        var total = node.Value;
        foreach (var child in node.Children)
        {
            total += CalculateTotalValue(child);
        }
        return total;
    }

    private int CalculateTotalHeight(FlameNode node)
    {
        if (node == null)
            return 0;

        var maxChildDepth = 0;
        foreach (var child in node.Children)
        {
            maxChildDepth = Math.Max(maxChildDepth, CalculateTotalHeight(child));
        }

        return (node.Depth + 1) * (int)_options.RowHeight;
    }

    private void DrawFramesRecursive(FlameNode node, int depth, float x, float y, float totalValue, int width, StringBuilder sb)
    {
        if (node == null || node.Value <= 0)
            return;

        // Calculate width proportional to node value
        var nodeWidth = (float)(node.Value / totalValue * width);

        // Ensure minimum width for visibility
        if (nodeWidth < 0.4f)
            nodeWidth = 0.4f;

        // Calculate height per depth level
        var rowHeight = _options.RowHeight;

        // Get color from palette
        var color = FramePalette.ForFrame(node.Name);

        // Draw rectangle
        var rectX = x;
        var rectY = y + depth * rowHeight;
        var rectWidth = nodeWidth;
        var rectHeight = rowHeight;

        sb.AppendLine($"  <rect x=\"{rectX:F1}\" y=\"{rectY:F1}\" width=\"{rectWidth:F1}\" height=\"{rectHeight:F1}\" fill=\"#{color.Red:X2}{color.Green:X2}{color.Blue:X2}\" class=\"frame\" data-name=\"{EscapeXml(node.Name)}\" data-value=\"{node.Value}\"/>");

        // Draw label if there's enough space
        if (nodeWidth > 28 && rowHeight > 12)
        {
            var labelX = rectX + 3;
            var labelY = rectY + (rowHeight / 2) + 3;
            var labelText = TruncateLabel(node.Name, (int)(nodeWidth - 6));

            sb.AppendLine($"  <text x=\"{labelX:F1}\" y=\"{labelY:F1}\" class=\"frame-label\">" + EscapeXml(labelText) + "</text>");
        }

        // Recursively draw children
        if (node.Children != null && node.Children.Count > 0)
        {
            var childX = x;
            foreach (var child in node.Children)
            {
                DrawFramesRecursive(child, depth + 1, childX, y, totalValue, width, sb);
                childX += (float)(child.Value / totalValue * width);
            }
        }
    }

    private string TruncateLabel(string label, int maxChars)
    {
        if (label.Length <= maxChars)
            return label;

        return label.Substring(0, maxChars - 3) + "...";
    }

    private string EscapeXml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
    }
}