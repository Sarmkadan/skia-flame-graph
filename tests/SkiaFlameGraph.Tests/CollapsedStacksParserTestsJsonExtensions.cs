using System;
using System.Text.Json;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// JSON extension methods for <see cref="CollapsedStacksParserTests"/>.
/// </summary>
public static class CollapsedStacksParserTestsJsonExtensions
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

    /// <summary>
    /// Serializes the <see cref="CollapsedStacksParserTests"/> to JSON.
    /// </summary>
    /// <param name="value">The instance to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation.</param>
    /// <returns>A JSON string representation of the object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static string ToJson(this CollapsedStacksParserTests value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!indented)
        {
            return JsonSerializer.Serialize(value, Options);
        }

        var indentedOptions = new JsonSerializerOptions(Options) { WriteIndented = true };
        return JsonSerializer.Serialize(value, indentedOptions);
    }

    /// <summary>
    /// Deserializes the <see cref="CollapsedStacksParserTests"/> from JSON.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is null or empty.</exception>
    public static CollapsedStacksParserTests? FromJson(string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);
        return JsonSerializer.Deserialize<CollapsedStacksParserTests>(json, Options);
    }

    /// <summary>
    /// Attempts to deserialize the <see cref="CollapsedStacksParserTests"/> from JSON.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">The deserialized instance, if successful.</param>
    /// <returns>True if deserialization succeeded, false otherwise.</returns>
    public static bool TryFromJson(string json, out CollapsedStacksParserTests? value)
    {
        try
        {
            value = JsonSerializer.Deserialize<CollapsedStacksParserTests>(json, Options);
            return true;
        }
        catch (JsonException)
        {
            value = default;
            return false;
        }
    }
}
