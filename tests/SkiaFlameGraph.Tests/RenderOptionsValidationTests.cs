using System;
using System.Text.RegularExpressions;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Contains unit tests for the validation logic of <see cref="RenderOptions"/>.
/// Covers the Validate, IsValid and EnsureValid entry points against fully valid configurations,
/// boundary values, individually invalid properties and combinations of invalid properties.
/// </summary>
public class RenderOptionsValidationTests : IRenderOptionsValidationTests
{
    /// <summary>
    /// Creates a <see cref="RenderOptions"/> instance whose properties are all set to values that pass validation.
    /// Tests mutate individual properties on this baseline to exercise specific validation rules.
    /// </summary>
    /// <returns>A <see cref="RenderOptions"/> instance configured with valid default values.</returns>
    private RenderOptions CreateValidOptions()
    {
        return new RenderOptions
        {
            Width = 800,
            RowHeight = 20f,
            MinLabelWidth = 0f,
            MinBoxWidth = 0f,
            Padding = 0f,
            FontSize = 12f,
            HighlightPattern = null
        };
    }

    /// <summary>
    /// Verifies that calling Validate on a null options reference throws an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void Validate_NullOptions_ThrowsArgumentNullException()
    {
        RenderOptions? options = null;
        Assert.Throws<ArgumentNullException>(() => options!.Validate());
    }

    /// <summary>
    /// Verifies that validation reports no errors when Width is set to its smallest positive value of 1.
    /// </summary>
    [Fact]
    public void Validate_WidthOne_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.Width = 1;

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    /// <summary>
    /// Verifies that validation reports no errors when RowHeight is set to a small positive value of 0.1.
    /// </summary>
    [Fact]
    public void Validate_RowHeightMinimum_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.RowHeight = 0.1f;

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    /// <summary>
    /// Verifies that validation reports no errors when FontSize is set to a small positive value of 0.1.
    /// </summary>
    [Fact]
    public void Validate_FontSizeMinimum_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.FontSize = 0.1f;

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    /// <summary>
    /// Verifies that validation returns one error for each of three invalid properties
    /// (negative Width, zero RowHeight, zero FontSize) and orders them as Width, RowHeight, FontSize.
    /// </summary>
    [Fact]
    public void Validate_MultipleErrors_ReturnsAllErrors()
    {
        var options = CreateValidOptions();
        options.Width = -1;
        options.RowHeight = 0f;
        options.FontSize = 0f;

        var errors = options.Validate();

        Assert.Equal(3, errors.Count);
        Assert.Contains("Width must be positive", errors[0]);
        Assert.Contains("RowHeight must be positive", errors[1]);
        Assert.Contains("FontSize must be positive", errors[2]);
    }

    /// <summary>
    /// Verifies that an empty HighlightPattern string is treated as "no highlight filter" and produces no validation errors.
    /// </summary>
    [Fact]
    public void Validate_EmptyHighlightPattern_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.HighlightPattern = string.Empty;

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    /// <summary>
    /// Verifies that a syntactically valid regular expression assigned to HighlightPattern produces no validation errors.
    /// </summary>
    [Fact]
    public void Validate_ValidHighlightPattern_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.HighlightPattern = "test.*";

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    /// <summary>
    /// Verifies that calling IsValid on a null options reference throws an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void Validate_NullOptions_IsValidThrowsArgumentNullException()
    {
        RenderOptions? options = null;
        Assert.Throws<ArgumentNullException>(() => options!.IsValid());
    }

    /// <summary>
    /// Verifies that calling EnsureValid on a null options reference throws an <see cref="ArgumentNullException"/>.
    /// </summary>
    [Fact]
    public void Validate_NullOptions_EnsureValidThrowsArgumentNullException()
    {
        RenderOptions? options = null;
        Assert.Throws<ArgumentNullException>(() => options!.EnsureValid());
    }

    /// <summary>
    /// Verifies that the <see cref="ArgumentException"/> thrown by EnsureValid when Width is negative and
    /// RowHeight is zero starts with "RenderOptions is invalid" and includes both individual error messages.
    /// </summary>
    [Fact]
    public void EnsureValid_ErrorMessageContainsAllValidationErrors()
    {
        var options = CreateValidOptions();
        options.Width = -10;
        options.RowHeight = 0f;

        var ex = Assert.Throws<ArgumentException>(() => options.EnsureValid());

        Assert.Contains("RenderOptions is invalid", ex.Message);
        Assert.Contains("Width must be positive", ex.Message);
        Assert.Contains("RowHeight must be positive", ex.Message);
    }

