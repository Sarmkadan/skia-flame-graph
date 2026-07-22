using System;
using Xunit;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Unit tests for <see cref="SpeedscopeFileJsonExtensions"/> JSON serialization/deserialization methods.
/// </summary>
public class SpeedscopeFileJsonExtensionsTests
{
    [Fact]
    public void ToJson_WithValidSpeedscopeFile_ReturnsJsonString()
    {
        // Arrange
        var file = new SpeedscopeFile
        {
            Name = "Test Profile",
            Exporter = "dotnet-trace",
            Shared = new SharedData
            {
                Frames = new List<Frame>
                {
                    new Frame { Name = "Frame1", File = "Program.cs", Line = 10 },
                    new Frame { Name = "Frame2", File = "Program.cs", Line = 20 }
                }
            },
            Profiles = new List<Profile>
            {
                new Profile
                {
                    Type = "evented",
                    Name = "Main Profile",
                    Unit = "milliseconds",
                    StartValue = 0,
                    EndValue = 100,
                    Events = new List<ProfileEvent>
                    {
                        new ProfileEvent { Type = "O", Frame = 0, At = 0 },
                        new ProfileEvent { Type = "C", Frame = 0, At = 50 },
                        new ProfileEvent { Type = "O", Frame = 1, At = 50 },
                        new ProfileEvent { Type = "C", Frame = 1, At = 100 }
                    }
                }
            }
        };

        // Act
        var json = file.ToJson();

        // Assert
        Assert.NotNull(json);
        Assert.NotEmpty(json);
        Assert.Contains("Test Profile", json);
        Assert.Contains("dotnet-trace", json);
        Assert.Contains("Frame1", json);
    }

    [Fact]
    public void ToJson_WithIndentedTrue_ReturnsFormattedJson()
    {
        // Arrange
        var file = new SpeedscopeFile
        {
            Name = "Test Profile",
            Profiles = new List<Profile>()
        };

        // Act
        var json = file.ToJson(indented: true);
        var nonIndentedJson = file.ToJson(indented: false);

        // Assert
        Assert.NotNull(json);
        Assert.NotNull(nonIndentedJson);
        Assert.True(json.Split('\n').Length > 2, "Indented JSON should have multiple lines");
        Assert.True(nonIndentedJson.Length < json.Length, "Non-indented JSON should be more compact");
    }

