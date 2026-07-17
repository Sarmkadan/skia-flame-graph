using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaFlameGraph.Core.Parsing;

namespace SkiaFlameGraph.Tests;

public static class SpeedscopeParserTestsJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    public static string ToJson(this SpeedscopeParserTests value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);
        return JsonSerializer.Serialize(value, _jsonSerializerOptions);
    }

    public static SpeedscopeParserTests? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            return JsonSerializer.Deserialize<SpeedscopeParserTests>(json, _jsonSerializerOptions);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    public static bool TryFromJson(string json, out SpeedscopeParserTests? value)
    {
        ArgumentNullException.ThrowIfNull(json);
        try
        {
            value = JsonSerializer.Deserialize<SpeedscopeParserTests>(json, _jsonSerializerOptions);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}
