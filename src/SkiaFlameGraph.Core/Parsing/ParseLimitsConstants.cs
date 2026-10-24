namespace SkiaFlameGraph.Core.Parsing;

/// <summary>
/// Default values for <see cref="ParseLimits"/>.
/// </summary>
internal static class ParseLimitsConstants
{
    /// <summary>Default maximum document size: 256 MiB.</summary>
    public const long DefaultMaxFileBytes = 256L * 1024 * 1024;

    /// <summary>Default maximum number of shared frames.</summary>
    public const int DefaultMaxFrames = 100_000;

    /// <summary>Default maximum number of events or samples per profile.</summary>
    public const int DefaultMaxEvents = 1_000_000;

    /// <summary>Default maximum call-tree depth.</summary>
    public const int DefaultMaxStackDepth = 1_024;
}
