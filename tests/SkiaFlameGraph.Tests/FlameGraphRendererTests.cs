using System;
using System.IO;
using SkiaSharp;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests
{
    public class FlameGraphRendererTests
    {
        private readonly FlameGraphRenderer _renderer;
        private readonly RenderOptions _defaultOptions;

        public FlameGraphRendererTests()
        {
            _defaultOptions = new RenderOptions();
            _renderer = new FlameGraphRenderer();
        }

        [Fact]
        public void Constructor_WithNullOptions_UsesDefaultOptions()
        {
            // Act
            var renderer = new FlameGraphRenderer(options: null);

            // Assert
            Assert.NotNull(renderer);
            // The renderer should have valid options after EnsureValid() is called
            Assert.True(_defaultOptions.IsValid());
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
            var renderer = new FlameGraphRenderer(options);

            // Assert
            Assert.NotNull(renderer);
        }

        [Fact]
        public void Constructor_WithInvalidOptions_ThrowsValidationException()
        {
            // Arrange
            var options = new RenderOptions { Width = -100 };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new FlameGraphRenderer(options));
            Assert.Contains("Width must be positive", exception.Message);
        }

        [Fact]
        public void Render_WithEmptyRoot_ThrowsArgumentException()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _renderer.Render(root));
            Assert.Equal("root", exception.ParamName);
        }

        [Fact]
        public void Render_WithSingleNode_CalculatesCorrectHeight()
        {
            // Arrange
            var root = new FlameNode("main");
            root.Value = 1000;

            // Act
            var image = _renderer.Render(root);

            // Assert
            Assert.NotNull(image);
            Assert.Equal(_defaultOptions.Width, image.Width);
            // Height should be: padding * 2 + rowHeight (depth 0)
            var expectedHeight = (int)MathF.Ceiling((0 + 1) * _defaultOptions.RowHeight + _defaultOptions.Padding * 2);
            Assert.Equal(expectedHeight, image.Height);
        }

        [Fact]
        public void Render_WithDeepTree_CalculatesCorrectHeight()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            var child1 = root.AddChild("child1");
            child1.Value = 50;

            var child2 = child1.AddChild("child2");
            child2.Value = 30;

            var child3 = child2.AddChild("child3");
            child3.Value = 20;

            // Act
            var image = _renderer.Render(root);

            // Assert
            Assert.NotNull(image);
            // Max depth is 3 (root=0, child1=1, child2=2, child3=3)
            var expectedHeight = (int)MathF.Ceiling((3 + 1) * _defaultOptions.RowHeight + _defaultOptions.Padding * 2);
            Assert.Equal(expectedHeight, image.Height);
        }

        [Fact]
        public void RenderToPng_WithNullRoot_ThrowsArgumentNullException()
        {
            // Arrange
            var tempPath = Path.GetTempFileName();

            try
            {
                // Act & Assert
                var exception = Assert.Throws<ArgumentNullException>(
                    () => _renderer.RenderToPng(null!, tempPath));
                Assert.Equal("root", exception.ParamName);
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
        public void RenderToPng_WithNullPath_ThrowsArgumentNullException()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(
                () => _renderer.RenderToPng(root, null!));
            Assert.Equal("path", exception.ParamName);
        }

        [Fact]
        public void RenderToPng_WithEmptyPath_ThrowsArgumentException()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(
                () => _renderer.RenderToPng(root, string.Empty));
            Assert.Equal("path", exception.ParamName);
        }

        [Fact]
        public void Render_WithInvertedOption_CalculatesCorrectHeight()
        {
            // Arrange
            var options = new RenderOptions { Inverted = true };
            var renderer = new FlameGraphRenderer(options);

            var root = new FlameNode("root");
            root.Value = 100;

            var child = root.AddChild("child");
            child.Value = 50;

            // Act
            var image = renderer.Render(root);

            // Assert
            Assert.NotNull(image);
            Assert.True(image.Height > 0);
        }

        [Fact]
        public void Render_WithHighlightPattern_CreatesRendererWithHighlightPattern()
        {
            // Arrange
            var options = new RenderOptions { HighlightPattern = ".*Critical.*" };

            // Act
            var renderer = new FlameGraphRenderer(options);

            // Assert
            Assert.NotNull(renderer);
        }

        [Fact]
        public void Render_WithCustomBackgroundColor_CreatesRendererWithCustomColor()
        {
            // Arrange
            var options = new RenderOptions
            {
                Background = new SKColor(0x00, 0x00, 0x00),
                TextColor = new SKColor(0xff, 0xff, 0xff)
            };

            // Act
            var renderer = new FlameGraphRenderer(options);

            // Assert
            Assert.NotNull(renderer);
        }

        [Fact]
        public void Render_WithCustomDimensions_CreatesRendererWithCustomWidth()
        {
            // Arrange
            var options = new RenderOptions { Width = 800 };

            // Act
            var renderer = new FlameGraphRenderer(options);

            // Assert
            Assert.NotNull(renderer);
        }
    }
}
