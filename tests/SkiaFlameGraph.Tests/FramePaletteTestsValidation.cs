using System;
using System.Collections.Generic;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Validation helpers for <see cref="FramePaletteTests"/>.
/// </summary>
public static class FramePaletteTestsValidation
{
    /// <summary>
    /// Validates the <see cref="FramePaletteTests"/> instance and returns a list of problems.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <returns>A list of problem messages, or an empty list if none.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    public static IReadOnlyList<string> Validate(this FramePaletteTests value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // FramePaletteTests declares only parameterless void test methods and no fields or properties,
        // so there are no strings, numbers or dates to check.
        return [];
    }

    /// <summary>
    /// Determines whether the <see cref="FramePaletteTests"/> instance is valid.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <returns><c>true</c> if the instance is valid; otherwise, <c>false</c>.</returns>
    public static bool IsValid(this FramePaletteTests value) => value is not null && value.Validate().Count == 0;

    /// <summary>
    /// Ensures the <see cref="FramePaletteTests"/> instance is valid, throwing an exception listing the problems if not.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> has one or more validation problems.</exception>
    public static void EnsureValid(this FramePaletteTests value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = value.Validate();
        if (problems.Count > 0)
        {
            throw new ArgumentException($"FramePaletteTests is invalid: {string.Join("; ", problems)}", nameof(value));
        }
    }
}
