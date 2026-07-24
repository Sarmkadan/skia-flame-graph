using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Renders the same call tree as a squarified treemap. Each frame's descendants
/// are packed into its rectangle; the treemap view is handy when you care more
/// about aggregate cost than call ordering.
/// </summary>
public sealed class TreemapRenderer : IDisposable
{
    private readonly RenderOptions _options;
    private readonly Dictionary<SKColor, SKPaint> _paintCache = new();
    private bool _disposed;

    public TreemapRenderer(RenderOptions? options = null)
    {
        _options = options ?? new RenderOptions();
    }

    public void RenderToPng(FlameNode root, string path, int? height = null)
    {
        using var image = Render(root, height);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var fs = File.OpenWrite(path);
        data.SaveTo(fs);
    }

    public SKImage Render(FlameNode root, int? height = null)
    {
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
    /// Gets or creates a cached SKPaint for the specified color.
    /// </summary>
    /// <param name="color">The color to get or create a paint for.</param>
    /// <returns>A cached SKPaint instance with the specified color.</returns>
    private SKPaint GetPaintForColor(SKColor color)
    {
        if (_paintCache.TryGetValue(color, out var cachedPaint))
        {
            return cachedPaint;
        }

        var paint = new SKPaint
        {
            IsAntialias = false,
            Style = SKPaintStyle.Fill,
            Color = color
        };
        _paintCache[color] = paint;
        return paint;
    }

    /// <summary>
    /// Disposes of all cached paint objects.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            foreach (var paint in _paintCache.Values)
            {
                paint.Dispose();
            }
            _paintCache.Clear();
        }

        _disposed = true;
    }

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
        if (total <= 0)
            return;

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

    private double WorstRatio(
        List<FlameNode> row, double candidateValue, double rowValue, double newRowValue,
        float shorter, float areaPerValue)
    {
        var side = shorter;
        var rowArea = newRowValue * areaPerValue;
        var rowLength = rowArea / side;
        if (rowLength <= 0)
            return double.MaxValue;

        var worst = 0.0;
        void Consider(double value)
        {
            var boxArea = value * areaPerValue;
            var boxLength = boxArea / rowLength;
            if (boxLength <= 0) return;
            var ratio = Math.Max(rowLength / boxLength, boxLength / rowLength);
            worst = Math.Max(worst, ratio);
        }

        foreach (var n in row)
            Consider(n.Value);
        Consider(candidateValue);
        return worst;
    }

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

    private static double RemainingValue(List<FlameNode> children, int from, double total)
    {
        var consumed = 0.0;
        for (var i = 0; i < from; i++)
            consumed += children[i].Value;
        var left = total - consumed;
        return left <= 0 ? total : left;
    }

    private static SKRect Deflate(SKRect r)
    {
        var d = new SKRect(r.Left + 1, r.Top + 1, r.Right - 1, r.Bottom - 1);
        return d.Width < 0 || d.Height < 0 ? r : d;
    }

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