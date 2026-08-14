using System;
using System.Collections.Generic;
using System.Linq;

namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Provides validation helpers for <see cref="FlameNode"/> instances.
/// </summary>
public static partial class FlameNodeValidation
{
    /// <summary>
    /// Validates a <see cref="FlameNode"/> instance and returns a list of human‑readable problems.
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

        // Validate Value (must be positive)
        if (value.Value <= 0)
        {
            problems.Add("Value must be positive.");
        }

        // Validate Depth (should be non‑negative)
        if (value.Depth < 0)
        {
            problems.Add("Depth cannot be negative.");
        }

        // Validate Children collection
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

        // Validate child weight invariant: sum of children <= parent value
        var path = string.Join(" → ", value.GetPathToNode());
        var childWeightProblem = value.ValidateChildWeightInvariant(path);
        if (childWeightProblem is not null)
        {
            problems.Add(childWeightProblem);
        }

        // Detect cycles in the node graph
        if (HasCycle(value, new HashSet<FlameNode>()))
        {
            problems.Add("FlameNode graph contains a cycle.");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Validates that the sum of children values does not exceed the parent value.
    /// This ensures the flame graph invariant is maintained.
    /// </summary>
    /// <param name="value">The node to validate.</param>
    /// <param name="path">The path to the node for error reporting.</param>
    /// <returns>A validation problem if the invariant is violated; otherwise, <c>null</c>.</returns>
    private static string? ValidateChildWeightInvariant(this FlameNode value, string path)
    {
        if (value.Children.Count == 0 || value.Value <= 0)
        {
            return null;
        }

        // Skip null children in the sum calculation
        var childSum = value.Children.Where(c => c is not null).Sum(c => c.Value);
        if (childSum > value.Value)
        {
            var overage = childSum - value.Value;
            return $"Child weight invariant violated at {path}: sum of children ({childSum}) exceeds parent value ({value.Value}) by {overage}.";
        }

        return null;
    }

    /// <summary>
    /// Detects a cycle in the <see cref="FlameNode"/> graph.
    /// </summary>
    /// <param name="node">The node to start the detection from.</param>
    /// <param name="visited">A set of nodes visited on the current path.</param>
    /// <returns><c>true</c> if a cycle is found; otherwise, <c>false</c>.</returns>
    private static bool HasCycle(FlameNode node, HashSet<FlameNode> visited)
    {
        if (!visited.Add(node))
        {
            // Node already on the current path → cycle
            return true;
        }

        if (node.Children is not null)
        {
            foreach (var child in node.Children)
            {
                if (child is not null && HasCycle(child, visited))
                {
                    return true;
                }
            }
        }

        // Remove node when backtracking
        visited.Remove(node);
        return false;
    }

    /// <summary>
    /// Determines whether a <see cref="FlameNode"/> instance is valid.
    /// </summary>
    /// <param name="value">The node to check.</param>
    /// <returns><c>true</c> if the node is valid; otherwise, <c>false</c>.</returns>
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
