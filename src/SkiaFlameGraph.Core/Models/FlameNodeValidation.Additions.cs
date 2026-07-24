namespace SkiaFlameGraph.Core.Models;

public static partial class FlameNodeValidation
{
	/// <summary>
	/// Validates a <see cref="FlameNode"/> instance with additional parsing warnings.
	/// This overload allows combining structural validation with warnings from event stream parsing.
	/// </summary>
	/// <param name="value">The node to validate.</param>
	/// <param name="parsingWarnings">Additional warnings from event stream parsing.</param>
	/// <returns>A list of validation problems; empty if the node is valid.</returns>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
	public static IReadOnlyList<string> Validate(this FlameNode value, IReadOnlyList<string>? parsingWarnings)
	{
		ArgumentNullException.ThrowIfNull(value);

		var problems = new List<string>();

		// Add parsing warnings first
		if (parsingWarnings is not null && parsingWarnings.Count > 0)
		{
			problems.AddRange(parsingWarnings);
		}

		// Add structural validation problems
		problems.AddRange(value.Validate());

		return problems.AsReadOnly();
	}

	/// <summary>
	/// Ensures that a <see cref="FlameNode"/> instance is valid with additional parsing warnings,
	/// throwing an <see cref="ArgumentException"/> with a detailed message if it is not.
	/// </summary>
	/// <param name="value">The node to validate.</param>
	/// <param name="parsingWarnings">Additional warnings from event stream parsing.</param>
	/// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
	/// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not valid.</exception>
	public static void EnsureValid(this FlameNode? value, IReadOnlyList<string>? parsingWarnings)
	{
		ArgumentNullException.ThrowIfNull(value);

		var problems = value.Validate(parsingWarnings);
		if (problems.Count > 0)
		{
			throw new ArgumentException(
				$"The FlameNode is not valid. Problems:\n{string.Join("\n", problems)}");
		}
	}
}