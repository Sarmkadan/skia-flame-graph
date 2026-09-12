using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

public interface IFlameGraphRenderer
{
    /// <summary>
    /// Renders the flame graph to a PNG file.
    /// </summary>
    /// <param name="root">The root node of the flame graph.</param>
    /// <param name="path">The file path to save the PNG.</param>
    void RenderToPng(FlameNode root, string path);
    /// <summary>
    /// Renders the flame graph and returns an SKImage.
    /// </summary>
    /// <param name="root">The root node of the flame graph.</param>
    /// <returns>The rendered flame graph as an SKImage.</returns>
    SKImage Render(FlameNode root);
}
