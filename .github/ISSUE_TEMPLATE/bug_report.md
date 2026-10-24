---
name: Bug report
about: Report a problem with parsing, rendering, or exporting
labels: bug
---

## Description

A clear and concise description of the bug.

## Steps to reproduce

1. Input used (format and source: speedscope / Chrome trace / collapsed stacks, or a minimal sample)
2. Code or command used (e.g. `SpeedscopeParser.ParseFile(...)`, `FlameGraphRenderer.RenderToPng(...)`)
3. Options that differ from defaults (`RenderOptions`)
4. See the error or wrong output

## Expected behavior

What you expected to happen.

## Actual behavior

What actually happened, including exception messages and stack traces if any.

## Environment

- OS: (e.g. Ubuntu 24.04, Windows 11, macOS 15)
- .NET version: (output of `dotnet --version`, e.g. 10.0.100)
- SkiaSharp version: (e.g. 3.119.0) and native assets package, if any (e.g. `SkiaSharp.NativeAssets.Linux.NoDependencies`)
- SkiaFlameGraph version or commit: (e.g. `main` @ `abc1234`)

## Screenshots / flame graph sample

If the bug is visual, attach the rendered PNG/SVG. If possible, also attach a minimal input file that reproduces the problem (do not include sensitive data from your profiles).
