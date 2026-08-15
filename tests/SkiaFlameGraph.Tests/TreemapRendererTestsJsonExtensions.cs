using System;
using System.Text.Json;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Provides JSON serialization and deserialization extensions for <see cref="TreemapRendererTests"/>.
/// </summary>
public static class TreemapRendererTestsJsonExtensions
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Converts the <see cref="TreemapRendererTests"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The instance to serialize.</param>
    /// <param name="indented">If true, writes the JSON with indentation; otherwise, writes compact JSON.</param>
    /// <returns>A JSON string representation of the instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static string ToJson(this TreemapRendererTests value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = indented
            ? new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = true }
            : Options;

        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Parses a JSON string into a <see cref="TreemapRendererTests"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The deserialized <see cref="TreemapRendererTests"/> instance, or null if deserialization fails.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="json"/> is null or empty.</exception>
    public static TreemapRendererTests? FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);

        return JsonSerializer.Deserialize<TreemapRendererTests>(json, Options);
    }

    /// <summary>
    /// Tries to parse a JSON string into a <see cref="TreemapRendererTests"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <param name="value">When this method returns, contains the deserialized instance if the parse succeeded; otherwise, null.</param>
    /// <returns>true if the JSON was parsed successfully; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="json"/> is null or empty.</exception>
    public static bool TryFromJson(string json, out TreemapRendererTests? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);

        try
        {
            value = JsonSerializer.Deserialize<TreemapRendererTests>(json, Options);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
