using System;
using System.Text.Json;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Provides System.Text.Json serialization helpers for <see cref="FlameGraphRendererTests"/>.
/// </summary>
public static class FlameGraphRendererTestsJsonExtensions
{
    // Cached options with camelCase naming policy.
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Serializes the specified <see cref="FlameGraphRendererTests"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The instance to serialize.</param>
    /// <param name="indented">
    /// If <c>true</c>, the output JSON will be formatted with indentation; otherwise it will be compact.
    /// </param>
    /// <returns>A JSON representation of <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    public static string ToJson(this FlameGraphRendererTests value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        var options = indented ? new JsonSerializerOptions(_options) { WriteIndented = true } : _options;
        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a JSON string into a <see cref="FlameGraphRendererTests"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>
    /// The deserialized <see cref="FlameGraphRendererTests"/> instance, or <c>null</c> if the JSON
    /// does not represent a valid object.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <c>null</c>.</exception>
    public static FlameGraphRendererTests? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        return JsonSerializer.Deserialize<FlameGraphRendererTests>(json, _options);
    }

    /// <summary>
    /// Attempts to deserialize a JSON string into a <see cref="FlameGraphRendererTests"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">
    /// When this method returns, contains the deserialized <see cref="FlameGraphRendererTests"/> instance
    /// if the operation succeeded; otherwise <c>null</c>.
    /// </param>
    /// <returns>
    /// <c>true</c> if the JSON was successfully deserialized; otherwise <c>false</c>.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <c>null</c>.</exception>
    public static bool TryFromJson(string json, out FlameGraphRendererTests? value)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            value = JsonSerializer.Deserialize<FlameGraphRendererTests>(json, _options);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
