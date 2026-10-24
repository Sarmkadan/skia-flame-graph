using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Parsing;

public static partial class SpeedscopeParser
{
    /// <summary>
    /// Resolves the caller-supplied limits, falling back to <see cref="ParseLimits.Default"/>, and validates them.
    /// </summary>
    /// <param name="limits">The limits supplied by the caller, or <c>null</c> for the defaults.</param>
    /// <returns>The validated limits to apply.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any limit is zero or negative.</exception>
    private static ParseLimits ResolveLimits(ParseLimits? limits)
    {
        var resolved = limits ?? ParseLimits.Default;
        resolved.Validate();
        return resolved;
    }

    /// <summary>
    /// Rejects a document whose size in bytes exceeds <see cref="ParseLimits.MaxFileBytes"/>.
    /// </summary>
    /// <param name="bytes">The size of the document in bytes.</param>
    /// <param name="limits">The limits to apply.</param>
    /// <exception cref="ParseLimitExceededException">Thrown when the document is larger than allowed.</exception>
    private static void EnforceByteLimit(long bytes, ParseLimits limits)
    {
        if (bytes > limits.MaxFileBytes)
            throw new ParseLimitExceededException(nameof(ParseLimits.MaxFileBytes), limits.MaxFileBytes, bytes);
    }

    /// <summary>
    /// Checks a deserialized document for null, the presence of profiles, and the frame and event count limits.
    /// </summary>
    /// <param name="file">The deserialized document.</param>
    /// <param name="limits">The limits to apply.</param>
    /// <returns>The same <paramref name="file"/> when it is within limits.</returns>
    /// <exception cref="FormatException">Thrown when the document is null or contains no profiles.</exception>
    /// <exception cref="ParseLimitExceededException">Thrown when a frame or event count exceeds its limit.</exception>
    private static SpeedscopeFile CheckDocument(SpeedscopeFile? file, ParseLimits limits)
    {
        if (file is null)
            throw new FormatException("speedscope document deserialized to null");
        if (file.Profiles.Count == 0)
            throw new FormatException("speedscope document contains no profiles");

        EnforceDocumentLimits(file, limits);
        return file;
    }

    /// <summary>
    /// Rejects frame and event/sample counts above their limits, before any tree is built from them.
    /// </summary>
    /// <param name="file">The document to check.</param>
    /// <param name="limits">The limits to apply.</param>
    /// <exception cref="ParseLimitExceededException">Thrown when a frame or event count exceeds its limit.</exception>
    private static void EnforceDocumentLimits(SpeedscopeFile file, ParseLimits limits)
    {
        var frameCount = file.Shared.Frames.Count;
        if (frameCount > limits.MaxFrames)
            throw new ParseLimitExceededException(nameof(ParseLimits.MaxFrames), limits.MaxFrames, frameCount);

        foreach (var profile in file.Profiles)
        {
            var eventCount = Math.Max(profile.Events?.Count ?? 0, profile.Samples?.Count ?? 0);
            if (eventCount > limits.MaxEvents)
                throw new ParseLimitExceededException(nameof(ParseLimits.MaxEvents), limits.MaxEvents, eventCount);
        }
    }

    /// <summary>
    /// Rejects a call stack deeper than <see cref="ParseLimits.MaxStackDepth"/>.
    /// </summary>
    /// <param name="depth">The depth that would be reached.</param>
    /// <param name="limits">The limits to apply.</param>
    /// <exception cref="ParseLimitExceededException">Thrown when <paramref name="depth"/> exceeds the limit.</exception>
    private static void EnforceStackDepth(int depth, ParseLimits limits)
    {
        if (depth > limits.MaxStackDepth)
            throw new ParseLimitExceededException(nameof(ParseLimits.MaxStackDepth), limits.MaxStackDepth, depth);
    }
}
