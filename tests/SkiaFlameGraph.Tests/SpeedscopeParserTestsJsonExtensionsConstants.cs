using System.Text.Json;
using System.Text.Json.Serialization;

namespace SkiaFlameGraph.Tests;

internal static class SpeedscopeParserTestsJsonExtensionsConstants
{
    public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };
}
