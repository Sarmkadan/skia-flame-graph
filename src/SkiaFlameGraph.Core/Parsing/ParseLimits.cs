namespace SkiaFlameGraph.Core.Parsing;

/// <summary>
/// Resource limits applied while parsing speedscope documents. Use these when the input is
/// untrusted (for example, traces uploaded to a service): each limit is checked before the work
/// it guards, so an oversized or pathological document fails with a
/// <see cref="ParseLimitExceededException"/> instead of exhausting memory or the call stack.
/// Defaults are documented in <c>docs/ParseLimits.md</c>.
/// </summary>
public sealed class ParseLimits
{
    /// <summary>
    /// Gets the limits used when a parser entry point is called without an explicit <see cref="ParseLimits"/>.
    /// </summary>
    public static ParseLimits Default { get; } = new();

    /// <summary>
    /// Gets the maximum size, in bytes, of a speedscope document (UTF-8 encoded for strings).
    /// Checked before the document is read. Defaults to 256 MiB.
    /// </summary>
    public long MaxFileBytes { get; init; } = ParseLimitsConstants.DefaultMaxFileBytes;

    /// <summary>
    /// Gets the maximum number of entries in <c>shared.frames</c>. Defaults to 100,000.
    /// </summary>
    public int MaxFrames { get; init; } = ParseLimitsConstants.DefaultMaxFrames;

    /// <summary>
    /// Gets the maximum number of entries in a single profile's <c>events</c> or <c>samples</c> array.
    /// Defaults to 1,000,000.
    /// </summary>
    public int MaxEvents { get; init; } = ParseLimitsConstants.DefaultMaxEvents;

    /// <summary>
    /// Gets the maximum number of simultaneously open frames, i.e. the depth of the resulting call tree.
    /// For sampled profiles this is the length of the longest sample stack. Defaults to 1,024.
    /// </summary>
    public int MaxStackDepth { get; init; } = ParseLimitsConstants.DefaultMaxStackDepth;

    /// <summary>
    /// Verifies that every limit is positive.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any limit is zero or negative.</exception>
    internal void Validate()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(MaxFileBytes, nameof(MaxFileBytes));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(MaxFrames, nameof(MaxFrames));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(MaxEvents, nameof(MaxEvents));
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(MaxStackDepth, nameof(MaxStackDepth));
    }
}
