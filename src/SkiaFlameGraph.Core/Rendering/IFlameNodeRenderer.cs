using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Defines a renderer that can render a <see cref="FlameNode"/> tree to an image.
/// </summary>
public interface IFlameNodeRenderer : IDisposable
{
    /// <summary>
    /// Renders the specified flame node tree to an SKImage.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree to render.</param>
    /// <returns>An SKImage containing the rendered result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="root"/> is null.</exception>
    SKImage Render(FlameNode root);

    /// <summary>
    /// Renders the specified flame node tree to a PNG file at the given path.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree to render.</param>
    /// <param name="path">The file path where the PNG should be saved.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="root"/> or <paramref name="path"/> is null.</exception>
    void RenderToPng(FlameNode root, string path);

    /// <summary>
    /// Gets the render options used by this renderer.
    /// </summary>
    RenderOptions Options { get; }
}
