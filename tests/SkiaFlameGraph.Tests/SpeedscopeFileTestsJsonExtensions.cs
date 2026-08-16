using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Provides JSON serialization extensions for <see cref="SpeedscopeFileTests"/>.
/// </summary>
public static class SpeedscopeFileTestsJsonExtensions
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Serializes a <see cref="SpeedscopeFileTests"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The instance to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON string representation of the instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string ToJson(this SpeedscopeFileTests value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = indented ? new JsonSerializerOptions(_options) { WriteIndented = true } : _options;
        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a JSON string to a <see cref="SpeedscopeFileTests"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized <see cref="SpeedscopeFileTests"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is null or empty.</exception>
    public static SpeedscopeFileTests? FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);

        return JsonSerializer.Deserialize<SpeedscopeFileTests>(json, _options);
    }

    /// <summary>
    /// Tries to deserialize a JSON string to a <see cref="SpeedscopeFileTests"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">The resulting <see cref="SpeedscopeFileTests"/> instance if successful; otherwise, null.</param>
    /// <returns>True if deserialization was successful; otherwise, false.</returns>
    public static bool TryFromJson(string json, [NotNullWhen(true)] out SpeedscopeFileTests? value)
    {
        try
        {
            value = FromJson(json);
            return value != null;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
