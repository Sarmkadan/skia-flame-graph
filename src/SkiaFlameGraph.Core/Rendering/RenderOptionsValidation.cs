using System;
using System.Collections.Generic;
using System.Globalization;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Provides validation helpers for <see cref="RenderOptions"/> instances.
/// </summary>
public static class RenderOptionsValidation
{
    /// <summary>
    /// Validates the specified <see cref="RenderOptions"/> instance.
    /// </summary>
    /// <param name="value">The render options to validate.</param>
    /// <returns>A list of validation errors; empty if valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static IReadOnlyList<string> Validate(this RenderOptions value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var errors = new List<string>();

        if (value.Width <= 0)
        {
            errors.Add(string.Format(CultureInfo.InvariantCulture, RenderOptionsValidationConstants.WidthMustBePositive, value.Width));
        }

        if (value.RowHeight <= 0f)
        {
            errors.Add(string.Format(CultureInfo.InvariantCulture, RenderOptionsValidationConstants.RowHeightMustBePositive, value.RowHeight));
        }

        if (value.MinLabelWidth < 0f)
        {
            errors.Add(string.Format(CultureInfo.InvariantCulture, RenderOptionsValidationConstants.MinLabelWidthMustBeNonNegative, value.MinLabelWidth));
        }

        if (value.MinBoxWidth < 0f)
        {
            errors.Add(string.Format(CultureInfo.InvariantCulture, RenderOptionsValidationConstants.MinBoxWidthMustBeNonNegative, value.MinBoxWidth));
        }

        if (value.MinSubtreeWidthPx < 0f)
        {
            errors.Add(string.Format(CultureInfo.InvariantCulture, RenderOptionsValidationConstants.MinSubtreeWidthPxMustBeNonNegative, value.MinSubtreeWidthPx));
        }

        if (value.Padding < 0f)
        {
            errors.Add(string.Format(CultureInfo.InvariantCulture, RenderOptionsValidationConstants.PaddingMustBeNonNegative, value.Padding));
        }

        if (value.FontSize <= 0f)
        {
            errors.Add(string.Format(CultureInfo.InvariantCulture, RenderOptionsValidationConstants.FontSizeMustBePositive, value.FontSize));
        }

        if (value.HighlightPattern is not null && value.HighlightPattern.Length > 0)
        {
            try
            {
                _ = System.Text.RegularExpressions.Regex.IsMatch(RenderOptionsValidationConstants.RegexTestString, value.HighlightPattern);
            }
            catch (System.Text.RegularExpressions.RegexParseException ex)
            {
                errors.Add(string.Format(CultureInfo.InvariantCulture, RenderOptionsValidationConstants.HighlightPatternMustBeValidRegex, ex.Message));
            }
        }

        return errors.AsReadOnly();
    }

    /// <summary>
    /// Determines whether the specified <see cref="RenderOptions"/> instance is valid.
    /// </summary>
    /// <param name="value">The render options to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static bool IsValid(this RenderOptions value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value.Validate().Count == 0;
    }

    /// <summary>
    /// Ensures that the specified <see cref="RenderOptions"/> instance is valid.
    /// </summary>
    /// <param name="value">The render options to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is invalid.</exception>
    public static void EnsureValid(this RenderOptions value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var errors = value.Validate();
        if (errors.Count == 0)
        {
            return;
        }

        throw new ArgumentException(
            $"{RenderOptionsValidationConstants.RenderOptionsIsInvalid}{string.Join(" ", errors)}",
            nameof(value));
    }
}
