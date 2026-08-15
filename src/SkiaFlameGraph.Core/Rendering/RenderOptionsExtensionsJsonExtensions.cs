using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Provides JSON (de)serialization helpers for <see cref="RenderOptions"/>.
/// </summary>
public static class RenderOptionsExtensionsJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Serializes the <see cref="RenderOptions"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The <see cref="RenderOptions"/> instance to serialize.</param>
    /// <param name="indented">If <c>true</c>, the output JSON will be formatted with indentation.</param>
    /// <returns>A JSON representation of the <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    public static string ToJson(this RenderOptions value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = indented
            ? new JsonSerializerOptions(_jsonOptions) { WriteIndented = true }
            : _jsonOptions;

        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a JSON string into a <see cref="RenderOptions"/> instance.
    /// </summary>
    /// <param name="json">The JSON string representing a <see cref="RenderOptions"/>.</param>
    /// <returns>The deserialized <see cref="RenderOptions"/> instance, or <c>null</c> if the JSON does not represent a valid object.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is <c>null</c> or empty.</exception>
    public static RenderOptions? FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);
        return JsonSerializer.Deserialize<RenderOptions>(json, _jsonOptions);
    }

    /// <summary>
    /// Attempts to deserialize a JSON string into a <see cref="RenderOptions"/> instance.
    /// </summary>
    /// <param name="json">The JSON string representing a <see cref="RenderOptions"/>.</param>
    /// <param name="value">
    /// When this method returns, contains the deserialized <see cref="RenderOptions"/> if the operation succeeded;
    /// otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if deserialization succeeded; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is <c>null</c> or empty.</exception>
    public static bool TryFromJson(string json, out RenderOptions? value)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);
        try
        {
            value = JsonSerializer.Deserialize<RenderOptions>(json, _jsonOptions);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
