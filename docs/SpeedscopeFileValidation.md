# SpeedscopeFile Validation Rules

This document describes the validation rules implemented in the `SpeedscopeFileValidation` static class for `SpeedscopeFile` instances in the Skia Flame Graph library.

## Overview

`SpeedscopeFileValidation` provides extension methods that check a deserialized speedscope document for missing required fields, empty optional strings, out-of-range values, malformed event streams, and resource limits. Validation does not modify the file. It collects every problem it finds, so a single call reports all issues rather than stopping at the first one.

The checks are structural and semantic. They do not parse the document into a flame graph; `SpeedscopeParser` performs that step separately.

## Validation Methods

### `Validate(this SpeedscopeFile value)`

Validates a `SpeedscopeFile` instance and returns a list of human-readable problems.

**Returns:** `IReadOnlyList<string>` - Empty list if the file is valid; otherwise, one message per problem, in the order the checks run (schema, shared data, profiles, name, exporter).

**Exceptions:** Throws `ArgumentNullException` if `value` is null.

### `IsValid(this SpeedscopeFile value)`

Determines whether a `SpeedscopeFile` instance is valid.

**Returns:** `true` if `Validate` returns no problems; otherwise, `false`.

**Exceptions:** Throws `ArgumentNullException` if `value` is null. Unlike `FlameNodeValidation.IsValid`, a null argument is not reported as `false`.

### `EnsureValid(this SpeedscopeFile value)`

Throws if the file is invalid.

**Exceptions:**
- `ArgumentNullException` if `value` is null.
- `ArgumentException` if one or more problems were found. The message begins with `SpeedscopeFile is invalid. Problems:` and lists each problem on its own line.

## Validation Rules

The rules below are listed in the order `Validate` applies them. Message text is shown in the form the library produces, with placeholders in braces.

### 1. Schema (optional)
- **Rule:** If `Schema` is set, it must not be an empty string. `null` is allowed.
- **Message:** `SpeedscopeFile.Schema must not be an empty string.`

### 2. Shared data (required)
- **Rule:** `Shared` must not be null. The rules in section 3 apply to its contents.
- **Message:** `SpeedscopeFile.Shared is required and cannot be null.`

### 3. Frame table (`Shared.Frames`)
- **Rule:** `Frames` must not be null.
  - **Message:** `SharedData.Frames is required and cannot be null.`
- **Rule:** `Frames` must contain at least one entry.
  - **Message:** `SharedData.Frames must contain at least one frame.`
