using System;
using System.Text.Json;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Provides JSON serialization helpers for <see cref="TreemapRendererJsonExtensionsTests"/>.
/// </summary>
public static class TreemapRendererJsonExtensionsTestsJsonExtensions
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Serializes the <paramref name="value"/> to a JSON string.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>The serialized JSON string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string ToJson(this TreemapRendererJsonExtensionsTests value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        var options = indented ? new JsonSerializerOptions(_options) { WriteIndented = true } : _options;
        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a <see cref="TreemapRendererJsonExtensionsTests"/> instance from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <returns>The deserialized <see cref="TreemapRendererJsonExtensionsTests"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is null or empty.</exception>
    /// <exception cref="JsonException">Thrown when deserialization fails.</exception>
    public static TreemapRendererJsonExtensionsTests? FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);
        return JsonSerializer.Deserialize<TreemapRendererJsonExtensionsTests>(json, _options);
    }

    /// <summary>
    /// Attempts to deserialize a <see cref="TreemapRendererJsonExtensionsTests"/> instance from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string.</param>
    /// <param name="value">The deserialized <see cref="TreemapRendererJsonExtensionsTests"/> instance, or null if deserialization failed.</param>
    /// <returns>True if deserialization succeeded, otherwise false.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is null or empty.</exception>
    public static bool TryFromJson(string json, out TreemapRendererJsonExtensionsTests? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);
        try
        {
            value = JsonSerializer.Deserialize<TreemapRendererJsonExtensionsTests>(json, _options);
            return true;
        }
        catch (JsonException)
        {
            value = default;
            return false;
        }
    }
}
