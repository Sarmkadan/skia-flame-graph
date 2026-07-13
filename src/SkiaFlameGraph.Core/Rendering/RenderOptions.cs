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

    public float Padding { get; set; } = 16f;

    public float FontSize { get; set; } = 12f;

    public SKColor Background { get; set; } = new(0x1e, 0x1e, 0x24);

    public SKColor TextColor { get; set; } = new(0xf0, 0xf0, 0xf0);

    /// <summary>Draw deepest frames at the top (icicle) instead of bottom (flame).</summary>
    public bool Inverted { get; set; }
}
