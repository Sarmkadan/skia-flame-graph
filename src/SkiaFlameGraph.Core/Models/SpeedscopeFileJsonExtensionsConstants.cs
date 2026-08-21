using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Provides constants for magic values in SpeedscopeFileJsonExtensions.
/// </summary>
public static class SpeedscopeFileJsonExtensionsConstants
{
    /// <summary>
    /// The naming policy used for JSON serialization and deserialization.
    /// </summary>
    public static readonly JsonNamingPolicy NamingPolicy = JsonNamingPolicy.CamelCase;
}