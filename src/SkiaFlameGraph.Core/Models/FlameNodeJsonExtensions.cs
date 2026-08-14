namespace SkiaFlameGraph.Core.Models;

using System.Text.Json;
using SkiaFlameGraph.Core;

/// <summary>
/// Provides System.Text.Json serialization helpers for <see cref="FlameNode"/>.
/// </summary>
public static class FlameNodeJsonExtensions
{
    /// <summary>
    /// Serializes the <see cref="FlameNode"/> to a JSON string.
    /// </summary>
    /// <param name="value">The node to serialize.</param>
    /// <param name="indented">Whether to indent the JSON for readability.</param>
    /// <returns>A JSON representation of the node.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static string ToJson(this FlameNode value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = new JsonSerializerOptions(JsonDefaults.Options)
        {
            WriteIndented = indented,
        };
        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a <see cref="FlameNode"/> from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized node, or <see langword="null"/> if the JSON is empty or whitespace.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid or cannot be deserialized.</exception>
    public static FlameNode? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        return string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<FlameNode>(json, JsonDefaults.Options);
    }

    /// <summary>
    /// Attempts to deserialize a <see cref="FlameNode"/> from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">Receives the deserialized node if successful.</param>
    /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    public static bool TryFromJson(string json, out FlameNode? value)
    {
        ArgumentNullException.ThrowIfNull(json);

        if (string.IsNullOrWhiteSpace(json))
        {
            value = null;
            return true;
        }

        try
        {
            value = JsonSerializer.Deserialize<FlameNode>(json, JsonDefaults.Options);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
