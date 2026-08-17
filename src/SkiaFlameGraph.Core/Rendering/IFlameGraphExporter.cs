using System.IO;
using SkiaFlameGraph.Core.Models;
using SkiaSharp;

namespace SkiaFlameGraph.Core.Rendering
{
    /// <summary>
    /// Interface for exporting flame graphs to various formats.
    /// </summary>
    public interface IFlameGraphExporter
    {
        /// <summary>
        /// Exports the flame graph to the specified output stream.
        /// </summary>
        /// <param name="root">Root node of the flame graph</param>
        /// <param name="options">Rendering options</param>
        /// <param name="output">Output stream to write to</param>
        void Export(FlameNode root, RenderOptions options, Stream output);
    }
}
