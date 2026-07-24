using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

public sealed class RenderOptions
{
    public int Width { get; set; } = 1600;

    /// <summary>Height of a single frame row, in pixels.</summary>
    public float RowHeight { get; set; } = 22f;

    /// <summary>Frames narrower than this are drawn but not labelled.</summary>
    public float MinLabelWidth { get; set; } = 28f;

    /// <summary>Frames narrower than this are skipped entirely.</summary>
    public float MinBoxWidth { get; set; } = 0.4f;

        /// <summary>
        /// Subtrees whose scaled width falls below this threshold are culled entirely during layout.
        /// This prevents drawing thousands of sub-pixel nodes that would render as &lt;1px boxes.
        /// Set to 0 to disable subtree culling and render all nodes individually.
        /// </summary>
        /// <remarks>
        /// When a frame's width is below this threshold, its entire subtree is skipped and a single
        /// aggregated sliver is rendered instead. This bounds the number of draw calls roughly by
        /// image width × depth instead of node count, significantly improving performance for deep traces.
        /// </remarks>
        public float MinSubtreeWidthPx { get; set; } = 0.5f;

    public float Padding { get; set; } = 16f;

    public float FontSize { get; set; } = 12f;

    public SKColor Background { get; set; } = new(0x1e, 0x1e, 0x24);

    public SKColor TextColor { get; set; } = new(0xf0, 0xf0, 0xf0);

    /// <summary>Draw deepest frames at the top (icicle) instead of bottom (flame).</summary>
    public bool Inverted { get; set; }

/// <summary>
/// Regex pattern for frame names to highlight. Frames matching this pattern will be drawn
/// with a distinct highlight color. If null or empty, no frames are highlighted.
/// </summary>
public string? HighlightPattern { get; set; }
}
