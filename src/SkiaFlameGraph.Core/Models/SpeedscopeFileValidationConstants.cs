namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Constants used by <see cref="SpeedscopeFileValidation"/> for validation limits.
/// </summary>
internal static class SpeedscopeFileValidationConstants
{
    /// <summary>
    /// Maximum number of frames allowed in <see cref="SharedData.Frames"/>.
    /// A speedscope file whose shared frame table exceeds this count fails validation with a
    /// message reporting the actual count and this limit.
    /// </summary>
    public const int MaxFrames = 100_000;

    /// <summary>
    /// Maximum number of events allowed in any single <see cref="Profile.Events"/> collection.
    /// The limit applies per profile, not to the file as a whole. A profile exceeding it fails
    /// validation with a message naming the profile index, its event count, and this limit.
    /// </summary>
    public const int MaxEvents = 1_000_000;

    /// <summary>
    /// Maximum allowed nesting depth of open/close <see cref="ProfileEvent"/> pairs.
    /// Depth is counted while walking the event stream; exceeding this value fails validation
    /// with a message naming the event index, the maximum, and the depth reached.
    /// </summary>
    public const int MaxStackDepth = 10_000;
}