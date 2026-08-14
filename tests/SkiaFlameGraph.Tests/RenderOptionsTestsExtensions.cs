using System;
using System.Collections.Generic;
using System.Linq;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Extension methods that aid the <c>RenderOptionsTests</c> test suite.
/// </summary>
public static class RenderOptionsTestsExtensions
{
    /// <summary>
    /// Creates a new <see cref="RenderOptions"/> instance populated with the library's default values.
    /// </summary>
    /// <param name="tests">The test class instance (used only for null‑checking).</param>
    /// <returns>A fresh <see cref="RenderOptions"/> object.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is <c>null</c>.</exception>
    public static RenderOptions CreateDefaultOptions(this RenderOptionsTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
        return new(); // target‑typed new (RenderOptions)
    }

    /// <summary>
    /// Asserts that the supplied <paramref name="actual"/> options match the <paramref name="expected"/> options
    /// property‑by‑property using <see cref="Xunit.Assert.Equal{T}(T, T)"/>.
    /// </summary>
    /// <param name="tests">The test class instance (used only for null‑checking).</param>
    /// <param name="actual">The options instance under test.</param>
    /// <param name="expected">The options instance containing the expected values.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="tests"/>, <paramref name="actual"/> or <paramref name="expected"/> is <c>null</c>.
    /// </exception>
    public static void AssertOptionsMatch(this RenderOptionsTests tests, RenderOptions actual, RenderOptions expected)
    {
        ArgumentNullException.ThrowIfNull(tests);
        ArgumentNullException.ThrowIfNull(actual);
        ArgumentNullException.ThrowIfNull(expected);

        // Use reflection to compare all public writable properties.
        var properties = typeof(RenderOptions).GetProperties()
            .Where(p => p.CanRead && p.CanWrite);

        foreach (var prop in properties)
        {
            var actualValue = prop.GetValue(actual);
            var expectedValue = prop.GetValue(expected);
            Assert.Equal(expectedValue, actualValue);
        }
    }

    /// <summary>
    /// Returns the names of all properties of <paramref name="options"/> whose values differ from the defaults.
    /// </summary>
    /// <param name="tests">The test class instance (used only for null‑checking).</param>
    /// <param name="options">The options instance to compare against defaults.</param>
    /// <returns>An <see cref="IReadOnlyList{T}"/> of property names that are non‑default.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="tests"/> or <paramref name="options"/> is <c>null</c>.
    /// </exception>
    public static IReadOnlyList<string> GetNonDefaultPropertyNames(this RenderOptionsTests tests, RenderOptions options)
    {
        ArgumentNullException.ThrowIfNull(tests);
        ArgumentNullException.ThrowIfNull(options);

        var defaultOptions = new RenderOptions();

        var diff = typeof(RenderOptions).GetProperties()
            .Where(p => p.CanRead && p.CanWrite)
            .Where(p => !Equals(p.GetValue(options), p.GetValue(defaultOptions)))
            .Select(p => p.Name)
            .ToArray();

        return diff;
    }
}
