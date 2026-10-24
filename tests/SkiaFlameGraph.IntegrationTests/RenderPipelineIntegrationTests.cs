using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;
using SkiaSharp;
using Xunit;
using static SkiaFlameGraph.IntegrationTests.RenderPipelineIntegrationTestsConstants;
using static SkiaFlameGraph.IntegrationTests.RenderPipelineIntegrationTestsExtensions;

namespace SkiaFlameGraph.IntegrationTests;

/// <summary>
/// End-to-end tests for the real pipeline: <see cref="CollapsedStacksParser"/> loads a fixture profile,
/// <see cref="FlameGraphRenderer"/> draws it, and the tests assert on the resulting pixels and PNG file.
/// No part of the pipeline is mocked.
/// </summary>
public class RenderPipelineIntegrationTests : IRenderPipelineIntegrationTests
{
    /// <summary>
    /// Verifies that parsing the fixture yields the expected total, depth, and parent-child structure.
    /// </summary>
    [Fact]
    public void ParseFile_Fixture_BuildsTreeWithCorrectDepthsAndTotals()
    {
        // Act
        var root = CollapsedStacksParser.ParseFile(FixturePath());

        // Assert
        Assert.Equal(0, root.Depth);
        Assert.Equal(FixtureTotalSamples, root.Value);
        Assert.Equal(FixtureMaxDepth, root.MaxDepth());

        var main = Assert.Single(root.Children, c => c.Name == "main");
        Assert.Equal(1, main.Depth);
        Assert.Equal(622, main.Value);

        var deflate = Assert.Single(
            main.Children.Single(c => c.Name == "render").Children.Single(c => c.Name == "encode").Children,
            c => c.Name == "deflate");
        Assert.Equal(4, deflate.Depth);
        Assert.Equal(90, deflate.Value);
    }

    /// <summary>
    /// Verifies that the rendered bitmap has the requested width and the height implied by the fixture depth.
    /// </summary>
    [Fact]
    public void Render_Fixture_ReturnsBitmapWithRequestedWidthAndDerivedHeight()
    {
        // Act
        using var bitmap = RenderFixture(out _, out var options);

        // Assert
        var expectedHeight = (int)MathF.Ceiling((FixtureMaxDepth + 1) * options.RowHeight + options.Padding * 2);
        Assert.True(bitmap.Width > 0);
        Assert.True(bitmap.Height > 0);
        Assert.Equal(RenderWidth, bitmap.Width);
        Assert.Equal(expectedHeight, bitmap.Height);
    }

    /// <summary>
    /// Verifies that the root frame fills the full plot width on the bottom row, with background outside the plot.
    /// </summary>
    [Fact]
    public void Render_Fixture_RootFrameSpansFullPlotWidthOnBottomRow()
    {
        // Act
        using var bitmap = RenderFixture(out _, out var options);

        // Assert
        var rows = FixtureMaxDepth + 1;
        var rootRowTop = (int)options.FrameRowTop(0, rows);
        var sampleY = rootRowTop + SampleInsetPx;
        var rootBarColor = new SKColor(RootBarRed, RootBarGreen, RootBarBlue);

        // Interior of the plot, skipping the 1px outline on each edge.
        var plotLeft = (int)options.Padding;
        var plotRight = options.Width - (int)options.Padding;
        for (var x = plotLeft + 2; x <= plotRight - 3; x++)
        {
            Assert.True(bitmap.GetPixel(x, sampleY) == rootBarColor, $"Root bar missing at x={x}, y={sampleY}.");
        }

        // Padding outside the plot, and the strip below the bottom row, stay background.
        Assert.True(bitmap.GetPixel(plotLeft / 2, sampleY) == options.Background);
        Assert.True(bitmap.GetPixel(options.Width / 2, rootRowTop + (int)options.RowHeight + 3) == options.Background);
    }

