using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Deterministic colouring for frames. The classic flame-graph look uses warm
/// hues; the exact shade is derived from a hash of the frame name so the same
/// method always gets the same colour across renders, which makes visual diffs
/// between two profiles readable.
/// </summary>
public static class FramePalette
{
    /// <summary>Highlight color for frames matching the highlight pattern.</summary>
    public static SKColor HighlightColor { get; } = new(0xff, 0x6b, 0x35); // Vibrant orange

    public static SKColor ForFrame(string name)
    {
        var hash = StableHash(name);

        // Hue band 8..45 degrees (reds -> oranges -> yellows).
        var hue = 8f + (hash % 37);
        var sat = 55f + ((hash >> 8) % 25); // 55..80
        var light = 45f + ((hash >> 16) % 15); // 45..60

        return SKColor.FromHsl(hue, sat, light);
    }

    /// <summary>
    /// Gets the color for a frame, using the highlight color if the frame name matches the pattern.
    /// </summary>
    /// <param name="name">The frame name to check.</param>
    /// <param name="highlightPattern">The regex pattern to match against; null or empty means no highlighting.</param>
    /// <returns>The color to use for the frame.</returns>
    public static SKColor ForFrame(string name, string? highlightPattern)
    {
        if (!string.IsNullOrEmpty(highlightPattern) && System.Text.RegularExpressions.Regex.IsMatch(name, highlightPattern))
        {
            return HighlightColor;
        }

        return ForFrame(name);
    }

    /// <summary>
    /// FNV-1a. We avoid <see cref="string.GetHashCode()"/> because it is
    /// randomised per-process, and we want stable colours between runs.
    /// </summary>
    private static uint StableHash(string s)
    {
        const uint offset = 2166136261;
        const uint prime = 16777619;
        var hash = offset;
        foreach (var ch in s)
        {
            hash ^= ch;
            hash *= prime;
        }
        return hash;
    }
}