using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Provides extension methods for <see cref="SpeedscopeFileJsonExtensionsTests"/> to assist with JSON validation and inspection.
/// </summary>
public static class SpeedscopeFileJsonExtensionsTestsExtensions
{
    /// <summary>
    /// Determines whether the specified string is valid JSON.
    /// </summary>
    /// <param name="_">The test class instance (unused).</param>
    /// <param name="json">The JSON string to validate.</param>
    /// <returns><c>true</c> if the JSON is valid; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="json"/> is null or empty.</exception>
    public static bool IsJsonValid(this SpeedscopeFileJsonExtensionsTests _, string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);
        try
        {
            JsonDocument.Parse(json);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieves the names of all properties at the root level of the JSON object.
    /// </summary>
    /// <param name="_">The test class instance (unused).</param>
    /// <param name="json">The JSON string to inspect.</param>
    /// <returns>A read-only list of property names.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="json"/> is null or empty.</exception>
    /// <exception cref="JsonException">Thrown if <paramref name="json"/> is not valid JSON or the root is not an object.</exception>
    public static IReadOnlyList<string> GetJsonPropertyNames(this SpeedscopeFileJsonExtensionsTests _, string json)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.EnumerateObject().Select(p => p.Name).ToList().AsReadOnly();
    }

    /// <summary>
    /// Retrieves the value of a specific property from the root level of the JSON object as a string.
    /// </summary>
    /// <param name="_">The test class instance (unused).</param>
    /// <param name="json">The JSON string to inspect.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns>The string value of the property, or <c>null</c> if the property does not exist or is null.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="json"/> or <paramref name="propertyName"/> is null or empty.</exception>
    /// <exception cref="JsonException">Thrown if <paramref name="json"/> is not valid JSON or the root is not an object.</exception>
    public static string? GetJsonPropertyValue(this SpeedscopeFileJsonExtensionsTests _, string json, string propertyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(json);
        ArgumentException.ThrowIfNullOrEmpty(propertyName);

        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.TryGetProperty(propertyName, out var element))
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString(),
                JsonValueKind.Null => null,
                _ => element.ToString()
            };
        }

        return null;
    }
}
