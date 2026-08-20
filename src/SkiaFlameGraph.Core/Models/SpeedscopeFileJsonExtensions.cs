using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Provides JSON serialization and deserialization extensions for <see cref="SpeedscopeFile"/>.
/// </summary>
public static class SpeedscopeFileJsonExtensions
{
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
    };

    /// <summary>
    /// Serializes the <see cref="SpeedscopeFile"/> instance to a JSON string.
    /// </summary>
    /// <param name="value">The SpeedscopeFile to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <returns>A JSON string representation of the SpeedscopeFile.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static string ToJson(this SpeedscopeFile value, bool indented = false)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = indented
            ? new JsonSerializerOptions(_jsonOptions)
            {
                PropertyNamingPolicy = _jsonOptions.PropertyNamingPolicy,
                WriteIndented = true,
                TypeInfoResolver = _jsonOptions.TypeInfoResolver,
            }
            : _jsonOptions;

        return JsonSerializer.Serialize(value, options);
    }

    /// <summary>
    /// Serializes the <see cref="SpeedscopeFile"/> instance to a JSON string asynchronously.
    /// </summary>
    /// <param name="value">The SpeedscopeFile to serialize.</param>
    /// <param name="indented">Whether to format the JSON with indentation for readability.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous serialization operation. The task result contains the JSON string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static async Task<string> ToJsonAsync(this SpeedscopeFile value, bool indented = false, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(value);

        var options = indented
            ? new JsonSerializerOptions(_jsonOptions)
            {
                PropertyNamingPolicy = _jsonOptions.PropertyNamingPolicy,
                WriteIndented = true,
                TypeInfoResolver = _jsonOptions.TypeInfoResolver,
            }
            : _jsonOptions;

        await using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, value, options, cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    /// <summary>
    /// Deserializes a <see cref="SpeedscopeFile"/> instance from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <returns>The deserialized SpeedscopeFile instance, or <see langword="null"/> if the JSON represents a null value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    /// <exception cref="JsonException">Thrown when the JSON is invalid or cannot be deserialized into a <see cref="SpeedscopeFile"/> instance.</exception>
    public static SpeedscopeFile? FromJson(string json)
    {
        ArgumentNullException.ThrowIfNull(json);

        return JsonSerializer.Deserialize<SpeedscopeFile>(json, _jsonOptions);
    }

    /// <summary>
    /// Attempts to deserialize a <see cref="SpeedscopeFile"/> instance from a JSON string.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="value">Receives the deserialized SpeedscopeFile if deserialization succeeds; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if deserialization succeeded; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <see langword="null"/>.</exception>
    public static bool TryFromJson(string json, out SpeedscopeFile? value)
    {
        ArgumentNullException.ThrowIfNull(json);

        try
        {
            value = JsonSerializer.Deserialize<SpeedscopeFile>(json, _jsonOptions);
            return true;
        }
        catch (JsonException)
        {
            value = null;
            return false;
        }
    }
}