namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Constants used by <see cref="SpeedscopeFileValidation"/> for validation limits.
/// </summary>
internal static class SpeedscopeFileValidationConstants
{
    /// <summary>
    /// Maximum number of frames allowed in <see cref="SharedData.Frames"/>.
    /// </summary>
    public const int MaxFrames = 100_000;

    /// <summary>
    /// Maximum number of events allowed in any <see cref="Profile.Events"/> collection.
    /// </summary>
    public const int MaxEvents = 1_000_000;

    /// <summary>
    /// Maximum allowed nesting depth of open/close <see cref="ProfileEvent"/> pairs.
    /// </summary>
    public const int MaxStackDepth = 10_000;
}