using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;
using SkiaSharp;

// Usage: dotnet run -- <input.collapsed.txt> <output.png>
const int Width = 1920;
const int Height = 1080;

var inputPath = args.Length > 0 ? args[0] : "stacks.txt";
var outputPath = args.Length > 1 ? args[1] : "flame.png";

// 1. Load collapsed stacks ("frame1;frame2;frame3 123" per line) into a call tree.
var root = CollapsedStacksParser.ParseFile(inputPath);

// 2. Pick a row height so the full stack depth fits inside 1080 px.
var options = new RenderOptions { Width = Width };
var rows = root.MaxDepth() + 1;
options.RowHeight = (Height - 2 * options.Padding) / rows;

// 3. Render to an SKImage, then draw it onto a 1920x1080 SKBitmap.
using var renderer = new FlameGraphRenderer(options);
using var image = renderer.Render(root);
using var bitmap = new SKBitmap(Width, Height);
using (var canvas = new SKCanvas(bitmap))
{
    canvas.Clear(options.Background);
    canvas.DrawImage(image, 0, 0, new SKSamplingOptions());
}

// 4. Encode as PNG and save to disk.
using var png = bitmap.Encode(SKEncodedImageFormat.Png, 100);
using var stream = File.Create(outputPath);
png.SaveTo(stream);

Console.WriteLine($"Wrote {Path.GetFullPath(outputPath)}");
