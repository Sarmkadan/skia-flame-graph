using System.Text.Json;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Reporting;

/// <summary>
/// Provides System.Text.Json serialization and deserialization extensions for <see cref="IHotFunctionsReport"/>.
/// </summary>
public static class HotFunctionsReportExtensions
{
    /// <summary>
    /// Serializes the <see cref="IHotFunctionsReport"/> instance to a JSON string.
    /// </summary>
    /// <param name="report">The report to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON string representation of the report.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="report"/> is <see langword="null"/>.</exception>
    public static string ToJson(this IHotFunctionsReport report, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(report);

        var options = new JsonSerializerOptions(JsonDefaults.Options)
        {
            WriteIndented = indented,
        };

        return JsonSerializer.Serialize(report, options);
    }

    /// <summary>
    /// Deserializes an <see cref="IHotFunctionsReport"/> instance from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized report, or <see langword="null"/> if the JSON is empty or whitespace.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid or cannot be deserialized.</exception>
    public static IHotFunctionsReport? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        return string.IsNullOrWhiteSpace(json)
            ? null
            : JsonSerializer.Deserialize<HotFunctionsReport>(json, HotFunctionsReportJsonExtensionsConstants.JsonOptions);
    }

    /// <summary>
    /// Attempts to deserialize an <see cref="IHotFunctionsReport"/> instance from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="report">Receives the deserialized report if successful.</param>
    /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    public static bool TryFromJson(string json, out IHotFunctionsReport? report)
    {
        ArgumentNullException.ThrowIfNull(json);

        if (string.IsNullOrWhiteSpace(json))
        {
            report = null;
            return true;
        }

        try
        {
            report = JsonSerializer.Deserialize<HotFunctionsReport>(json, HotFunctionsReportJsonExtensionsConstants.JsonOptions);
            return true;
        }
        catch (JsonException)
        {
            report = null;
            return false;
        }
    }
}