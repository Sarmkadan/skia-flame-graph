using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;
using SkiaSharp;

// Usage:
// dotnet run --project samples/CustomColors -- [input.speedscope.json] [outDir]
//
// Renders the same profile three times to show how colours are controlled:
// the default palette, a dark theme, and a highlight pattern.

var inputPath = args.Length > 0 ? args[0] : "samples/sample-trace.speedscope.json";
var outDir = args.Length > 1 ? args[1] : "out";

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Input trace not found: {inputPath}");
    return 1;
}

Directory.CreateDirectory(outDir);
var root = SpeedscopeParser.ParseFile(inputPath);

// 1. Default palette: each frame gets FramePalette.ForFrame(name), a warm hue from its name hash.
new FlameGraphRenderer(new RenderOptions { Width = 1600 })
    .RenderToPng(root, Path.Combine(outDir, "colors-default.png"));

// 2. Dark theme: background and label colours. Frame colours are unchanged.
var dark = new RenderOptions
{
    Width = 1600,
    Background = new SKColor(0x10, 0x10, 0x14),
    TextColor = new SKColor(0xf5, 0xf5, 0xf5)
};
new FlameGraphRenderer(dark).RenderToPng(root, Path.Combine(outDir, "colors-dark.png"));

// 3. Highlight: frames whose name matches the regex are drawn in FramePalette.HighlightColor (#FF6B35).
var highlight = new RenderOptions
{
    Width = 1600,
    HighlightPattern = @"^(OrdersController|OrderService|PricingEngine)\."
};
new FlameGraphRenderer(highlight).RenderToPng(root, Path.Combine(outDir, "colors-highlight.png"));

Console.WriteLine($"Wrote 3 images to {Path.GetFullPath(outDir)}");
return 0;
