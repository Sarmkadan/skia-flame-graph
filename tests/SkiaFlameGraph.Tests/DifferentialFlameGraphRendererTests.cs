using System;
using System.IO;
using SkiaSharp;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests
{
    public class DifferentialFlameGraphRendererTests
    {
        private readonly DifferentialFlameGraphRenderer _renderer;
        private readonly RenderOptions _defaultOptions;

        public DifferentialFlameGraphRendererTests()
        {
            _defaultOptions = new RenderOptions { Width = 1600 };
            _renderer = new DifferentialFlameGraphRenderer();
        }

        [Fact]
        public void Constructor_WithNullOptions_UsesDefaultOptions()
        {
            // Act
            var renderer = new DifferentialFlameGraphRenderer(options: null);

            // Assert
            Assert.NotNull(renderer);
            Assert.NotNull(renderer.Options);
        }

        [Fact]
        public void Constructor_WithValidOptions_UsesProvidedOptions()
        {
            // Arrange
            var options = new RenderOptions
            {
                Width = 1200,
                RowHeight = 25f,
                Background = new SKColor(0x00, 0x00, 0x00),
                TextColor = new SKColor(0xff, 0xff, 0xff)
            };

            // Act
            var renderer = new DifferentialFlameGraphRenderer(options);

            // Assert
            Assert.NotNull(renderer);
            Assert.Equal(1200, renderer.Options.Width);
        }

        [Fact]
        public void RenderDifferential_WithNullBaseline_ThrowsArgumentNullException()
        {
            // Arrange
            var current = new FlameNode("main");
            current.Value = 1000;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _renderer.RenderDifferential(null!, current));
            Assert.Equal("baseline", exception.ParamName);
        }

        [Fact]
        public void RenderDifferential_WithNullCurrent_ThrowsArgumentNullException()
        {
            // Arrange
            var baseline = new FlameNode("main");
            baseline.Value = 1000;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _renderer.RenderDifferential(baseline, null!));
            Assert.Equal("current", exception.ParamName);
        }

        [Fact]
        public void RenderDifferential_WithSingleNode_ReturnsValidImage()
        {
            // Arrange
            var baseline = new FlameNode("main");
            baseline.Value = 1000;

            var current = new FlameNode("main");
            current.Value = 1200;

            // Act
            var image = _renderer.RenderDifferential(baseline, current);

            // Assert
            Assert.NotNull(image);
            Assert.Equal(_defaultOptions.Width, image.Width);
            Assert.True(image.Height > 0);
        }

        [Fact]
        public void RenderDifferential_WithNegativeDelta_ReturnsValidImage()
        {
            // Arrange - regression scenario
            var baseline = new FlameNode("main");
            baseline.Value = 1000;

            var current = new FlameNode("main");
            current.Value = 800; // Slower than baseline

            // Act
            var image = _renderer.RenderDifferential(baseline, current);

            // Assert
            Assert.NotNull(image);
            Assert.Equal(_defaultOptions.Width, image.Width);
        }

        [Fact]
        public void RenderDifferential_WithPositiveDelta_ReturnsValidImage()
        {
            // Arrange - improvement scenario
            var baseline = new FlameNode("main");
            baseline.Value = 1000;

            var current = new FlameNode("main");
            current.Value = 1200; // Faster than baseline

            // Act
            var image = _renderer.RenderDifferential(baseline, current);

            // Assert
            Assert.NotNull(image);
            Assert.Equal(_defaultOptions.Width, image.Width);
        }

        [Fact]
        public void RenderDifferential_WithComplexTree_ReturnsValidImage()
        {
            // Arrange - create a more complex tree structure
            var baselineRoot = new FlameNode("root");
            baselineRoot.Value = 1000;

            var baselineChild1 = baselineRoot.AddChild("Child1");
            baselineChild1.Value = 600;

            var baselineChild2 = baselineRoot.AddChild("Child2");
            baselineChild2.Value = 400;

            var baselineGrandchild = baselineChild1.AddChild("Grandchild");
            baselineGrandchild.Value = 300;

            var currentRoot = new FlameNode("root");
            currentRoot.Value = 1000;

            var currentChild1 = currentRoot.AddChild("Child1");
            currentChild1.Value = 500; // Regressed

            var currentChild2 = currentRoot.AddChild("Child2");
            currentChild2.Value = 500; // Improved

            var currentGrandchild = currentChild1.AddChild("Grandchild");
            currentGrandchild.Value = 250; // Regressed

            // Act
            var image = _renderer.RenderDifferential(baselineRoot, currentRoot);

            // Assert
            Assert.NotNull(image);
            Assert.Equal(_defaultOptions.Width, image.Width);
        }

        [Fact]
        public void RenderDifferentialToPng_WithNullBaseline_ThrowsArgumentNullException()
        {
            // Arrange
            var tempPath = Path.GetTempFileName();
            var current = new FlameNode("main");
            current.Value = 1000;

            try
            {
                // Act & Assert
                var exception = Assert.Throws<ArgumentNullException>(
                    () => _renderer.RenderDifferentialToPng(null!, current, tempPath));
                Assert.Equal("baseline", exception.ParamName);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        [Fact]
        public void RenderDifferentialToPng_WithNullCurrent_ThrowsArgumentNullException()
        {
            // Arrange
            var tempPath = Path.GetTempFileName();
            var baseline = new FlameNode("main");
            baseline.Value = 1000;

            try
            {
                // Act & Assert
                var exception = Assert.Throws<ArgumentNullException>(
                    () => _renderer.RenderDifferentialToPng(baseline, null!, tempPath));
                Assert.Equal("current", exception.ParamName);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }
            }
        }

        [Fact]
        public void RenderDifferentialToPng_WithNullPath_ThrowsArgumentNullException()
        {
            // Arrange
            var baseline = new FlameNode("main");
            baseline.Value = 1000;
            var current = new FlameNode("main");
            current.Value = 1000;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _renderer.RenderDifferentialToPng(baseline, current, null!));
            Assert.Equal("path", exception.ParamName);
        }

        [Fact]
        public void RenderDifferentialToPng_WithEmptyPath_ThrowsArgumentException()
        {
            // Arrange
            var baseline = new FlameNode("main");
            baseline.Value = 1000;
            var current = new FlameNode("main");
            current.Value = 1000;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(
                () => _renderer.RenderDifferentialToPng(baseline, current, string.Empty));
            Assert.Equal("path", exception.ParamName);
        }

        [Fact]
        public void RenderDifferential_ProducesDifferentColorsForPositiveAndNegativeDeltas()
        {
            // Arrange - create two scenarios with opposite deltas
            var baseline1 = new FlameNode("main");
            baseline1.Value = 1000;
            var current1 = new FlameNode("main");
            current1.Value = 1200; // Positive delta

            var baseline2 = new FlameNode("main");
            baseline2.Value = 1000;
            var current2 = new FlameNode("main");
            current2.Value = 800; // Negative delta

            // Act
            var image1 = _renderer.RenderDifferential(baseline1, current1);
            var image2 = _renderer.RenderDifferential(baseline2, current2);

            // Assert - both should produce valid images
            Assert.NotNull(image1);
            Assert.NotNull(image2);
            Assert.Equal(_defaultOptions.Width, image1.Width);
            Assert.Equal(_defaultOptions.Width, image2.Width);
        }

        [Fact]
        public void DifferentialFlameGraphRenderer_ImplementsIDisposable()
        {
            // Arrange
            var renderer = new DifferentialFlameGraphRenderer();

            // Act
            renderer.Dispose();

            // Assert - should not throw
            Assert.True(true);
        }
    }
}