namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Contains constant values used in SpeedscopeFileExtensions.
/// </summary>
internal static class SpeedscopeFileExtensionsConstants
{
    /// <summary>
    /// Fallback display name used for a speedscope profile that has no <c>name</c>.
    /// Mirrors the literal used when profile names are listed from a speedscope file.
    /// </summary>
    public const string UnnamedProfileDefault = "Unnamed Profile";

    /// <summary>
    /// Speedscope event type <c>"O"</c> marking a frame being opened (entered) at a given time
    /// in an evented profile. Increases the stack depth by one.
    /// </summary>
    public const string OpenEventType = "O";

    /// <summary>
    /// Speedscope event type <c>"C"</c> marking a frame being closed (exited) at a given time
    /// in an evented profile. Decreases the stack depth by one.
    /// </summary>
    public const string CloseEventType = "C";

    /// <summary>
    /// Weight added for each sample in a sampled profile. Each sample counts as one unit,
    /// so the sample count and the accumulated weight are equal.
    /// </summary>
    public const double SampleIncrement = 1.0;
}
