using SkiaSharp;
using System;

namespace SkiaFlameGraph.Core.Rendering
{
    public interface IRenderOptions
    {
        /// <summary>
        /// Gets the width of the rendered flame graph, in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of a single frame row, in pixels.
        /// </summary>
        float RowHeight { get; }

        /// <summary>
        /// Gets the minimum frame width required to draw a label.
        /// </summary>
        float MinLabelWidth { get; }

        /// <summary>
        /// Gets the minimum frame width required to draw the frame.
        /// </summary>
        float MinBoxWidth { get; }

        /// <summary>
        /// Gets the padding around the rendered flame graph, in pixels.
        /// </summary>
        float Padding { get; }

        /// <summary>
        /// Gets the font size used for frame labels, in pixels.
        /// </summary>
        float FontSize { get; }

        /// <summary>
        /// Gets the background color of the rendered flame graph.
        /// </summary>
        SKColor Background { get; }

        /// <summary>
        /// Gets the color used for frame label text.
        /// </summary>
        SKColor TextColor { get; }

        /// <summary>
        /// Gets a value indicating whether the deepest frames are drawn at the top.
        /// </summary>
        bool Inverted { get; }

        /// <summary>
        /// Gets the regular expression pattern used to highlight matching frame names, or <see langword="null"/> when highlighting is disabled.
        /// </summary>
        string? HighlightPattern { get; }
    }
}
