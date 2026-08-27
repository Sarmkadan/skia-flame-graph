using System.IO;
using Xunit;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Rendering;

namespace SkiaFlameGraph.Core.Tests.Rendering;

/// <summary>
/// Test class for FlameGraphSvgExporter, verifying SVG generation functionality
/// including proper handling of edge cases, XML structure, and visual elements.
/// </summary>
public class FlameGraphSvgExporterTests
{
    private readonly RenderOptions _defaultOptions = new()
    {
        Width = FlameGraphSvgExporterTestsConstants.DefaultWidth,
        RowHeight = FlameGraphSvgExporterTestsConstants.DefaultRowHeight
    };

    /// <summary>
    /// Verifies that RenderToSvg throws ArgumentNullException when root parameter is null.
    /// </summary>
    [Fact]
    public void RenderToSvg_WithNullRoot_ThrowsArgumentNullException()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();

        Assert.Throws<ArgumentNullException>(() => exporter.RenderToSvg(null!, tempFile));

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg throws ArgumentException when the file path is empty or consists only of whitespace.
    /// </summary>
    [Fact]
    public void RenderToSvg_WithEmptyPath_ThrowsArgumentException()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var root = new FlameNode("root");

        Assert.Throws<ArgumentException>(() => exporter.RenderToSvg(root, ""));
        Assert.Throws<ArgumentException>(() => exporter.RenderToSvg(root, "   "));
    }

    /// <summary>
    /// Verifies that FlameGraphSvgExporter constructor throws ArgumentNullException when options parameter is null.
    /// </summary>
    [Fact]
    public void RenderToSvg_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FlameGraphSvgExporter(null!));
    }

    /// <summary>
    /// Verifies that RenderToSvg produces a valid SVG file with proper XML declaration,
    /// SVG tags, and closing tag when given a single-node tree.
    /// </summary>
    [Fact]
    public void RenderToSvg_EmptyTree_ProducesValidSvgFile()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("root");
        root.Value = 100;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.StartsWith(FlameGraphSvgExporterTestsConstants.XmlDeclaration, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.SvgTag, content);
        Assert.True(content.Contains(FlameGraphSvgExporterTestsConstants.SvgEndTag));

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg includes a rect element with proper data attributes
    /// when exporting a single-frame flame graph.
    /// </summary>
    [Fact]
    public void RenderToSvg_SingleFrame_ContainsRectElement()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("main");
        root.Value = 100;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.Contains(FlameGraphSvgExporterTestsConstants.RectTag, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.RectClass, content);
        Assert.Contains("data-name=\"main\"", content);
        Assert.Contains("data-value=\"100\"", content);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg includes XML declaration and DOCTYPE in the output
    /// when exporting a single-frame flame graph.
    /// </summary>
    [Fact]
    public void RenderToSvg_SingleFrame_ContainsXmlDeclarationAndDoctype()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("test");
        root.Value = 50;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.StartsWith(FlameGraphSvgExporterTestsConstants.XmlDeclaration, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.DoctypeSvg, content);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg includes CSS style section with frame, rect hover, and label styles
    /// when exporting a single-frame flame graph.
    /// </summary>
    [Fact]
    public void RenderToSvg_SingleFrame_ContainsStyleSection()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("test");
        root.Value = 50;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.Contains(FlameGraphSvgExporterTestsConstants.StyleSection, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.FrameStyle, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.RectHoverClass, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.FrameLabelStyle, content);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg properly XML-escapes special characters (<, >, &, ", ') in frame names
    /// to prevent XML injection and ensure valid SVG output.
    /// </summary>
    [Fact]
    public void RenderToSvg_SpecialCharactersInFrameName_AreXmlEscaped()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("frame<>&\"'");
        root.Value = 100;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.Contains("data-name=", content);
        Assert.Contains("frame", content);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg creates the correct number of rect elements (one per node)
    /// when exporting a flame graph with multiple frames (root and two children).
    /// </summary>
    [Fact]
    public void RenderToSvg_MultipleFrames_ContainsMultipleRectElements()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("root");
        root.Value = 100;

        var child1 = root.AddChild("child1");
        child1.Value = 60;

        var child2 = root.AddChild("child2");
        child2.Value = 40;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        var rectCount = CountOccurrences(content, FlameGraphSvgExporterTestsConstants.RectTag);
        Assert.Equal(3, rectCount);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg includes a text element for frame labels when the frame is wide enough
    /// to accommodate the label text.
    /// </summary>
    [Fact]
    public void RenderToSvg_FrameWithLabel_ContainsTextElement()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("main");
        root.Value = 1000;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.Contains(FlameGraphSvgExporterTestsConstants.TextTag, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.FrameLabelClass, content);
        Assert.Contains("main", content);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg does not render text labels for frames that are too narrow
    /// to accommodate the label text, preventing overlapping or clipped labels.
    /// </summary>
    [Fact]
    public void RenderToSvg_FrameTooNarrow_LabelNotRendered()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("narrow");
        root.Value = 1;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.Contains(FlameGraphSvgExporterTestsConstants.RectTag, content);
        Assert.DoesNotContain(FlameGraphSvgExporterTestsConstants.TextTag + " " + FlameGraphSvgExporterTestsConstants.FrameLabelClass, content);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg calculates the correct SVG height attribute
    /// based on the depth of the flame graph tree and row height settings.
    /// </summary>
    [Fact]
    public void RenderToSvg_DeepTree_CalculatesCorrectHeight()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("root");
        root.Value = 100;

        var child1 = root.AddChild("child1");
        child1.Value = 60;

        var grandchild = child1.AddChild("grandchild");
        grandchild.Value = 40;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.Contains("height=", content);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg does not render rect elements for frames with zero value,
    /// effectively hiding them from the flame graph visualization.
    /// </summary>
    [Fact]
    public void RenderToSvg_FrameWithZeroValue_NotRendered()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("root");
        root.Value = 100;

        var child = root.AddChild("zero");
        child.Value = 0;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        var rectCount = CountOccurrences(content, FlameGraphSvgExporterTestsConstants.RectTag);
        Assert.Equal(1, rectCount);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg does not render rect elements for frames with negative value,
    /// effectively hiding them from the flame graph visualization.
    /// </summary>
    [Fact]
    public void RenderToSvg_FrameWithNegativeValue_NotRendered()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("root");
        root.Value = 100;

        var child = root.AddChild("negative");
        child.Value = -5;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        var rectCount = CountOccurrences(content, FlameGraphSvgExporterTestsConstants.RectTag);
        Assert.Equal(1, rectCount);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg outputs a file with correct content including XML declaration
    /// and SVG tag when exporting a simple flame graph.
    /// </summary>
    [Fact]
    public void RenderToSvg_OutputsFileWithCorrectContent()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("test");
        root.Value = 100;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);
        Assert.StartsWith(FlameGraphSvgExporterTestsConstants.XmlDeclaration, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.SvgTag, content);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Verifies that RenderToSvg produces correct SVG structure for a complex tree
    /// with multiple levels and branches, including proper data attributes and element nesting.
    /// </summary>
    [Fact]
    public void RenderToSvg_ComplexTree_ContainsCorrectStructure()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("main");
        root.Value = 100;

        var child1 = root.AddChild("functionA");
        child1.Value = 40;

        var child2 = root.AddChild("functionB");
        child2.Value = 30;

        var grandchild = child1.AddChild("helper");
        grandchild.Value = 20;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.StartsWith(FlameGraphSvgExporterTestsConstants.XmlDeclaration, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.SvgTag, content);
        Assert.Contains("data-name=\"main\"", content);
        Assert.Contains("data-value=\"100\"", content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.RectTag, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.SvgEndTag, content);

        File.Delete(tempFile);
    }

    [Fact]
    public void RenderToSvg_EmptyFrameName_HandledGracefully()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();
        var root = new FlameNode("");
        root.Value = 100;

        exporter.RenderToSvg(root, tempFile);

        Assert.True(File.Exists(tempFile));
        var content = File.ReadAllText(tempFile);

        Assert.StartsWith(FlameGraphSvgExporterTestsConstants.XmlDeclaration, content);
        Assert.Contains(FlameGraphSvgExporterTestsConstants.SvgTag, content);

        File.Delete(tempFile);
    }

    /// <summary>
    /// Counts the number of occurrences of a substring within a string using ordinal comparison.
    /// </summary>
    /// <param name="haystack">The string to search within.</param>
    /// <param name="needle">The substring to search for.</param>
    /// <returns>The number of times needle appears in haystack.</returns>
    private static int CountOccurrences(string haystack, string needle)
    {
        var count = 0;
        var index = 0;
        while ((index = haystack.IndexOf(needle, index, StringComparison.Ordinal)) != -1)
        {
            count++;
            index += needle.Length;
        }
        return count;
    }
}