- **Rule:** `Frames.Count` must not exceed `MaxFrames` (100,000). See [Resource limits](#resource-limits).
  - **Message:** `SharedData.Frames contains {count} frames, which exceeds the allowed maximum of {MaxFrames}.`

Each frame is then checked individually. Its index is shown as `{i}`.

- **Null entry:** A `null` frame is reported and the remaining checks for that frame are skipped.
  - **Message:** `Frame at index {i} is null.`
- **Name (required):** `Name` must not be null.
  - **Message:** `Frame at index {i}.Name is required and cannot be null.`
- **Name (non-empty):** If `Name` is set, it must not be an empty string.
  - **Message:** `Frame at index {i}.Name must not be an empty string.`
- **File (optional):** If `File` is set, it must not be an empty string. `null` is allowed.
  - **Message:** `Frame at index {i}.File must not be an empty string.`
- **Line (optional, non-negative):** If `Line` has a value, it must be zero or greater.
  - **Message:** `Frame at index {i}.Line must be non-negative, but was {line}.`
- **Col (optional, non-negative):** If `Col` has a value, it must be zero or greater.
  - **Message:** `Frame at index {i}.Col must be non-negative, but was {col}.`

### 4. Profiles (`Profiles`)
- **Rule:** `Profiles` must not be null.
  - **Message:** `SpeedscopeFile.Profiles is required and cannot be null.`
- **Rule:** `Profiles` must contain at least one profile.
  - **Message:** `SpeedscopeFile.Profiles must contain at least one profile.`

Each profile is then checked individually. Its index is shown as `{i}`.

- **Null entry:** A `null` profile is reported and its remaining checks are skipped.
  - **Message:** `Profile at index {i} is null.`
- **Type (required):** `Type` must not be null.
  - **Message:** `Profile at index {i}.Type is required and cannot be null.`
- **Type (value):** If `Type` is set, it must be exactly `evented` or `sampled`. An empty string gets its own message.
  - **Message (empty):** `Profile at index {i}.Type must not be an empty string.`
  - **Message (other):** `Profile at index {i}.Type must be either 'evented' or 'sampled', but was '{type}'.`
- **Unit (required):** `Unit` must not be null.
  - **Message:** `Profile at index {i}.Unit is required and cannot be null.`
- **Unit (non-empty):** If `Unit` is set, it must not be an empty string.
  - **Message:** `Profile at index {i}.Unit must not be an empty string.`
- **StartValue:** Must be zero or greater.
  - **Message:** `Profile at index {i}.StartValue must be non-negative, but was {value}.`
- **EndValue:** Must be greater than or equal to `StartValue`.
  - **Message:** `Profile at index {i}.EndValue ({end}) must be greater than or equal to StartValue ({start}).`
- **Required collection by type:**
  - An `evented` profile must have a non-null `Events`.
    - **Message:** `Profile at index {i} of type 'evented' must have an Events collection.`
  - A `sampled` profile must have a non-null `Samples`.
    - **Message:** `Profile at index {i} of type 'sampled' must have a Samples collection.`
  - A profile with any other `Type` (including a null one) is not checked for either collection.

A profile's `Events`, `Samples`, and `Weights` are validated whenever they are non-null, regardless of `Type`. The rules for each are in sections 5, 6, and 7.

### 5. Events (`Profile.Events`)

This section applies to each non-null `Events` collection.

- **Rule:** The collection must not be empty when present.
  - **Message:** `Profile.Events must contain at least one event when not null.`
- **Rule:** The collection must not contain more than `MaxEvents` (1,000,000) entries. The limit is applied per profile. See [Resource limits](#resource-limits).
  - **Message:** `Profile at index {i}.Events contains {count} events, which exceeds the allowed maximum of {MaxEvents}.`

Each event is then checked individually. Its index is shown as `{i}`.

- **Null entry:** A `null` event is reported and skipped. It does not change the nesting depth.
  - **Message:** `ProfileEvent at index {i} is null.`
- **Type (required):** `Type` must not be null.
  - **Message:** `ProfileEvent at index {i}.Type is required and cannot be null.`
- **Type (value):** If `Type` is set, it must be `O` (open) or `C` (close). An empty string gets its own message.
  - **Message (empty):** `ProfileEvent at index {i}.Type must not be an empty string.`
  - **Message (other):** `ProfileEvent at index {i}.Type must be either 'O' or 'C', but was '{type}'.`
- **Stack depth:** For each valid `O` or `C` event, depth is increased on `O` and decreased on `C`. Depth is tracked across the whole collection, starting at zero.
  - **Negative depth:** A `C` event that brings depth below zero is reported. Every later event that leaves depth below zero is reported too.
    - **Message:** `ProfileEvent at index {i} closes more frames than opened (negative stack depth).`
  - **Maximum depth:** An event that brings depth above `MaxStackDepth` (10,000) is reported.
    - **Message:** `ProfileEvent at index {i} exceeds maximum allowed stack depth of {MaxStackDepth}. Current depth: {depth}.`
  - Events with a null or invalid `Type` do not change depth.
- **Frame index:** `Frame` must be at least zero and less than the number of entries in `Shared.Frames`. See [Frame index references](#frame-index-references).
  - **Message (negative):** `ProfileEvent at index {i}.Frame must be non-negative, but was {frame}.`
  - **Message (out of range):** `ProfileEvent at index {i}.Frame references out-of-range frame index {frame} (max {max}).`
- **At (non-negative):** `At` must be zero or greater.
  - **Message:** `ProfileEvent at index {i}.At must be non-negative, but was {at}.`
- **Balance at end:** After all events are processed, depth must be zero. A non-zero final depth means some opened frames were never closed.
  - **Message:** `Profile.Events ends with a non-zero stack depth ({depth}); open events were not properly closed.`

### 6. Samples (`Profile.Samples`)

This section applies to each non-null `Samples` collection.

- **Rule:** The collection must not be empty when present.
  - **Message:** `Profile.Samples must contain at least one sample when not null.`

Each sample is a list of frame indices, ordered from the outermost frame to the innermost. Its index is shown as `[{i}]`.

- **Null sample:** A `null` sample is reported and skipped.
  - **Message:** `Profile.Samples[{i}] is null.`
- **Non-empty sample:** A sample must contain at least one frame index.
  - **Message:** `Profile.Samples[{i}] must contain at least one frame index.`
- **Frame index:** Each entry, shown as `[{i}][{j}]`, must be at least zero and less than the number of entries in `Shared.Frames`. See [Frame index references](#frame-index-references).
  - **Message (negative):** `Profile.Samples[{i}][{j}] contains a negative frame index ({frame}).`
  - **Message (out of range):** `Profile.Samples[{i}][{j}] references out-of-range frame index {frame} (max {max}).`

Unlike events, the sample collection has no `MaxEvents` check in this validator. The parser enforces its own limits; see the `ParseLimits` documentation.

### 7. Weights (`Profile.Weights`)

This section applies to each non-null `Weights` collection.

- **Rule:** The collection must not be empty when present.
  - **Message:** `Profile.Weights must contain at least one weight when not null.`
- **Rule:** Each weight must be zero or greater. Zero is allowed.
  - **Message:** `Profile.Weights[{i}] must be non-negative, but was {weight}.`

Weights are not checked against the length of `Samples`. That correspondence is not enforced here.

### 8. Name and Exporter (optional)
- **Rule:** If `Name` is set, it must not be an empty string. `null` is allowed.
  - **Message:** `SpeedscopeFile.Name must not be an empty string.`
- **Rule:** If `Exporter` is set, it must not be an empty string. `null` is allowed.
  - **Message:** `SpeedscopeFile.Exporter must not be an empty string.`

## Frame index references

Event `Frame` values and sample entries are indices into `Shared.Frames`. The valid range is `0` to `Shared.Frames.Count - 1`.

If `Shared` or `Shared.Frames` is null, the validator treats the frame count as zero. In that case every non-negative frame index is reported as out of range, and the `max` value in the message is `-1`. Fix the `Shared` problem first. The index messages after it are a consequence of that problem, not separate defects.

## Resource limits

The validator enforces three limits. They protect callers that validate untrusted input from documents that are valid JSON but too large or deeply nested to process safely.

| Constant | Value | Applies to |
|----------|-------|------------|
| `MaxFrames` | `100,000` | Entries in `Shared.Frames`, checked once per file. |
| `MaxEvents` | `1,000,000` | Entries in a single profile's `Events`, checked per profile. |
| `MaxStackDepth` | `10,000` | Open/close nesting depth while walking one `Events` collection. |

These values are declared as `private const` fields in `SpeedscopeFileValidation`. The `internal` class `SpeedscopeFileValidationConstants` declares the same three names and values in a separate file, but the validator does not read from it. If you change one set, change the other.

The `MaxStackDepth` used here is separate from `ParseLimits.MaxStackDepth`, which defaults to `1,024` and is enforced when the parser builds the call tree. A file can pass `Validate` and still be rejected by the parser.

## Notes

- All checks are read-only. Validation does not fix, reorder, or remove any data.
- Message text is intended for people reading logs and error output. Do not match on exact text in code; check `IsValid` or the list length instead.
- `Validate` returns a read-only list, so callers cannot add to or remove from the results.
