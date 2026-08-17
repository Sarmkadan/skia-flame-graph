using System;
using System.Collections.Generic;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Validation helpers for <see cref="SpeedscopeFileJsonExtensionsTests"/>.
/// </summary>
public static class SpeedscopeFileJsonExtensionsTestsValidation
{
    /// <summary>
    /// Validates the instance of <see cref="SpeedscopeFileJsonExtensionsTests"/>.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <returns>A list of problems found, or an empty list if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static IReadOnlyList<string> Validate(this SpeedscopeFileJsonExtensionsTests value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Array.Empty<string>();
    }

    /// <summary>
    /// Checks if the instance of <see cref="SpeedscopeFileJsonExtensionsTests"/> is valid.
    /// </summary>
    /// <param name="value">The instance to check.</param>
    /// <returns>True if valid, otherwise false.</returns>
    public static bool IsValid(this SpeedscopeFileJsonExtensionsTests value)
    {
        return value != null;
    }

    /// <summary>
    /// Ensures the instance of <see cref="SpeedscopeFileJsonExtensionsTests"/> is valid, throwing an exception if it is not.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static void EnsureValid(this SpeedscopeFileJsonExtensionsTests value)
    {
        ArgumentNullException.ThrowIfNull(value);
    }
}
