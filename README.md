# SkiaFlameGraph

Renders flame graphs and treemaps from .NET profiler output using
[SkiaSharp](https://github.com/mono/SkiaSharp). Point it at a `speedscope` file
produced by `dotnet-trace` and it draws a PNG - no browser, no JavaScript, no
headless Chromium.

The intent is a small, embeddable library you can call from a CI step or a
dashboard service to turn a captured trace into an image artifact.

## Why

The usual way to look at a `.nettrace` is to convert it to speedscope and open
[speedscope.app](https://www.speedscope.app) in a browser. That is great
interactively, but it is awkward when you just want a picture attached to a CI
run, a regression report, or a Slack message. This produces that picture
directly from managed code.

## Layout

```
SkiaFlameGraph.sln
├── src/SkiaFlameGraph.Core        class library (parser + renderers)
│   ├── Models/                    speedscope DTOs + the FlameNode call tree
│   ├── Parsing/                   SpeedscopeParser (evented + sampled)
│   └── Rendering/                 FlameGraphRenderer, TreemapRenderer, palette
├── samples/SkiaFlameGraph.Sample  console app that renders a trace to PNG
│   └── sample-trace.speedscope.json
└── tests/SkiaFlameGraph.Tests     xUnit tests for the parser
```

## Getting a trace

```bash
# capture
dotnet-trace collect --process-id <pid> --output app.nettrace

# convert to speedscope
dotnet-trace convert app.nettrace --format speedscope
# -> app.speedscope.json
```

Both profile shapes the converter can emit are handled:

- **evented** - a stream of open/close frame events (default from recent
  `dotnet-trace`). Elapsed time between events is attributed to the frame on top
  of the stack.
- **sampled** - stacks plus per-sample weights.

## Running the sample

```bash
# uses the bundled sample-trace.speedscope.json when no path is given
dotnet run --project samples/SkiaFlameGraph.Sample

# or against your own trace, writing PNGs into ./out
dotnet run --project samples/SkiaFlameGraph.Sample -- app.speedscope.json ./out
```

This writes `flame.png` and `treemap.png`.

## Using the library

```csharp
using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;

var root = SpeedscopeParser.ParseFile("app.speedscope.json");

var options = new RenderOptions { Width = 1920, Inverted = false };
new FlameGraphRenderer(options).RenderToPng(root, "flame.png");
new TreemapRenderer(options).RenderToPng(root, "treemap.png");
```

`Render(...)` returns an `SKImage` if you want to composite it yourself instead
of writing straight to disk.

### Rendering notes

- Frame colours are deterministic - derived from an FNV-1a hash of the frame
  name - so the same method keeps the same shade between runs. That makes two
  side-by-side graphs comparable at a glance.
- Recursive / repeated frames are merged into a single wider box rather than a
  tower of one-pixel slivers.
- `RenderOptions.Inverted` flips the flame graph into an icicle (root on top).

## Native dependencies

SkiaSharp needs its native library at runtime. The sample references
`SkiaSharp.NativeAssets.Linux.NoDependencies` so it runs on a headless server
without an X stack. On Windows/macOS the base `SkiaSharp` package already ships
the native bits. Text labels use the default system typeface; install a font
package (e.g. `fontconfig` + `ttf-dejavu`) if labels come out blank on a minimal
container.

## Status

Active. Parser and both renderers work end to end; the sample produces images.
Next up: per-thread profile selection, a search/highlight overlay, and SVG
output alongside PNG.

## RenderOptions

Configuration class that controls the appearance of flame graphs and treemaps. Set
public properties to change canvas size, spacing, colours, and minimum sizing
thresholds before rendering. All values are in pixels except colours which use
SkiaSharp's `SKColor`.

Example usage:

```csharp
var options = new RenderOptions
{
    Width = 1920,
    RowHeight = 24f,
    MinLabelWidth = 32f,
    MinBoxWidth = 1f,
    Padding = 20f,
    FontSize = 14f,
    Background = new SKColor(0x28, 0x28, 0x2e),
    TextColor = new SKColor(0xff, 0xff, 0xff),
    Inverted = false
};
```

## SpeedscopeFileExtensions

Provides a set of practical extension methods for analyzing, transforming, and querying speedscope profile data. These methods offer utilities for calculating metrics, retrieving frames, checking profile types, and identifying performance hotspots within your traces.

Example usage when analyzing profile data:

```csharp
using SkiaFlameGraph.Core.Models;
using System;

// Load a speedscope file
var speedscopeFile = SpeedscopeParser.ParseFile("app.speedscope.json");

// Calculate metrics about the trace
Console.WriteLine($"Total duration: {speedscopeFile.GetTotalDuration():F2}ms");
Console.WriteLine($"Total events: {speedscopeFile.GetTotalEventCount()}");
Console.WriteLine($"Unique frames: {speedscopeFile.GetUniqueFrameCount()}");
Console.WriteLine($"Average duration per event: {speedscopeFile.GetAverageDurationPerEvent():F2}ms");
Console.WriteLine($"Max profile duration: {speedscopeFile.GetMaxProfileDuration():F2}ms");

// Check profile types
Console.WriteLine($"Has evented profiles: {speedscopeFile.HasEventedProfiles()}");
Console.WriteLine($"Has sampled profiles: {speedscopeFile.HasSampledProfiles()}");

// Get all profile names
foreach (var profileName in speedscopeFile.GetProfileNames())
{
    Console.WriteLine($"Profile: {profileName}");
}

// Find the hottest frame (most time spent)
var hottestFrame = speedscopeFile.GetHottestFrame();
if (hottestFrame.HasValue)
{
    var (frameIndex, cumulativeTime) = hottestFrame.Value;
    var frame = speedscopeFile.GetFrame(frameIndex);
    Console.WriteLine($"Hottest frame: {frame?.Name ?? "Unknown"} ({cumulativeTime:F2}ms cumulative time)");
}

// Access individual frames
var mainFrame = speedscopeFile.GetFrame(0);
if (mainFrame != null)
{
    Console.WriteLine($"Frame 0: {mainFrame.Name} at {mainFrame.File}:{mainFrame.Line}");
}
```

## SpeedscopeFile

Represents the root object model for a speedscope file format, which is the standard JSON format produced by `dotnet-trace convert --format speedscope`. This class contains all the metadata and profile data needed to render flame graphs and treemaps.



The `SpeedscopeFile` class includes:
- **Schema**: Optional JSON schema reference
- **Shared**: Shared frame data across all profiles
- **Profiles**: List of profile data (evented or sampled)
- **Name**: Optional name for the trace
- **Exporter**: Optional exporter information





Example usage when working with the raw model directly:

```csharp
using System.Text.Json;
using SkiaFlameGraph.Core.Models;

// Load a speedscope file
var json = await File.ReadAllTextAsync("app.speedscope.json");
var speedscopeFile = JsonSerializer.Deserialize<SpeedscopeFile>(json);

// Access shared frame data
var frame = speedscopeFile.Shared.Frames[0];
Console.WriteLine($"Frame: {frame.Name}, File: {frame.File}, Line: {frame.Line}");

// Access profile data
foreach (var profile in speedscopeFile.Profiles)
{
    Console.WriteLine($"Profile: {profile.Name}, Type: {profile.Type}, Unit: {profile.Unit}");
    
    if (profile.Events != null)
    {
        // Evented profile - stream of open/close frame events
        foreach (var ev in profile.Events)
        {
            Console.WriteLine($"  Event at {ev.At}: {ev.Type} frame {ev.Frame}");
        }
    }
    else if (profile.Samples != null)
    {
        // Sampled profile - stacks with weights
        Console.WriteLine($"  First sample: [{string.Join(", ", profile.Samples[0])}]");
        if (profile.Weights != null)
        {
            Console.WriteLine($"  Weight: {profile.Weights[0]}");
        }
    }
}

// Create a minimal speedscope file programmatically
var newFile = new SpeedscopeFile
{
    Name = "My Application Trace",
    Exporter = "SkiaFlameGraph",
    Shared = new SharedData
    {
        Frames = new List<Frame>
        {
            new Frame { Name = "Main", File = "Program.cs", Line = 10 },
            new Frame { Name = "ProcessRequest", File = "ApiController.cs", Line = 45 }
        }
    },
    Profiles = new List<Profile>
    {
        new Profile
        {
            Type = "evented",
            Name = "CPU Profile",
            Unit = "milliseconds",
            StartValue = 0,
            EndValue = 1000,
            Events = new List<ProfileEvent>
            {
                new ProfileEvent { Type = "O", Frame = 0, At = 0 },
                new ProfileEvent { Type = "O", Frame = 1, At = 100 },
                new ProfileEvent { Type = "C", Frame = 1, At = 200 },
                new ProfileEvent { Type = "C", Frame = 0, At = 1000 }
            }
        }
    }
};

// Serialize back to JSON
var jsonOutput = JsonSerializer.Serialize(newFile, new JsonSerializerOptions { WriteIndented = true });
await File.WriteAllTextAsync("output.speedscope.json", jsonOutput);
```

## RenderOptionsExtensions

Provides extension methods for fluently configuring `RenderOptions` and performing
common calculations needed during rendering. These methods allow you to chain
configuration calls and compute layout dimensions based on your rendering requirements.

Example usage:

```csharp
using SkiaFlameGraph.Core.Rendering;

// Fluent configuration with extension methods
var options = new RenderOptions { Width = 1200, RowHeight = 20f }
    .WithWidth(1920)
    .WithRowHeight(24f)
    .WithBackground(new SKColor(0x28, 0x28, 0x2e));

// Calculate dimensions for rendering
int totalHeight = options.CalculateTotalHeight(45);
int contentWidth = options.CalculateContentWidth();

// Check rendering conditions
bool shouldLabel = options.ShouldLabelFrame(40f);
bool shouldRender = options.ShouldRenderFrame(2f);

// Get padding values
float[] padding2D = options.GetPadding();
float[] padding4D = options.GetPaddingAllSides();
```

## HotFunctionsReport

Aggregates FlameNode self-time by frame name across the tree and produces a top-N list of the hottest functions. Call `ToText()` to get a formatted text report showing self-time, total time, and percentage for each function.

Example usage:

```csharp
using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Reporting;

// Parse the trace and build the flame graph
var root = SpeedscopeParser.ParseFile("app.speedscope.json");

// Generate the hot functions report
var report = new HotFunctionsReport(root);

// Output the top 10 functions by self-time
Console.WriteLine(report.ToText(10));
```

## FlameNode

Represents a node in the aggregated call tree used by both the flame graph and treemap renderers. Each node tracks a frame's name, source location, accumulated time across the subtree, and its position in the call hierarchy. The tree is built by recursively merging identical frames so recursive calls collapse into wider boxes rather than tall stacks.

Example usage when constructing a call tree programmatically:

```csharp
using SkiaFlameGraph.Core.Models;

// Create a root node for the main thread
var root = new FlameNode("Main")
{
    File = "Program.cs",
    Line = 10,
    Value = 1000.0,
    Depth = 0
};

// Add child frames with timing information
var processRequest = root.AddChild("ProcessRequest", "ApiController.cs", 45)
{
    Value = 800.0
};

var dbQuery = processRequest.AddChild("QueryDatabase", "UserRepository.cs", 120)
{
    Value = 600.0
};

var renderView = processRequest.AddChild("RenderView", "HomeController.cs", 50)
{
    Value = 200.0
};

// Add another branch
var apiCall = root.AddChild("ExternalApiCall", "HttpClient.cs", 200)
{
    Value = 150.0
};

// Calculate metrics
Console.WriteLine($"Total tree depth: {root.MaxDepth()}");
Console.WriteLine($"Node count: {root.Children.Count + 1}");
Console.WriteLine($"Self value of ProcessRequest: {processRequest.SelfValue:F2}");

// Traverse the tree
foreach (var child in root.Children)
{
    Console.WriteLine($"Frame: {child.Name}, Value: {child.Value:F2}ms, Depth: {child.Depth}");
}
```

## FlameNodeExtensions

Provides utility methods for querying and analyzing the flame graph node tree structure. These methods allow you to search for specific nodes, calculate metrics, traverse the tree, and extract information about the call stack hierarchy.

Example usage when analyzing call stacks:

```csharp
using SkiaFlameGraph.Core.Models;
using System;
using System.Linq;

// Parse a speedscope file to get the flame graph root node
var root = SpeedscopeParser.ParseFile("app.speedscope.json");

// Find a specific node by name in the call tree
var mainNode = root.FindByName("Main");
if (mainNode != null)
{
    Console.WriteLine($"Found Main node with value: {mainNode.Value}");
}

// Get all nodes at a specific depth level (0 = root)
var nodesAtDepth2 = root.GetNodesAtDepth(2);
Console.WriteLine($"Nodes at depth 2: {nodesAtDepth2.Count()}");

// Calculate the sum of values for nodes matching a condition
var totalLibraryTime = root.SumValuesWhere(node => node.Name.Contains("Library"));
Console.WriteLine($"Total time in library code: {totalLibraryTime:F2}ms");

// Get all leaf nodes (nodes without children)
var leafNodes = root.GetLeafNodes();
Console.WriteLine($"Leaf nodes count: {leafNodes.Count()}");

// Calculate what percentage a specific node contributes to the total
var mainPercentage = root.CalculatePercentageOfTotal(mainNode);
Console.WriteLine($"Main contributes {mainPercentage:P2} of total time");

// Get the path from root to a specific node
var pathToNode = root.GetPathToNode(mainNode);
Console.WriteLine($"Path to Main: {string.Join(" → ", pathToNode)}");

// Find the deepest node in the tree
var deepestNode = root.GetDeepestNode();
Console.WriteLine($"Deepest node: {deepestNode.Name} at depth {deepestNode.GetDepth()}");

// Calculate cumulative value from a node up to the root
var cumulativeValue = mainNode.GetCumulativeValueToRoot();
Console.WriteLine($"Cumulative value to root: {cumulativeValue:F2}ms");

// Get the value of a node
var nodeValue = mainNode.Value;
Console.WriteLine($"Node value: {nodeValue:F2}ms");
```

## FramePaletteTests

xUnit test suite covering `FramePalette`, the deterministic frame-name to `SKColor` mapper used by both renderers. These tests pin down the palette's core guarantees: the same frame name always resolves to the same colour, different names resolve to different colours, and every returned colour falls within a valid range. They also cover the optional highlight-pattern behaviour, where a frame whose name matches the pattern gets a distinct colour while non-matching frames keep the standard one.

Example usage when exercising the suite programmatically:

```csharp
using SkiaFlameGraph.Tests;

// Instantiate the test suite and verify the colour-mapping guarantees
var tests = new FramePaletteTests();

// Deterministic mapping
tests.ForFrame_SameName_ReturnsSameColor();
tests.ForFrame_DifferentNames_ReturnsDifferentColors();
tests.ForFrame_ReturnsValidColorRange();

// Highlight-pattern behaviour
tests.ForFrame_WithHighlightPattern_Match_ReturnsValidColor();
tests.ForFrame_WithHighlightPattern_NoMatch_ReturnsSameAsStandard();
tests.ForFrame_WithHighlightPattern_Match_ReturnsDifferentColorThanStandard();
```

Or run just this suite from the CLI:

```bash
dotnet test --filter "FullyQualifiedName~SkiaFlameGraph.Tests.FramePaletteTests"
```

## FlameNodeTests

xUnit test suite covering `FlameNode`, the aggregated call-tree node used by both the flame graph and treemap renderers. These tests pin down `AddChild` semantics - children land at the correct depth, identically named siblings stay distinct nodes instead of being merged, and source file/line metadata is carried onto the child - as well as `MaxDepth` for both a lone leaf node and a three-level tree.

Example usage when exercising the suite programmatically:

```csharp
using SkiaFlameGraph.Tests;

// Instantiate the test suite and verify FlameNode behaviour
var tests = new FlameNodeTests();

// AddChild builds the call tree correctly
tests.AddChild_CreatesChildWithCorrectDepth();
tests.AddChild_WithSameName_CreatesDistinctNodes();
tests.AddChild_WithFileAndLine_SetsMetadata();

// MaxDepth measures tree height
tests.MaxDepth_LeafNode_ReturnsZero();
tests.MaxDepth_ThreeLevelTree_ReturnsTwo();
```

Or run just this suite from the CLI:

```bash
dotnet test --filter "FullyQualifiedName~SkiaFlameGraph.Tests.FlameNodeTests"
```

## ChromeTraceParserTests

xUnit test suite covering `ChromeTraceParser`, which turns Chrome Trace event JSON (`traceEvents`) into the aggregated call tree consumed by the renderers. These tests pin down how complete ("X") events and nested begin/end ("B"/"E") events become frames - including source file/line metadata, per-thread grouping via `tid`, and ordering by timestamp - and how malformed input behaves: events without a `tid` or without a matching close are ignored or skipped, while an empty events array or null deserialization throws.

Example usage when exercising the suite programmatically:

```csharp
using SkiaFlameGraph.Tests;

// Instantiate the test suite and verify Chrome Trace parsing behaviour
var tests = new ChromeTraceParserTests();

// Complete ("X") events
tests.SimpleCompleteEvents_AreParsedCorrectly();
tests.CompleteEvents_WithFileAndLine_AreParsedCorrectly();

// Nested begin/end ("B"/"E") events, threads and ordering
tests.NestedBeginEndEvents_BuildProperCallTree();
tests.MultipleThreads_AreGroupedCorrectly();
tests.Events_AreSortedByTimestamp();

// Malformed / ignorable input
tests.EventsWithoutTid_AreIgnored();
tests.NonCompleteBeginEndEvents_AreSkipped();
tests.EmptyEventsArray_Throws();
tests.NullDeserialization_Throws();
```

Or run just this suite from the CLI:

```bash
dotnet test --filter "FullyQualifiedName~SkiaFlameGraph.Tests.ChromeTraceParserTests"
```

## TreemapRendererJsonExtensionsTests
xUnit test suite covering the JSON serialization and deserialization extensions for `TreemapRenderer`. These tests verify argument null handling, compact JSON output, and proper exception throwing for invalid or empty JSON input.

Example usage when exercising the suite programmatically:

```csharp
using SkiaFlameGraph.Tests;
var tests = new TreemapRendererJsonExtensionsTests();
tests.ToJson_NullValue_ThrowsArgumentNullException();
tests.ToJson_IndentsFalse_ReturnsCompactJson();
tests.FromJson_NullJson_ThrowsArgumentNullException();
tests.FromJson_EmptyOrWhitespaceJson_ThrowsJsonException();
tests.TryFromJson_NullJson_ThrowsArgumentNullException();
tests.TryFromJson_InvalidJson_ThrowsJsonException();
```

Or run just this suite from the CLI:

```bash
dotnet test --filter "FullyQualifiedName~SkiaFlameGraph.Tests.TreemapRendererJsonExtensionsTests"
```

## TreemapRendererTests

xUnit test suite covering the `TreemapRenderer` class, which renders flame graph data as treemaps using SkiaSharp. These tests verify constructor behavior, rendering with various inputs (null, empty, single node, trees), depth guard protection, custom height parameters, PNG output functionality, and option-dependent output variations.

Example usage when exercising the suite programmatically:

```csharp
using SkiaFlameGraph.Tests;

// Instantiate the test suite and verify TreemapRenderer behaviour
var tests = new TreemapRendererTests();

// Constructor behavior
tests.Constructor_WithNullOptions_UsesDefaultOptions();
tests.Constructor_WithValidOptions_UsesProvidedOptions();

// Rendering with various inputs
tests.Render_WithNullRoot_ThrowsArgumentNullException();
tests.Render_WithEmptyRoot_ReturnsValidImage();
tests.Render_WithSingleNode_ReturnsImageWithCorrectDimensions();
tests.Render_WithTreeWithChildren_ReturnsValidImage();
tests.Render_WithDeepTree_DepthGuardPreventsStackOverflow();
tests.Render_WithZeroWidthNode_DoesNotThrow();
tests.Render_WithCustomHeightParameter_ReturnsImageWithCustomHeight();

// PNG output functionality
tests.RenderToPng_WithNullRoot_ThrowsArgumentNullException();
tests.RenderToPng_WithNullPath_ThrowsArgumentNullException();
tests.RenderToPng_WithEmptyPath_ThrowsArgumentException();
tests.RenderToPng_WithValidInput_CreatesPngFile();
tests.RenderToPng_WithCustomHeight_CreatesPngWithCorrectDimensions();

// Additional rendering tests
tests.Render_WithLargeTree_ReturnsValidImage();
tests.Render_WithDifferentOptions_ProducesDifferentOutput();
```

Or run just this suite from the CLI:

```bash
dotnet test --filter "FullyQualifiedName~SkiaFlameGraph.Tests.TreemapRendererTests"
```

## SpeedscopeFileTests
xUnit test suite covering the `SpeedscopeFile` class, which represents the root object model for the speedscope file format. These tests verify correct deserialization from JSON, property getters and setters for Schema, Name, and Exporter, initialization of Shared and Profiles collections, and frame property behaviors including nullability of File, Line, and Col fields.

Example usage when exercising the suite programmatically:

```csharp
using SkiaFlameGraph.Tests;
var tests = new SpeedscopeFileTests();
tests.Deserialize_WithValidJson_ReturnsPopulatedFile();
tests.Deserialize_WithMinimalJson_ReturnsValidFile();
tests.Schema_GetAndSet_ReturnsExpectedValue();
tests.Schema_SetToNull_ReturnsNull();
tests.Name_GetAndSet_ReturnsExpectedValue();
tests.Name_SetToNull_ReturnsNull();
tests.Exporter_GetAndSet_ReturnsExpectedValue();
tests.Exporter_SetToNull_ReturnsNull();
tests.Shared_Get_ReturnsInitializedInstance();
tests.Shared_Set_ReplacesInstance();
tests.Profiles_Get_ReturnsInitializedList();
tests.Profiles_AddItems_ListContainsItems();
tests.Profiles_Clear_ListIsEmpty();
tests.Shared_Frames_Get_ReturnsInitializedList();
tests.Shared_Frames_AddItems_ListContainsItems();
tests.Frame_Name_Get_ReturnsEmptyStringByDefault();
tests.Frame_Name_Set_ReturnsExpectedValue();
tests.Frame_File_CanBeNull();
tests.Frame_Line_CanBeNull();
tests.Frame_Col_CanBeNull();
```

Or run just this suite from the CLI:

```bash
dotnet test --filter "FullyQualifiedName~SkiaFlameGraph.Tests.SpeedscopeFileTests"
```

## SpeedscopeFileJsonExtensionsTests
xUnit test suite covering the JSON serialization and deserialization extensions for `SpeedscopeFile`. These tests verify that `ToJson` produces valid JSON strings with proper formatting and camelCase property names, that `FromJson` correctly deserializes valid JSON and handles edge cases like null or invalid input, and that `TryFromJson` provides a safe parsing alternative that returns success/failure status without throwing exceptions.

Example usage when exercising the suite programmatically:

```csharp
using SkiaFlameGraph.Tests;
var tests = new SpeedscopeFileJsonExtensionsTests();
tests.ToJson_WithValidSpeedscopeFile_ReturnsJsonString();
tests.ToJson_WithIndentedTrue_ReturnsFormattedJson();
tests.ToJson_WithNullValue_ThrowsArgumentNullException();
tests.FromJson_WithValidJson_ReturnsSpeedscopeFile();
tests.FromJson_WithNullJson_ThrowsArgumentNullException();
tests.FromJson_WithInvalidJson_ThrowsJsonException();
tests.FromJson_WithEmptyJson_ReturnsSpeedscopeFileWithDefaults();
tests.TryFromJson_WithValidJson_ReturnsTrueAndDeserializes();
tests.TryFromJson_WithInvalidJson_ReturnsFalseAndNull();
tests.TryFromJson_WithNullJson_ThrowsArgumentNullException();
tests.RoundtripSerialization_ProducesEquivalentObject();
tests.RoundtripSerialization_WithTryFromJson_ProducesEquivalentObject();
tests.ToJson_ProducesCamelCasePropertyNames();
tests.FromJson_WithMinimalValidJson_ReturnsSpeedscopeFile();
tests.TryFromJson_WithEmptyObject_ReturnsTrueWithEmptyFile();
```

Or run just this suite from the CLI:

```bash
dotnet test --filter "FullyQualifiedName~SkiaFlameGraph.Tests.SpeedscopeFileJsonExtensionsTests"
```