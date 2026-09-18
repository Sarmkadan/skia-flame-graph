# CLAUDE.md

SkiaFlameGraph: a .NET 10 class library that parses profiler output (speedscope, Chrome trace, collapsed stacks) and renders flame graphs, treemaps and differential flame graphs to PNG/SVG with SkiaSharp - no browser required.

## Build

```bash
dotnet build SkiaFlameGraph.slnx
dotnet build src/SkiaFlameGraph.Core          # library only
dotnet run --project samples/SkiaFlameGraph.Sample -- [trace.speedscope.json] [outDir]
dotnet run --project samples/SkiaFlameGraph.DifferentialSample
```

Requires .NET SDK 10.x. On headless Linux the samples reference `SkiaSharp.NativeAssets.Linux.NoDependencies`; install `fontconfig` + a TTF font if labels render blank.

## Test

```bash
dotnet test                                    # whole solution (xUnit 2.9, coverlet)
dotnet test tests/SkiaFlameGraph.Tests
dotnet test --filter "FullyQualifiedName~FlameNodeTests"
```

`aider_buildcmd.py` is just a wrapper that runs `dotnet test` from the repo root.

## Lint / format

No analyzers, `.editorconfig` or CI configured. `Nullable` and `ImplicitUsings` are enabled in every project; keep code warning-free under those.

## Layout

```
SkiaFlameGraph.slnx                     solution (src, tests, samples/Sample)
src/SkiaFlameGraph.Core/
  Models/        FlameNode call tree, Speedscope DTOs, builders, validation, JSON extensions
  Parsing/       SpeedscopeParser (evented + sampled), ChromeTraceParser, CollapsedStacksParser
  Rendering/     FlameGraphRenderer, TreemapRenderer, DifferentialFlameGraphRenderer,
                 FlameGraphSvgExporter, CollapsedStacksExporter, RenderOptions, FramePalette
  Reporting/     HotFunctionsReport (top-N self/total time)
  FlameDiff.cs   baseline vs current tree diff
  JsonDefaults.cs shared JsonSerializerOptions
tests/SkiaFlameGraph.Tests/             xUnit tests (one file per class + companion files)
samples/SkiaFlameGraph.Sample/          renders flame.png + treemap.png from a speedscope file
samples/SkiaFlameGraph.DifferentialSample/  diff demo (not listed in the .slnx)
samples/sample-trace.speedscope.json    bundled input used when no path is given
docs/                                   per-class markdown docs (FlameNode.md, RenderOptions.md, ...)
```

Entry points: `SpeedscopeParser.ParseFile(path)` -> `FlameNode` root -> `new FlameGraphRenderer(options).RenderToPng(root, path)` / `.Render(root)` (returns `SKImage`).

Stray files at repo root (`}`, `renderer.RenderToPng(...)`, `test_unbalanced_events.csx`, `.aider*`) and `tests/SkiaFlameGraph.Core/` are leftovers not referenced by any project; do not build on them.

## Conventions

- Namespaces mirror folders: `SkiaFlameGraph.Core.Models`, `.Parsing`, `.Rendering`, `.Reporting`; tests in `SkiaFlameGraph.Tests`.
- File-scoped namespaces, XML doc comments on all public members, `ArgumentNullException.ThrowIfNull` for guards.
- Each public class `X` typically has an interface `IX`, plus partial/companion files: `XExtensions.cs`, `XValidation.cs`, `XJsonExtensions.cs`, `XConstants.cs`, `X.Additions.cs`, `XBuilder.cs`. Follow the same split when adding to a class.
- Tests mirror that scheme: `XTests.cs` implements `IXTests`, magic values live in `XTestsConstants.cs`, helpers in `XTestsExtensions.cs`. Test names are `Method_Scenario_Expectation` style (`AddChild_CreatesChildWithCorrectDepth`).
- Frame colours are deterministic (FNV-1a hash of frame name) - do not introduce randomness in rendering.
- Rendered output (`flame.png`, `treemap.png`, `out/`) and traces are gitignored; only `samples/sample-trace.speedscope.json` is tracked.
- Public API changes should be reflected in `README.md` and the matching `docs/*.md`.
