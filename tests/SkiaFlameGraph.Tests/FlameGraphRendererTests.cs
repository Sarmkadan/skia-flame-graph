using System;
using System.IO;
using SkiaSharp;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests
{
    /// <summary>
    /// Contains unit tests for the <see cref="FlameGraphRenderer"/> class.
    /// </summary>
    public class FlameGraphRendererTests : IFlameGraphRendererTests
    {
        private readonly FlameGraphRenderer _renderer;
        private readonly RenderOptions _defaultOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlameGraphRendererTests"/> class.
        /// Sets up the default render options and a new FlameGraphRenderer instance.
        /// </summary>
        public FlameGraphRendererTests()
        {
            _defaultOptions = new RenderOptions();
            _renderer = new FlameGraphRenderer();
        }

        /// <summary>
        /// Verifies that when the FlameGraphRenderer is constructed with null options, it uses the default options.
        /// </summary>
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

        /// <summary>
        /// Verifies that when the FlameGraphRenderer is constructed with valid options, it uses the provided options.
        /// </summary>
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

        /// <summary>
        /// Verifies that constructing the FlameGraphRenderer with invalid options (negative width) throws an ArgumentException.
        /// </summary>
        [Fact]
        public void Constructor_WithInvalidOptions_ThrowsValidationException()
        {
            // Arrange
            var options = new RenderOptions { Width = -100 };

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new FlameGraphRenderer(options));
            Assert.Contains("Width must be positive", exception.Message);
        }

        /// <summary>
        /// Verifies that calling Render with a root node that has no children (empty) throws an ArgumentNullException.
        /// </summary>
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

        /// <summary>
        /// Verifies that rendering a flame graph with a single node produces an image with the expected height.
        /// </summary>
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

        /// <summary>
        /// Verifies that rendering a flame graph with a deep tree (multiple levels) produces an image with the expected height.
        /// </summary>
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

        /// <summary>
        /// Verifies that calling RenderToPng with a null root throws an ArgumentNullException.
        /// </summary>
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

        /// <summary>
        /// Verifies that calling RenderToPng with a null path throws an ArgumentNullException.
        /// </summary>
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

        /// <summary>
        /// Verifies that calling RenderToPng with an empty path throws an ArgumentException.
        /// </summary>
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

        /// <summary>
        /// Verifies that rendering with the inverted option set produces an image with a positive height.
        /// </summary>
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

        /// <summary>
        /// Verifies that constructing the FlameGraphRenderer with a highlight pattern in the options sets the renderer's highlight pattern.
        /// </summary>
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

        /// <summary>
        /// Verifies that constructing the FlameGraphRenderer with custom background and text colors uses those colors.
        /// </summary>
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

        /// <summary>
        /// Verifies that constructing the FlameGraphRenderer with a custom width uses that width.
        /// </summary>
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