using System.Text.Json;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Parsing;

/// <summary>
/// Parses a speedscope JSON document into an aggregated <see cref="FlameNode"/> tree ready for rendering.
/// Supports both evented and sampled profiles.
/// </summary>
public static partial class SpeedscopeParser
{
    /// <summary>
    /// Summarizes non-fatal conditions encountered while parsing a profile.
    /// </summary>
    public sealed class ParseDiagnostics
    {
        private readonly List<string> _warnings = new();

        public int ProfilesParsed { get; internal set; }
        public int EventsProcessed { get; internal set; }
        public int UnbalancedCloseEventsSkipped { get; internal set; }
        public int FramesReferencedOutOfRange { get; internal set; }
        public IReadOnlyList<string> Warnings => _warnings;

        internal void Warn(string message, Action<string>? log)
        {
            _warnings.Add(message);
            log?.Invoke(message);
        }
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>
    /// Deserializes a JSON string into a <see cref="SpeedscopeFile"/> instance.
    /// </summary>
    /// <param name="json">The JSON document to deserialize.</param>
    /// <returns>The deserialized <see cref="SpeedscopeFile"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="json"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="json"/> is an empty string.</exception>
    /// <exception cref="FormatException">Thrown when the document deserializes to null or contains no profiles.</exception>
    public static SpeedscopeFile Deserialize(string json)
    {
        ArgumentNullException.ThrowIfNull(json);
        ArgumentException.ThrowIfNullOrEmpty(json);

        var file = JsonSerializer.Deserialize<SpeedscopeFile>(json, Options)
                   ?? throw new FormatException("speedscope document deserialized to null");
        if (file.Profiles.Count == 0)
            throw new FormatException("speedscope document contains no profiles");
        return file;
    }

    /// <summary>
    /// Parses a speedscope file from disk and builds a <see cref="FlameNode"/> tree.
    /// </summary>
    /// <param name="path">The path to the speedscope JSON file.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The root <see cref="FlameNode"/> of the aggregated call tree.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="path"/> is null.</exception>
    /// <exception cref="FormatException">Thrown when the document deserializes to null or contains no profiles.</exception>
    public static FlameNode ParseFile(string path, CancellationToken cancellationToken = default)
        => ParseFile(path, out _, cancellationToken, log: null);

    /// <summary>
    /// Parses a speedscope file from disk, returning diagnostics for non-fatal anomalies.
    /// </summary>
    public static FlameNode ParseFile(
        string path,
        out ParseDiagnostics diagnostics,
        CancellationToken cancellationToken = default,
        Action<string>? log = null)
    {
        ArgumentNullException.ThrowIfNull(path);

        using var stream = File.OpenRead(path);
        var file = JsonSerializer.Deserialize<SpeedscopeFile>(stream, Options)
                   ?? throw new FormatException("speedscope document deserialized to null");
        if (file.Profiles.Count == 0)
            throw new FormatException("speedscope document contains no profiles");
        return BuildTree(file, out diagnostics, 0, cancellationToken, log);
    }

    /// <summary>
    /// Aggregates one profile into a call tree. The returned node is a synthetic
    /// "root" whose value equals the sum of all top‑level samples.
    /// </summary>
    /// <param name="file">The speedscope file containing the profiles.</param>
    /// <param name="profileIndex">The zero‑based index of the profile to aggregate.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>The root <see cref="FlameNode"/> of the aggregated tree.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="file"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="profileIndex"/> is outside the valid range.</exception>
    /// <exception cref="FormatException">Thrown when the profile type is unsupported.</exception>
    public static FlameNode BuildTree(SpeedscopeFile file, int profileIndex = 0, CancellationToken cancellationToken = default)
        => BuildTree(file, out _, profileIndex, cancellationToken, log: null);

    /// <summary>
    /// Aggregates one profile into a call tree and returns diagnostics for non-fatal anomalies.
    /// </summary>
    public static FlameNode BuildTree(
        SpeedscopeFile file,
        out ParseDiagnostics diagnostics,
        int profileIndex = 0,
        CancellationToken cancellationToken = default,
        Action<string>? log = null)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (profileIndex < 0 || profileIndex >= file.Profiles.Count) throw new ArgumentException("profileIndex out of range");

