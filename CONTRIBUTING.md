# Contributing to SkiaFlameGraph

Thanks for helping out. This is a small .NET library, so the process is light.

## Prerequisites

- **.NET SDK 10.x** (the projects target `net10.0`). Check with `dotnet --version`.
- **Linux only, headless:** the samples and tests use SkiaSharp. Install
  `fontconfig` and a TTF font if rendered labels come out blank.
  The samples reference `SkiaSharp.NativeAssets.Linux.NoDependencies`.

## Clone and build

```bash
git clone https://github.com/<owner>/skia-flame-graph.git
cd skia-flame-graph
dotnet build SkiaFlameGraph.slnx
```

To build only the library:

```bash
dotnet build src/SkiaFlameGraph.Core
```

## Run the tests

```bash
dotnet test                                    # whole solution
dotnet test tests/SkiaFlameGraph.Tests         # unit tests only
dotnet test --filter "FullyQualifiedName~FlameNodeTests"   # one class
```

Tests use xUnit 2.9 and coverlet.

## Coding conventions

- Follow the existing code. Namespaces mirror folders (`SkiaFlameGraph.Core.Models`, `.Parsing`, `.Rendering`, and so on). Use file-scoped namespaces.
- Every public member needs an XML doc comment.
- Guard public inputs with `ArgumentNullException.ThrowIfNull`.
- `Nullable` and `ImplicitUsings` are enabled everywhere. Keep the build free of warnings. Treat warnings as errors: a change with new warnings will not be merged.
- Keep rendering deterministic. Frame colours come from an FNV-1a hash of the frame name, so do not introduce randomness.
- Split classes the same way the existing code does: `XExtensions.cs`, `XValidation.cs`, `XJsonExtensions.cs`, `XConstants.cs`, `XBuilder.cs`. Tests follow `XTests.cs`, `XTestsConstants.cs`, and `XTestsExtensions.cs`.
- Name tests `Method_Scenario_Expectation`, for example `AddChild_CreatesChildWithCorrectDepth`.
- There is no `.editorconfig` or analyzer setup yet. Match the surrounding code's style, and do not reformat unrelated lines.

## Branch naming

Branch from `main` and use a short prefix plus a kebab-case description:

- `feature/<short-description>` for new functionality, e.g. `feature/collapsed-stacks-exporter`
- `fix/<short-description>` for bug fixes, e.g. `fix/treemap-zero-width-nodes`
- `docs/<short-description>` for documentation only
- `chore/<short-description>` for tooling and maintenance

## Commit messages

Use a [Conventional Commits](https://www.conventionalcommits.org/) style subject line
(`feat:`, `fix:`, `docs:`, `test:`, `refactor:`, `chore:`), at most about 72 characters, in the imperative mood:

```
fix: clamp negative frame widths in treemap layout

Frames with zero or negative totals were rendered as huge rectangles.
Clamp them to zero width and add a regression test.
```

Explain *why* in the body when the reason is not obvious from the diff.

## Pull request checklist

Before you open a PR, confirm that:

- [ ] `dotnet build SkiaFlameGraph.slnx` succeeds with no warnings.
- [ ] `dotnet test` passes locally.
- [ ] New behaviour and bug fixes have tests. A bug fix includes a test that failed before the fix.
- [ ] Public API changes are reflected in `README.md` and the matching `docs/*.md` file.
- [ ] The PR description explains what changed and why, and says how you tested it.
- [ ] No generated output (`flame.png`, `treemap.png`, `out/`, traces) or secrets are committed.

CI runs build and test on every PR to `main`. A PR is ready to review when CI is green.
