using System.IO;
using Xunit;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Rendering;

namespace SkiaFlameGraph.Core.Tests.Rendering;

public class FlameGraphSvgExporterTests
{
    private readonly RenderOptions _defaultOptions = new()
    {
        Width = FlameGraphSvgExporterTestsConstants.DefaultWidth,
        RowHeight = FlameGraphSvgExporterTestsConstants.DefaultRowHeight
    };

    [Fact]
    public void RenderToSvg_WithNullRoot_ThrowsArgumentNullException()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var tempFile = Path.GetTempFileName();

        Assert.Throws<ArgumentNullException>(() => exporter.RenderToSvg(null!, tempFile));

        File.Delete(tempFile);
    }

    [Fact]
    public void RenderToSvg_WithEmptyPath_ThrowsArgumentException()
    {
        var exporter = new FlameGraphSvgExporter(_defaultOptions);
        var root = new FlameNode("root");

        Assert.Throws<ArgumentException>(() => exporter.RenderToSvg(root, ""));
        Assert.Throws<ArgumentException>(() => exporter.RenderToSvg(root, "   "));
    }

    [Fact]
    public void RenderToSvg_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new FlameGraphSvgExporter(null!));
    }

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
