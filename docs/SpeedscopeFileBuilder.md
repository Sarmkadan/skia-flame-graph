# SpeedscopeFileBuilder

`SpeedscopeFileBuilder` is a fluent builder for `SpeedscopeFile`. It lets you assemble a
speedscope document step by step (schema, name, exporter, frames, profiles) and then
produce an immutable-by-convention `SpeedscopeFile` with `Build()`. It is useful in tests and
in tools that convert other profiler formats into speedscope JSON.

The builder is a `sealed` class with a public parameterless constructor. Every setter
returns the same builder instance, so calls chain. `Build()` can be called at any point and
always returns a non-null `SpeedscopeFile`; properties that were never set get their defaults.

## Builder pattern

1. Create a builder with `new SpeedscopeFileBuilder()`, or seed it from an existing file with
   `SpeedscopeFileBuilder.From(template)`.
2. Chain the `With*` setters you need. Each one validates its argument and stores it.
3. Call `Build()` to create the `SpeedscopeFile`.

## API

### `public SpeedscopeFileBuilder()`
Creates a builder with no values set.  
*Purpose*: Starting point for a new document.  
*Returns*: A new builder.  
*Throws*: Never.

### `public static SpeedscopeFileBuilder From(SpeedscopeFile template)`
Creates a builder pre-populated with the values of an existing file.  
*Purpose*: Derive a modified copy of a document without editing the original properties.  
*Returns*: A new builder. `Profiles` is copied into a new list. `Shared` is **not** copied:
the builder keeps a reference to the same `SharedData` instance (see Notes).  
*Throws*: `ArgumentNullException` if `template` is `null`.

### `public SpeedscopeFileBuilder WithSchema(string? schema)`
Sets the `$schema` property.  
*Purpose*: Identifies the JSON schema the document conforms to.  
*Returns*: This builder.  
*Throws*: `ArgumentNullException` if `schema` is `null`. Passing `null` is rejected even though the parameter type is nullable.

### `public SpeedscopeFileBuilder WithShared(SharedData shared)`
Replaces the `SharedData` object (frames and other shared data).  
*Purpose*: Supply a fully prepared shared-data object.  
*Returns*: This builder.  
*Throws*: `ArgumentNullException` if `shared` is `null`.

### `public SpeedscopeFileBuilder WithProfiles(List<Profile> profiles)`
Sets the profile collection.  
*Purpose*: Provide the evented or sampled profiles the document contains.  
*Returns*: This builder. The list is stored by reference, not copied.  
*Throws*: `ArgumentNullException` if `profiles` is `null`.

### `public SpeedscopeFileBuilder WithName(string? name)`
Sets the display name of the document.  
*Purpose*: Title shown by tools that open the file.  
*Returns*: This builder.  
*Throws*: `ArgumentNullException` if `name` is `null`.

### `public SpeedscopeFileBuilder WithExporter(string? exporter)`
Sets the exporter identifier.  
*Purpose*: Records which tool produced the file.  
*Returns*: This builder.  
*Throws*: `ArgumentNullException` if `exporter` is `null`.

### `public SpeedscopeFileBuilder WithFrames(List<Frame> frames)`
Sets `Shared.Frames`. If no `SharedData` has been set yet, a new one is created.  
*Purpose*: Provide the frame table that profile events refer to by index.  
*Returns*: This builder. The list is stored by reference, not copied.  
*Throws*: `ArgumentNullException` if `frames` is `null`.

### `public SpeedscopeFile Build()`
Creates a `SpeedscopeFile` from the current values.  
*Purpose*: Produce the final document.  
*Returns*: A new `SpeedscopeFile`. Unset `Shared` becomes a new empty `SharedData`; unset
`Profiles` becomes an empty list. `Schema`, `Name` and `Exporter` stay `null` if never set.  
*Throws*: Never.

## Example

```csharp
using SkiaFlameGraph.Core.Models;

var file = new SpeedscopeFileBuilder()
    .WithSchema("https://www.speedscope.app/file-format-schema.json")
    .WithName("Sample trace")
    .WithExporter("docs-example")
    .WithFrames(new List<Frame>
    {
        new() { Name = "Main", File = "Program.cs", Line = 10 },
        new() { Name = "Work", File = "Program.cs", Line = 20 },
    })
    .WithProfiles(new List<Profile>
    {
        new()
        {
            Name = "main",
            Type = "evented",
            Unit = "milliseconds",
            StartValue = 0,
            EndValue = 30,
            Events = new List<ProfileEvent>
            {
                new() { Type = "O", Frame = 0, At = 0 },   // open Main
                new() { Type = "O", Frame = 1, At = 5 },   // open Work
                new() { Type = "C", Frame = 1, At = 25 },  // close Work
                new() { Type = "C", Frame = 0, At = 30 },  // close Main
            },
        },
    })
    .Build();

// Derive a variant from the first document, changing only the name.
var renamed = SpeedscopeFileBuilder.From(file)
    .WithName("Sample trace (renamed)")
    .Build();
```

The example compiles against the current `SkiaFlameGraph.Core` and produces a document with
two frames and one evented profile. The derived `renamed` file has the same frames and profiles.

## Notes

- **Reference semantics.** `WithFrames`, `WithProfiles` and `WithShared` store the objects you
  pass in, without copying them. Changing the list afterwards changes the built document.
- **`From` shares frames.** `From` copies the profile list but reuses the same `SharedData`
  instance. Calling `WithFrames` on a builder created with `From` therefore also changes the
  frames of the template file. To keep the template unchanged, pass a new frame list through
  `WithShared(new SharedData { Frames = newFrames })` instead, or copy the frames first.
- **`Build` shares state.** Each `Build()` call returns a file that refers to the builder's
  current `SharedData` and profile list. Changing one built file's frames or profiles also
  changes any other file built from the same builder.
- **Nulls.** Setters reject `null` with `ArgumentNullException`, so a value cannot be cleared
  by passing `null`. Create a new builder if you need a document without a given property.
- **Validation.** The builder does not check the content of the document. For example, it
  does not verify that event frame indices are in range. Use `SpeedscopeFileValidation` for that.
