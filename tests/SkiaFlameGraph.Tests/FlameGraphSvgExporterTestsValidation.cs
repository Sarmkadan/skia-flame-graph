using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace SkiaFlameGraph.Core.Tests.Rendering;

/// <summary>
/// Validation helpers for <see cref="FlameGraphSvgExporterTests"/>.
/// </summary>
public static class FlameGraphSvgExporterTestsValidation
{
    private const char NameSeparator = '_';
    private const int MinNameSegments = 3;

    /// <summary>
    /// Validates the <see cref="FlameGraphSvgExporterTests"/> instance and returns a list of problems.
    /// </summary>
    /// <remarks>
    /// The class holds no data of its own, so the checks cover its public <c>[Fact]</c> methods:
    /// each must be parameterless and return <c>void</c>, and its name must follow the
    /// <c>Method_Scenario_Expectation</c> convention with no empty segments.
    /// </remarks>
    /// <param name="value">The instance to validate.</param>
    /// <returns>A read-only list of problem messages, or an empty list if none.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public static IReadOnlyList<string> Validate(this FlameGraphSvgExporterTests value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(method => method.GetCustomAttribute<FactAttribute>() is not null)
            .SelectMany(MethodProblems)
            .ToList();
    }

    /// <summary>
    /// Determines whether the <see cref="FlameGraphSvgExporterTests"/> instance is valid.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <returns><see langword="true"/> if the instance is valid; otherwise, <see langword="false"/>.</returns>
    public static bool IsValid(this FlameGraphSvgExporterTests value) => value is not null && value.Validate().Count == 0;

    /// <summary>
    /// Ensures the <see cref="FlameGraphSvgExporterTests"/> instance is valid, throwing an exception listing the problems if not.
    /// </summary>
    /// <param name="value">The instance to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> has one or more validation problems.</exception>
    public static void EnsureValid(this FlameGraphSvgExporterTests value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = value.Validate();
        if (problems.Count > 0)
        {
            throw new ArgumentException($"FlameGraphSvgExporterTests is invalid: {string.Join("; ", problems)}", nameof(value));
        }
    }

    /// <summary>
    /// Yields a message for each rule a test method breaks.
    /// </summary>
    /// <param name="method">The <c>[Fact]</c> method to check.</param>
    /// <returns>Zero or more problem messages for <paramref name="method"/>.</returns>
    private static IEnumerable<string> MethodProblems(MethodInfo method)
    {
        if (method.ReturnType != typeof(void) || method.GetParameters().Length > 0)
        {
            yield return $"{method.Name}: [Fact] methods must be parameterless and return void.";
        }

        if (!HasScenarioNaming(method.Name))
        {
            yield return $"{method.Name}: name does not follow Method_Scenario_Expectation.";
        }
    }

    /// <summary>
    /// Checks that a test name has at least three underscore-separated segments and none of them is empty.
    /// </summary>
    /// <param name="name">The test method name.</param>
    /// <returns><see langword="true"/> if the name follows the convention; otherwise, <see langword="false"/>.</returns>
    private static bool HasScenarioNaming(string name) =>
        name.Split(NameSeparator) is { Length: >= MinNameSegments } segments && segments.All(segment => segment.Length > 0);
}
