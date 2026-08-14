namespace SkiaFlameGraph.Core;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

/// <summary>
/// Provides default <see cref="JsonSerializerOptions"/> for the application.
/// </summary>
public static class JsonDefaults
{
    /// <summary>
    /// Gets the shared default <see cref="JsonSerializerOptions"/>.
    /// </summary>
    public static JsonSerializerOptions Options { get; } = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };
}
