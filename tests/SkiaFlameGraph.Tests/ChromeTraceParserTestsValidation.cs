using System;
using System.Collections.Generic;

namespace SkiaFlameGraph.Tests
{
    /// <summary>
    /// Validation helpers for <see cref="ChromeTraceParserTests"/>.
    /// </summary>
    public static class ChromeTraceParserTestsValidation
    {
        /// <summary>
        /// Validates the <paramref name="value"/> and returns a list of problems.
        /// </summary>
        /// <param name="value">The test instance to validate.</param>
        /// <returns>
        /// A read‑only list of validation error messages. Empty if the instance is valid.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public static IReadOnlyList<string> Validate(this ChromeTraceParserTests value)
        {
            ArgumentNullException.ThrowIfNull(value);
            // The test class only contains methods; there are no stateful members to validate.
            // Returning an empty list indicates the instance is structurally valid.
            return Array.Empty<string>();
        }

        /// <summary>
        /// Determines whether the <paramref name="value"/> is valid.
        /// </summary>
        /// <param name="value">The test instance to check.</param>
        /// <returns>
        /// <c>true</c> if there are no validation problems; otherwise <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <c>null</c>.
        /// </exception>
        public static bool IsValid(this ChromeTraceParserTests value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return value.Validate().Count == 0;
        }

        /// <summary>
        /// Ensures that the <paramref name="value"/> is valid, throwing an <see cref="ArgumentException"/>
        /// if any validation problems are found.
        /// </summary>
        /// <param name="value">The test instance to validate.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="value"/> is <c>null</c>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when validation problems are found.
        /// </exception>
        public static void EnsureValid(this ChromeTraceParserTests value)
        {
            ArgumentNullException.ThrowIfNull(value);
            var problems = value.Validate();
            if (problems.Count > 0)
            {
                throw new ArgumentException(string.Join(Environment.NewLine, problems), nameof(value));
            }
        }
    }
}
