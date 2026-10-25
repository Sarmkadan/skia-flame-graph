using System;
using System.Collections.Generic;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Validation helpers for <see cref="TreemapRendererTests"/>.
/// </summary>
public static class TreemapRendererTestsValidation
{
    /// <summary>
    /// Validates the <see cref="TreemapRendererTests"/> instance and returns a list of problems.
    /// </summary>
    /// <remarks>
    /// The fixture holds no state that a caller can set: its only public member is the constructor, and every other
    /// public member is a test method. Its private fields are created by the constructor and never exposed. There are
    /// therefore no null, range or date rules that apply to its members, and a non-null instance has no problems.
    /// </remarks>
    /// <param name="value">The instance to validate.</param>
    /// <returns>A list of problem messages, or an empty list if none.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="value"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this TreemapRendererTests value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return [];
    }

    /// <summary>
    /// Determines whether the <see cref="TreemapRendererTests"/> instance is valid.
    /// </summary>
    /// <param name="value">The instance to validate; may be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the instance is valid; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(this TreemapRendererTests? value) => value is not null && value.Validate().Count == 0;

    /// <summary>
    /// Ensures the <see cref="TreemapRendererTests"/> instance is valid, throwing an exception listing the problems if not.
    /// </summary>
    /// <param name="value">The instance to validate; may be <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">If <paramref name="value"/> is <see langword="null"/>.</exception>
    public static void EnsureValid(this TreemapRendererTests? value)
    {
        if (value is null)
        {
            throw new ArgumentException("TreemapRendererTests is invalid: Instance is null.", nameof(value));
        }

        var problems = value.Validate();
        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"TreemapRendererTests is invalid: {string.Join("; ", problems)}", nameof(value));
        }
    }
}
