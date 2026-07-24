using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;
using SkiaSharp;

// Usage:
// dotnet run -- <baseline.speedscope.json> <current.speedscope.json> <output.png> [--width <value>] [--inverted]
//
// This sample demonstrates differential flame graph rendering for regression reports.
// It computes the difference between two profiles and visualizes changes with a red/blue diverging palette.

if (args.Length < 3 || args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("Usage: dotnet run -- <baseline.speedscope.json> <current.speedscope.json> <output.png> [options]");
    Console.WriteLine("Options:");
    Console.WriteLine(" --width <value> Output image width in pixels (default: 1920)");
    Console.WriteLine(" --inverted Invert colors (white becomes black, black becomes white)");
    Console.WriteLine(" --help, -h Show this help message");
    return 1;
}

string baselinePath = args[0];
string currentPath = args[1];
string outputPath = args[2];
int width = 1920;
bool inverted = false;

// Parse arguments
for (int i = 3; i < args.Length; i++)
{
    var arg = args[i];

    if (arg.StartsWith("--"))
    {
        if (arg == "--width" && i + 1 < args.Length)
        {
            if (!int.TryParse(args[i + 1], out width) || width <= 0)
            {
                Console.Error.WriteLine($"Invalid width value: {args[i + 1]}");
                return 1;
            }
            i++;
        }
        else if (arg == "--inverted")
        {
            inverted = true;
        }
        else
        {
            Console.Error.WriteLine($"Unknown option: {arg}");
            return 1;
        }
    }
    else
    {
        Console.Error.WriteLine($"Unexpected argument: {arg}");
        return 1;
    }
}

// Validate input files
if (!File.Exists(baselinePath))
{
    Console.Error.WriteLine($"Baseline trace not found: {baselinePath}");
    return 1;
}

if (!File.Exists(currentPath))
{
    Console.Error.WriteLine($"Current trace not found: {currentPath}");
    return 1;
}

Console.WriteLine($"Parsing baseline: {baselinePath}");
var baselineRoot = SpeedscopeParser.ParseFile(baselinePath);
Console.WriteLine($"  root '{baselineRoot.Name}', total weight {baselineRoot.Value:0.##}, max depth {baselineRoot.MaxDepth()}");

Console.WriteLine($"Parsing current: {currentPath}");
var currentRoot = SpeedscopeParser.ParseFile(currentPath);
Console.WriteLine($"  root '{currentRoot.Name}', total weight {currentRoot.Value:0.##}, max depth {currentRoot.MaxDepth()}");

// Configure rendering options for differential rendering
var options = new RenderOptions
{
    Width = width,
    Inverted = inverted,
    Background = new SKColor(0x1e, 0x1e, 0x24),
    TextColor = new SKColor(0xf0, 0xf0, 0xf0),
    RowHeight = 24f,
    MinLabelWidth = 32f,
    MinBoxWidth = 1f,
    Padding = 20f,
    FontSize = 14f
};

Console.WriteLine("Computing differential flame graph...");

// Create and render differential flame graph
var differentialRenderer = new DifferentialFlameGraphRenderer(options);
var differentialImage = differentialRenderer.RenderDifferential(baselineRoot, currentRoot);

// Save the result
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
using var data = differentialImage.Encode(SKEncodedImageFormat.Png, 100);
using var fs = File.OpenWrite(outputPath);
data.SaveTo(fs);

Console.WriteLine($"Wrote differential flame graph to {outputPath}");
Console.WriteLine("\nColor legend:");
Console.WriteLine("- Blue hues: Performance improvements (current is faster than baseline)");
Console.WriteLine("- Red hues: Performance regressions (current is slower than baseline)");
Console.WriteLine("- White/neutral: No significant change");

return 0;