using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;
using SkiaSharp;

namespace SkiaFlameGraph.IntegrationTests;

/// <summary>
/// Expected on-screen position of one frame, derived from the same layout rules the renderer uses.
/// </summary>
/// <param name="Node">The frame.</param>
/// <param name="Left">Left edge in pixels.</param>
/// <param name="Width">Width in pixels.</param>
/// <param name="Top">Top edge of the frame's row in pixels.</param>
internal readonly record struct FrameBox(FlameNode Node, float Left, float Width, float Top);

/// <summary>
/// Helpers for loading the fixture, computing expected frame layout, and reading pixels back from a render.
/// </summary>
internal static class RenderPipelineIntegrationTestsExtensions
{
    /// <summary>
    /// Returns the absolute path of the fixture copied into the test output directory.
    /// </summary>
    public static string FixturePath() =>
        Path.Combine(AppContext.BaseDirectory, "Fixtures", RenderPipelineIntegrationTestsConstants.FixtureFileName);

    /// <summary>
    /// Returns the top pixel row of a frame at the given depth, for the non-inverted (flame) layout.
    /// </summary>
    public static float FrameRowTop(this RenderOptions options, int depth, int rows) =>
        options.Padding + (rows - 1 - depth) * options.RowHeight;

    /// <summary>
    /// Computes where every frame should be drawn, mirroring the renderer's left-to-right layout.
    /// </summary>
    public static List<FrameBox> ExpectedLayout(FlameNode root, RenderOptions options)
    {
        var rows = root.MaxDepth() + 1;
        var plotWidth = options.Width - options.Padding * 2;
        var boxes = new List<FrameBox>();
        CollectBoxes(root, options.Padding, plotWidth, root.Value, rows, options, boxes);
        return boxes;
    }

    /// <summary>
    /// Counts the contiguous pixels around <paramref name="centerX"/> on row <paramref name="y"/> that exactly match
    /// <paramref name="color"/>. Returns 0 when the centre pixel itself does not match.
    /// </summary>
    public static int MeasureRunWidth(this SKBitmap bitmap, int y, int centerX, SKColor color)
    {
        if (bitmap.GetPixel(centerX, y) != color)
        {
            return 0;
        }

        var left = centerX;
        while (left > 0 && bitmap.GetPixel(left - 1, y) == color)
        {
            left--;
        }

        var right = centerX;
        while (right < bitmap.Width - 1 && bitmap.GetPixel(right + 1, y) == color)
        {
            right++;
        }

        return right - left + 1;
    }

    /// <summary>
    /// Reads a big-endian 32-bit integer from a byte array.
    /// </summary>
    public static int ReadBigEndianInt32(this byte[] bytes, int offset) =>
        (bytes[offset] << 24) | (bytes[offset + 1] << 16) | (bytes[offset + 2] << 8) | bytes[offset + 3];

    private static void CollectBoxes(
        FlameNode node, float left, float width, double total, int rows, RenderOptions options, List<FrameBox> boxes)
    {
        var top = options.FrameRowTop(node.Depth, rows);
        boxes.Add(new FrameBox(node, left, width, top));

        var childLeft = left;
        foreach (var child in node.Children)
        {
            var childWidth = (float)(child.Value / total * (options.Width - options.Padding * 2));
            CollectBoxes(child, childLeft, childWidth, total, rows, options, boxes);
            childLeft += childWidth;
        }
    }
}
