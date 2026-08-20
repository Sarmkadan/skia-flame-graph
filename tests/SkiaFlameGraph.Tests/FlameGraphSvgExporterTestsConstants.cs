namespace SkiaFlameGraph.Core.Tests.Rendering;

internal static class FlameGraphSvgExporterTestsConstants
{
    public const int DefaultWidth = 1600;
    public const float DefaultRowHeight = 22f;

    public const string XmlDeclaration = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
    public const string SvgTag = "<svg";
    public const string SvgEndTag = "</svg>";
    public const string RectTag = "<rect";
    public const string RectClass = "class=\"frame\"";
    public const string RectHoverClass = ".frame:hover { stroke-width: 1; stroke: #000; }";
    public const string FrameStyle = ".frame { stroke: #333; stroke-width: 0.5; }";
    public const string TextTag = "<text";
    public const string FrameLabelClass = "class=\"frame-label\"";
    public const string StyleSection = "<style type=\"text/css\"><![CDATA[";
    public const string FrameLabelStyle = ".frame-label { font-family: Arial, sans-serif; font-size: 11px; fill: #fff; text-shadow: 0 0 2px #000; }";
    public const string DoctypeSvg = "<!DOCTYPE svg";
}
