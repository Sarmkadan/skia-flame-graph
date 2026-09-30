using System;
using System.Collections.Generic;
using System.Linq;

namespace SkiaFlameGraph.Core.Parsing
{
    /// <summary>
    /// Validation helpers for <see cref="ChromeTraceEvent"/>.
    /// </summary>
    public static class ChromeTraceParserValidation
    {
        /// <summary>
        /// Validates the <see cref="ChromeTraceEvent"/> instance and returns a list of validation errors.
        /// </summary>
        /// <param name="value">The <see cref="ChromeTraceEvent"/> instance to validate.</param>
        /// <returns>A list of human-readable validation error messages, or empty if valid.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        public static IReadOnlyList<string> Validate(this ChromeTraceEvent value)
        {
            ArgumentNullException.ThrowIfNull(value);
            var problems = new List<string>();

            if (string.IsNullOrEmpty(value.Ph))
                problems.Add("Event phase (ph) is null or empty.");

            if (string.IsNullOrEmpty(value.Name))
                problems.Add("Event name (name) is null or empty.");

            if (value.Ts < 0)
                problems.Add("Timestamp (ts) is negative.");

            if (value.Dur.HasValue && value.Dur.Value < 0)
                problems.Add("Duration (dur) is negative.");

            if (value.Tid.HasValue && value.Tid.Value < 0)
                problems.Add("Thread ID (tid) is negative.");

            if (value.Pid.HasValue && value.Pid.Value < 0)
                problems.Add("Process ID (pid) is negative.");

            if (string.IsNullOrEmpty(value.File))
                problems.Add("Source file (file) is null or empty.");

            if (value.Line.HasValue && value.Line.Value < 1)
                problems.Add("Source line number (line) is less than 1.");

            if (string.IsNullOrEmpty(value.Category))
                problems.Add("Event category (cat) is null or empty.");

            return problems.ToArray();
        }

        /// <summary>
        /// Determines whether the specified <see cref="ChromeTraceEvent"/> is valid.
        /// </summary>
        /// <param name="value">The <see cref="ChromeTraceEvent"/> instance to validate.</param>
        /// <returns>true if the instance is valid; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        public static bool IsValid(this ChromeTraceEvent value) => Validate(value).Count == 0;

        /// <summary>
        /// Ensures that the specified <see cref="ChromeTraceEvent"/> is valid. Throws an exception if validation fails.
        /// </summary>
        /// <param name="value">The <see cref="ChromeTraceEvent"/> instance to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the instance is invalid, with a message listing the validation errors.</exception>
        public static void EnsureValid(this ChromeTraceEvent value)
        {
            var problems = Validate(value);
            if (problems.Count > 0)
            {
                throw new ArgumentException(
                    $"ChromeTraceEvent is invalid: {string.Join("; ", problems)}",
                    nameof(value));
            }
        }
    }
}