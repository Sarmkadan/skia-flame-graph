using System;
using System.Collections.Generic;
using SkiaSharp;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Tests for <see cref="FramePalette"/> ensuring deterministic colour assignment
/// and reasonable collision behaviour.
/// </summary>
public sealed class FramePaletteTests : IFramePaletteTests
{
    /// <summary>
    /// Verifies that <see cref="FramePalette.ForFrame(string)"/> returns the same color for the same frame name.
    /// </summary>
    [Fact]
    public void ForFrame_ReturnsSameColor_ForSameName()
    {
        const string name = "MyFunction";

        var first = FramePalette.ForFrame(name);
        var second = FramePalette.ForFrame(name);

        Assert.Equal(first, second);
    }

    /// <summary>
    /// Verifies that <see cref="FramePalette.ForFrame(string, string)"/> returns the highlight color when the frame name matches the highlight pattern.
    /// </summary>
    [Fact]
    public void ForFrame_WithHighlightPattern_ReturnsHighlightColor_WhenMatched()
    {
        const string name = FramePaletteTestsConstants.HighlightedFunctionName;
        const string pattern = FramePaletteTestsConstants.HighlightPattern;

        var colour = FramePalette.ForFrame(name, pattern);

        Assert.Equal(FramePalette.HighlightColor, colour);
    }

    /// <summary>
    /// Verifies that <see cref="FramePalette.ForFrame(string)"/> and <see cref="FramePalette.ForFrame(string, string)"/> throw <see cref="ArgumentException"/> when the frame name is null or empty.
    /// </summary>
    [Fact]
    public void ForFrame_ThrowsArgumentException_WhenNameIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => FramePalette.ForFrame(null!));
        Assert.Throws<ArgumentException>(() => FramePalette.ForFrame(string.Empty));

        Assert.Throws<ArgumentException>(() => FramePalette.ForFrame(null!, "pattern"));
        Assert.Throws<ArgumentException>(() => FramePalette.ForFrame(string.Empty, "pattern"));
    }

    /// <summary>
        /// Verifies that <see cref="FramePalette.ForFrame(string)"/> distributes colors across many distinct function names with a high uniqueness ratio (at least 90%).
        /// </summary>
    [Fact]
    public void ForFrame_DistributesColorsAcrossManyDistinctNames()
    {
        const int sampleSize = FramePaletteTestsConstants.SampleSize;
        var colors = new HashSet<SKColor>();
        var random = new Random(0);

        for (int i = 0; i < sampleSize; i++)
        {
            // Generate a pseudo‑random but deterministic name.
            var name = $"Func_{Guid.NewGuid():N}_{random.Next()}";
            colors.Add(FramePalette.ForFrame(name));
        }

        // Expect at least 90 % uniqueness – collisions are possible but should be rare.
        var uniquenessRatio = (double)colors.Count / sampleSize;
        Assert.True(uniquenessRatio > FramePaletteTestsConstants.ExpectedUniquenessRatio, $"Uniqueness ratio was {uniquenessRatio:P2}");
    }
}
