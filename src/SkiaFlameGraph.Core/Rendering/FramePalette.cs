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
    public static SKColor ForFrame(string name)
    {
        var hash = StableHash(name);

        // Hue band 8..45 degrees (reds -> oranges -> yellows).
        var hue = 8f + (hash % 37);
        var sat = 55f + ((hash >> 8) % 25);   // 55..80
        var light = 45f + ((hash >> 16) % 15); // 45..60

        return SKColor.FromHsl(hue, sat, light);
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
