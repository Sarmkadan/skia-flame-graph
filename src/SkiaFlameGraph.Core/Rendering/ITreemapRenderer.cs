using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering
{
    public interface ITreemapRenderer
    {
        /// <summary>
        /// Renders the treemap to a PNG file.
        /// </summary>
        /// <param name="root">The root node of the treemap.</param>
        /// <param name="path">The file path to save the PNG.</param>
        void RenderToPng(FlameNode root, string path);

        /// <summary>
        /// Renders the treemap and returns an image.
        /// </summary>
        /// <param name="root">The root node of the treemap.</param>
        /// <returns>The rendered treemap image.</returns>
        SKImage Render(FlameNode root);

        /// <summary>
        /// Renders the treemap at the specified height and returns an image.
        /// </summary>
        /// <param name="root">The root node of the treemap.</param>
        /// <param name="height">The image height, or <see langword="null"/> to use the default height.</param>
        /// <returns>The rendered treemap image.</returns>
        SKImage Render(FlameNode root, int? height);
    }
}
