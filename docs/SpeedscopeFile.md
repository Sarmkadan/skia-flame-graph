# SpeedscopeFile

`SpeedscopeFile` represents the root object of a speedscope JSON document.  
It contains metadata about the profiling run, shared data that can be referenced by
individual profiles, and a collection of profiles that describe the recorded
samples or events. The type is used by the parser (`SpeedscopeParser`) and can be
serialized back to JSON via the provided extension methods.

## API

### `public string? Schema`
The `$schema` property of the speedscope file.  
*Purpose*: Indicates the JSON schema URL that validates the document.  
*Returns*: The schema URL as a string, or `null` if not present.  
*Throws*: Never.

### `public SharedData Shared`
Shared data that can be referenced by profiles.  
*Purpose*: Holds information such as shared frames that are common to all profiles.  
*Returns*: An instance of `SharedData`.  
*Throws*: Never.

### `public List<Profile> Profiles`
The list of profiling runs contained in the file.  
*Purpose*: Each `Profile` describes either a sampled or evented profile.  
*Returns*: A mutable `List<Profile>`; the list may be empty but never `null`.  
*Throws*: Never.

### `public string? Name`
Optional human‑readable name for the speedscope document.  
*Purpose*: Provides a title that tools may display.  
*Returns*: The name string or `null` if omitted.  
*Throws*: Never.

### `public string? Exporter`
Optional identifier of the tool that generated the file.  
*Purpose*: Allows consumers to recognise the source of the profiling data.  
*Returns*: The exporter string or `null`.  
*Throws*: Never.

### `public List<Frame> Frames`
All frames defined in the document.  
*Purpose*: Frames are the building blocks referenced by profiles and events.  
*Returns*: A mutable list of `Frame` objects; never `null`.  
*Throws*: Never.

---

#### `public class Frame`

| Member | Type | Description |
|--------|------|-------------|
| `public string Name` | `string` | The display name of the frame (e.g., method name). |
| `public string? File` | `string?` | Source file path associated with the frame, if known. |
| `public int? Line` | `int?` | Source line number, if known. |
| `public int? Col` | `int?` | Source column number, if known. |
| `public string Type` | `string` | The type of frame (e.g., `"function"`). |

#### `public class Profile`

| Member | Type | Description |
|--------|------|-------------|
| `public string Name` | `string` | Name of the profile (e.g., thread name). |
| `public string Unit` | `string` | Unit of measurement for timestamps (e.g., `"seconds"`). |
| `public double StartValue` | `double` | Start timestamp of the profile. |
| `public double EndValue` | `double` | End timestamp of the profile. |
| `public List<ProfileEvent>? Events` | `List<ProfileEvent>?` | List of evented profile entries; `null` for sampled profiles. |
| `public List<List<int>>? Samples` | `List<List<int>>?` | Sampled stack traces; each inner list contains frame indices. |
| `public List<double>? Weights` | `List<double>?` | Weight for each sample; `null` defaults to `1.0`. |
| `public string Type` | `string` | `"evented"` or `"sampled"` indicating the profile kind. |
| `public int Frame` | `int` | Index of the frame that represents the profile itself (used for hierarchical profiles). |

#### `public class ProfileEvent`

| Member | Type | Description |
|--------|------|-------------|
| `public string Type` | `string` | Event type (e.g., `"O"` for open, `"C"` for close). |
| `public int Frame` | `int` | Index into the `Frames` collection identifying the frame the event belongs to. |

---

## Usage

### Example 1 – Loading a speedscope file and enumerating frames

