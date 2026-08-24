using System.Text.Json.Serialization;

namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Minimal object model for the speedscope file format as emitted by
/// <c>dotnet-trace convert --format speedscope</c>. Only the fields we actually
/// consume are mapped - speedscope files carry a bit more metadata that we
/// happily ignore.
/// Format reference: https://github.com/jlfwong/speedscope/blob/main/src/lib/file-format-spec.ts
/// </summary>
public sealed class SpeedscopeFile : ISpeedscopeFile, IEquatable<SpeedscopeFile>
{
    /// <summary>
    /// Gets or sets the schema version.
    /// </summary>
    [JsonPropertyName(SpeedscopeFileConstants.JsonPropertyNameSchema)]
    public string? Schema { get; set; }

    /// <summary>
    /// Gets or sets the shared data.
    /// </summary>
    [JsonPropertyName(SpeedscopeFileConstants.JsonPropertyNameShared)]
    public SharedData Shared { get; set; } = new();

    /// <summary>
    /// Gets or sets the profiles.
    /// </summary>
    [JsonPropertyName("profiles")]
    public List<Profile> Profiles { get; set; } = new();

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the exporter.
    /// </summary>
    [JsonPropertyName("exporter")]
    public string? Exporter { get; set; }

    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
    public bool Equals(SpeedscopeFile? other)
    {
        if (other is null) return false;
        return Schema == other.Schema
            && EqualityComparer<SharedData>.Default.Equals(Shared, other.Shared)
            && EqualityComparer<List<Profile>>.Default.Equals(Profiles, other.Profiles)
            && Name == other.Name
            && Exporter == other.Exporter;
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj">The object to compare with the current object.</param>
    /// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
    public override bool Equals(object? obj) => Equals(obj as SpeedscopeFile);

    /// <summary>
    /// Serves as the default hash function.
    /// </summary>
    /// <returns>A hash code for the current object.</returns>
    public override int GetHashCode()
    {
        return HashCode.Combine(Schema, Shared, Profiles, Name, Exporter);
    }

    /// <summary>
    /// Determines whether two specified instances of <see cref="SpeedscopeFile"/> are equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if the objects are equal; otherwise, false.</returns>
    public static bool operator ==(SpeedscopeFile? left, SpeedscopeFile? right) =>
        EqualityComparer<SpeedscopeFile>.Default.Equals(left, right);

    /// <summary>
    /// Determines whether two specified instances of <see cref="SpeedscopeFile"/> are not equal.
    /// </summary>
    /// <param name="left">The first object to compare.</param>
    /// <param name="right">The second object to compare.</param>
    /// <returns>true if the objects are not equal; otherwise, false.</returns>
    public static bool operator !=(SpeedscopeFile? left, SpeedscopeFile? right) =>
        !(EqualityComparer<SpeedscopeFile>.Default.Equals(left, right));

    /// <summary>
    /// Returns a string representation of the SpeedscopeFile.
    /// </summary>
    /// <returns>A string representation of the SpeedscopeFile.</returns>
    public override string ToString()
    {
        return $"SpeedscopeFile {{ Schema = {Schema}, Shared = {Shared}, Profiles = {Profiles}, Name = {Name}, Exporter = {Exporter}, Frames = {Shared.Frames} }}";
    }
}

/// <summary>
/// Contains the shared data definitions, such as frames, used by profiles in the speedscope file.
/// </summary>
public sealed class SharedData
{
    /// <summary>
    /// Gets or sets the list of frames.
    /// </summary>
    [JsonPropertyName("frames")]
    public List<Frame> Frames { get; set; } = new();
}

/// <summary>
/// Represents a single stack frame in the speedscope file.
/// </summary>
public sealed class Frame
{
    /// <summary>
    /// Gets or sets the name of the frame.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    /// <summary>
    /// Gets or sets the file name.
    /// </summary>
    [JsonPropertyName("file")]
    public string? File { get; set; }

    /// <summary>
    /// Gets or sets the line number.
    /// </summary>
    [JsonPropertyName("line")]
    public int? Line { get; set; }

    /// <summary>
    /// Gets or sets the column number.
    /// </summary>
    [JsonPropertyName("col")]
    public int? Col { get; set; }
}

/// <summary>
/// A single profile. dotnet-trace emits "evented" profiles (a stream of
/// open/close frame events); speedscope also defines a "sampled" variant. We
/// support both because real traces mix them depending on the converter version.
/// </summary>
public sealed class Profile
{
    /// <summary>
    /// Gets or sets the type of the profile.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "evented";

    /// <summary>
    /// Gets or sets the name of the profile.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the unit of the profile values.
    /// </summary>
    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "none";

    /// <summary>
    /// Gets or sets the start value.
    /// </summary>
    [JsonPropertyName("startValue")]
    public double StartValue { get; set; }

    /// <summary>
    /// Gets or sets the end value.
    /// </summary>
    [JsonPropertyName("endValue")]
    public double EndValue { get; set; }

    /// <summary>
    /// Gets or sets the events for "evented" profiles.
    /// </summary>
    [JsonPropertyName("events")]
    public List<ProfileEvent>? Events { get; set; }

    /// <summary>
    /// Gets or sets the samples for "sampled" profiles.
    /// </summary>
    [JsonPropertyName("samples")]
    public List<List<int>>? Samples { get; set; }

    /// <summary>
    /// Gets or sets the weights for "sampled" profiles.
    /// </summary>
    [JsonPropertyName("weights")]
    public List<double>? Weights { get; set; }
}

/// <summary>
/// Represents a single event in an "evented" profile.
/// </summary>
public sealed class ProfileEvent
{
    /// <summary>
    /// Gets or sets the type of the event ("O" for open-frame, "C" for close-frame).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "O";

    /// <summary>
    /// Gets or sets the frame index.
    /// </summary>
    [JsonPropertyName("frame")]
    public int Frame { get; set; }

    /// <summary>
    /// Gets or sets the timestamp or value at which the event occurred.
    /// </summary>
    [JsonPropertyName("at")]
    public double At { get; set; }
}
