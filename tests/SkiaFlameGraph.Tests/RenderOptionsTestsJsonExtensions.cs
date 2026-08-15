using System;
using System.Text.Json;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Provides JSON serialization and deserialization extensions for <see cref="RenderOptionsTests"/>.
/// </summary>
public static class RenderOptionsTestsJsonExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Converts the <see cref="RenderOptionsTests"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The instance to serialize.</param>
    /// <param name="indented">If true, writes the JSON with indentation; otherwise, writes compact JSON.</param>
    /// <returns>A JSON string representation of the instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static string ToJson(this RenderOptionsTests value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        return indented
            ? JsonSerializer.Serialize(value, new JsonSerializerOptions(Options) { WriteIndented = true })
            : JsonSerializer.Serialize(value, Options);
    }

    /// <summary>
    /// Parses a JSON string into a <see cref="RenderOptionsTests"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The parsed <see cref="RenderOptionsTests"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="json"/> is null or empty.</exception>
    /// <exception cref="JsonException">Thrown if the JSON is invalid.</exception>
    public static RenderOptionsTests? FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);
        return JsonSerializer.Deserialize<RenderOptionsTests>(json, Options);
    }

    /// <summary>
    /// Attempts to parse a JSON string into a <see cref="RenderOptionsTests"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <param name="value">When this method returns, contains the parsed value if the parse succeeded, or null if it failed.</param>
    /// <returns>true if the JSON was parsed successfully; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="json"/> is null or empty.</exception>
    public static bool TryFromJson(string json, out RenderOptionsTests? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);

        try
        {
            value = JsonSerializer.Deserialize<RenderOptionsTests>(json, Options);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
