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

        return (int)(rowCount * options.RowHeight) + (int)(RenderOptionsExtensionsConstants.PaddingMultiplier * options.Padding);
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
        return options.Width - (int)(RenderOptionsExtensionsConstants.PaddingMultiplier * options.Padding);
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
    /// Creates a new <see cref="RenderOptions"/> instance with the specified minimum label width, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="minLabelWidth">The new minimum label width value.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated minimum label width.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minLabelWidth"/> is negative.</exception>
    public static RenderOptions WithMinLabelWidth(this RenderOptions options, float minLabelWidth)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfNegative(minLabelWidth);

        return new RenderOptions
        {
            Width = options.Width,
            RowHeight = options.RowHeight,
            MinBoxWidth = options.MinBoxWidth,
            Padding = options.Padding,
            FontSize = options.FontSize,
            Background = options.Background,
            TextColor = options.TextColor,
            Inverted = options.Inverted,
            MinLabelWidth = minLabelWidth
        };
    }

    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance with the specified minimum box width, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="minBoxWidth">The new minimum box width value.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated minimum box width.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minBoxWidth"/> is negative.</exception>
    public static RenderOptions WithMinBoxWidth(this RenderOptions options, float minBoxWidth)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfNegative(minBoxWidth);

        return new RenderOptions
        {
            Width = options.Width,
            RowHeight = options.RowHeight,
            MinLabelWidth = options.MinLabelWidth,
            Padding = options.Padding,
            FontSize = options.FontSize,
            Background = options.Background,
            TextColor = options.TextColor,
            Inverted = options.Inverted,
            MinBoxWidth = minBoxWidth
        };
    }

    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance with the specified minimum subtree width, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="minSubtreeWidthPx">The new minimum subtree width value.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated minimum subtree width.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="minSubtreeWidthPx"/> is negative.</exception>
    public static RenderOptions WithMinSubtreeWidthPx(this RenderOptions options, float minSubtreeWidthPx)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfNegative(minSubtreeWidthPx);

        return new RenderOptions
        {
            Width = options.Width,
            RowHeight = options.RowHeight,
            MinLabelWidth = options.MinLabelWidth,
            MinBoxWidth = options.MinBoxWidth,
            Padding = options.Padding,
            FontSize = options.FontSize,
            Background = options.Background,
            TextColor = options.TextColor,
            Inverted = options.Inverted,
            MinSubtreeWidthPx = minSubtreeWidthPx
        };
    }

    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance with the specified padding, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="padding">The new padding value.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated padding.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="padding"/> is negative.</exception>
    public static RenderOptions WithPadding(this RenderOptions options, float padding)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfNegative(padding);

        return new RenderOptions
        {
            Width = options.Width,
            RowHeight = options.RowHeight,
            MinLabelWidth = options.MinLabelWidth,
            MinBoxWidth = options.MinBoxWidth,
            FontSize = options.FontSize,
            Background = options.Background,
            TextColor = options.TextColor,
            Inverted = options.Inverted,
            Padding = padding
        };
    }

    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance with the specified font size, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="fontSize">The new font size value.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated font size.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="fontSize"/> is not positive.</exception>
    public static RenderOptions WithFontSize(this RenderOptions options, float fontSize)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(fontSize, 0);

        return new RenderOptions
        {
            Width = options.Width,
            RowHeight = options.RowHeight,
            MinLabelWidth = options.MinLabelWidth,
            MinBoxWidth = options.MinBoxWidth,
            Padding = options.Padding,
            Background = options.Background,
            TextColor = options.TextColor,
            Inverted = options.Inverted,
            FontSize = fontSize
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
    /// Creates a new <see cref="RenderOptions"/> instance with the specified text color, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="textColor">The new text color.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated text color.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static RenderOptions WithTextColor(this RenderOptions options, SKColor textColor)
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
            Background = options.Background,
            Inverted = options.Inverted,
            TextColor = textColor
        };
    }

    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance with the specified inverted flag, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="inverted">The new inverted value.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated inverted flag.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static RenderOptions WithInverted(this RenderOptions options, bool inverted)
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
            Background = options.Background,
            TextColor = options.TextColor,
            Inverted = inverted
        };
    }

    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance with the specified highlight pattern, copying all other properties from the original.
    /// </summary>
    /// <param name="options">The original render options.</param>
    /// <param name="highlightPattern">The new highlight pattern.</param>
    /// <returns>A new <see cref="RenderOptions"/> instance with the updated highlight pattern.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
    public static RenderOptions WithHighlightPattern(this RenderOptions options, string? highlightPattern)
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
            Background = options.Background,
            TextColor = options.TextColor,
            Inverted = options.Inverted,
            HighlightPattern = highlightPattern
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

    /// <summary>
    /// Creates a new <see cref="DifferentialFlameGraphRenderer"/> instance with the specified render options.
    /// </summary>
    /// <param name="options">The render options to use.</param>
    /// <returns>A new DifferentialFlameGraphRenderer instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    public static DifferentialFlameGraphRenderer WithDifferentialRenderer(this RenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        return new DifferentialFlameGraphRenderer(options);
    }
}