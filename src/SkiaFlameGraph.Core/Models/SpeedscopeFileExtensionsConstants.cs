namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Contains constant values used in SpeedscopeFileExtensions.
/// </summary>
internal static class SpeedscopeFileExtensionsConstants
{
    /// <summary>
    /// Default name for unnamed profiles.
    /// </summary>
    public const string UnnamedProfileDefault = "Unnamed Profile";

    /// <summary>
    /// Identifier for open event type in evented profiles.
    /// </summary>
    public const string OpenEventType = "O";

    /// <summary>
    /// Identifier for close event type in evented profiles.
    /// </summary>
    public const string CloseEventType = "C";

    /// <summary>
    /// Increment value for each sample in sampled profiles.
    /// </summary>
    public const double SampleIncrement = 1.0;
}