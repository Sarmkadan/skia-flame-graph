using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Abstract base class for flame node renderers, providing common functionality
/// such as paint caching, resource disposal, and PNG encoding.
/// </summary>
public abstract class BaseFlameNodeRenderer : IFlameNodeRenderer
{
    protected readonly RenderOptions _options;
    protected readonly Dictionary<SKColor, SKPaint> _paintCache = new();
    protected bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseFlameNodeRenderer"/> class.
    /// </summary>
    /// <param name="options">The render options to use. If null, a default RenderOptions is created.</param>
    protected BaseFlameNodeRenderer(RenderOptions? options = null)
    {
        _options = options ?? new RenderOptions();
        _options.EnsureValid();
    }

    /// <summary>
    /// Gets the render options used by this renderer.
    /// </summary>
    public RenderOptions Options => _options;

    /// <summary>
    /// Renders the specified flame node tree to a PNG file at the given path.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree to render.</param>
    /// <param name="path">The file path where the PNG should be saved.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="root"/> or <paramref name="path"/> is null.</exception>
    public virtual void RenderToPng(FlameNode root, string path)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(path);

        using var image = Render(root);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        using var fs = File.OpenWrite(path);
        data.SaveTo(fs);
    }

    /// <summary>
    /// Gets or creates a cached SKPaint for the specified color.
    /// </summary>
    /// <param name="color">The color to get or create a paint for.</param>
    /// <returns>A cached SKPaint instance with the specified color.</returns>
    protected SKPaint GetPaintForColor(SKColor color)
    {
        if (_paintCache.TryGetValue(color, out var cachedPaint))
        {
            return cachedPaint;
        }

        var paint = new SKPaint
        {
            IsAntialias = true,
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

    /// <summary>
    /// Disposes of managed resources.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
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

    /// <summary>
    /// Renders the specified flame node tree to an SKImage.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree to render.</param>
    /// <returns>An SKImage containing the rendered result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="root"/> is null.</exception>
    public abstract SKImage Render(FlameNode root);

    /// <summary>
    /// Renders the specified flame node tree to an SKImage with an optional height parameter.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree to render.</param>
    /// <param name="height">Optional height in pixels. If null, a default height is calculated.</param>
    /// <returns>An SKImage containing the rendered result.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="root"/> is null.</exception>
    public virtual SKImage Render(FlameNode root, int? height)
    {
        ArgumentNullException.ThrowIfNull(root);
        return Render(root);
    }
}
