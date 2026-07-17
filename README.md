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
