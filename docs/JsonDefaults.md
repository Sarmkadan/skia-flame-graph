# JsonDefaults Class

## Responsibilities

`JsonDefaults` exposes one shared `JsonSerializerOptions` instance, `JsonDefaults.Options`,
that the library uses whenever it reads or writes JSON. Keeping the configuration in one
place means every JSON entry point (for example `HotFunctionsReport` serialization) uses the
same naming, number and cycle rules, so output does not drift between call sites.

The class has no instance members and cannot be constructed.

## Properties

### Options
- **Type**: `JsonSerializerOptions`
- **Description**: The shared default options. Initialized once when the type is first accessed.
- **Remarks**: The instance is mutable and shared across the process. Do not change its
  properties. To customize behavior for one call, copy it first:

  ```csharp
  var options = new JsonSerializerOptions(JsonDefaults.Options)
  {
      WriteIndented = true,
  };
  ```

  `HotFunctionsReportExtensions.ToJson` uses this pattern to toggle `WriteIndented`.

## Configured Defaults

`Options` starts from `JsonSerializerDefaults.Web` and then overrides or sets the following:

| Setting | Value | Why |
|---------|-------|-----|
| Base preset | `JsonSerializerDefaults.Web` | Web preset: case-insensitive property matching on read, camelCase naming, and numbers accepted as JSON strings on read. |
| `PropertyNamingPolicy` | `JsonNamingPolicy.CamelCase` | Written explicitly so output stays camelCase even if the preset changes. |
| `WriteIndented` | `false` | Compact output by default, since whitespace only adds size. Callers that want readable output opt in on their own copy. |
| `TypeInfoResolver` | `new DefaultJsonTypeInfoResolver()` | Uses reflection-based metadata for types without source-generated contexts. Set explicitly so the options work without the `System.Text.Json` source generator. |
| `ReferenceHandler` | `ReferenceHandler.IgnoreCycles` | Prevents serialization from throwing or recursing forever when an object graph contains a cycle. `FlameNode` links each child back to its `Parent`, which forms such a cycle if the node graph is serialized. With `IgnoreCycles`, the repeated reference is written as `null` instead. |

### Notes on the preset

- `PropertyNameCaseInsensitive` comes from `JsonSerializerDefaults.Web`. It lets input such as
  `"Name"` and `"name"` both bind to the same property when reading.
- `NumberHandling` comes from the preset. It lets numeric properties be read from JSON strings
  (for example `"42"`), which some exporters produce.

## Behavior Notes

- **Shared state**: Because `Options` is one instance, changing it affects every caller in the
  process. Always copy it before modifying (see the example above).
- **Cycles are dropped, not reported**: `IgnoreCycles` writes `null` where a cycle is detected.
  Code that reads the output back will not see the back-reference, so do not rely on
  `Parent` being present after a round trip.
- **Trust**: The options do not add input limits. Size and depth limits for untrusted input are
  handled by `ParseLimits` in the parsers, not by the serializer.

## Usage

```csharp
using System.Text.Json;
using SkiaFlameGraph.Core;

// Read with the shared defaults
HotFunctionsReport? report = JsonSerializer.Deserialize<HotFunctionsReport>(json, JsonDefaults.Options);

// Write with a per-call copy that adds indentation
var indented = new JsonSerializerOptions(JsonDefaults.Options) { WriteIndented = true };
string text = JsonSerializer.Serialize(report, indented);
```

## See Also

- `HotFunctionsReportExtensions` - `ToJson`, `FromJson` and `TryFromJson` built on these defaults.
- [ParseLimits](ParseLimits.md) - limits applied to untrusted speedscope input.
