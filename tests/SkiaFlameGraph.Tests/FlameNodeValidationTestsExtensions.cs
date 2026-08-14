using System;
using System.Collections.Generic;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Provides extension methods for <see cref="FlameNodeValidationTests"/> to facilitate testing workflows.
/// </summary>
public static class FlameNodeValidationTestsExtensions
{
    /// <summary>
    /// Ensures that the provided test instance is not null.
    /// </summary>
    /// <param name="tests">The test instance.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="tests"/> is null.</exception>
    public static void EnsureInstanceNotNull(this FlameNodeValidationTests tests)
    {
        ArgumentNullException.ThrowIfNull(tests);
    }

    /// <summary>
    /// Executes a test action and ensures that no unexpected exceptions are thrown.
    /// </summary>
    /// <param name="tests">The test instance.</param>
    /// <param name="testAction">The action representing the test.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="testAction"/> is null.</exception>
    public static void RunTestWithoutException(this FlameNodeValidationTests tests, Action testAction)
    {
        ArgumentNullException.ThrowIfNull(tests);
        ArgumentNullException.ThrowIfNull(testAction);

        testAction();
    }
}
