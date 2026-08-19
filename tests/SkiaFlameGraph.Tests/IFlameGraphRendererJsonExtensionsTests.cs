namespace SkiaFlameGraph.Tests;

/// <summary>
/// Interface for unit tests of <see cref="FlameGraphRendererJsonExtensions"/>.
/// </summary>
public interface IFlameGraphRendererJsonExtensionsTests
{
    void ToJson_NullValue_ThrowsArgumentNullException();
    void ToJson_IndentsFalse_ReturnsCompactJson();
    void ToJson_IndentsTrue_ReturnsPrettyJson();
    void FromJson_NullJson_ThrowsArgumentNullException();
    void FromJson_EmptyOrWhitespaceJson_ThrowsJsonException();
    void FromJson_ValidJson_ReturnsRenderer();
    void TryFromJson_NullJson_ThrowsArgumentNullException();
    void TryFromJson_InvalidJson_ReturnsFalse();
    void TryFromJson_ValidJson_ReturnsTrueAndRenderer();
}