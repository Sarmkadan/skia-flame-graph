using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Parsing;

/// <summary>
/// Extension methods for SpeedscopeParser that provide enhanced functionality
/// for handling unbalanced event streams and validation warnings.
/// </summary>
public static class SpeedscopeParserExtensions
{
	/// <summary>
	/// Aggregate one profile into a call tree, detecting and reporting unbalanced events.
	/// Returns a tuple containing the root node and a list of warning messages.
	/// </summary>
	public static (FlameNode root, IReadOnlyList<string> warnings) BuildTreeWithWarnings(this SpeedscopeFile file, int profileIndex = 0)
	{
		return SpeedscopeParser.BuildTreeWithWarnings(file, profileIndex);
	}
}