        diagnostics = new ParseDiagnostics { ProfilesParsed = 1 };
        var profile = file.Profiles[profileIndex];
        var frames = file.Shared.Frames;

        return profile.Type switch
        {
            "evented" => BuildFromEvents(profile, frames, diagnostics, log),
            "sampled" => BuildFromSamples(profile, frames, diagnostics, log),
            _ => throw new FormatException($"unsupported profile type '{profile.Type}'"),
        };
    }

    private static FlameNode BuildFromEvents(
        Profile profile,
        List<Frame> frames,
        ParseDiagnostics diagnostics,
        Action<string>? log)
    {
        var root = new FlameNode(profile.Name ?? "root");
        var stack = new List<(FlameNode node, double openedAt)>();
        var current = root;
        double lastAt = profile.StartValue;

        var events = profile.Events ?? new List<ProfileEvent>();
        for (var i = 0; i < events.Count; i++)
        {
            var ev = events[i];
            diagnostics.EventsProcessed++;

            // Attribute the elapsed slice to whatever frame was on top.
            var delta = ev.At - lastAt;
            if (delta > 0)
                AddValueToPath(current, delta);
            lastAt = ev.At;

            if (ev.Type == "O")
            {
                var frame = FrameAt(frames, ev.Frame, diagnostics, log);
                current = current.AddChild(frame.Name, frame.File, frame.Line);
                stack.Add((current, ev.At));
            }
            else if (ev.Type == "C")
            {
                if (stack.Count > 0)
                    stack.RemoveAt(stack.Count - 1);
                else
                {
                    diagnostics.UnbalancedCloseEventsSkipped++;
                    diagnostics.Warn($"speedscope: skipped unbalanced close event at index {i}", log);
                }
                current = stack.Count > 0 ? stack[^1].node : root;
            }
        }

        return root;
    }

    private static FlameNode BuildFromSamples(
        Profile profile,
        List<Frame> frames,
        ParseDiagnostics diagnostics,
        Action<string>? log)
    {
        var root = new FlameNode(profile.Name ?? "root");
        var samples = profile.Samples ?? new List<List<int>>();
        var weights = profile.Weights;

        for (var i = 0; i < samples.Count; i++)
        {
            var stack = samples[i];
            var weight = weights is not null && i < weights.Count ? weights[i] : 1.0;

            var node = root;
            node.Value += weight;
            foreach (var frameIndex in stack)
            {
                var frame = FrameAt(frames, frameIndex, diagnostics, log);
                node = node.AddChild(frame.Name, frame.File, frame.Line);
                node.Value += weight;
            }
        }

        return root;
    }

    private static FlameNode BuildFromSamples(Profile profile, List<Frame> frames)
        => BuildFromSamples(profile, frames, new ParseDiagnostics(), log: null);

    /// <summary>
    /// Adds <paramref name="value"/> to a node and every ancestor.
    /// </summary>
    /// <param name="leaf">The leaf node to start from.</param>
    /// <param name="value">The value to add.</param>
    private static void AddValueToPath(FlameNode leaf, double value)
    {
        var n = leaf;
        while (n is not null)
        {
            n.Value += value;
            n = n.Parent;
        }
    }

    /// <summary>
    /// Retrieves a <see cref="Frame"/> from <paramref name="frames"/> at the given <paramref name="index"/>.
    /// If the index is out of range, a placeholder frame with a descriptive name is returned.
    /// </summary>
    /// <param name="frames">The list of frames.</param>
    /// <param name="index">The zero‑based index of the frame.</param>
    /// <returns>The resolved <see cref="Frame"/> or a placeholder for an invalid index.</returns>
    private static Frame FrameAt(
        List<Frame> frames,
        int index,
        ParseDiagnostics diagnostics,
        Action<string>? log)
    {
        if (index < 0 || index >= frames.Count)
        {
            diagnostics.FramesReferencedOutOfRange++;
            diagnostics.Warn($"speedscope: frame index {index} out of range", log);
            return new Frame { Name = $"<frame {index}>" };
        }
        return frames[index];
    }

    private static Frame FrameAt(List<Frame> frames, int index)
        => FrameAt(frames, index, new ParseDiagnostics(), log: null);
}
