# SpeedscopeFileExtensions

Provides static extension methods for querying and aggregating data from a parsed Speedscope profile file. These methods offer convenient access to total durations, event counts, frame statistics, and profile metadata without requiring manual traversal of the underlying profile structure.

## API

### GetTotalDuration

```csharp
public static double GetTotalDuration(this SpeedscopeFile file)
```

Returns the total wall-clock duration of the profile in milliseconds. Computed by summing the durations of all contained profiles. Returns `0.0` if the file contains no profiles.

### GetTotalEventCount

```csharp
public static int GetTotalEventCount(this SpeedscopeFile file)
```

Returns the total number of events across all evented profiles in the file. Sampled profiles are not counted. Returns `0` if no evented profiles are present.

### GetUniqueFrameCount

```csharp
public static int GetUniqueFrameCount(this SpeedscopeFile file)
```

Returns the number of distinct frame names appearing across all profiles. Deduplication is performed by frame name, not by frame index. Returns `0` if no frames are present.

### GetFrame

```csharp
public static Frame? GetFrame(this SpeedscopeFile file, int frameIndex)
```

Retrieves the `Frame` object corresponding to the given zero-based `frameIndex` from the shared frame table. Returns `null` if `frameIndex` is out of range or if the frame table is empty.

**Parameters:**
- `frameIndex`: Zero-based index into the shared frame table.

**Throws:** No exceptions; out-of-range indices silently return `null`.

### GetProfileNames

```csharp
public static IEnumerable<string> GetProfileNames(this SpeedscopeFile file)
```

Enumerates the names of all profiles contained in the file. The enumeration is lazy and reflects the order in which profiles appear. Returns an empty sequence if no profiles are present.

### HasEventedProfiles

```csharp
public static bool HasEventedProfiles(this SpeedscopeFile file)
```

Returns `true` if the file contains at least one evented profile (a profile whose type records discrete start/end events). Returns `false` for files containing only sampled profiles or no profiles at all.

### HasSampledProfiles

```csharp
public static bool HasSampledProfiles(this SpeedscopeFile file)
```

Returns `true` if the file contains at least one sampled profile (a profile whose type records periodic stack samples). Returns `false` for files containing only evented profiles or no profiles at all.

### GetAverageDurationPerEvent

```csharp
public static double GetAverageDurationPerEvent(this SpeedscopeFile file)
```

Returns the mean duration per event across all evented profiles, in milliseconds. Computed as total evented duration divided by total event count. Returns `0.0` if there are no evented profiles or no events.

### GetHottestFrame

```csharp
public static (int FrameIndex, double CumulativeTime)? GetHottestFrame(this SpeedscopeFile file)
```

Identifies the frame with the highest cumulative self-time across all profiles. Returns a nullable tuple containing the frame's index and its total cumulative time in milliseconds. Returns `null` if no frames are present or no cumulative time data is available. When multiple frames tie for the highest time, the one with the lowest frame index is returned.

### GetMaxProfileDuration

```csharp
public static double GetMaxProfileDuration(this SpeedscopeFile file)
```

Returns the maximum duration among all individual profiles in the file, in milliseconds. Returns `0.0` if the file contains no profiles.

## Usage

### Example 1: Basic profile inspection

```csharp
SpeedscopeFile file = SpeedscopeFile.Load("trace.speedscope.json");

Console.WriteLine($"Total duration: {file.GetTotalDuration():F2} ms");
Console.WriteLine($"Profiles: {string.Join(", ", file.GetProfileNames())}");

if (file.HasEventedProfiles())
{
    Console.WriteLine($"Events: {file.GetTotalEventCount()}");
    Console.WriteLine($"Avg duration/event: {file.GetAverageDurationPerEvent():F4} ms");
}

if (file.HasSampledProfiles())
{
    Console.WriteLine($"Unique frames: {file.GetUniqueFrameCount()}");
}
```

### Example 2: Identifying and inspecting the hottest frame

```csharp
SpeedscopeFile file = SpeedscopeFile.Load("trace.speedscope.json");

var hottest = file.GetHottestFrame();
if (hottest is not null)
{
    var (frameIndex, cumulativeTime) = hottest.Value;
    Frame? frame = file.GetFrame(frameIndex);

    string frameName = frame?.Name ?? "<unknown>";
    Console.WriteLine($"Hottest frame: {frameName} (index {frameIndex})");
    Console.WriteLine($"Cumulative time: {cumulativeTime:F2} ms");

    double totalDuration = file.GetTotalDuration();
    double percentage = totalDuration > 0
        ? (cumulativeTime / totalDuration) * 100.0
        : 0.0;
    Console.WriteLine($"Percentage of total: {percentage:F1}%");
}
else
{
    Console.WriteLine("No frame data available.");
}
```

## Notes

- All duration values are returned in milliseconds and may include fractional components.
- Methods returning aggregate statistics (`GetTotalDuration`, `GetAverageDurationPerEvent`, `GetMaxProfileDuration`) return `0.0` rather than throwing when no relevant data exists.
- `GetTotalEventCount` and `GetAverageDurationPerEvent` operate exclusively on evented profiles; sampled profiles are ignored in these calculations.
- `GetHottestFrame` aggregates across both evented and sampled profiles where cumulative time data is available. The returned frame index refers to the shared frame table accessible via `GetFrame`.
- These methods perform read-only queries over immutable or effectively immutable data structures. They are safe to call from multiple threads concurrently without external synchronization, provided the underlying `SpeedscopeFile` instance is not being mutated during the call.
- `GetProfileNames` returns a lazy enumeration. If the underlying profile collection is modified during enumeration, behavior is undefined. Avoid concurrent modification while iterating.
