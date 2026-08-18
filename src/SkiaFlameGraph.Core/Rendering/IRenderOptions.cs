using SkiaSharp;
using System;

namespace SkiaFlameGraph.Core.Rendering
{
    public interface IRenderOptions
    {
        int Width { get; }
        float RowHeight { get; }
        float MinLabelWidth { get; }
        float MinBoxWidth { get; }
        float Padding { get; }
        float FontSize { get; }
        SKColor Background { get; }
        SKColor TextColor { get; }
        bool Inverted { get; }
        string? HighlightPattern { get; }
    }
}
