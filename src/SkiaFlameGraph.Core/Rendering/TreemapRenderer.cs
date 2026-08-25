using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Renders the same call tree as a squarified treemap. Each frame's descendants
/// are packed into its rectangle; the treemap view is handy when you care more
/// about aggregate cost than call ordering.
/// </summary>
public sealed class TreemapRenderer : BaseFlameNodeRenderer, ITreemapRenderer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TreemapRenderer"/> class.
    /// </summary>
    /// <param name="options">The render options to use. If null, a default RenderOptions is created.</param>
    public TreemapRenderer(RenderOptions? options = null)
        : base(options)
    {
    }

    /// <summary>
    /// Validates the arguments and delegates PNG export to the base renderer implementation.
    /// </summary>
    /// <param name="root">Root node of the tree to render.</param>
    /// <param name="path">Destination file path for the PNG image.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="root"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="path"/> is null or empty.</exception>
    public override void RenderToPng(FlameNode root, string path)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentException.ThrowIfNullOrEmpty(path);
        base.RenderToPng(root, path);
    }

    /// <summary>
    /// Renders the treemap using the default height derived from the configured width.
    /// </summary>
    /// <param name="root">Root node of the tree to render.</param>
    /// <returns>The rendered treemap image.</returns>
    public override SKImage Render(FlameNode root)
    {
        ArgumentNullException.ThrowIfNull(root);
        return Render(root, null);
    }

    /// <summary>
    /// Renders the squarified treemap onto a new surface: clears the background, computes
    /// the drawing area inset by the configured padding, and recursively lays out the tree.
    /// </summary>
    /// <param name="root">Root node of the tree to render.</param>
    /// <param name="height">
    /// Explicit image height in pixels; when null, defaults to roughly 62% of the width.
    /// </param>
    /// <returns>A snapshot of the rendered treemap surface.</returns>
    public override SKImage Render(FlameNode root, int? height)
    {
        ArgumentNullException.ThrowIfNull(root);
        var h = height ?? (int)(_options.Width * 0.62f);
        var info = new SKImageInfo(_options.Width, h, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;
        canvas.Clear(_options.Background);

        using var stroke = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = _options.Background,
        };
        using var font = new SKFont(SKTypeface.Default, _options.FontSize);
        using var textPaint = new SKPaint { IsAntialias = true, Color = _options.TextColor };

        var area = new SKRect(
            _options.Padding, _options.Padding,
            _options.Width - _options.Padding, h - _options.Padding);

        Layout(canvas, root, area, stroke, font, textPaint, 0);
        return surface.Snapshot();
    }

    /// <summary>
    /// Draws one subtree's cell: leaves (and subtrees past the depth guard of 12) get a
    /// palette fill, border stroke and clipped label; internal nodes sort their children
    /// largest-first and pack them via <see cref="Squarify"/>.
    /// </summary>
    /// <param name="canvas">Canvas to draw on.</param>
    /// <param name="node">Subtree whose descendants occupy <paramref name="rect"/>.</param>
    /// <param name="rect">Rectangle available to this subtree.</param>
    /// <param name="stroke">Shared stroke paint used for cell borders.</param>
    /// <param name="font">Font used for labels.</param>
    /// <param name="textPaint">Paint used for label text.</param>
    /// <param name="depth">Current recursion depth; recursion stops at depth 12.</param>
    private void Layout(
        SKCanvas canvas, FlameNode node, SKRect rect,
        SKPaint stroke, SKFont font, SKPaint textPaint, int depth)
    {
        if (rect.Width < 2 || rect.Height < 2)
            return;

        if (node.Children.Count == 0 || depth >= 12)
        {
            var fillColor = FramePalette.ForFrame(node.Name);
            using var fill = GetPaintForColor(fillColor);
            canvas.DrawRect(rect, fill);
            canvas.DrawRect(rect, stroke);
            DrawLabel(canvas, node.Name, rect, font, textPaint);
            return;
        }

        // Sort children largest-first for a tidier squarified packing.
        var children = new List<FlameNode>(node.Children);
        children.Sort((a, b) => b.Value.CompareTo(a.Value));

        Squarify(canvas, children, rect, stroke, font, textPaint, depth);
    }

    /// <summary>
    /// Squarified treemap algorithm (Bruls, Huizing, van Wijk 2000): greedily
    /// pack children into rows, keeping aspect ratios as close to 1 as possible.
    /// </summary>
    private void Squarify(
        SKCanvas canvas, List<FlameNode> children, SKRect rect,
        SKPaint stroke, SKFont font, SKPaint textPaint, int depth)
    {
        var total = 0.0;
        foreach (var c in children)
            total += c.Value;
        if (total <= 0) return;

        var remaining = rect;
        var index = 0;

        while (index < children.Count)
        {
            var shorter = Math.Min(remaining.Width, remaining.Height);
            var areaPerValue = (remaining.Width * remaining.Height) / (float)RemainingValue(children, index, total);

            var row = new List<FlameNode>();
            var rowValue = 0.0;
            var bestRatio = double.MaxValue;

            while (index + row.Count < children.Count)
            {
                var candidate = children[index + row.Count];
                var newRowValue = rowValue + candidate.Value;
                var ratio = WorstRatio(row, candidate.Value, rowValue, newRowValue, shorter, areaPerValue);

                if (row.Count > 0 && ratio > bestRatio)
                    break;

                row.Add(candidate);
                rowValue = newRowValue;
                bestRatio = ratio;
            }

            remaining = PlaceRow(canvas, row, rowValue, remaining, areaPerValue,
                stroke, font, textPaint, depth);
            index += row.Count;
        }
    }

    /// <summary>
    /// Computes the worst length-to-width aspect ratio the row would exhibit if the
    /// candidate joined it; the greedy loop closes the row once this ratio worsens.
    /// </summary>
    /// <param name="row">Nodes already placed in the current row.</param>
    /// <param name="candidateValue">Value of the node being considered for the row.</param>
    /// <param name="rowValue">Summed value of the nodes already in the row.</param>
    /// <param name="newRowValue">Row value if the candidate is admitted.</param>
    /// <param name="shorter">Length of the remaining rectangle's shorter side.</param>
    /// <param name="areaPerValue">Pixels of area allotted per unit of node value.</param>
    /// <returns>The worst aspect ratio over all boxes in the hypothetical row.</returns>
    private double WorstRatio(
        List<FlameNode> row, double candidateValue, double rowValue, double newRowValue,
        float shorter, float areaPerValue)
    {
        var side = shorter;
        var rowArea = newRowValue * areaPerValue;
        var rowLength = rowArea / side;
        if (rowLength <= 0) return double.MaxValue;

        var worst = 0.0;
        void Consider(double value)
        {
            var boxArea = value * areaPerValue;
            var boxLength = boxArea / rowLength;
            if (boxLength <= 0) return;
            var ratio = Math.Max(rowLength / boxLength, boxLength / rowLength);
            worst = Math.Max(worst, ratio);
        }

        foreach (var n in row) Consider(n.Value);
        Consider(candidateValue);
        return worst;
    }

    /// <summary>
    /// Places a finished row along the shorter side of the remaining rectangle, slicing it
    /// proportionally to each node's value, then recurses into <see cref="Layout"/> per cell.
    /// </summary>
    /// <param name="canvas">Canvas to draw on.</param>
    /// <param name="row">Nodes belonging to the row being placed.</param>
    /// <param name="rowValue">Summed value of the row's nodes.</param>
    /// <param name="remaining">Rectangle still unoccupied by placed rows.</param>
    /// <param name="areaPerValue">Pixels of area allotted per unit of node value.</param>
    /// <param name="stroke">Shared stroke paint used for cell borders.</param>
    /// <param name="font">Font used for labels.</param>
    /// <param name="textPaint">Paint used for label text.</param>
    /// <param name="depth">Current recursion depth passed down to child layouts.</param>
    /// <returns>The remainder of the rectangle left for subsequent rows.</returns>
    private SKRect PlaceRow(
        SKCanvas canvas, List<FlameNode> row, double rowValue, SKRect remaining, float areaPerValue,
        SKPaint stroke, SKFont font, SKPaint textPaint, int depth)
    {
        var rowArea = (float)(rowValue * areaPerValue);
        var horizontal = remaining.Width >= remaining.Height;

        if (horizontal)
        {
            var rowWidth = rowArea / remaining.Height;
            var y = remaining.Top;
            foreach (var n in row)
            {
                var boxHeight = (float)(n.Value / rowValue) * remaining.Height;
                var cell = new SKRect(remaining.Left, y, remaining.Left + rowWidth, y + boxHeight);
                Layout(canvas, n, Deflate(cell), stroke, font, textPaint, depth + 1);
                y += boxHeight;
            }
            return new SKRect(remaining.Left + rowWidth, remaining.Top, remaining.Right, remaining.Bottom);
        }
        else
        {
            var rowHeight = rowArea / remaining.Width;
            var x = remaining.Left;
            foreach (var n in row)
            {
                var boxWidth = (float)(n.Value / rowValue) * remaining.Width;
                var cell = new SKRect(x, remaining.Top, x + boxWidth, remaining.Top + rowHeight);
                Layout(canvas, n, Deflate(cell), stroke, font, textPaint, depth + 1);
                x += boxWidth;
            }
            return new SKRect(remaining.Left, remaining.Top + rowHeight, remaining.Right, remaining.Bottom);
        }
    }

    /// <summary>
    /// Sums the values of the children not yet packed into rows, falling back to the total
    /// when rounding would leave a non-positive remainder.
    /// </summary>
    /// <param name="children">Children being packed, in placement order.</param>
    /// <param name="from">Index of the first child not yet placed.</param>
    /// <param name="total">Total value of all children.</param>
    /// <returns>Unpacked value remaining, guaranteed to be positive.</returns>
    private static double RemainingValue(List<FlameNode> children, int from, double total)
    {
        ArgumentNullException.ThrowIfNull(children);
        var consumed = 0.0;
        for (var i = 0; i < from; i++)
            consumed += children[i].Value;
        var left = total - consumed;
        return left <= 0 ? total : left;
    }

    /// <summary>
    /// Insets a rectangle by one pixel on every side to create a visual gap between cells,
    /// returning the original rectangle when insetting would collapse it.
    /// </summary>
    /// <param name="r">Rectangle to shrink.</param>
    /// <returns>The deflated rectangle, or <paramref name="r"/> if deflation would invert it.</returns>
    private static SKRect Deflate(SKRect r)
    {
        var d = new SKRect(r.Left + 1, r.Top + 1, r.Right - 1, r.Bottom - 1);
        return d.Width < 0 || d.Height < 0 ? r : d;
    }

    /// <summary>
    /// Draws a frame's name inside its cell, clipped to the cell bounds; skipped when the
    /// cell is too small to fit readable text.
    /// </summary>
    /// <param name="canvas">Canvas to draw on.</param>
    /// <param name="text">Frame name to display.</param>
    /// <param name="rect">Cell rectangle constraining the label.</param>
    /// <param name="font">Font used for the label.</param>
    /// <param name="paint">Paint used for the label text.</param>
    private void DrawLabel(SKCanvas canvas, string text, SKRect rect, SKFont font, SKPaint paint)
    {
        if (rect.Width < 34 || rect.Height < _options.FontSize + 2)
            return;
        canvas.Save();
        canvas.ClipRect(rect);
        canvas.DrawText(text, rect.Left + 3, rect.Top + _options.FontSize, SKTextAlign.Left, font, paint);
        canvas.Restore();
    }
}
