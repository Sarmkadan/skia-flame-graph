using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Contains constant values used in RenderOptionsExtensions.
/// </summary>
internal static class RenderOptionsExtensionsConstants
{
    /// <summary>
    /// The multiplier for padding when calculating total height or content width (accounts for both sides).
    /// </summary>
    public const int PaddingMultiplier = 2;

    /// <summary>
    /// The minimum valid value for positive dimensions (used in validation checks).
    /// </summary>
    public const int MinValidValue = 0;
}