using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;

// Usage:
// dotnet run -- <input.speedscope.json> <output.png> [--width <value>] [--height <value>] [--inverted]
//
// If no arguments are given we fall back to the bundled sample trace so the
// project renders something out of the box.

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    Console.WriteLine("Usage: dotnet run -- <input.speedscope.json> <output.png> [options]");
    Console.WriteLine("Options:");
    Console.WriteLine("  --width <value>    Output image width in pixels (default: 1600)");
    Console.WriteLine("  --inverted         Invert colors (white becomes black, black becomes white)");
    Console.WriteLine("  --help, -h        Show this help message");
    Console.WriteLine("");
    Console.WriteLine("If only input path is provided, writes to flame.png and treemap.png in current directory");
    Console.WriteLine("If input and output PNG path are provided, writes only flame graph to specified path");
    return 2;
}

string inputPath = string.Empty;
string outputPath = string.Empty;
int width = 1600;
bool inverted = false;

// Parse arguments
for (int i = 0; i < args.Length; i++)
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
    else if (string.IsNullOrEmpty(inputPath))
    {
        inputPath = arg;
    }
    else if (string.IsNullOrEmpty(outputPath))
    {
        outputPath = arg;
    }
    else
    {
        Console.Error.WriteLine($"Unexpected argument: {arg}");
        return 1;
    }
}

// Determine input file
if (string.IsNullOrEmpty(inputPath))
{
    inputPath = FindBundledSample();
}

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Input trace not found: {inputPath}");
    return 1;
}

Console.WriteLine($"Parsing {inputPath}");
var root = SpeedscopeParser.ParseFile(inputPath);
Console.WriteLine($" root '{root.Name}', total weight {root.Value:0.##}, max depth {root.MaxDepth()}");

// Configure rendering options
var options = new RenderOptions
{
    Width = width,
    Inverted = inverted
};

if (string.IsNullOrEmpty(outputPath))
{
    // Default behavior: write both flame and treemap to current directory
    var outDir = Directory.GetCurrentDirectory();
    Directory.CreateDirectory(outDir);

    var flamePath = Path.Combine(outDir, "flame.png");
    new FlameGraphRenderer(options).RenderToPng(root, flamePath);
    Console.WriteLine($"Wrote {flamePath}");

    var treemapPath = Path.Combine(outDir, "treemap.png");
    new TreemapRenderer(options).RenderToPng(root, treemapPath);
    Console.WriteLine($"Wrote {treemapPath}");
}
else
{
    // Single output file specified
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
    new FlameGraphRenderer(options).RenderToPng(root, outputPath);
    Console.WriteLine($"Wrote {outputPath}");
}

return 0;

static string FindBundledSample()
{
    // Walk up from the binary looking for the checked-in sample trace.
    var dir = AppContext.BaseDirectory;
    for (var i = 0; i < 8 && dir is not null; i++)
    {
        var candidate = Path.Combine(dir, "samples", "sample-trace.speedscope.json");
        if (File.Exists(candidate))
            return candidate;
        var local = Path.Combine(dir, "sample-trace.speedscope.json");
        if (File.Exists(local))
            return local;
        dir = Path.GetDirectoryName(dir);
    }
    return "sample-trace.speedscope.json";
}