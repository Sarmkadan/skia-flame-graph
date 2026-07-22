using System.Text.Json;
using SkiaFlameGraph.Core.Models;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Contains unit tests for the <see cref="SpeedscopeFile"/> class.
/// These tests verify the correct behavior of SpeedscopeFile properties and serialization.
/// </summary>
public class SpeedscopeFileTests
{
    /// <summary>
    /// Tests that SpeedscopeFile can be deserialized from a valid JSON string.
    /// </summary>
    [Fact]
    public void Deserialize_WithValidJson_ReturnsPopulatedFile()
    {
        // Arrange
        var json = """
        {
            "$schema": "https://www.speedscope.app/file-format-schema.json",
            "name": "Test Profile",
            "exporter": "dotnet-trace",
            "shared": {
                "frames": [
                    { "name": "Main", "file": "Program.cs", "line": 10, "col": 5 },
                    { "name": "MethodA", "file": "Program.cs", "line": 20, "col": 15 }
                ]
            },
            "profiles": [
                {
                    "type": "evented",
                    "name": "CPU Profile",
                    "unit": "milliseconds",
                    "startValue": 0,
                    "endValue": 100,
                    "events": [
                        { "type": "O", "frame": 0, "at": 0 },
                        { "type": "O", "frame": 1, "at": 10 },
                        { "type": "C", "frame": 1, "at": 50 },
                        { "type": "C", "frame": 0, "at": 100 }
                    ]
                }
            ]
        }
        """;

        // Act
        var file = JsonSerializer.Deserialize<SpeedscopeFile>(json);

        // Assert
        Assert.NotNull(file);
        Assert.Equal("https://www.speedscope.app/file-format-schema.json", file.Schema);
        Assert.Equal("Test Profile", file.Name);
        Assert.Equal("dotnet-trace", file.Exporter);
        Assert.NotNull(file.Shared);
        Assert.Equal(2, file.Shared.Frames.Count);
        Assert.Equal("Main", file.Shared.Frames[0].Name);
        Assert.Equal("Program.cs", file.Shared.Frames[0].File);
        Assert.Equal(10, file.Shared.Frames[0].Line);
        Assert.Equal(5, file.Shared.Frames[0].Col);
        Assert.Equal("MethodA", file.Shared.Frames[1].Name);
        Assert.Equal(1, file.Profiles.Count);
        Assert.Equal("CPU Profile", file.Profiles[0].Name);
        Assert.Equal("evented", file.Profiles[0].Type);
        Assert.Equal("milliseconds", file.Profiles[0].Unit);
    }

    /// <summary>
    /// Tests that SpeedscopeFile can be deserialized with minimal required fields.
    /// </summary>
    [Fact]
    public void Deserialize_WithMinimalJson_ReturnsValidFile()
    {
        // Arrange
        var json = """
        {
            "shared": { "frames": [] },
            "profiles": []
        }
        """;

        // Act
        var file = JsonSerializer.Deserialize<SpeedscopeFile>(json);

        // Assert
        Assert.NotNull(file);
        Assert.Null(file.Schema);
        Assert.Null(file.Name);
        Assert.Null(file.Exporter);
        Assert.NotNull(file.Shared);
        Assert.Empty(file.Shared.Frames);
        Assert.NotNull(file.Profiles);
        Assert.Empty(file.Profiles);
    }

    /// <summary>
    /// Tests that Schema property can be set and retrieved.
    /// </summary>
    [Fact]
    public void Schema_GetAndSet_ReturnsExpectedValue()
    {
        // Arrange
        var file = new SpeedscopeFile();

        // Act
        file.Schema = "https://example.com/schema.json";

        // Assert
        Assert.Equal("https://example.com/schema.json", file.Schema);
    }

    /// <summary>
    /// Tests that Schema property can be set to null.
    /// </summary>
    [Fact]
    public void Schema_SetToNull_ReturnsNull()
    {
        // Arrange
        var file = new SpeedscopeFile { Schema = "https://example.com/schema.json" };

        // Act
        file.Schema = null;

        // Assert
        Assert.Null(file.Schema);
    }

    /// <summary>
    /// Tests that Name property can be set and retrieved.
    /// </summary>
    [Fact]
    public void Name_GetAndSet_ReturnsExpectedValue()
    {
        // Arrange
        var file = new SpeedscopeFile();

        // Act
        file.Name = "My Application Profile";

        // Assert
        Assert.Equal("My Application Profile", file.Name);
    }

    /// <summary>
    /// Tests that Name property can be set to null.
    /// </summary>
    [Fact]
    public void Name_SetToNull_ReturnsNull()
    {
        // Arrange
        var file = new SpeedscopeFile { Name = "My Application Profile" };

        // Act
        file.Name = null;

        // Assert
        Assert.Null(file.Name);
    }

    /// <summary>
    /// Tests that Exporter property can be set and retrieved.
    /// </summary>
    [Fact]
    public void Exporter_GetAndSet_ReturnsExpectedValue()
    {
        // Arrange
        var file = new SpeedscopeFile();

        // Act
        file.Exporter = "dotnet-trace";

        // Assert
        Assert.Equal("dotnet-trace", file.Exporter);
    }

