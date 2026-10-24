using SkiaSharp;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

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
                errors.Add(Format(RenderOptionsValidationConstants.WidthMustBePositive, options.Width));
            else if (options.Width > RenderOptionsValidationConstants.MaxWidth)
                errors.Add(Format(RenderOptionsValidationConstants.WidthMustNotExceedMaxWidth, options.Width));
            if (options.RowHeight <= 0)
                errors.Add(Format(RenderOptionsValidationConstants.RowHeightMustBePositive, options.RowHeight));
            if (options.MinLabelWidth < 0)
                errors.Add(Format(RenderOptionsValidationConstants.MinLabelWidthMustBeNonNegative, options.MinLabelWidth));
            if (options.MinBoxWidth < 0)
                errors.Add(Format(RenderOptionsValidationConstants.MinBoxWidthMustBeNonNegative, options.MinBoxWidth));
            if (options.MinSubtreeWidthPx < 0)
                errors.Add(Format(RenderOptionsValidationConstants.MinSubtreeWidthPxMustBeNonNegative, options.MinSubtreeWidthPx));
            if (options.Padding < 0)
                errors.Add(Format(RenderOptionsValidationConstants.PaddingMustBeNonNegative, options.Padding));
            if (options.FontSize <= 0)
                errors.Add(Format(RenderOptionsValidationConstants.FontSizeMustBePositive, options.FontSize));
            if (options.FontFamily is not null && string.IsNullOrWhiteSpace(options.FontFamily))
                errors.Add(Format(RenderOptionsValidationConstants.FontFamilyMustBeNonEmpty, options.FontFamily));
            if (!string.IsNullOrEmpty(options.HighlightPattern) && !IsValidRegex(options.HighlightPattern))
                errors.Add(Format(RenderOptionsValidationConstants.HighlightPatternMustBeValidRegex, options.HighlightPattern));
            return errors.AsReadOnly();
        }

        private static string Format(string template, object value) =>
            string.Format(CultureInfo.InvariantCulture, template, value);

        private static bool IsValidRegex(string pattern)
        {
            try
            {
                _ = new Regex(pattern);
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
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
                throw new ArgumentException(RenderOptionsValidationConstants.RenderOptionsIsInvalid + string.Join(" ", errors), nameof(options));
            }
        }
    }
}