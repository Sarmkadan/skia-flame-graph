using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering
{
    public interface ITreemapRenderer
    {
        void RenderToPng(FlameNode root, string path);
        SKImage Render(FlameNode root);
        SKImage Render(FlameNode root, int? height);
    }
}