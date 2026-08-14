using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using SkiaFlameGraph.Core;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Provides System.Text.Json serialization and deserialization extensions for <see cref="TreemapRenderer"/>.
/// </summary>
public static class TreemapRendererJsonExtensions
{
    /// <summary>
    /// Serializes the <see cref="TreemapRenderer"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The renderer to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON string representation of the renderer.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static string ToJson(this TreemapRenderer value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = new JsonSerializerOptions(JsonDefaults.Options)
        {
            WriteIndented = indented,
        };

        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a <see cref="TreemapRenderer"/> instance from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized renderer instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid or cannot be deserialized.</exception>
    public static TreemapRenderer? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        return string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<TreemapRenderer>(json, JsonDefaults.Options);
    }

    /// <summary>
    /// Attempts to deserialize a <see cref="TreemapRenderer"/> instance from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">Receives the deserialized renderer if successful.</param>
    /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    public static bool TryFromJson(string json, out TreemapRenderer? value)
    {
        ArgumentNullException.ThrowIfNull(json);

        if (string.IsNullOrWhiteSpace(json))
        {
            value = null;
            return true;
        }

        try
        {
            value = JsonSerializer.Deserialize<TreemapRenderer>(json, JsonDefaults.Options);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}