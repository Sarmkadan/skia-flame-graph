using System;
using System.Text.Json;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Provides JSON (de)serialization helpers for <see cref="RenderOptions"/>.
/// </summary>
public static class RenderOptionsValidationJsonExtensions
{
    // Cached options with camel‑case naming. The <c>WriteIndented</c> flag is set per call.
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    /// <summary>
    /// Serializes the specified <see cref="RenderOptions"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The <see cref="RenderOptions"/> instance to serialize.</param>
    /// <param name="indented">
    /// If <c>true</c>, the output JSON will be formatted with indentation; otherwise it will be compact.
    /// </param>
    /// <returns>A JSON representation of <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
    public static string ToJson(this RenderOptions value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        var options = new JsonSerializerOptions(_jsonOptions) { WriteIndented = indented };
        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Deserializes a JSON string into a <see cref="RenderOptions"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>
    /// A <see cref="RenderOptions"/> instance, or <c>null</c> if <paramref name="json"/> is empty or whitespace.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <c>null</c>.</exception>
    /// <exception cref="JsonException">
    /// Thrown when the JSON is malformed or cannot be mapped to <see cref="RenderOptions"/>.
    /// </exception>
    public static RenderOptions? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        if (string.IsNullOrWhiteSpace(json))
        {
            return null;
        }

        return JsonSerializer.Deserialize<RenderOptions>(json, _jsonOptions);
    }

    /// <summary>
    /// Attempts to deserialize a JSON string into a <see cref="RenderOptions"/> instance.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">
    /// When this method returns, contains the deserialized <see cref="RenderOptions"/> if the operation succeeded;
    /// otherwise, <c>null</c>.
    /// </param>
    /// <returns><c>true</c> if deserialization succeeded; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <c>null</c>.</exception>
    public static bool TryFromJson(string json, out RenderOptions? value)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            value = FromJson(json);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
