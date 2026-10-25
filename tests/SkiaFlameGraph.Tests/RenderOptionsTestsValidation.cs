using System;
using System.Collections.Generic;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Validation helpers for <see cref="RenderOptionsTests"/>.
/// </summary>
public static class RenderOptionsTestsValidation
{
    /// <summary>
    /// Validates the <see cref="RenderOptionsTests"/> instance and returns a list of problems.
    /// </summary>
    /// <remarks>
    /// The fixture holds no state: its members are test methods, so there are no null, range or date rules to apply.
    /// The only problem it can have is being <see langword="null"/>, which is reported as a problem rather than thrown
    /// so that <see cref="IsValid"/> and <see cref="EnsureValid"/> can share this single check.
    /// </remarks>
    /// <param name="value">The instance to validate; may be <see langword="null"/>.</param>
    /// <returns>A list of human-readable problems, or an empty list if the instance is valid.</returns>
    public static IReadOnlyList<string> Validate(this RenderOptionsTests? value) =>
        value is null ? ["Instance is null."] : [];

    /// <summary>
    /// Determines whether the <see cref="RenderOptionsTests"/> instance is valid.
    /// </summary>
    /// <param name="value">The instance to validate; may be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the instance is valid; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(this RenderOptionsTests? value) => value.Validate().Count == 0;

    /// <summary>
    /// Ensures the <see cref="RenderOptionsTests"/> instance is valid, throwing an exception listing the problems if not.
    /// </summary>
    /// <param name="value">The instance to validate; may be <see langword="null"/>.</param>
    /// <exception cref="ArgumentException">If the instance has one or more validation problems.</exception>
    public static void EnsureValid(this RenderOptionsTests? value)
    {
        var problems = value.Validate();
        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"RenderOptionsTests is invalid: {string.Join("; ", problems)}", nameof(value));
        }
    }
}
