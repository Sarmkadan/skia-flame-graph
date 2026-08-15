using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkiaFlameGraph.Tests;

public static class FlameGraphRendererJsonExtensionsTestsJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public static string ToJson(this FlameGraphRendererJsonExtensionsTests value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        _jsonSerializerOptions.WriteIndented = indented;
        return JsonSerializer.Serialize(value, _jsonSerializerOptions);
    }

    public static FlameGraphRendererJsonExtensionsTests? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            return JsonSerializer.Deserialize<FlameGraphRendererJsonExtensionsTests>(json, _jsonSerializerOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static bool TryFromJson(string json, out FlameGraphRendererJsonExtensionsTests? value)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            value = JsonSerializer.Deserialize<FlameGraphRendererJsonExtensionsTests>(json, _jsonSerializerOptions);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