    [Fact]
    public void ToJson_WithNullValue_ThrowsArgumentNullException()
    {
        // Arrange
        SpeedscopeFile? file = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => file!.ToJson());
    }

    [Fact]
    public void FromJson_WithValidJson_ReturnsSpeedscopeFile()
    {
        // Arrange
        var json = "{\r\n" +
            "  \"$schema\": \"https://www.speedscope.app/file-format-schema.json\",\r\n" +
            "  \"name\": \"Test Profile\",\r\n" +
            "  \"exporter\": \"dotnet-trace\",\r\n" +
            "  \"shared\": {\r\n" +
            "    \"frames\": [\r\n" +
            "      { \"name\": \"Frame1\", \"file\": \"Program.cs\", \"line\": 10 },\r\n" +
            "      { \"name\": \"Frame2\", \"file\": \"Program.cs\", \"line\": 20 }\r\n" +
            "    ]\r\n" +
            "  },\r\n" +
            "  \"profiles\": [\r\n" +
            "    {\r\n" +
            "      \"type\": \"evented\",\r\n" +
            "      \"name\": \"Main Profile\",\r\n" +
            "      \"unit\": \"milliseconds\",\r\n" +
            "      \"startValue\": 0,\r\n" +
            "      \"endValue\": 100,\r\n" +
            "      \"events\": [\r\n" +
            "        { \"type\": \"O\", \"frame\": 0, \"at\": 0 },\r\n" +
            "        { \"type\": \"C\", \"frame\": 0, \"at\": 50 },\r\n" +
            "        { \"type\": \"O\", \"frame\": 1, \"at\": 50 },\r\n" +
            "        { \"type\": \"C\", \"frame\": 1, \"at\": 100 }\r\n" +
            "      ]\r\n" +
            "    }\r\n" +
            "  ]\r\n" +
            "}";

        // Act
        var file = SpeedscopeFileJsonExtensions.FromJson(json);

        // Assert
        Assert.NotNull(file);
        Assert.Equal("Test Profile", file.Name);
        Assert.Equal("dotnet-trace", file.Exporter);
        Assert.Equal(2, file.Shared.Frames.Count);
        Assert.Equal("Frame1", file.Shared.Frames[0].Name);
        Assert.Equal(1, file.Profiles.Count);
        Assert.Equal("Main Profile", file.Profiles[0].Name);
        Assert.Equal(4, file.Profiles[0].Events?.Count);
    }

    [Fact]
    public void FromJson_WithNullJson_ThrowsArgumentNullException()
    {
        // Arrange
        string? json = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SpeedscopeFileJsonExtensions.FromJson(json!));
    }

    [Fact]
    public void FromJson_WithInvalidJson_ThrowsJsonException()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act & Assert
        Assert.Throws<System.Text.Json.JsonException>(() => SpeedscopeFileJsonExtensions.FromJson(invalidJson));
    }

    [Fact]
    public void FromJson_WithEmptyJson_ReturnsSpeedscopeFileWithDefaults()
    {
        // Arrange
        var emptyJson = "{}";

        // Act
        var file = SpeedscopeFileJsonExtensions.FromJson(emptyJson);

        // Assert
        Assert.NotNull(file);
        Assert.NotNull(file.Shared);
        Assert.NotNull(file.Profiles);
        Assert.Empty(file.Shared.Frames);
        Assert.Empty(file.Profiles);
    }

    [Fact]
    public void TryFromJson_WithValidJson_ReturnsTrueAndDeserializes()
    {
        // Arrange
        var json = "{\r\n" +
            "  \"$schema\": \"https://www.speedscope.app/file-format-schema.json\",\r\n" +
            "  \"name\": \"Test Profile\",\r\n" +
            "  \"shared\": {\r\n" +
            "    \"frames\": [\r\n" +
            "      { \"name\": \"Frame1\" }\r\n" +
            "    ]\r\n" +
            "  },\r\n" +
            "  \"profiles\": []\r\n" +
            "}";

        // Act
        var result = SpeedscopeFileJsonExtensions.TryFromJson(json, out var file);

        // Assert
        Assert.True(result);
        Assert.NotNull(file);
        Assert.Equal("Test Profile", file.Name);
    }

    [Fact]
    public void TryFromJson_WithInvalidJson_ReturnsFalseAndNull()
    {
        // Arrange
        var invalidJson = "{ invalid json }";

        // Act
        var result = SpeedscopeFileJsonExtensions.TryFromJson(invalidJson, out var file);

        // Assert
        Assert.False(result);
        Assert.Null(file);
    }

    [Fact]
    public void TryFromJson_WithNullJson_ThrowsArgumentNullException()
    {
        // Arrange
        string? json = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => SpeedscopeFileJsonExtensions.TryFromJson(json!, out _));
    }

    [Fact]
    public void RoundtripSerialization_ProducesEquivalentObject()
    {
        // Arrange
        var originalFile = new SpeedscopeFile
        {
            Name = "Roundtrip Test",
            Exporter = "test-exporter",
            Shared = new SharedData
            {
                Frames = new List<Frame>
                {
                    new Frame { Name = "Method1", File = "Program.cs", Line = 10 },
                    new Frame { Name = "Method2", File = "Program.cs", Line = 20 },
                    new Frame { Name = "Method3", File = "Program.cs", Line = 30 }
                }
            },
            Profiles = new List<Profile>
            {
                new Profile
                {
                    Type = "evented",
                    Name = "Profile1",
                    Unit = "seconds",
                    StartValue = 0,
                    EndValue = 10,
                    Events = new List<ProfileEvent>
                    {
                        new ProfileEvent { Type = "O", Frame = 0, At = 0 },
                        new ProfileEvent { Type = "C", Frame = 0, At = 5 },
                        new ProfileEvent { Type = "O", Frame = 1, At = 5 },
                        new ProfileEvent { Type = "C", Frame = 1, At = 10 }
                    }
                },
                new Profile
                {
                    Type = "sampled",
                    Name = "Profile2",
                    Unit = "milliseconds",
                    StartValue = 0,
                    EndValue = 100,
                    Samples = new List<List<int>>
                    {
                        new List<int> { 0, 1 },
                        new List<int> { 1, 2 },
                        new List<int> { 2 }
                    },
                    Weights = new List<double> { 1.0, 1.0, 1.0 }
                }
            }
        };

        // Act - serialize and deserialize
        var json = originalFile.ToJson();
        var deserializedFile = SpeedscopeFileJsonExtensions.FromJson(json);

        // Assert
        Assert.NotNull(deserializedFile);
        Assert.Equal(originalFile.Name, deserializedFile.Name);
        Assert.Equal(originalFile.Exporter, deserializedFile.Exporter);
        Assert.Equal(originalFile.Shared.Frames.Count, deserializedFile.Shared.Frames.Count);

        for (int i = 0; i < originalFile.Shared.Frames.Count; i++)
        {
            Assert.Equal(originalFile.Shared.Frames[i].Name, deserializedFile.Shared.Frames[i].Name);
            Assert.Equal(originalFile.Shared.Frames[i].File, deserializedFile.Shared.Frames[i].File);
            Assert.Equal(originalFile.Shared.Frames[i].Line, deserializedFile.Shared.Frames[i].Line);
        }

        Assert.Equal(originalFile.Profiles.Count, deserializedFile.Profiles.Count);

        for (int i = 0; i < originalFile.Profiles.Count; i++)
        {
            var originalProfile = originalFile.Profiles[i];
            var deserializedProfile = deserializedFile.Profiles[i];

            Assert.Equal(originalProfile.Type, deserializedProfile.Type);
            Assert.Equal(originalProfile.Name, deserializedProfile.Name);
            Assert.Equal(originalProfile.Unit, deserializedProfile.Unit);
            Assert.Equal(originalProfile.StartValue, deserializedProfile.StartValue);
            Assert.Equal(originalProfile.EndValue, deserializedProfile.EndValue);

            if (originalProfile.Events != null)
            {
                Assert.NotNull(deserializedProfile.Events);
                Assert.Equal(originalProfile.Events.Count, deserializedProfile.Events.Count);
            }
            else
            {
                Assert.Null(deserializedProfile.Events);
            }

            if (originalProfile.Samples != null)
            {
                Assert.NotNull(deserializedProfile.Samples);
                Assert.Equal(originalProfile.Samples.Count, deserializedProfile.Samples.Count);
            }
            else
            {
                Assert.Null(deserializedProfile.Samples);
            }
        }
    }

    [Fact]
    public void RoundtripSerialization_WithTryFromJson_ProducesEquivalentObject()
    {
        // Arrange
        var originalFile = new SpeedscopeFile
        {
            Name = "TryFromJson Test",
            Shared = new SharedData
            {
                Frames = new List<Frame> { new Frame { Name = "TestFrame" } }
            },
            Profiles = new List<Profile>()
        };

        // Act - serialize and deserialize using TryFromJson
        var json = originalFile.ToJson();
        var result = SpeedscopeFileJsonExtensions.TryFromJson(json, out var deserializedFile);

        // Assert
        Assert.True(result);
        Assert.NotNull(deserializedFile);
        Assert.Equal(originalFile.Name, deserializedFile.Name);
    }

    [Fact]
    public void ToJson_ProducesCamelCasePropertyNames()
    {
        // Arrange
        var file = new SpeedscopeFile
        {
            Name = "CamelCase Test",
            Exporter = "test-exporter",
            Shared = new SharedData
            {
                Frames = new List<Frame> { new Frame { Name = "TestFrame" } }
            },
            Profiles = new List<Profile>()
        };

        // Act
        var json = file.ToJson();

        // Assert - should use camelCase for JSON properties
        Assert.Contains("\"name\"", json);
        Assert.Contains("\"exporter\"", json);
        Assert.Contains("\"shared\"", json);
        Assert.Contains("\"profiles\"", json);
        Assert.Contains("\"frames\"", json);
    }

    [Fact]
    public void FromJson_WithMinimalValidJson_ReturnsSpeedscopeFile()
    {
        // Arrange
        var json = "{\r\n" +
            "  \"shared\": {\r\n" +
            "    \"frames\": []\r\n" +
            "  },\r\n" +
            "  \"profiles\": []\r\n" +
            "}";

        // Act
        var file = SpeedscopeFileJsonExtensions.FromJson(json);

        // Assert
        Assert.NotNull(file);
        Assert.NotNull(file.Shared);
        Assert.NotNull(file.Profiles);
    }

    [Fact]
    public void TryFromJson_WithEmptyObject_ReturnsTrueWithEmptyFile()
    {
        // Arrange
        var json = "{}";

        // Act
        var result = SpeedscopeFileJsonExtensions.TryFromJson(json, out var file);

        // Assert
        Assert.True(result);
        Assert.NotNull(file);
        Assert.NotNull(file.Shared);
        Assert.NotNull(file.Profiles);
    }
}
