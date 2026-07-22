using System;
using System.IO;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Rendering;
using SkiaSharp;
using Xunit;

namespace SkiaFlameGraph.Tests
{
    public class TreemapRendererTests
    {
        private readonly TreemapRenderer _renderer;
        private readonly RenderOptions _defaultOptions;

        public TreemapRendererTests()
        {
            _defaultOptions = new RenderOptions();
            _renderer = new TreemapRenderer();
        }

        [Fact]
        public void Constructor_WithNullOptions_UsesDefaultOptions()
        {
            // Act
            var renderer = new TreemapRenderer(options: null);

            // Assert
            Assert.NotNull(renderer);
        }

        [Fact]
        public void Constructor_WithValidOptions_UsesProvidedOptions()
        {
            // Arrange
            var options = new RenderOptions
            {
                Width = 1200,
                Background = new SKColor(0x00, 0x00, 0x00),
                TextColor = new SKColor(0xff, 0xff, 0xff)
            };

            // Act
            var renderer = new TreemapRenderer(options);

            // Assert
            Assert.NotNull(renderer);
        }

        [Fact]
        public void Render_WithNullRoot_ThrowsArgumentNullException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _renderer.Render(null!));
            Assert.Equal("root", exception.ParamName);
        }

        [Fact]
        public void Render_WithEmptyRoot_ReturnsValidImage()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            // Act
            var image = _renderer.Render(root);

            // Assert
            Assert.NotNull(image);
            Assert.True(image.Width > 0);
            Assert.True(image.Height > 0);
        }

        [Fact]
        public void Render_WithSingleNode_ReturnsImageWithCorrectDimensions()
        {
            // Arrange
            var root = new FlameNode("main");
            root.Value = 1000;

            // Act
            var image = _renderer.Render(root);

            // Assert
            Assert.NotNull(image);
            Assert.Equal(_defaultOptions.Width, image.Width);
            // Height should be: padding * 2 + some minimal height for the node
            Assert.True(image.Height > _defaultOptions.Padding * 2);
        }

        [Fact]
        public void Render_WithTreeWithChildren_ReturnsValidImage()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            var child1 = root.AddChild("child1");
            child1.Value = 50;

            var child2 = root.AddChild("child2");
            child2.Value = 30;

            // Act
            var image = _renderer.Render(root);

            // Assert
            Assert.NotNull(image);
            Assert.True(image.Width > 0);
            Assert.True(image.Height > 0);
        }

        [Fact]
        public void Render_WithDeepTree_DepthGuardPreventsStackOverflow()
        {
            // Arrange - create a tree deeper than the depth guard (depth >= 12)
            var root = new FlameNode("root");
            root.Value = 100;

            var current = root;
            for (int i = 0; i < 20; i++)
            {
                var child = current.AddChild($"level{i}");
                child.Value = 10;
                current = child;
            }

            // Act
            var image = _renderer.Render(root);

            // Assert - should not throw and should produce a valid image
            Assert.NotNull(image);
            Assert.True(image.Width > 0);
            Assert.True(image.Height > 0);
        }

        [Fact]
        public void Render_WithZeroWidthNode_DoesNotThrow()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            var zeroChild = root.AddChild("zero");
            zeroChild.Value = 0; // Zero value node

            var normalChild = root.AddChild("normal");
            normalChild.Value = 50;

            // Act
            var image = _renderer.Render(root);

            // Assert
            Assert.NotNull(image);
            Assert.True(image.Width > 0);
            Assert.True(image.Height > 0);
        }

        [Fact]
        public void Render_WithCustomHeightParameter_ReturnsImageWithCustomHeight()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            // Act
            var image = _renderer.Render(root, height: 500);

            // Assert
            Assert.NotNull(image);
            Assert.Equal(_defaultOptions.Width, image.Width);
            Assert.Equal(500, image.Height);
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
        public void RenderToPng_WithValidInput_CreatesPngFile()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            var tempPath = Path.Combine(Path.GetTempPath(), $"treemap_test_{Guid.NewGuid()}.png");

            try
            {
                // Act
                _renderer.RenderToPng(root, tempPath);

                // Assert
                Assert.True(File.Exists(tempPath));
                var fileInfo = new FileInfo(tempPath);
                Assert.True(fileInfo.Length > 0);
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
        public void RenderToPng_WithCustomHeight_CreatesPngWithCorrectDimensions()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            var tempPath = Path.Combine(Path.GetTempPath(), $"treemap_test_{Guid.NewGuid()}.png");

            try
            {
                // Act
                _renderer.RenderToPng(root, tempPath, height: 400);

                // Assert
                Assert.True(File.Exists(tempPath));

                // Verify the image has the expected dimensions
                using var image = SKImage.FromEncodedData(tempPath);
                Assert.Equal(_defaultOptions.Width, image.Width);
                Assert.Equal(400, image.Height);
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
        public void Render_WithLargeTree_ReturnsValidImage()
        {
            // Arrange - create a large tree with many nodes
            var root = new FlameNode("root");
            root.Value = 1000;

            // Add 50 children
            for (int i = 0; i < 50; i++)
            {
                var child = root.AddChild($"child{i}");
                child.Value = 20 + i;

                // Add 2-3 children to each
                int childCount = i % 3 + 2;
                for (int j = 0; j < childCount; j++)
                {
                    var grandchild = child.AddChild($"grandchild{j}");
                    grandchild.Value = 10 + j;
                }
            }

            // Act
            var image = _renderer.Render(root);

            // Assert
            Assert.NotNull(image);
            Assert.True(image.Width > 0);
            Assert.True(image.Height > 0);
        }

        [Fact]
        public void Render_WithDifferentOptions_ProducesDifferentOutput()
        {
            // Arrange
            var root = new FlameNode("root");
            root.Value = 100;

            var options1 = new RenderOptions { Width = 800 };
            var renderer1 = new TreemapRenderer(options1);

            var options2 = new RenderOptions { Width = 1200 };
            var renderer2 = new TreemapRenderer(options2);

            // Act
            var image1 = renderer1.Render(root);
            var image2 = renderer2.Render(root);

            // Assert
            Assert.NotNull(image1);
            Assert.NotNull(image2);
            Assert.Equal(800, image1.Width);
            Assert.Equal(1200, image2.Width);
        }
    }
}
