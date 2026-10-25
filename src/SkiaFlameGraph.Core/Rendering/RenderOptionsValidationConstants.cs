namespace SkiaFlameGraph.Core.Rendering;

internal static class RenderOptionsValidationConstants
{
    /// <summary>The largest accepted value for <see cref="RenderOptions.Width"/>, in pixels.</summary>
    public const int MaxWidth = 16384;

    /// <summary>
    /// Error message used when <c>RenderOptions.Width</c> is zero or negative. The <c>{0}</c> placeholder receives the rejected value.
    /// </summary>
    public const string WidthMustBePositive = "Width must be positive, but was {0}.";

    /// <summary>
    /// Error message used when <c>RenderOptions.Width</c> exceeds <see cref="MaxWidth"/>. The <c>{0}</c> placeholder receives the rejected value.
    /// </summary>
    public const string WidthMustNotExceedMaxWidth = "Width must be at most 16384, but was {0}.";

    /// <summary>
    /// Error message used when <c>RenderOptions.RowHeight</c> is zero or negative. The <c>{0}</c> placeholder receives the rejected value.
    /// </summary>
    public const string RowHeightMustBePositive = "RowHeight must be positive, but was {0}.";

    /// <summary>
    /// Error message used when <c>RenderOptions.MinLabelWidth</c> is negative. A value of zero is accepted. The <c>{0}</c> placeholder receives the rejected value.
    /// </summary>
    public const string MinLabelWidthMustBeNonNegative = "MinLabelWidth must be non-negative, but was {0}.";

    /// <summary>
    /// Error message used when <c>RenderOptions.MinBoxWidth</c> is negative. A value of zero is accepted. The <c>{0}</c> placeholder receives the rejected value.
    /// </summary>
    public const string MinBoxWidthMustBeNonNegative = "MinBoxWidth must be non-negative, but was {0}.";

    /// <summary>
    /// Error message used when <c>RenderOptions.MinSubtreeWidthPx</c> is negative. A value of zero is accepted. The <c>{0}</c> placeholder receives the rejected value.
    /// </summary>
    public const string MinSubtreeWidthPxMustBeNonNegative = "MinSubtreeWidthPx must be non-negative, but was {0}.";

    /// <summary>
    /// Error message used when <c>RenderOptions.Padding</c> is negative. A value of zero is accepted. The <c>{0}</c> placeholder receives the rejected value.
    /// </summary>
    public const string PaddingMustBeNonNegative = "Padding must be non-negative, but was {0}.";

    /// <summary>
    /// Error message used when <c>RenderOptions.FontSize</c> is zero or negative. The <c>{0}</c> placeholder receives the rejected value.
    /// </summary>
    public const string FontSizeMustBePositive = "FontSize must be positive, but was {0}.";

    /// <summary>
    /// Error message used when <c>RenderOptions.FontFamily</c> is set but empty or whitespace-only. The <c>{0}</c> placeholder receives the rejected value.
    /// </summary>
    public const string FontFamilyMustBeNonEmpty = "FontFamily must be non-empty when specified, but was '{0}'.";

    /// <summary>
    /// Error message used when <c>RenderOptions.HighlightPattern</c> is non-empty but not a valid regular expression. The <c>{0}</c> placeholder receives the rejected pattern.
    /// </summary>
    public const string HighlightPatternMustBeValidRegex = "HighlightPattern must be a valid regular expression, but was invalid: {0}";

    /// <summary>
    /// Prefix prepended to the combined list of validation errors when <c>RenderOptions</c> validation fails. The errors are appended after it, separated by spaces.
    /// </summary>
    public const string RenderOptionsIsInvalid = "RenderOptions is invalid. ";

    /// <summary>
    /// Sample input string for exercising a regular expression. It is not a validation limit and is not referenced by the current validation code.
    /// </summary>
    public const string RegexTestString = "test";
}