    /// <summary>
    /// Verifies that a Width of zero is rejected with the "Width must be positive" validation error.
    /// </summary>
    [Fact]
    public void Validate_WidthZero_ReturnsError()
    {
        var options = CreateValidOptions();
        options.Width = 0;

        var errors = options.Validate();

        Assert.Contains("Width must be positive", errors[0]);
    }

    /// <summary>
    /// Verifies that a RowHeight of zero is rejected with the "RowHeight must be positive" validation error.
    /// </summary>
    [Fact]
    public void Validate_RowHeightZero_ReturnsError()
    {
        var options = CreateValidOptions();
        options.RowHeight = 0f;

        var errors = options.Validate();

        Assert.Contains("RowHeight must be positive", errors[0]);
    }

    /// <summary>
    /// Verifies that a negative MinLabelWidth is rejected with the "MinLabelWidth must be non-negative" validation error.
    /// </summary>
    [Fact]
    public void Validate_MinLabelWidthNegative_ReturnsError()
    {
        var options = CreateValidOptions();
        options.MinLabelWidth = -1f;

        var errors = options.Validate();

        Assert.Contains("MinLabelWidth must be non-negative", errors[0]);
    }

    /// <summary>
    /// Verifies that a negative MinBoxWidth is rejected with the "MinBoxWidth must be non-negative" validation error.
    /// </summary>
    [Fact]
    public void Validate_MinBoxWidthNegative_ReturnsError()
    {
        var options = CreateValidOptions();
        options.MinBoxWidth = -0.5f;

        var errors = options.Validate();

        Assert.Contains("MinBoxWidth must be non-negative", errors[0]);
    }

    /// <summary>
    /// Verifies that a negative Padding is rejected with the "Padding must be non-negative" validation error.
    /// </summary>
    [Fact]
    public void Validate_PaddingNegative_ReturnsError()
    {
        var options = CreateValidOptions();
        options.Padding = -2f;

        var errors = options.Validate();

        Assert.Contains("Padding must be non-negative", errors[0]);
    }

    /// <summary>
    /// Verifies that a FontSize of zero is rejected with the "FontSize must be positive" validation error.
    /// </summary>
    [Fact]
    public void Validate_FontSizeZero_ReturnsError()
    {
        var options = CreateValidOptions();
        options.FontSize = 0f;

        var errors = options.Validate();

        Assert.Contains("FontSize must be positive", errors[0]);
    }

    /// <summary>
    /// Verifies that a malformed regular expression assigned to HighlightPattern is rejected with the
    /// "HighlightPattern must be a valid regular expression" validation error.
    /// </summary>
    [Fact]
    public void Validate_HighlightPatternInvalid_ReturnsError()
    {
        var options = CreateValidOptions();
        options.HighlightPattern = "[unclosed";

        var errors = options.Validate();

        Assert.Contains("HighlightPattern must be a valid regular expression", errors[0]);
    }

    /// <summary>
    /// Verifies that an unmodified set of valid options produces no validation errors.
    /// </summary>
    [Fact]
    public void Validate_AllValid_ReturnsNoErrors()
    {
        var options = CreateValidOptions();

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    /// <summary>
    /// Verifies that IsValid returns true for a fully valid set of options.
    /// </summary>
    [Fact]
    public void IsValid_Valid_ReturnsTrue()
    {
        var options = CreateValidOptions();

        Assert.True(options.IsValid());
    }

    /// <summary>
    /// Verifies that IsValid returns false when Width is set to a negative value.
    /// </summary>
    [Fact]
    public void IsValid_Invalid_ReturnsFalse()
    {
        var options = CreateValidOptions();
        options.Width = -10;

        Assert.False(options.IsValid());
    }

    /// <summary>
    /// Verifies that EnsureValid completes without throwing for a fully valid set of options.
    /// </summary>
    [Fact]
    public void EnsureValid_Valid_DoesNotThrow()
    {
        var options = CreateValidOptions();

        var exception = Record.Exception(() => options.EnsureValid());

        Assert.Null(exception);
    }

    /// <summary>
    /// Verifies that EnsureValid throws an <see cref="ArgumentException"/> mentioning "RenderOptions is invalid"
    /// when FontSize is set to zero.
    /// </summary>
    [Fact]
    public void EnsureValid_Invalid_ThrowsArgumentException()
    {
        var options = CreateValidOptions();
        options.FontSize = 0f;

        var ex = Assert.Throws<ArgumentException>(() => options.EnsureValid());

        Assert.Contains("RenderOptions is invalid", ex.Message);
    }
}
