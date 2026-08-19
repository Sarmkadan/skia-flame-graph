namespace SkiaFlameGraph.Core.Rendering;

internal static class RenderOptionsValidationConstants
{
    public const string WidthMustBePositive = "Width must be positive, but was {0}.";
    public const string RowHeightMustBePositive = "RowHeight must be positive, but was {0}.";
    public const string MinLabelWidthMustBeNonNegative = "MinLabelWidth must be non-negative, but was {0}.";
    public const string MinBoxWidthMustBeNonNegative = "MinBoxWidth must be non-negative, but was {0}.";
    public const string MinSubtreeWidthPxMustBeNonNegative = "MinSubtreeWidthPx must be non-negative, but was {0}.";
    public const string PaddingMustBeNonNegative = "Padding must be non-negative, but was {0}.";
    public const string FontSizeMustBePositive = "FontSize must be positive, but was {0}.";
    public const string HighlightPatternMustBeValidRegex = "HighlightPattern must be a valid regular expression, but was invalid: {0}";
    public const string RenderOptionsIsInvalid = "RenderOptions is invalid. ";
    public const string RegexTestString = "test";
}
