using System;
using System.Text.RegularExpressions;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests;

public class RenderOptionsValidationTests
{
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

    [Fact]
    public void Validate_NullOptions_ThrowsArgumentNullException()
    {
        RenderOptions? options = null;
        Assert.Throws<ArgumentNullException>(() => options!.Validate());
    }

    [Fact]
    public void Validate_WidthOne_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.Width = 1;

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_RowHeightMinimum_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.RowHeight = 0.1f;

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_FontSizeMinimum_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.FontSize = 0.1f;

        var errors = options.Validate();

        Assert.Empty(errors);
    }

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

    [Fact]
    public void Validate_EmptyHighlightPattern_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.HighlightPattern = string.Empty;

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_ValidHighlightPattern_ReturnsNoErrors()
    {
        var options = CreateValidOptions();
        options.HighlightPattern = "test.*";

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    [Fact]
    public void Validate_NullOptions_IsValidThrowsArgumentNullException()
    {
        RenderOptions? options = null;
        Assert.Throws<ArgumentNullException>(() => options!.IsValid());
    }

    [Fact]
    public void Validate_NullOptions_EnsureValidThrowsArgumentNullException()
    {
        RenderOptions? options = null;
        Assert.Throws<ArgumentNullException>(() => options!.EnsureValid());
    }

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

    [Fact]
    public void Validate_WidthZero_ReturnsError()
    {
        var options = CreateValidOptions();
        options.Width = 0;

        var errors = options.Validate();

        Assert.Contains("Width must be positive", errors[0]);
    }

    [Fact]
    public void Validate_RowHeightZero_ReturnsError()
    {
        var options = CreateValidOptions();
        options.RowHeight = 0f;

        var errors = options.Validate();

        Assert.Contains("RowHeight must be positive", errors[0]);
    }

    [Fact]
    public void Validate_MinLabelWidthNegative_ReturnsError()
    {
        var options = CreateValidOptions();
        options.MinLabelWidth = -1f;

        var errors = options.Validate();

        Assert.Contains("MinLabelWidth must be non-negative", errors[0]);
    }

    [Fact]
    public void Validate_MinBoxWidthNegative_ReturnsError()
    {
        var options = CreateValidOptions();
        options.MinBoxWidth = -0.5f;

        var errors = options.Validate();

        Assert.Contains("MinBoxWidth must be non-negative", errors[0]);
    }

    [Fact]
    public void Validate_PaddingNegative_ReturnsError()
    {
        var options = CreateValidOptions();
        options.Padding = -2f;

        var errors = options.Validate();

        Assert.Contains("Padding must be non-negative", errors[0]);
    }

    [Fact]
    public void Validate_FontSizeZero_ReturnsError()
    {
        var options = CreateValidOptions();
        options.FontSize = 0f;

        var errors = options.Validate();

        Assert.Contains("FontSize must be positive", errors[0]);
    }

    [Fact]
    public void Validate_HighlightPatternInvalid_ReturnsError()
    {
        var options = CreateValidOptions();
        options.HighlightPattern = "[unclosed";

        var errors = options.Validate();

        Assert.Contains("HighlightPattern must be a valid regular expression", errors[0]);
    }

    [Fact]
    public void Validate_AllValid_ReturnsNoErrors()
    {
        var options = CreateValidOptions();

        var errors = options.Validate();

        Assert.Empty(errors);
    }

    [Fact]
    public void IsValid_Valid_ReturnsTrue()
    {
        var options = CreateValidOptions();

        Assert.True(options.IsValid());
    }

    [Fact]
    public void IsValid_Invalid_ReturnsFalse()
    {
        var options = CreateValidOptions();
        options.Width = -10;

        Assert.False(options.IsValid());
    }

    [Fact]
    public void EnsureValid_Valid_DoesNotThrow()
    {
        var options = CreateValidOptions();

        var exception = Record.Exception(() => options.EnsureValid());

        Assert.Null(exception);
    }

    [Fact]
    public void EnsureValid_Invalid_ThrowsArgumentException()
    {
        var options = CreateValidOptions();
        options.FontSize = 0f;

        var ex = Assert.Throws<ArgumentException>(() => options.EnsureValid());

        Assert.Contains("RenderOptions is invalid", ex.Message);
    }
}
