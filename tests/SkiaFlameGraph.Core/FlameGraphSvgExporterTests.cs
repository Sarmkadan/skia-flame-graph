using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Rendering;

namespace SkiaFlameGraph.Core.Tests
{
    [TestClass]
    public class FlameGraphSvgExporterTests
    {
        [TestMethod]
        public void Export_EscapesSpecialCharactersInFrameNames()
        {
            // Arrange
            var root = new FlameNode
            {
                Name = "Host<a> & 'b' >c</a>"
            };

            var exporter = new FlameGraphSvgExporter(new RenderOptions { Width = 100, RowHeight = 20 });

            // Act
            var svg = exporter.GenerateSvg(root);

            // Assert
            Assert.IsTrue(svg.Contains("&lt;a&gt; &amp; 'b' &gt;c&lt;/a&gt;"), "Frame name was not properly XML-escaped");
        }
    }
}
