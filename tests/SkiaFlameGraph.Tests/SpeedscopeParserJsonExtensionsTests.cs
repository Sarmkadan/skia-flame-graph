// SPDX-License-Identifier: MIT
// Unit tests for SpeedscopeParserJsonExtensions
// Uses the existing test framework (xUnit) and project namespaces.

using System;
using System.Text.Json;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Parsing;
using Xunit;

namespace SkiaFlameGraph.Tests;

public sealed class SpeedscopeParserJsonExtensionsTests
{
    [Fact]
    public void ToJson_NullArgument_ThrowsArgumentNullException()
    {
        // Arrange
        SpeedscopeFile? file = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SpeedscopeParserJsonExtensions.ToJson(file!));
    }

    [Fact]
    public void ToJson_HappyPath_ReturnsValidJson()
    {
        // Arrange
        var file = new SpeedscopeFile();

        // Act
        string json = SpeedscopeParserJsonExtensions.ToJson(file);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(json));
        // The JSON should be deserializable back to a SpeedscopeFile instance.
        var roundTrip = SpeedscopeParserJsonExtensions.FromJson(json);
        Assert.NotNull(roundTrip);
    }

    [Fact]
    public void ToJson_Indented_ProducesMultilineJson()
    {
        // Arrange
        var file = new SpeedscopeFile();

        // Act
        string json = SpeedscopeParserJsonExtensions.ToJson(file, indented: true);

        // Assert
        // Indented JSON contains at least one newline character.
        Assert.Contains('\n', json);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FromJson_NullOrWhiteSpace_ReturnsNull(string? json)
    {
        // Act
        var result = SpeedscopeParserJsonExtensions.FromJson(json!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void FromJson_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        const string invalidJson = "this is not json";

        // Act & Assert
        Assert.Throws<JsonException>(() => SpeedscopeParserJsonExtensions.FromJson(invalidJson));
    }

    [Fact]
    public void TryFromJson_ValidJson_ReturnsTrueAndValue()
    {
        // Arrange
        const string json = "{}";

        // Act
        bool success = SpeedscopeParserJsonExtensions.TryFromJson(json, out var value);

        // Assert
        Assert.True(success);
        Assert.NotNull(value);
    }

    [Fact]
    public void TryFromJson_InvalidJson_ReturnsFalseAndNull()
    {
        // Arrange
        const string json = "not a json";

        // Act
        bool success = SpeedscopeParserJsonExtensions.TryFromJson(json, out var value);

        // Assert
        Assert.False(success);
        Assert.Null(value);
    }

    [Fact]
    public void TryFromJson_NullOrWhiteSpace_ReturnsFalseAndNull()
    {
        // Null input
        bool successNull = SpeedscopeParserJsonExtensions.TryFromJson(null!, out var valueNull);
        Assert.False(successNull);
        Assert.Null(valueNull);

        // Empty string
        bool successEmpty = SpeedscopeParserJsonExtensions.TryFromJson(string.Empty, out var valueEmpty);
        Assert.False(successEmpty);
        Assert.Null(valueEmpty);
    }
}
