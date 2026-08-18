using System;
using System.Text.RegularExpressions;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests
{
    public interface IRenderOptionsValidationTests
    {
        void Validate_NullOptions_ThrowsArgumentNullException();
        void Validate_WidthOne_ReturnsNoErrors();
        void Validate_RowHeightMinimum_ReturnsNoErrors();
        void Validate_FontSizeMinimum_ReturnsNoErrors();
        void Validate_MultipleErrors_ReturnsAllErrors();
        void Validate_EmptyHighlightPattern_ReturnsNoErrors();
        void Validate_ValidHighlightPattern_ReturnsNoErrors();
        void Validate_NullOptions_IsValidThrowsArgumentNullException();
        void Validate_NullOptions_EnsureValidThrowsArgumentNullException();
        void EnsureValid_ErrorMessageContainsAllValidationErrors();
        void Validate_WidthZero_ReturnsError();
        void Validate_RowHeightZero_ReturnsError();
        void Validate_MinLabelWidthNegative_ReturnsError();
        void Validate_MinBoxWidthNegative_ReturnsError();
        void Validate_PaddingNegative_ReturnsError();
        void Validate_FontSizeZero_ReturnsError();
        void Validate_HighlightPatternInvalid_ReturnsError();
        void Validate_AllValid_ReturnsNoErrors();
        void IsValid_Valid_ReturnsTrue();
        void IsValid_Invalid_ReturnsFalse();
        void EnsureValid_Valid_DoesNotThrow();
        void EnsureValid_Invalid_ThrowsArgumentException();
    }
}
