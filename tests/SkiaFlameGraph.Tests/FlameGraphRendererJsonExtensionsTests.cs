using System;
using System.Text.Json;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Unit tests for <see cref="FlameGraphRendererJsonExtensions"/>.
/// </summary>
public class FlameGraphRendererJsonExtensionsTests
{
    /// <summary>
    /// Creates a minimal <see cref="FlameGraphRenderer"/> instance for testing.
    /// </summary>
    private static FlameGraphRenderer CreateRenderer()
    {
        // The renderer has a public parameterless constructor.
        // If additional setup is required, it can be added here.
        return new FlameGraphRenderer();
    }

    [Fact]
    public void ToJson_NullValue_ThrowsArgumentNullException()
    {
        // Arrange
        FlameGraphRenderer? renderer = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => renderer!.ToJson());
    }

    [Fact]
    public void ToJson_IndentsFalse_ReturnsCompactJson()
    {
        // Arrange
        var renderer = CreateRenderer();

        // Act
        var json = renderer.ToJson(indented: false);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(json));
        Assert.DoesNotContain("\n", json);
    }

    [Fact]
    public void ToJson_IndentsTrue_ReturnsPrettyJson()
    {
        // Arrange
        var renderer = CreateRenderer();

        // Act
        var json = renderer.ToJson(indented: true);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(json));
        Assert.Contains("\n", json);
    }

    [Fact]
    public void FromJson_NullJson_ThrowsArgumentNullException()
    {
        // Arrange
        string? json = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FlameGraphRendererJsonExtensions.FromJson(json!));
    }

    [Fact]
    public void FromJson_EmptyOrWhitespaceJson_ThrowsJsonException()
    {
        // Arrange
        var emptyJson = "   ";

        // Act & Assert
        Assert.Throws<JsonException>(() => FlameGraphRendererJsonExtensions.FromJson(emptyJson));
    }

    [Fact]
    public void FromJson_ValidJson_ReturnsRenderer()
    {
        // Arrange
        var renderer = CreateRenderer();
        var json = renderer.ToJson();

        // Act
        var result = FlameGraphRendererJsonExtensions.FromJson(json);

        // Assert
        Assert.NotNull(result);
        // Basic sanity: round‑trip should produce a non‑null object.
        var roundTripJson = result!.ToJson();
        Assert.Equal(json, roundTripJson);
    }

    [Fact]
    public void TryFromJson_NullJson_ThrowsArgumentNullException()
    {
        // Arrange
        string? json = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => FlameGraphRendererJsonExtensions.TryFromJson(json!, out _));
    }

    [Fact]
    public void TryFromJson_InvalidJson_ReturnsFalse()
    {
        // Arrange
        var invalidJson = "{ this is not valid json }";

        // Act
        var success = FlameGraphRendererJsonExtensions.TryFromJson(invalidJson, out var renderer);

        // Assert
        Assert.False(success);
        Assert.Null(renderer);
    }

    [Fact]
    public void TryFromJson_ValidJson_ReturnsTrueAndRenderer()
    {
        // Arrange
        var renderer = CreateRenderer();
        var json = renderer.ToJson();

        // Act
        var success = FlameGraphRendererJsonExtensions.TryFromJson(json, out var result);

        // Assert
        Assert.True(success);
        Assert.NotNull(result);
        // Ensure the deserialized object matches the original via round‑trip.
        var roundTripJson = result!.ToJson();
        Assert.Equal(json, roundTripJson);
    }
}
