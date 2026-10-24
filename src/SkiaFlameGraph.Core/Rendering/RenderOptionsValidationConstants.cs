namespace SkiaFlameGraph.Core.Rendering;

internal static class RenderOptionsValidationConstants
{
    /// <summary>The largest accepted value for <see cref="RenderOptions.Width"/>, in pixels.</summary>
    public const int MaxWidth = 16384;

    public const string WidthMustBePositive = "Width must be positive, but was {0}.";
    public const string WidthMustNotExceedMaxWidth = "Width must be at most 16384, but was {0}.";
    public const string RowHeightMustBePositive = "RowHeight must be positive, but was {0}.";
    public const string MinLabelWidthMustBeNonNegative = "MinLabelWidth must be non-negative, but was {0}.";
    public const string MinBoxWidthMustBeNonNegative = "MinBoxWidth must be non-negative, but was {0}.";
    public const string MinSubtreeWidthPxMustBeNonNegative = "MinSubtreeWidthPx must be non-negative, but was {0}.";
    public const string PaddingMustBeNonNegative = "Padding must be non-negative, but was {0}.";
    public const string FontSizeMustBePositive = "FontSize must be positive, but was {0}.";
    public const string FontFamilyMustBeNonEmpty = "FontFamily must be non-empty when specified, but was '{0}'.";
    public const string HighlightPatternMustBeValidRegex = "HighlightPattern must be a valid regular expression, but was invalid: {0}";
    public const string RenderOptionsIsInvalid = "RenderOptions is invalid. ";
    public const string RegexTestString = "test";
}