    /// <summary>
    /// Verifies that the measured on-screen width of every frame matches its share of the total sample count.
    /// </summary>
    [Fact]
    public void Render_Fixture_FrameWidthsAreProportionalToSampleCounts()
    {
        // Act
        using var bitmap = RenderFixture(out var root, out var options);
        var frames = ExpectedLayout(root, options).Where(f => f.Node.Depth > 0).ToList();

        // Assert
        Assert.Equal(FixtureFrameCount, frames.Count);
        foreach (var frame in frames)
        {
            var color = FramePalette.ForFrame(frame.Node.Name);
            var sampleX = (int)(frame.Left + frame.Width / 2);
            var sampleY = (int)frame.Top + SampleInsetPx;

            var measured = bitmap.MeasureRunWidth(sampleY, sampleX, color);

            Assert.True(
                Math.Abs(measured - frame.Width) <= WidthTolerancePx,
                $"Frame '{frame.Node.Name}' (value {frame.Node.Value}) measured {measured}px, expected about {frame.Width:F1}px.");
        }
    }

    /// <summary>
    /// Verifies that known frames are drawn at their expected positions with their palette colour, not the background.
    /// </summary>
    [Theory]
    [InlineData("main", 1)]
    [InlineData("runtime", 1)]
    [InlineData("build_graph", 2)]
    [InlineData("layout", 3)]
    [InlineData("deflate", 4)]
    public void Render_Fixture_KnownFramePositionsHavePaletteColor(string name, int depth)
    {
        // Act
        using var bitmap = RenderFixture(out var root, out var options);

        // Assert
        var frame = Assert.Single(ExpectedLayout(root, options), f => f.Node.Name == name && f.Node.Depth == depth);
        var sampleX = (int)(frame.Left + frame.Width / 2);
        var sampleY = (int)frame.Top + SampleInsetPx;

        var pixel = bitmap.GetPixel(sampleX, sampleY);

        Assert.Equal(FramePalette.ForFrame(name), pixel);
        Assert.NotEqual(options.Background, pixel);
    }

    /// <summary>
    /// Verifies that <see cref="FlameGraphRenderer.RenderToPng"/> writes a file with a valid PNG signature
    /// and an IHDR chunk that matches the rendered dimensions.
    /// </summary>
    [Fact]
    public void RenderToPng_Fixture_WritesValidPngFileToTempDirectory()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), "skia-flame-integration-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);
        try
        {
            var pngPath = Path.Combine(tempDir, "fixture.png");
            var options = new RenderOptions { Width = RenderWidth };
            var root = CollapsedStacksParser.ParseFile(FixturePath());

            // Act
            using (var renderer = new FlameGraphRenderer(options))
            {
                renderer.RenderToPng(root, pngPath);
            }

            // Assert
            var bytes = File.ReadAllBytes(pngPath);
            Assert.True(bytes.AsSpan(0, PngSignature.Length).SequenceEqual(PngSignature), "File does not start with the PNG signature.");
            Assert.Equal(RenderWidth, bytes.ReadBigEndianInt32(PngWidthOffset));

            using var decoded = SKBitmap.Decode(pngPath);
            Assert.NotNull(decoded);
            Assert.Equal(RenderWidth, decoded.Width);
            Assert.Equal(bytes.ReadBigEndianInt32(PngHeightOffset), decoded.Height);
        }
        finally
        {
            Directory.Delete(tempDir, recursive: true);
        }
    }

    /// <summary>
    /// Parses the fixture and renders it with the requested width, returning the bitmap and the inputs used.
    /// </summary>
    private static SKBitmap RenderFixture(out FlameNode root, out RenderOptions options)
    {
        root = CollapsedStacksParser.ParseFile(FixturePath());
        options = new RenderOptions { Width = RenderWidth };

        using var renderer = new FlameGraphRenderer(options);
        using var image = renderer.Render(root);
        return SKBitmap.FromImage(image);
    }
}
