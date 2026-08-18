using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering;

public interface IFlameGraphRenderer
{
    void RenderToPng(FlameNode root, string path);
    SKImage Render(FlameNode root);
}
