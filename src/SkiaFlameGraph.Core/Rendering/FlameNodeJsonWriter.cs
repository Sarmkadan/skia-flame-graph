using System;
using System.Collections.Generic;
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
/// The tree is walked with an explicit stack, so depth is limited by heap memory rather than the call stack.
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
        ArgumentNullException.ThrowIfNull(root);

        var options = new JsonWriterOptions { Indented = indented, MaxDepth = int.MaxValue };
        using var stream = new MemoryStream();
        using (var writer = new Utf8JsonWriter(stream, options))
        {
            WriteTree(writer, root);
        }

        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// Writes the JSON representation of <paramref name="root"/> and all of its descendants to the
    /// supplied <paramref name="writer"/>, using an explicit stack of (node, next child index) pairs
    /// instead of recursion.
    /// </summary>
    private static void WriteTree(Utf8JsonWriter writer, FlameNode root)
    {
        var pending = new Stack<(FlameNode Node, int NextChild)>();
        WriteOpen(writer, root);
        pending.Push((root, 0));

        while (pending.Count > 0)
        {
            var (node, nextChild) = pending.Pop();
            if (nextChild < node.Children.Count)
            {
                pending.Push((node, nextChild + 1));
                var child = node.Children[nextChild];
                WriteOpen(writer, child);
                pending.Push((child, 0));
                continue;
            }

            if (node.Children.Count > 0)
                writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }

    /// <summary>
    /// Writes the opening of a node object: its name, its value and, when it has children, the start of the <c>children</c> array.
    /// </summary>
    private static void WriteOpen(Utf8JsonWriter writer, FlameNode node)
    {
        writer.WriteStartObject();

        writer.WriteString(FlameNodeJsonWriterConstants.NameKey, node.Name);
        writer.WriteNumber(FlameNodeJsonWriterConstants.ValueKey, node.Value);

        if (node.Children.Count > 0)
        {
            writer.WritePropertyName("children");
            writer.WriteStartArray();
        }
    }
}
