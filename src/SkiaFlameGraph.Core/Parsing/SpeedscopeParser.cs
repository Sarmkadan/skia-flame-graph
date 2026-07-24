using System.Text.Json;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Parsing;

/// <summary>
/// Turns a speedscope JSON document into an aggregated <see cref="FlameNode"/>
/// tree ready for rendering. Both evented and sampled profiles are supported.
/// </summary>
public static partial class SpeedscopeParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static SpeedscopeFile Deserialize(string json)
    {
        var file = JsonSerializer.Deserialize<SpeedscopeFile>(json, Options)
                   ?? throw new FormatException("speedscope document deserialized to null");
        if (file.Profiles.Count == 0)
            throw new FormatException("speedscope document contains no profiles");
        return file;
    }

    public static FlameNode ParseFile(string path)
    {
        using var stream = File.OpenRead(path);
        var file = JsonSerializer.Deserialize<SpeedscopeFile>(stream, Options)
                   ?? throw new FormatException("speedscope document deserialized to null");
        if (file.Profiles.Count == 0)
            throw new FormatException("speedscope document contains no profiles");
        return BuildTree(file, 0);
    }

    /// <summary>
    /// Aggregate one profile into a call tree. The returned node is a synthetic
    /// "root" whose value equals the sum of all top-level samples.
    /// </summary>
    public static FlameNode BuildTree(SpeedscopeFile file, int profileIndex = 0)
    {
        if (profileIndex < 0 || profileIndex >= file.Profiles.Count)
            throw new ArgumentOutOfRangeException(nameof(profileIndex));

        var profile = file.Profiles[profileIndex];
        var frames = file.Shared.Frames;

        return profile.Type switch
        {
            "evented" => BuildFromEvents(profile, frames),
            "sampled" => BuildFromSamples(profile, frames),
            _ => throw new FormatException($"unsupported profile type '{profile.Type}'"),
        };
    }

    private static FlameNode BuildFromEvents(Profile profile, List<Frame> frames)
    {
        var root = new FlameNode(profile.Name ?? "root");
        var stack = new List<(FlameNode node, double openedAt)>();
        var current = root;
        double lastAt = profile.StartValue;

        var events = profile.Events ?? new List<ProfileEvent>();
        foreach (var ev in events)
        {
            // Attribute the elapsed slice to whatever frame was on top.
            var delta = ev.At - lastAt;
            if (delta > 0)
                AddValueToPath(current, delta);
            lastAt = ev.At;

            if (ev.Type == "O")
            {
                var frame = FrameAt(frames, ev.Frame);
                current = current.AddChild(frame.Name, frame.File, frame.Line);
                stack.Add((current, ev.At));
            }
            else if (ev.Type == "C")
            {
                if (stack.Count > 0)
                    stack.RemoveAt(stack.Count - 1);
                current = stack.Count > 0 ? stack[^1].node : root;
            }
        }

        return root;
    }

    private static FlameNode BuildFromSamples(Profile profile, List<Frame> frames)
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
                var frame = FrameAt(frames, frameIndex);
                node = node.AddChild(frame.Name, frame.File, frame.Line);
                node.Value += weight;
            }
        }

        return root;
    }

    /// <summary>Add <paramref name="value"/> to a node and every ancestor.</summary>
    private static void AddValueToPath(FlameNode leaf, double value)
    {
        var n = leaf;
        while (n is not null)
        {
            n.Value += value;
            n = n.Parent;
        }
    }

    private static Frame FrameAt(List<Frame> frames, int index)
    {
        if (index < 0 || index >= frames.Count)
            return new Frame { Name = $"<frame {index}>" };
        return frames[index];
    }
}
