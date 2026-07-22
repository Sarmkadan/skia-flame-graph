using System;
using System.Text.Json;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Unit tests for <see cref="TreemapRendererJsonExtensions"/>.
/// </summary>
public class TreemapRendererJsonExtensionsTests
{
    /// <summary>
    /// Creates a minimal <see cref="TreemapRenderer"/> instance for testing.
    /// </summary>
    private static TreemapRenderer CreateRenderer()
    {
        // The renderer has a public parameterless constructor via optional parameter.
        return new TreemapRenderer();
    }

    [Fact]
    public void ToJson_NullValue_ThrowsArgumentNullException()
    {
        // Arrange
        TreemapRenderer? renderer = null;

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
        Assert.Equal("{}", json);
    }

    // Note: ToJson with indented=true produces pretty JSON, but this test
    // is omitted because TreemapRenderer serializes to "{}" with no properties.

    [Fact]
    public void FromJson_NullJson_ThrowsArgumentNullException()
    {
        // Arrange
        string? json = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => TreemapRendererJsonExtensions.FromJson(json!));
    }

    [Fact]
    public void FromJson_EmptyOrWhitespaceJson_ThrowsJsonException()
    {
        // Arrange
        var emptyJson = " ";

        // Act & Assert
        Assert.Throws<JsonException>(() => TreemapRendererJsonExtensions.FromJson(emptyJson));
    }

    [Fact]
    public void TryFromJson_NullJson_ThrowsArgumentNullException()
    {
        // Arrange
        string? json = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => TreemapRendererJsonExtensions.TryFromJson(json!, out _));
    }

    [Fact]
    public void TryFromJson_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = "{ this is not valid json }";

        // Act & Assert
        Assert.Throws<JsonException>(
            () => TreemapRendererJsonExtensions.TryFromJson(invalidJson, out _));
    }
}