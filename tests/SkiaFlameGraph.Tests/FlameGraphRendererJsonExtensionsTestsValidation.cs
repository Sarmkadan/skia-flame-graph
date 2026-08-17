using System;
using System.Collections.Generic;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Validation extensions for <see cref="FlameGraphRendererJsonExtensionsTests"/>.
/// </summary>
public static class FlameGraphRendererJsonExtensionsTestsValidation
{
    /// <summary>
    /// Validates the instance of <see cref="FlameGraphRendererJsonExtensionsTests"/>.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <returns>A list of validation problems. Empty if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown if value is null.</exception>
    public static IReadOnlyList<string> Validate(this FlameGraphRendererJsonExtensionsTests value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Array.Empty<string>();
    }

    /// <summary>
    /// Checks if the instance is valid.
    /// </summary>
    /// <param name="value">The instance to check.</param>
    /// <returns>True if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown if value is null.</exception>
    public static bool IsValid(this FlameGraphRendererJsonExtensionsTests value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return true;
    }

    /// <summary>
    /// Ensures the instance is valid.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if value is null.</exception>
    /// <exception cref="ArgumentException">Thrown if validation fails.</exception>
    public static void EnsureValid(this FlameGraphRendererJsonExtensionsTests value)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!value.IsValid())
        {
            throw new ArgumentException("FlameGraphRendererJsonExtensionsTests instance is invalid.");
        }
    }
}
