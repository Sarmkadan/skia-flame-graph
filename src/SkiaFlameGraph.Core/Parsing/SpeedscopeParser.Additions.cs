using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Parsing;

public static partial class SpeedscopeParser
{
	/// <summary>
	/// Aggregate one profile into a call tree, detecting and reporting unbalanced events.
	/// Returns a tuple containing the root node and a list of warning messages.
	/// </summary>
	public static (FlameNode root, IReadOnlyList<string> warnings) BuildTreeWithWarnings(SpeedscopeFile file, int profileIndex = 0)
	{
		if (profileIndex < 0 || profileIndex >= file.Profiles.Count)
			throw new ArgumentOutOfRangeException(nameof(profileIndex));

		var profile = file.Profiles[profileIndex];
		var frames = file.Shared.Frames;

		return profile.Type switch
		{
			"evented" => BuildFromEventsWithWarnings(profile, frames),
			"sampled" => (BuildFromSamples(profile, frames), Array.Empty<string>()),
			_ => throw new FormatException($"unsupported profile type '{profile.Type}'"),
		};
	}

	private static (FlameNode root, IReadOnlyList<string> warnings) BuildFromEventsWithWarnings(Profile profile, List<Frame> frames)
	{
		var root = new FlameNode(profile.Name ?? "root");
		var stack = new List<(FlameNode node, double openedAt)>();
		var current = root;
		double lastAt = profile.StartValue;
		var warnings = new List<string>();

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
				{
					stack.RemoveAt(stack.Count - 1);
					current = stack.Count > 0 ? stack[^1].node : root;
				}
				else
				{
					// Orphan close event - no matching open
					warnings.Add($"Orphan close event at time {ev.At} for frame index {ev.Frame} with no matching open event");
				}
			}
		}

		// Auto-close any remaining open frames (dangling opens at profile end)
		if (stack.Count > 0)
		{
			var autoCloseCount = stack.Count;
			warnings.Add($"Profile ended with {autoCloseCount} unclosed frame(s). Auto-closing remaining frames.");

			// Attribute remaining time to the open frames before closing them
			var remainingTime = profile.EndValue - lastAt;
			if (remainingTime > 0)
			{
				AddValueToPath(current, remainingTime);
			}

			// Close all remaining frames in reverse order
			for (var i = stack.Count - 1; i >= 0; i--)
			{
				stack.RemoveAt(i);
			}
			current = root;
		}

		return (root, warnings.AsReadOnly());
	}
}