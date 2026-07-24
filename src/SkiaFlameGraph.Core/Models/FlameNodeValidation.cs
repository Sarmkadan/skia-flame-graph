namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Provides validation helpers for <see cref="FlameNode"/> instances.
/// </summary>
public static partial class FlameNodeValidation
{
    /// <summary>
    /// Validates a <see cref="FlameNode"/> instance and returns a list of human-readable problems.
    /// </summary>
    /// <param name="value">The node to validate.</param>
    /// <returns>A list of validation problems; empty if the node is valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static IReadOnlyList<string> Validate(this FlameNode value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // Validate Name
        if (string.IsNullOrWhiteSpace(value.Name))
        {
            problems.Add("Name cannot be null or whitespace.");
        }

        // Validate Value (should be non-negative)
        if (value.Value < 0)
        {
            problems.Add("Value cannot be negative.");
        }

        // Validate Depth (should be non-negative)
        if (value.Depth < 0)
        {
            problems.Add("Depth cannot be negative.");
        }

        // Validate Children
        if (value.Children is null)
        {
            problems.Add("Children collection cannot be null.");
        }
        else
        {
            // Check for null children
            foreach (var child in value.Children)
            {
                if (child is null)
                {
                    problems.Add("Children collection contains a null element.");
                    break;
                }
            }
        }

        // Validate Line if present (should be positive)
        if (value.Line is int line && line <= 0)
        {
            problems.Add("Line, if specified, must be a positive integer.");
        }

        // Validate File if present (should not be empty/whitespace when present)
        if (!string.IsNullOrWhiteSpace(value.File) && string.IsNullOrWhiteSpace(value.File.Trim()))
        {
            problems.Add("File, if specified, cannot be empty or whitespace.");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether a <see cref="FlameNode"/> instance is valid.
    /// </summary>
    /// <param name="value">The node to check.</param>
    /// <returns>True if the node is valid; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    public static bool IsValid(this FlameNode? value) => value?.Validate().Count == 0;

    /// <summary>
    /// Ensures that a <see cref="FlameNode"/> instance is valid, throwing an <see cref="ArgumentException"/>
    /// with a detailed message if it is not.
    /// </summary>
    /// <param name="value">The node to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not valid.</exception>
    public static void EnsureValid(this FlameNode? value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = value.Validate();
        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"The FlameNode is not valid. Problems:\n{string.Join("\n", problems)}");
        }
    }
}