    /// <summary>
    /// Tests that Exporter property can be set to null.
    /// </summary>
    [Fact]
    public void Exporter_SetToNull_ReturnsNull()
    {
        // Arrange
        var file = new SpeedscopeFile { Exporter = "dotnet-trace" };

        // Act
        file.Exporter = null;

        // Assert
        Assert.Null(file.Exporter);
    }

    /// <summary>
    /// Tests that Shared property is initialized to a new SharedData instance.
    /// </summary>
    [Fact]
    public void Shared_Get_ReturnsInitializedInstance()
    {
        // Arrange & Act
        var file = new SpeedscopeFile();

        // Assert
        Assert.NotNull(file.Shared);
        Assert.IsType<SharedData>(file.Shared);
    }

    /// <summary>
    /// Tests that Shared property can be replaced with a new instance.
    /// </summary>
    [Fact]
    public void Shared_Set_ReplacesInstance()
    {
        // Arrange
        var file = new SpeedscopeFile();
        var newShared = new SharedData { Frames = { new Frame { Name = "Test" } } };

        // Act
        file.Shared = newShared;

        // Assert
        Assert.Same(newShared, file.Shared);
        Assert.Single(file.Shared.Frames);
        Assert.Equal("Test", file.Shared.Frames[0].Name);
    }

    /// <summary>
    /// Tests that Profiles property is initialized to an empty list.
    /// </summary>
    [Fact]
    public void Profiles_Get_ReturnsInitializedList()
    {
        // Arrange & Act
        var file = new SpeedscopeFile();

        // Assert
        Assert.NotNull(file.Profiles);
        Assert.Empty(file.Profiles);
    }

    /// <summary>
    /// Tests that Profiles property can have items added.
    /// </summary>
    [Fact]
    public void Profiles_AddItems_ListContainsItems()
    {
        // Arrange
        var file = new SpeedscopeFile();
        var profile1 = new Profile { Name = "Profile 1", Type = "evented" };
        var profile2 = new Profile { Name = "Profile 2", Type = "sampled" };

        // Act
        file.Profiles.Add(profile1);
        file.Profiles.Add(profile2);

        // Assert
        Assert.Equal(2, file.Profiles.Count);
        Assert.Same(profile1, file.Profiles[0]);
        Assert.Same(profile2, file.Profiles[1]);
    }

    /// <summary>
    /// Tests that Profiles property can be cleared.
    /// </summary>
    [Fact]
    public void Profiles_Clear_ListIsEmpty()
    {
        // Arrange
        var file = new SpeedscopeFile();
        file.Profiles.Add(new Profile());
        file.Profiles.Add(new Profile());

        // Act
        file.Profiles.Clear();

        // Assert
        Assert.Empty(file.Profiles);
    }

    /// <summary>
    /// Tests that Shared.Frames property is initialized to an empty list.
    /// </summary>
    [Fact]
    public void Shared_Frames_Get_ReturnsInitializedList()
    {
        // Arrange & Act
        var file = new SpeedscopeFile();

        // Assert
        Assert.NotNull(file.Shared.Frames);
        Assert.Empty(file.Shared.Frames);
    }

    /// <summary>
    /// Tests that Shared.Frames property can have items added.
    /// </summary>
    [Fact]
    public void Shared_Frames_AddItems_ListContainsItems()
    {
        // Arrange
        var file = new SpeedscopeFile();
        var frame1 = new Frame { Name = "Frame 1", File = "file1.cs", Line = 10, Col = 5 };
        var frame2 = new Frame { Name = "Frame 2", File = "file2.cs", Line = 20, Col = 15 };

        // Act
        file.Shared.Frames.Add(frame1);
        file.Shared.Frames.Add(frame2);

        // Assert
        Assert.Equal(2, file.Shared.Frames.Count);
        Assert.Same(frame1, file.Shared.Frames[0]);
        Assert.Same(frame2, file.Shared.Frames[1]);
        Assert.Equal("Frame 1", file.Shared.Frames[0].Name);
        Assert.Equal("file1.cs", file.Shared.Frames[0].File);
        Assert.Equal(10, file.Shared.Frames[0].Line);
        Assert.Equal(5, file.Shared.Frames[0].Col);
    }

    /// <summary>
    /// Tests that Frame.Name property is required and cannot be null.
    /// </summary>
    [Fact]
    public void Frame_Name_Get_ReturnsEmptyStringByDefault()
    {
        // Arrange & Act
        var frame = new Frame();

        // Assert
        Assert.Equal("", frame.Name);
    }

    /// <summary>
    /// Tests that Frame.Name property can be set.
    /// </summary>
    [Fact]
    public void Frame_Name_Set_ReturnsExpectedValue()
    {
        // Arrange
        var frame = new Frame();

        // Act
        frame.Name = "MyMethod";

        // Assert
        Assert.Equal("MyMethod", frame.Name);
    }

