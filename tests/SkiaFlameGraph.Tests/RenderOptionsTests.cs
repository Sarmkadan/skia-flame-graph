using System;
using SkiaSharp;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests
{
    /// <summary>
    /// Tests for the <see cref="RenderOptions"/> class.
    /// </summary>
    public class RenderOptionsTests : IRenderOptionsTests
    {
        /// <summary>
        /// Verifies that the default values of a newly created RenderOptions instance match the expected values.
        /// </summary>
        [Fact]
        public void DefaultValues_ShouldMatchExpected()
        {
            var options = new RenderOptions();

            Assert.Equal(1600, options.Width);
            Assert.Equal(22f, options.RowHeight);
            Assert.Equal(28f, options.MinLabelWidth);
            Assert.Equal(0.4f, options.MinBoxWidth);
            Assert.Equal(16f, options.Padding);
            Assert.Equal(12f, options.FontSize);
            Assert.Equal(new SKColor(0x1e, 0x1e, 0x24), options.Background);
            Assert.Equal(new SKColor(0xf0, 0xf0, 0xf0), options.TextColor);
            Assert.False(options.Inverted);
            Assert.Null(options.HighlightPattern);
        }

        /// <summary>
        /// Verifies that the Width and RowHeight properties can be set to custom values.
        /// </summary>
        [Fact]
        public void CanSetWidthAndRowHeight()
        {
            var options = new RenderOptions
            {
                Width = 800,
                RowHeight = 30f
            };

            Assert.Equal(800, options.Width);
            Assert.Equal(30f, options.RowHeight);
        }

        /// <summary>
        /// Verifies that the MinLabelWidth, MinBoxWidth, and Padding properties can be set to custom values.
        /// </summary>
        [Fact]
        public void CanSetMinWidthsAndPadding()
        {
            var options = new RenderOptions
            {
                MinLabelWidth = 10f,
                MinBoxWidth = 0.1f,
                Padding = 5f
            };

            Assert.Equal(10f, options.MinLabelWidth);
            Assert.Equal(0.1f, options.MinBoxWidth);
            Assert.Equal(5f, options.Padding);
        }

        /// <summary>
        /// Verifies that the Background and TextColor properties can be set to custom SKColor values.
        /// </summary>
        [Fact]
        public void CanSetColors()
        {
            var options = new RenderOptions
            {
                Background = new SKColor(0x00, 0x00, 0x00),
                TextColor = new SKColor(0xff, 0xff, 0xff)
            };

            Assert.Equal(new SKColor(0x00, 0x00, 0x00), options.Background);
            Assert.Equal(new SKColor(0xff, 0xff, 0xff), options.TextColor);
        }

        /// <summary>
        /// Verifies that the Inverted property can be set to true and the HighlightPattern property can be set to a custom string.
        /// </summary>
        [Fact]
        public void CanToggleInvertedAndHighlightPattern()
        {
            var options = new RenderOptions
            {
                Inverted = true,
                HighlightPattern = ".*Critical.*"
            };

            Assert.True(options.Inverted);
            Assert.Equal(".*Critical.*", options.HighlightPattern);
        }

        /// <summary>
        /// Verifies that setting negative values for numeric properties does not throw an exception and the values are stored as provided.
        /// </summary>
        [Fact]
        public void SettingNegativeValues_ShouldNotThrow()
        {
            var options = new RenderOptions();

            var exception = Record.Exception(() =>
            {
                options.Width = -1;
                options.RowHeight = -5f;
                options.MinLabelWidth = -10f;
                options.MinBoxWidth = -0.1f;
                options.Padding = -2f;
                options.FontSize = -12f;
            });

            Assert.Null(exception);
            Assert.Equal(-1, options.Width);
            Assert.Equal(-5f, options.RowHeight);
            Assert.Equal(-10f, options.MinLabelWidth);
            Assert.Equal(-0.1f, options.MinBoxWidth);
            Assert.Equal(-2f, options.Padding);
            Assert.Equal(-12f, options.FontSize);
        }
    }
}
