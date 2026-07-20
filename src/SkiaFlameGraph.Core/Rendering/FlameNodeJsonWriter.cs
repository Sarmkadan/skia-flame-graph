using System;
using System.IO;
using System.Text;
using System.Text.Json;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Rendering;

/// <summary>
/// Writes a <see cref="FlameNode"/> tree to JSON in the format expected by
/// d3‑flame‑graph (objects with <c>name</c>, <c>value</c> and optional <c>children</c>).
/// The implementation uses <see cref="Utf8JsonWriter"/> directly to avoid any
/// reflection‑based serializers and to keep the output deterministic.
/// </summary>
public static class FlameNodeJsonWriter
{
    /// <summary>
    /// Serialises the supplied <paramref name="root"/> node to a JSON string.
    /// </summary>
    /// <param name="root">The flame‑graph root node. Must not be <c>null</c>.</param>
    /// <param name="indented">If <c>true</c>, the output will be pretty‑printed.</param>
    /// <returns>A JSON string representing the flame‑graph.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="root"/> is <c>null</c>.</exception>
    public static string ToJson(FlameNode root, bool indented = false)
    {
        if (root == null) throw new ArgumentNullException(nameof(root));

        var options = new JsonWriterOptions { Indented = indented };
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, options))
        {
            WriteNode(writer, root);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Writes the JSON representation of <paramref name="node"/> (and its children)
    /// to the supplied <paramref name="writer"/>.
    /// </summary>
    private static void WriteNode(Utf8JsonWriter writer, FlameNode node)
    {
        writer.WriteStartObject();

        writer.WriteString("name", node.Name);
        writer.WriteNumber("value", node.Value);

        if (node.Children != null && node.Children.Count > 0)
        {
            writer.WritePropertyName("children");
            writer.WriteStartArray();

            foreach (var child in node.Children)
            {
                WriteNode(writer, child);
            }

            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }
}
