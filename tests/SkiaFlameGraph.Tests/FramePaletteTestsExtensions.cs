using System;
using SkiaSharp;
using SkiaFlameGraph.Core.Rendering;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Extension methods that aid <see cref="FramePaletteTests"/> in asserting the behaviour of <see cref="FramePalette"/>.
/// </summary>
public static class FramePaletteTestsExtensions
{
    /// <summary>
    /// Retrieves the colour that <see cref="FramePalette.ForFrame(string)"/> would return for the specified <paramref name="frameName"/>.
    /// </summary>
    /// <param name="_">The test instance (unused, required for extension method syntax).</param>
    /// <param name="frameName">The name of the frame.</param>
    /// <returns>The <see cref="SKColor"/> associated with <paramref name="frameName"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="frameName"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="frameName"/> is an empty string.</exception>
    public static SKColor GetStandardColor(this FramePaletteTests _, string frameName)
    {
        ArgumentNullException.ThrowIfNull(frameName);
        ArgumentException.ThrowIfNullOrEmpty(frameName);
        return FramePalette.ForFrame(frameName);
    }

    /// <summary>
    /// Retrieves the colour that <see cref="FramePalette.ForFrame(string, string?)"/> would return for the specified <paramref name="frameName"/>
    /// and <paramref name="highlightPattern"/>.
    /// </summary>
    /// <param name="_">The test instance (unused, required for extension method syntax).</param>
    /// <param name="frameName">The name of the frame.</param>
    /// <param name="highlightPattern">A pattern used to highlight the frame; may be <c>null</c>.</param>
    /// <returns>The <see cref="SKColor"/> associated with the highlighted frame.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="frameName"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="frameName"/> is an empty string.</exception>
    public static SKColor GetHighlightedColor(this FramePaletteTests _, string frameName, string? highlightPattern)
    {
        ArgumentNullException.ThrowIfNull(frameName);
        ArgumentException.ThrowIfNullOrEmpty(frameName);
        // highlightPattern may be null – FramePalette handles that internally.
        return FramePalette.ForFrame(frameName, highlightPattern);
    }

    /// <summary>
    /// Determines whether two colours are distinct.
    /// </summary>
    /// <param name="_">The test instance (unused, required for extension method syntax).</param>
    /// <param name="first">The first colour.</param>
    /// <param name="second">The second colour.</param>
    /// <returns><c>true</c> if the colours differ; otherwise, <c>false</c>.</returns>
    public static bool AreColorsDistinct(this FramePaletteTests _, SKColor first, SKColor second) => first != second;
}
