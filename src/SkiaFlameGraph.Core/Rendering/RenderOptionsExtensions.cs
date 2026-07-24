using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Provides extension methods for <see cref="RenderOptions"/> to facilitate common rendering operations and calculations.
/// </summary>
public static class RenderOptionsExtensions
{
    /// <summary>
    /// Calculates the total height required to render all frames based on the number of rows and row height.
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <param name="rowCount">The number of rows to render.</param>
    /// <returns>The total height in pixels required for rendering.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static int CalculateTotalHeight(this RenderOptions options, int rowCount)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfLessThan(rowCount, 0);

        return (int)(rowCount * options.RowHeight) + (int)(2 * options.Padding);
    }

    /// <summary>
    /// Calculates the available width for frame content after accounting for padding.
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <returns>The available width in pixels for frame content.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static int CalculateContentWidth(this RenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return options.Width - (int)(2 * options.Padding);
    }

    /// <summary>
    /// Determines whether a frame with the specified width should be labeled based on <see cref="RenderOptions.MinLabelWidth"/>.
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <param name="frameWidth">The width of the frame in pixels.</param>
    /// <returns><see langword="true"/> if the frame should be labeled; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="frameWidth"/> is negative.</exception>
    public static bool ShouldLabelFrame(this RenderOptions options, float frameWidth)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfNegative(frameWidth);

        return frameWidth >= options.MinLabelWidth;
    }

    /// <summary>
    /// Determines whether a frame with the specified width should be rendered based on <see cref="RenderOptions.MinBoxWidth"/>.
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <param name="frameWidth">The width of the frame in pixels.</param>
    /// <returns><see langword="true"/> if the frame should be rendered; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="frameWidth"/> is negative.</exception>
    public static bool ShouldRenderFrame(this RenderOptions options, float frameWidth)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfNegative(frameWidth);

        return frameWidth >= options.MinBoxWidth;
    }

    /// <summary>
    /// Determines whether a subtree with the specified width should be rendered based on <see cref="RenderOptions.MinSubtreeWidthPx"/>.
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <param name="subtreeWidth">The width of the subtree in pixels.</param>
    /// <returns><see langword="true"/> if the subtree should be rendered; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="subtreeWidth"/> is negative.</exception>
    public static bool ShouldRenderSubtree(this RenderOptions options, float subtreeWidth)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfNegative(subtreeWidth);

        return subtreeWidth >= options.MinSubtreeWidthPx;
    }

    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance with the specified width, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="width">The new width value.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated width.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="width"/> is not positive.</exception>
    public static RenderOptions WithWidth(this RenderOptions options, int width)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(width, 0);

        return new RenderOptions
        {
            RowHeight = options.RowHeight,
            MinLabelWidth = options.MinLabelWidth,
            MinBoxWidth = options.MinBoxWidth,
            Padding = options.Padding,
            FontSize = options.FontSize,
            Background = options.Background,
            TextColor = options.TextColor,
            Inverted = options.Inverted,
            Width = width
        };
    }

    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance with the specified row height, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="rowHeight">The new row height value.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated row height.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="rowHeight"/> is not positive.</exception>
    public static RenderOptions WithRowHeight(this RenderOptions options, float rowHeight)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(rowHeight, 0);

        return new RenderOptions
        {
            Width = options.Width,
            MinLabelWidth = options.MinLabelWidth,
            MinBoxWidth = options.MinBoxWidth,
            Padding = options.Padding,
            FontSize = options.FontSize,
            Background = options.Background,
            TextColor = options.TextColor,
            Inverted = options.Inverted,
            RowHeight = rowHeight
        };
    }

    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance with the specified background color, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="background">The new background color.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated background color.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static RenderOptions WithBackground(this RenderOptions options, SKColor background)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new RenderOptions
        {
            Width = options.Width,
            RowHeight = options.RowHeight,
            MinLabelWidth = options.MinLabelWidth,
            MinBoxWidth = options.MinBoxWidth,
            Padding = options.Padding,
            FontSize = options.FontSize,
            TextColor = options.TextColor,
            Inverted = options.Inverted,
            Background = background
        };
    }

    /// <summary>
    /// Gets the padding value as a float array with two elements: [horizontal, vertical].
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <returns>An array containing the padding values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static float[] GetPadding(this RenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return [options.Padding, options.Padding];
    }

    /// <summary>
    /// Gets the padding value as a float array with four elements: [left, top, right, bottom].
    /// </summary>
    /// <param name="options">The render options.</param>
    /// <returns>An array containing the padding values for all four sides.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static float[] GetPaddingAllSides(this RenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return [options.Padding, options.Padding, options.Padding, options.Padding];
    }
}