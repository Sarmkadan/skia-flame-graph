using System.Text.Json.Serialization;

namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Minimal object model for the speedscope file format as emitted by
/// <c>dotnet-trace convert --format speedscope</c>. Only the fields we actually
/// consume are mapped - speedscope files carry a bit more metadata that we
/// happily ignore.
/// Format reference: https://github.com/jlfwong/speedscope/blob/main/src/lib/file-format-spec.ts
/// </summary>
public sealed class SpeedscopeFile
{
    [JsonPropertyName("$schema")]
    public string? Schema { get; set; }

    [JsonPropertyName("shared")]
    public SharedData Shared { get; set; } = new();

    [JsonPropertyName("profiles")]
    public List<Profile> Profiles { get; set; } = new();

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("exporter")]
    public string? Exporter { get; set; }
}

public sealed class SharedData
{
    [JsonPropertyName("frames")]
    public List<Frame> Frames { get; set; } = new();
}

public sealed class Frame
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("file")]
    public string? File { get; set; }

    [JsonPropertyName("line")]
    public int? Line { get; set; }

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
    [JsonPropertyName("type")]
    public string Type { get; set; } = "evented";

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = "none";

    [JsonPropertyName("startValue")]
    public double StartValue { get; set; }

    [JsonPropertyName("endValue")]
    public double EndValue { get; set; }

    // evented profiles
    [JsonPropertyName("events")]
    public List<ProfileEvent>? Events { get; set; }

    // sampled profiles
    [JsonPropertyName("samples")]
    public List<List<int>>? Samples { get; set; }

    [JsonPropertyName("weights")]
    public List<double>? Weights { get; set; }
}

public sealed class ProfileEvent
{
    /// <summary>"O" for open-frame, "C" for close-frame.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = "O";

    [JsonPropertyName("frame")]
    public int Frame { get; set; }

    [JsonPropertyName("at")]
    public double At { get; set; }
}
