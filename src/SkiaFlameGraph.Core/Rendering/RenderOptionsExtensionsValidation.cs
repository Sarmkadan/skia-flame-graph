using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace SkiaFlameGraph.Core.Rendering
{
    /// <summary>
    /// Provides validation helper methods for <see cref="RenderOptions"/> instances.
    /// </summary>
    public static class RenderOptionsExtensionsValidation
    {
        /// <summary>
        /// Validates the render options and returns a list of error messages.
        /// </summary>
        /// <param name="options">The render options to validate.</param>
        /// <returns>A read-only list of error messages, or an empty list if valid.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        public static IReadOnlyList<string> Validate(this RenderOptions options)
        {
            ArgumentNullException.ThrowIfNull(options);
            var errors = new List<string>();
            if (options.Width <= 0)
                errors.Add("Width must be greater than 0.");
            if (options.RowHeight <= 0)
                errors.Add("RowHeight must be greater than 0.");
            if (options.MinLabelWidth < 0)
                errors.Add("MinLabelWidth must be greater than or equal to 0.");
            if (options.MinBoxWidth < 0)
                errors.Add("MinBoxWidth must be greater than or equal to 0.");
            if (options.MinSubtreeWidthPx < 0)
                errors.Add("MinSubtreeWidthPx must be greater than or equal to 0.");
            if (options.Padding < 0)
                errors.Add("Padding must be greater than or equal to 0.");
            if (options.FontSize <= 0)
                errors.Add("FontSize must be greater than 0.");
            return errors.AsReadOnly();
        }

        /// <summary>
        /// Determines whether the specified render options is valid.
        /// </summary>
        /// <param name="options">The render options to validate.</param>
        /// <returns><see langword="true"/> if the options is valid; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        public static bool IsValid(this RenderOptions options) =>
            Validate(options).Count == 0;

        /// <summary>
        /// Ensures that the specified render options is valid. Throws an <see cref="ArgumentException"/> if it is not.
        /// </summary>
        /// <param name="options">The render options to validate.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="options"/> is invalid.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is <see langword="null"/>.</exception>
        public static void EnsureValid(this RenderOptions options)
        {
            var errors = Validate(options);
            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(" ", errors), nameof(options));
            }
        }
    }
}