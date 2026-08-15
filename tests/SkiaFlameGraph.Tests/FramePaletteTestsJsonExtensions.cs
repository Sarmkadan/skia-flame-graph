using System;
using System.Text.Json;

namespace SkiaFlameGraph.Tests
{
    /// <summary>
    /// JSON serialization helpers for <see cref="FramePaletteTests"/>.
    /// </summary>
    public static class FramePaletteTestsJsonExtensions
    {
        private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Serializes the <see cref="FramePaletteTests"/> instance to a JSON string.
        /// </summary>
        /// <param name="value">The instance to serialize.</param>
        /// <param name="indented">If <c>true</c>, the output JSON will be formatted with indentation.</param>
        /// <returns>A JSON representation of <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public static string ToJson(this FramePaletteTests value, bool indented = false)
        {
            ArgumentNullException.ThrowIfNull(value);
            var options = indented ? new JsonSerializerOptions(_options) { WriteIndented = true } : _options;
            return JsonSerializer.Serialize(value, options);
        }

        /// <summary>
        /// Deserializes a JSON string into a <see cref="FramePaletteTests"/> instance.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <returns>The deserialized <see cref="FramePaletteTests"/> instance, or <c>null</c> if the JSON represents a null value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <c>null</c>.</exception>
        public static FramePaletteTests? FromJson(string json)
        {
            ArgumentNullException.ThrowIfNull(json);
            return JsonSerializer.Deserialize<FramePaletteTests>(json, _options);
        }

        /// <summary>
        /// Attempts to deserialize a JSON string into a <see cref="FramePaletteTests"/> instance.
        /// </summary>
        /// <param name="json">The JSON string to deserialize.</param>
        /// <param name="value">
        /// When this method returns, contains the deserialized <see cref="FramePaletteTests"/> instance if the operation succeeded;
        /// otherwise, <c>null</c>.
        /// </param>
        /// <returns><c>true</c> if deserialization succeeded; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is <c>null</c>.</exception>
        public static bool TryFromJson(string json, out FramePaletteTests? value)
        {
            ArgumentNullException.ThrowIfNull(json);
            try
            {
                value = JsonSerializer.Deserialize<FramePaletteTests>(json, _options);
                return true;
            }
            catch (JsonException)
            {
                value = null;
                return false;
            }
        }
    }
}
