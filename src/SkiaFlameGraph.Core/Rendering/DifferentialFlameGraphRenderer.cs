using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Renders differential flame graphs showing changes between two profiles (baseline vs current).
/// Uses a red/blue diverging color palette where:
/// - Red hues indicate regressions (negative delta values - current is slower than baseline)
/// - Blue hues indicate improvements (positive delta values - current is faster than baseline)
/// - White/neutral colors indicate no change (delta close to zero)
/// </summary>
/// <remarks>
/// This renderer is designed for regression reports in CI/CD pipelines, allowing visual comparison
/// of performance changes between two versions of the same application.
/// </remarks>
public sealed class DifferentialFlameGraphRenderer : BaseFlameNodeRenderer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DifferentialFlameGraphRenderer"/> class.
    /// </summary>
    /// <param name="options">The render options to use. If null, a default RenderOptions is created.</param>
    public DifferentialFlameGraphRenderer(RenderOptions? options = null)
        : base(options)
    {
    }

    /// <summary>
    /// Computes and renders a differential flame graph showing the difference between baseline and current profiles.
    /// </summary>
    /// <param name="baseline">The baseline flame graph (typically the older/previous profile).</param>
    /// <param name="current">The current flame graph (typically the newer/current profile).</param>
    /// <returns>An SKImage containing the rendered differential flame graph.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="baseline"/> or <paramref name="current"/> is null.</exception>
    public SKImage RenderDifferential(FlameNode baseline, FlameNode current)
    {
        ArgumentNullException.ThrowIfNull(baseline);
        ArgumentNullException.ThrowIfNull(current);

        // Compute the delta between baseline and current
        var deltaRoot = FlameDiff.Diff(baseline, current);

        // Render the differential flame graph
        return Render(deltaRoot);
    }

    /// <summary>
    /// Computes and renders a differential flame graph showing the difference between baseline and current profiles,
    /// writing the result to a PNG file.
    /// </summary>
    /// <param name="baseline">The baseline flame graph (typically the older/previous profile).</param>
    /// <param name="current">The current flame graph (typically the newer/current profile).</param>
    /// <param name="path">The file path where the PNG should be saved.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null.</exception>
    public void RenderDifferentialToPng(FlameNode baseline, FlameNode current, string path)
    {
        ArgumentNullException.ThrowIfNull(baseline);
        ArgumentNullException.ThrowIfNull(current);
        ArgumentNullException.ThrowIfNull(path);

        using var image = RenderDifferential(baseline, current);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var fs = File.OpenWrite(path);
        data.SaveTo(fs);
    }

    public override SKImage Render(FlameNode root)
    {
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

        // For differential rendering, we need to handle negative values
        // Find the maximum absolute value to normalize the color scale
        var maxAbsValue = Math.Abs(root.Value);
        if (root.Children.Count > 0)
        {
            maxAbsValue = Math.Max(maxAbsValue, FindMaxAbsValue(root));
        }

        // Ensure we have a reasonable scale even for small differences
        if (maxAbsValue <= 0)
        {
            maxAbsValue = 1;
        }

        DrawDifferentialNode(canvas, root, _options.Padding, plotWidth, maxAbsValue, rows, font, stroke, textPaint);

        return surface.Snapshot();
    }

    private double FindMaxAbsValue(FlameNode node)
    {
        var max = Math.Abs(node.Value);
        foreach (var child in node.Children)
        {
            max = Math.Max(max, FindMaxAbsValue(child));
        }
        return max;
    }

    private void DrawDifferentialNode(
        SKCanvas canvas, FlameNode node, float x, float width, double maxAbsValue, int rows,
        SKFont font, SKPaint stroke, SKPaint textPaint)
    {
        if (width < _options.MinBoxWidth)
        {
            return;
        }

        float y = _options.Inverted
            ? _options.Padding + node.Depth * _options.RowHeight
            : _options.Padding + (rows - 1 - node.Depth) * _options.RowHeight;

        var rect = new SKRect(x, y, x + width, y + _options.RowHeight - 1);

        // Use differential color palette based on delta value
        var fillColor = GetDifferentialColor(node.Value, maxAbsValue);
        using var fill = GetPaintForColor(fillColor);
        canvas.DrawRect(rect, fill);
        canvas.DrawRect(rect, stroke);

        // Show delta value as label for debugging/clarity
        var label = FormatDeltaLabel(node.Value);
        if (width >= _options.MinLabelWidth)
        {
            DrawLabel(canvas, label, rect, font, textPaint);
        }

        // Apply subtree culling for differential rendering
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

        // Lay children left-to-right, each scaled to its share of the parent
        var childX = x;
        var parentRightEdge = x + width;

        foreach (var child in node.Children)
        {
            var childWidth = (float)(Math.Abs(child.Value) / maxAbsValue * (_options.Width - _options.Padding * 2));
            // Clamp child width to parent's bounds to prevent overflow
            var clampedChildWidth = Math.Min(childWidth, parentRightEdge - childX);
            DrawDifferentialNode(canvas, child, childX, clampedChildWidth, maxAbsValue, rows, font, stroke, textPaint);
            childX += clampedChildWidth;
        }
    }

    private SKColor GetDifferentialColor(double deltaValue, double maxAbsValue)
    {
        // Normalize the delta to a value between -1 and 1
        var normalized = deltaValue / maxAbsValue;

        // Clamp to avoid floating point issues
        normalized = Math.Clamp(normalized, -1.0, 1.0);

        // Use a diverging red-blue palette:
        // - Negative values (regressions) -> red hues (0-30 degrees)
        // - Positive values (improvements) -> blue hues (210-270 degrees)
        // - Zero (no change) -> white/neutral
        if (Math.Abs(normalized) < 0.05) // Small values appear neutral
        {
            // White/neutral for values close to zero
            var neutralFactor = 1.0 - Math.Abs(normalized) * 20; // Fade to white as we approach zero
            return new SKColor(
                (byte)(255 * neutralFactor),
                (byte)(255 * neutralFactor),
                (byte)(255 * neutralFactor)
            );
        }
        else if (normalized < 0)
        {
            // Red palette for regressions (negative deltas)
            // Scale from -1 to 0 -> hue from 0 (red) to 30 (orange-red)
            var hue = 0f + (float)(-normalized * 30f);
            var saturation = 70f + (float)(-normalized * 30f); // 70-100%
            var lightness = 40f + (float)(-normalized * 20f); // 40-60%
            return SKColor.FromHsl(hue, saturation, lightness);
        }
        else
        {
            // Blue palette for improvements (positive deltas)
            // Scale from 0 to 1 -> hue from 210 (blue) to 270 (purple-blue)
            var hue = 210f + (float)(normalized * 60f);
            var saturation = 70f + (float)(normalized * 30f); // 70-100%
            var lightness = 40f + (float)(normalized * 20f); // 40-60%
            return SKColor.FromHsl(hue, saturation, lightness);
        }
    }

    private string FormatDeltaLabel(double deltaValue)
    {
        if (Math.Abs(deltaValue) < 0.01)
        {
            return "≈0";
        }

        var sign = deltaValue > 0 ? "+" : "";
        var absValue = Math.Abs(deltaValue);

        if (absValue >= 1000)
        {
            return $"{(int)deltaValue:+#;-#;0}";
        }
        else if (absValue >= 100)
        {
            return $"{(int)(deltaValue * 10) / 10.0:+#.#;-#.#;0.0}";
        }
        else if (absValue >= 10)
        {
            return $"{(int)(deltaValue * 100) / 100.0:+#.##;-#.##;0.00}";
        }
        else
        {
            return $"{(int)(deltaValue * 1000) / 1000.0:+#.###;-#.###;0.000}";
        }
    }

    private void DrawLabel(SKCanvas canvas, string text, SKRect rect, SKFont font, SKPaint paint)
    {
        var padded = rect.Width - 6f;
        var display = Ellipsize(text, font, padded);
        if (display.Length == 0)
        {
            return;
        }

        var baseline = rect.MidY + _options.FontSize * 0.35f;
        canvas.Save();
        canvas.ClipRect(rect);
        canvas.DrawText(display, rect.Left + 3f, baseline, SKTextAlign.Left, font, paint);
        canvas.Restore();
    }

    private static string Ellipsize(string text, SKFont font, float maxWidth)
    {
        if (maxWidth <= 0)
        {
            return string.Empty;
        }
        if (font.MeasureText(text) <= maxWidth)
        {
            return text;
        }

        const string ell = "…";
        for (var len = text.Length - 1; len > 0; len--)
        {
            var candidate = text[..len] + ell;
            if (font.MeasureText(candidate) <= maxWidth)
            {
                return candidate;
            }
        }
        return string.Empty;
    }
}