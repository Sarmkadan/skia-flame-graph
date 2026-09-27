using System;
using System.Text.Json;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering
{
    /// <summary>
    /// JSON serialization extensions for <see cref="FramePalette"/>.
    /// </summary>
    public static class FramePaletteJsonExtensions
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IgnoreReadOnlyProperties = false
        };

        /// <summary>
        /// Converts the <see cref="FramePalette"/> instance to a JSON string.
        /// </summary>
        /// <param name="value">The <see cref="FramePalette"/> instance to convert.</param>
        /// <param name="indented">Whether to format the JSON with indentation.</param>
        /// <returns>A JSON string representing the <see cref="FramePalette"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
        public static string ToJson(this FramePalette value, bool indented = false)
        {
            ArgumentNullException.ThrowIfNull(value);
            var options = indented ? new JsonSerializerOptions(_jsonOptions) { WriteIndented = true } : _jsonOptions;
            return JsonSerializer.Serialize(value, options);
        }

        /// <summary>
        /// Converts a JSON string to a <see cref="FramePalette"/> instance.
        /// </summary>
        /// <param name="json">The JSON string to convert.</param>
        /// <returns>A <see cref="FramePalette"/> instance, or <see langword="null"/> if the JSON is invalid.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is <see langword="null"/> or empty.</exception>
        public static FramePalette? FromJson(string json)
        {
            ArgumentException.ThrowIfNullOrEmpty(json);
            try
            {
                return JsonSerializer.Deserialize<FramePalette>(json, _jsonOptions);
            }
            catch (JsonException)
            {
                return null;
            }
        }

        /// <summary>
        /// Attempts to convert a JSON string to a <see cref="FramePalette"/> instance.
        /// </summary>
        /// <param name="json">The JSON string to convert.</param>
        /// <param name="value">When this method returns, contains the <see cref="FramePalette"/> instance if the conversion succeeded, or <see langword="null"/> if it failed.</param>
        /// <returns><see langword="true"/> if <paramref name="json"/> was successfully converted; otherwise, <see langword="false"/>.</returns>
        public static bool TryFromJson(string json, out FramePalette? value)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    throw new ArgumentException(null, nameof(json));
                }

                value = JsonSerializer.Deserialize<FramePalette>(json, _jsonOptions);
                return value is not null;
            }
            catch (ArgumentException)
            {
                value = null;
                return false;
            }
            catch (JsonException)
            {
                value = null;
                return false;
            }
        }
    }
}