namespace SkiaFlameGraph.Tests;

/// <summary>
/// Interface for unit tests of <see cref="SpeedscopeFileJsonExtensions"/> JSON serialization/deserialization methods.
/// </summary>
public interface ISpeedscopeFileJsonExtensionsTests
{
    void ToJson_WithValidSpeedscopeFile_ReturnsJsonString();
    void ToJson_WithIndentedTrue_ReturnsFormattedJson();
    void ToJson_WithNullValue_ThrowsArgumentNullException();
    void FromJson_WithValidJson_ReturnsSpeedscopeFile();
    void FromJson_WithNullJson_ThrowsArgumentNullException();
    void FromJson_WithInvalidJson_ThrowsJsonException();
    void FromJson_WithEmptyJson_ReturnsSpeedscopeFileWithDefaults();
    void TryFromJson_WithValidJson_ReturnsTrueAndDeserializes();
    void TryFromJson_WithInvalidJson_ReturnsFalseAndNull();
    void TryFromJson_WithNullJson_ThrowsArgumentNullException();
    void RoundtripSerialization_ProducesEquivalentObject();
    void RoundtripSerialization_WithTryFromJson_ProducesEquivalentObject();
    void ToJson_ProducesCamelCasePropertyNames();
    void FromJson_WithMinimalValidJson_ReturnsSpeedscopeFile();
    void TryFromJson_WithEmptyObject_ReturnsTrueWithEmptyFile();
}