    /// <summary>
    /// Tests that Frame.File property can be null.
    /// </summary>
    [Fact]
    public void Frame_File_CanBeNull()
    {
        // Arrange
        var frame = new Frame { Name = "Test" };

        // Act
        frame.File = null;

        // Assert
        Assert.Null(frame.File);
    }

    /// <summary>
    /// Tests that Frame.Line property can be set to null.
    /// </summary>
    [Fact]
    public void Frame_Line_CanBeNull()
    {
        // Arrange
        var frame = new Frame { Name = "Test" };

        // Act
        frame.Line = null;

        // Assert
        Assert.Null(frame.Line);
    }

    /// <summary>
    /// Tests that Frame.Col property can be set to null.
    /// </summary>
    [Fact]
    public void Frame_Col_CanBeNull()
    {
        // Arrange
        var frame = new Frame { Name = "Test" };

        // Act
        frame.Col = null;

        // Assert
        Assert.Null(frame.Col);
    }

    /// <summary>
    /// Tests serialization round-trip preserves all properties.
    /// </summary>
    [Fact]
    public void SerializeAndDeserialize_RoundTrip_PreservesAllProperties()
    {
        // Arrange
        var original = new SpeedscopeFile
        {
            Schema = "https://example.com/schema.json",
            Name = "Test File",
            Exporter = "dotnet-trace",
            Shared = new SharedData
            {
                Frames =
                {
                    new Frame { Name = "Main", File = "Program.cs", Line = 10, Col = 5 },
                    new Frame { Name = "MethodA", File = "Program.cs", Line = 20, Col = 15 }
                }
            },
            Profiles =
            {
                new Profile
                {
                    Name = "CPU Profile",
                    Type = "evented",
                    Unit = "milliseconds",
                    StartValue = 0,
                    EndValue = 100,
                    Events = new List<ProfileEvent>
                    {
                        new ProfileEvent { Type = "O", Frame = 0, At = 0 },
                        new ProfileEvent { Type = "C", Frame = 0, At = 100 }
                    }
                }
            }
        };

        // Act
        var json = JsonSerializer.Serialize(original, new JsonSerializerOptions { WriteIndented = true });
        var deserialized = JsonSerializer.Deserialize<SpeedscopeFile>(json);

        // Assert
        Assert.NotNull(deserialized);
        Assert.Equal(original.Schema, deserialized.Schema);
        Assert.Equal(original.Name, deserialized.Name);
        Assert.Equal(original.Exporter, deserialized.Exporter);
        Assert.Equal(original.Shared.Frames.Count, deserialized.Shared.Frames.Count);
        Assert.Equal(original.Shared.Frames[0].Name, deserialized.Shared.Frames[0].Name);
        Assert.Equal(original.Shared.Frames[0].File, deserialized.Shared.Frames[0].File);
        Assert.Equal(original.Shared.Frames[0].Line, deserialized.Shared.Frames[0].Line);
        Assert.Equal(original.Shared.Frames[0].Col, deserialized.Shared.Frames[0].Col);
        Assert.Equal(original.Profiles.Count, deserialized.Profiles.Count);
        Assert.Equal(original.Profiles[0].Name, deserialized.Profiles[0].Name);
        Assert.Equal(original.Profiles[0].Type, deserialized.Profiles[0].Type);
    }

    /// <summary>
    /// Tests that a SpeedscopeFile with null Shared property can still be created.
    /// </summary>
    [Fact]
    public void Constructor_AllowsNullSharedProperty()
    {
        // Arrange & Act
        var file = new SpeedscopeFile { Shared = null };

        // Assert
        Assert.Null(file.Shared);
    }

    /// <summary>
    /// Tests that a SpeedscopeFile with null Profiles property can still be created.
    /// </summary>
    [Fact]
    public void Constructor_AllowsNullProfilesProperty()
    {
        // Arrange & Act
        var file = new SpeedscopeFile { Profiles = null };

        // Assert
        Assert.Null(file.Profiles);
    }

    /// <summary>
    /// Tests deserialization with null input throws appropriate exception.
    /// </summary>
    [Fact]
    public void Deserialize_NullJson_Throws()
    {
        // Arrange
        string json = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => JsonSerializer.Deserialize<SpeedscopeFile>(json));
    }

    /// <summary>
    /// Tests that Frame with all properties set serializes correctly.
    /// </summary>
    [Fact]
    public void Frame_WithAllProperties_SerializesCorrectly()
    {
        // Arrange
        var frame = new Frame
        {
            Name = "CompleteMethod",
            File = "Program.cs",
            Line = 42,
            Col = 10
        };

        // Act
        var json = JsonSerializer.Serialize(frame);

        // Assert
        Assert.Contains("\"name\":\"CompleteMethod\"", json);
        Assert.Contains("\"file\":\"Program.cs\"", json);
        Assert.Contains("\"line\":42", json);
        Assert.Contains("\"col\":10", json);
    }
}
