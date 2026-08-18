namespace SkiaFlameGraph.Tests
{
    public interface IFlameNodeValidationTests
    {
        void Validate_ValidNode_ReturnsEmptyList();
        void Validate_NullName_ReturnsError();
        void Validate_WhitespaceName_ReturnsError();
        void Validate_NegativeValue_ReturnsError();
        void Validate_NegativeDepth_ReturnsError();
        void Validate_NullChildren_ReturnsError();
        void Validate_NullChildInCollection_ReturnsError();
        void Validate_ZeroOrNegativeLine_ReturnsError();
        void Validate_EmptyFile_IsValid();
        void Validate_NullNode_ThrowsArgumentNullException();
        void IsValid_ValidNode_ReturnsTrue();
        void IsValid_InvalidNode_ReturnsFalse();
        void IsValid_NullNode_ReturnsFalse();
        void EnsureValid_ValidNode_DoesNotThrow();
        void EnsureValid_NullNode_ThrowsArgumentNullException();
        void EnsureValid_InvalidNode_ThrowsArgumentException();
        void Validate_MultipleProblems_ReturnsAllErrors();
        void Validate_EmptyChildrenCollection_IsValid();
        void Validate_PositiveLine_IsValid();
        void Validate_ValidNodeWithFileAndLine_IsValid();

}
}
