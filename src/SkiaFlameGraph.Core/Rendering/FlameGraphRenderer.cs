using SkiaFlameGraph.Core.Models;
using SkiaSharp;
using System;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Renders a call tree as a flame graph: every frame is a box whose width is 8
/// proportional to its total time, stacked by call depth.
/// </summary>
public sealed class FlameGraphRenderer : BaseFlameNodeRenderer, IFlameGraphRenderer
{
    private readonly Dictionary<string, string> _labelCache = new(StringComparer.Ordinal);

    /// <summary>
    /// Initializes a new instance of the <see cref="FlameGraphRenderer"/> class.
    /// </summary>
    /// <param name="options">The render options to use. If null, a default RenderOptions is created.</param>
    public FlameGraphRenderer(RenderOptions? options = null)
        : base(options)
    {
    }

    /// <summary>Render to an encoded PNG written to <paramref name="path"/>.</summary>
    public override void RenderToPng(FlameNode root, string path)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentException.ThrowIfNullOrEmpty(path);

        base.RenderToPng(root, path);
    }

    public override SKImage Render(FlameNode root)
    {
        ArgumentNullException.ThrowIfNull(root);

        var depth = root.MaxDepth();
        var rows = depth + 1;
        var height = (int)MathF.Ceiling(rows * _options.RowHeight + _options.Padding * 2);
        var plotWidth = _options.Width - _options.Padding * 2;

        var info = new SKImageInfo(_options.Width, height, SKColorType.Rgba8888, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info);
        var canvas = surface.Canvas;
        canvas.Clear(_options.Background);

        using var font = new SKFont(SKTypeface.Default, _options.FontSize);
        using var stroke = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 1,
            Color = _options.Background,
        };
        using var textPaint = new SKPaint { IsAntialias = true, Color = _options.TextColor };

        var total = root.Value;
        if (total <= 0) total = 1;

        DrawNode(canvas, root, _options.Padding, plotWidth, total, rows, font, stroke, textPaint);

        return surface.Snapshot();
    }

    private void DrawNode(
        SKCanvas canvas, FlameNode node, float x, float width, double total, int rows,
        SKFont font, SKPaint stroke, SKPaint textPaint)
    {
        if (width < _options.MinBoxWidth)
            return;

        float y = _options.Inverted
            ? _options.Padding + node.Depth * _options.RowHeight
            : _options.Padding + (rows - 1 - node.Depth) * _options.RowHeight;

        var rect = new SKRect(x, y, x + width, y + _options.RowHeight - 1);

        // The synthetic root gets a flat bar; real frames get palette colours.
        var fillColor = node.Depth == 0
            ? new SKColor(0x3a, 0x3a, 0x44)
            : FramePalette.ForFrame(node.Name, _options.HighlightPattern);
        using var fill = GetPaintForColor(fillColor);
        canvas.DrawRect(rect, fill);
        canvas.DrawRect(rect, stroke);

        if (width >= _options.MinLabelWidth)
            DrawLabel(canvas, node.Name, rect, font, textPaint);

        // Apply subtree culling: if the parent frame itself is below the threshold, skip rendering children entirely
        // and render a single aggregated elided sliver instead
        if (node.Children.Count > 0 && !_options.ShouldRenderSubtree(width))
        {
            // Render a single aggregated sliver representing all culled children
            if (_options.ShouldRenderFrame(width))
            {
                float yElided = _options.Inverted
                    ? _options.Padding + node.Depth * _options.RowHeight
                    : _options.Padding + (rows - 1 - node.Depth) * _options.RowHeight;

                var rectElided = new SKRect(x, yElided, x + width, yElided + _options.RowHeight - 1);
                var elidedFillColor = FramePalette.ForFrame("[...]", _options.HighlightPattern);
                using var elidedFill = GetPaintForColor(elidedFillColor);
                canvas.DrawRect(rectElided, elidedFill);
                canvas.DrawRect(rectElided, stroke);
            }
            return;
        }

        // Lay children left-to-right, each scaled to its share of the parent.
        // Clamp each child's width to ensure it doesn't exceed the parent's remaining width,
        // which prevents drawing past the parent's right edge when child weights exceed parent weight.
        var childX = x;
        var parentRightEdge = x + width;

        foreach (var child in node.Children)
        {
            var childWidth = (float)(child.Value / total * (_options.Width - _options.Padding * 2));
            // Clamp child width to parent's bounds to prevent overflow
            var clampedChildWidth = Math.Min(childWidth, parentRightEdge - childX);
            DrawNode(canvas, child, childX, clampedChildWidth, total, rows, font, stroke, textPaint);
            childX += clampedChildWidth;
        }
    }

    private void DrawLabel(SKCanvas canvas, string text, SKRect rect, SKFont font, SKPaint paint)
    {
        var padded = rect.Width - 6f;
        var display = Ellipsize(text, font, padded);
        if (display.Length == 0)
            return;

        var baseline = rect.MidY + _options.FontSize * 0.35f;
        canvas.Save();
        canvas.ClipRect(rect);
        canvas.DrawText(display, rect.Left + 3f, baseline, SKTextAlign.Left, font, paint);
        canvas.Restore();
    }

    private static string Ellipsize(string text, SKFont font, float maxWidth)
    {
        if (maxWidth <= 0)
            return string.Empty;
        if (font.MeasureText(text) <= maxWidth)
            return text;

        const string ell = "…";
        for (var len = text.Length - 1; len > 0; len--)
        {
            var candidate = text[..len] + ell;
            if (font.MeasureText(candidate) <= maxWidth)
                return candidate;
        }
        return string.Empty;
    }
}
