# ParseLimits Class

## Responsibilities

`ParseLimits` holds the resource limits that `SpeedscopeParser` enforces while
reading a speedscope document. Use it when the input is untrusted, for example a
trace uploaded to a dashboard service. Each limit is checked before the work it
guards, so an oversized or pathological document is rejected with a
`ParseLimitExceededException` instead of exhausting memory or the call stack.

`ParseLimits` is an options object: create it with an object initializer and pass
it to a `SpeedscopeParser` entry point. Passing `null` (or omitting the argument)
uses `ParseLimits.Default`.

## Properties

| Property | Type | Default | Limit applies to |
|----------|------|---------|------------------|
| `MaxFileBytes` | `long` | `256 MiB` (`268435456`) | Size of the document in bytes. For `ParseFile` this is the file length; for `Deserialize(string)` it is the UTF-8 byte count. Checked before the document is read. |
| `MaxFrames` | `int` | `100,000` | Entries in `shared.frames`. |
| `MaxEvents` | `int` | `1,000,000` | Entries in a single profile's `events` or `samples` array. |
| `MaxStackDepth` | `int` | `1,024` | Simultaneously open frames for evented profiles (the depth of the resulting call tree), and the length of the longest stack for sampled profiles. |

Every property is `init`-only, so a `ParseLimits` instance does not change after
it is created. `ParseLimits.Default` is a shared instance with the defaults above.

All limits must be greater than zero; a zero or negative value is rejected with
`ArgumentOutOfRangeException`. To disable a limit in trusted scenarios, set it to
the largest value of its type (for example `int.MaxValue`).

### `public static ParseLimits Default { get; }`
The limits used when no explicit `ParseLimits` is supplied.  
*Returns*: A shared, read-only instance with the default values listed above.  
*Throws*: Never.

## Entry points that accept limits

| Method | Parameter |
|--------|-----------|
| `SpeedscopeParser.Deserialize(string json, ParseLimits? limits = null)` | `MaxFileBytes`, `MaxFrames`, `MaxEvents` |
| `SpeedscopeParser.ParseFile(string path, CancellationToken, ParseLimits? limits = null)` | all four |
| `SpeedscopeParser.ParseFile(string path, out ParseDiagnostics, CancellationToken, Action<string>?, ParseLimits? limits = null)` | all four |
| `SpeedscopeParser.BuildTree(SpeedscopeFile, int, CancellationToken, ParseLimits? limits = null)` | `MaxFrames`, `MaxEvents`, `MaxStackDepth` |
| `SpeedscopeParser.BuildTree(SpeedscopeFile, out ParseDiagnostics, int, CancellationToken, Action<string>?, ParseLimits? limits = null)` | `MaxFrames`, `MaxEvents`, `MaxStackDepth` |
| `SpeedscopeParser.BuildTreeWithWarnings(SpeedscopeFile, int, ParseLimits? limits = null)` | `MaxFrames`, `MaxEvents`, `MaxStackDepth` |

`MaxStackDepth` is enforced while the tree is built, so `Deserialize` accepts a
document with deeper stacks and the error is raised by the tree-building call.

## Exceptions

`ParseLimitExceededException` derives from `FormatException`, so existing handlers
for malformed input also catch it. It exposes the limit that was exceeded:

| Member | Type | Description |
|--------|------|-------------|
| `LimitName` | `string` | Name of the `ParseLimits` property, such as `"MaxStackDepth"`. |
| `Limit` | `long` | The configured limit. |
| `Actual` | `long` | The observed value that exceeded it. |

## Notes

- `MaxFileBytes` bounds the memory used by the document object model, because the
  whole document is deserialized before the frame and event counts are checked.
- Frame and event counts are checked before any call tree is built from them, so
  they bound the work done on the tree.
- JSON nesting is separately bounded by `System.Text.Json` (default depth 64),
  which surfaces as `JsonException`.
- The defaults are chosen as conservative starting points. They have not been
  benchmarked against a particular memory budget; tune them for your service.

## Usage

```csharp
using SkiaFlameGraph.Core.Parsing;

var limits = new ParseLimits
{
    MaxFileBytes = 64 * 1024 * 1024,
    MaxFrames = 50_000,
    MaxEvents = 500_000,
    MaxStackDepth = 512,
};

try
{
    var root = SpeedscopeParser.ParseFile(uploadedPath, limits: limits);
}
catch (ParseLimitExceededException ex)
{
    // ex.LimitName, ex.Limit and ex.Actual describe what was rejected.
}
```

## Thread Safety

`ParseLimits` instances are immutable after construction and can be shared across
threads. The parser methods do not modify the limits they receive.
