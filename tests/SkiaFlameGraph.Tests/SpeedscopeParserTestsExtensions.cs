using System;
using System.Collections.Generic;
using System.Globalization;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Parsing;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Extension methods for <see cref="SpeedscopeParserTests"/> that provide convenient assertions and helpers
/// for testing speedscope parser functionality.
/// </summary>
public static class SpeedscopeParserTestsExtensions
{
    /// <summary>
    /// Creates a sampled profile JSON string with the given parameters.
    /// </summary>
    /// <param name="frameNames">Names of frames in order</param>
    /// <param name="samples">Sample indices for each stack frame</param>
    /// <param name="weights">Weights for each sample</param>
    /// <returns>JSON string representing a sampled profile</returns>
    /// <exception cref="ArgumentNullException"><paramref name="frameNames"/> or <paramref name="samples"/> or <paramref name="weights"/> is null.</exception>
    public static string CreateSampledProfileJson(
        this SpeedscopeParserTests _,
        IReadOnlyList<string> frameNames,
        IReadOnlyList<IReadOnlyList<int>> samples,
        IReadOnlyList<int> weights)
    {
        ArgumentNullException.ThrowIfNull(frameNames);
        ArgumentNullException.ThrowIfNull(samples);
        ArgumentNullException.ThrowIfNull(weights);

        var frames = new List<string>();
        foreach (var name in frameNames)
        {
            frames.Add(string.Format(CultureInfo.InvariantCulture, "{\"name\": \"{0}\"}", name));
        }

        var samplesJson = new List<string>();
        foreach (var sample in samples)
        {
            var indices = string.Join(", ", sample);
            samplesJson.Add(string.Format(CultureInfo.InvariantCulture, "[{0}]", indices));
        }

        var weightsJson = string.Join(", ", weights);

        return "{" +
               "\"shared\": {" +
               "\"frames\": [" + string.Join(", ", frames) + "]" +
               "}," +
               "\"profiles\": [{" +
               "\"type\": \"sampled\"," +
               "\"unit\": \"milliseconds\"," +
               "\"startValue\": 0," +
               "\"endValue\": " + weights.Count.ToString(CultureInfo.InvariantCulture) + "," +
               "\"samples\": [" + string.Join(", ", samplesJson) + "]," +
               "\"weights\": [" + weightsJson + "]" +
               "}]" +
               "}";
    }

    /// <summary>
    /// Creates an evented profile JSON string with the given parameters.
    /// </summary>
    /// <param name="frameNames">Names of frames in order</param>
    /// <param name="events">List of event tuples (type, frameIndex, timestamp)</param>
    /// <returns>JSON string representing an evented profile</returns>
    /// <exception cref="ArgumentNullException"><paramref name="frameNames"/> or <paramref name="events"/> is null.</exception>
    public static string CreateEventedProfileJson(
        this SpeedscopeParserTests _,
        IReadOnlyList<string> frameNames,
        IReadOnlyList<(string Type, int FrameIndex, double Timestamp)> events)
    {
        ArgumentNullException.ThrowIfNull(frameNames);
        ArgumentNullException.ThrowIfNull(events);

        var frames = new List<string>();
        foreach (var name in frameNames)
        {
            frames.Add(string.Format(CultureInfo.InvariantCulture, "{\"name\": \"{0}\"}", name));
        }

        var eventsJson = new List<string>();
        foreach (var (type, frameIndex, timestamp) in events)
        {
            eventsJson.Add(string.Format(
                CultureInfo.InvariantCulture,
                "{\"type\": \"{0}\", \"frame\": {1}, \"at\": {2}",
                type,
                frameIndex,
                timestamp.ToString(CultureInfo.InvariantCulture)));
        }

        var lastTimestamp = events[^1].Timestamp;

        return "{" +
               "\"shared\": {" +
               "\"frames\": [" + string.Join(", ", frames) + "]" +
               "}," +
               "\"profiles\": [{" +
               "\"type\": \"evented\"," +
               "\"unit\": \"milliseconds\"," +
               "\"startValue\": 0," +
               "\"endValue\": " + lastTimestamp.ToString(CultureInfo.InvariantCulture) + "," +
               "\"events\": [" + string.Join(", ", eventsJson) + "]" +
               "}]" +
               "}";
    }

    /// <summary>
    /// Asserts that a node has exactly the expected children by name.
    /// </summary>
    /// <param name="node">The node to check</param>
    /// <param name="expectedChildNames">Expected names of children in order</param>
    /// <exception cref="ArgumentNullException"><paramref name="node"/> or <paramref name="expectedChildNames"/> is null.</exception>
    public static void ShouldHaveChildren(
        this SpeedscopeParserTests _,
        FlameNode node,
        IReadOnlyList<string> expectedChildNames)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(expectedChildNames);

        if (node.Children.Count != expectedChildNames.Count)
        {
            throw new ArgumentException(
                $"Node '{node.Name}' should have {expectedChildNames.Count} children but has {node.Children.Count}");
        }

        for (int i = 0; i < expectedChildNames.Count; i++)
        {
            var expectedName = expectedChildNames[i];
            var actualChild = node.Children[i];

            if (actualChild.Name != expectedName)
            {
                throw new ArgumentException(
                    $"Child at index {i} of '{node.Name}' should be '{expectedName}' but is '{actualChild.Name}'");
            }
        }
    }

    /// <summary>
    /// Finds a child node by name and returns it, or null if not found.
    /// </summary>
    /// <param name="node">The parent node.</param>
    /// <param name="name">Name of the child to find.</param>
    /// <returns>The child node with the matching name, or null if not found</returns>
    /// <exception cref="ArgumentNullException">Thrown if node or name is null</exception>
    public static FlameNode? FindChildByName(
        this SpeedscopeParserTests _,
        FlameNode node,
        string name)
    {
        ArgumentNullException.ThrowIfNull(node);
        ArgumentNullException.ThrowIfNull(name);

        foreach (var child in node.Children)
        {
            if (child.Name == name)
            {
                return child;
            }
        }

        return null;
    }
}
