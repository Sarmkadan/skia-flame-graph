using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;

// Usage:
//   dotnet run -- <trace.speedscope.json> [outputDir]
//
// If no arguments are given we fall back to the bundled sample trace so the
// project renders something out of the box.

var input = args.Length > 0 ? args[0] : FindBundledSample();
var outDir = args.Length > 1 ? args[1] : Directory.GetCurrentDirectory();

if (!File.Exists(input))
{
    Console.Error.WriteLine($"input trace not found: {input}");
    return 1;
}

Directory.CreateDirectory(outDir);

Console.WriteLine($"parsing {input}");
var root = SpeedscopeParser.ParseFile(input);
Console.WriteLine($"  root '{root.Name}', total weight {root.Value:0.##}, max depth {root.MaxDepth()}");

var options = new RenderOptions { Width = 1600 };

var flamePath = Path.Combine(outDir, "flame.png");
new FlameGraphRenderer(options).RenderToPng(root, flamePath);
Console.WriteLine($"wrote {flamePath}");

var treemapPath = Path.Combine(outDir, "treemap.png");
new TreemapRenderer(options).RenderToPng(root, treemapPath);
Console.WriteLine($"wrote {treemapPath}");

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
