using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using SkiaFlameGraph.Core.Parsing;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Provides JSON serialization and deserialization extension methods for <see cref="SpeedscopeParserTests"/> test data.
/// </summary>
/// <remarks>
/// This class offers utility methods to convert between <see cref="SpeedscopeParserTests"/> objects and JSON strings,
/// enabling easier testing of speedscope file parsing and serialization scenarios.
/// </remarks>
public static class SpeedscopeParserTestsJsonExtensions
{
	private static readonly JsonSerializerOptions _jsonSerializerOptions = new(JsonSerializerDefaults.Web)
	{
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		WriteIndented = true,
		NumberHandling = JsonNumberHandling.AllowReadingFromString
	};

	/// <summary>
	/// Serializes a <see cref="SpeedscopeParserTests"/> instance to a JSON string.
	/// </summary>
	/// <param name="value">The instance to serialize.</param>
	/// <param name="indented">Whether to format the JSON with indentation for readability.</param>
	/// <returns>A JSON string representation of the object.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
	public static string ToJson(this SpeedscopeParserTests value, bool indented = false)
	{
		ArgumentNullException.ThrowIfNull(value);
		var options = indented
			? new JsonSerializerOptions(_jsonSerializerOptions) { WriteIndented = true }
			: _jsonSerializerOptions;
		return JsonSerializer.Serialize(value, options);
	}

	/// <summary>
	/// Deserializes a JSON string to a <see cref="SpeedscopeParserTests"/> instance.
	/// </summary>
	/// <param name="json">The JSON string to deserialize.</param>
	/// <returns>The deserialized object, or <see langword="null"/> if deserialization fails.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="json"/> is <see langword="null"/>.</exception>
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

	/// <summary>
	/// Attempts to deserialize a JSON string to a <see cref="SpeedscopeParserTests"/> instance.
	/// </summary>
	/// <param name="json">The JSON string to deserialize.</param>
	/// <param name="value">Receives the deserialized object if successful; otherwise, <see langword="null"/>.</param>
	/// <returns><see langword="true"/> if deserialization succeeds; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="json"/> is <see langword="null"/>.</exception>
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
