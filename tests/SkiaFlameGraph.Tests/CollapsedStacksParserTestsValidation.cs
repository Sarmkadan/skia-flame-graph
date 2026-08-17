using System.Collections.Generic;
using System.Linq;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Validation helpers for <see cref="CollapsedStacksParserTests"/>.
/// </summary>
public static class CollapsedStacksParserTestsValidation
{
    /// <summary>
    /// Validates the <see cref="CollapsedStacksParserTests"/> instance and returns a list of problems.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <returns>A list of problem messages, or empty if none.</returns>
    /// <exception cref="System.ArgumentNullException">If <paramref name="value"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this CollapsedStacksParserTests value)
    {
        ArgumentNullException.ThrowIfNull(value);

        // CollapsedStacksParserTests has no state to validate; all test methods are void and have no parameters.
        // Therefore, there are no validation rules based on members.
        return System.Array.Empty<string>();
    }

    /// <summary>
    /// Determines whether the <see cref="CollapsedStacksParserTests"/> instance is valid.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <returns><see langword="true"/> if the instance is valid; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(this CollapsedStacksParserTests value)
    {
        // Null is invalid; otherwise, no state to invalidate.
        return value != null;
    }

    /// <summary>
    /// Ensures the <see cref="CollapsedStacksParserTests"/> instance is valid, throwing an exception if not.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <exception cref="System.ArgumentException">If the instance is invalid.</exception>
    public static void EnsureValid(this CollapsedStacksParserTests value)
    {
        if (value == null)
        {
            throw new ArgumentException("Instance is null.", nameof(value));
        }

        // No further validation needed.
    }
}