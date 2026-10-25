# RenderOptions Validation Rules

This document describes the validation rules implemented in the `RenderOptionsExtensionsValidation` class for `RenderOptions` instances in the Skia Flame Graph library. The error message text and the `MaxWidth` limit come from the internal `RenderOptionsValidationConstants` class.

## Overview

`RenderOptionsExtensionsValidation` provides static extension methods that check a `RenderOptions` instance against the rules below. Every rule is checked on every call, so one call can report several problems. The renderers (`FlameGraphRenderer`, `TreemapRenderer`, `DifferentialFlameGraphRenderer`) call `EnsureValid()` before they render, so invalid options are rejected before any drawing starts.

## Validation Methods

### `Validate(this RenderOptions options)`

Validates the options and returns a list of human-readable error messages.

**Returns:** `IReadOnlyList<string>` - Empty list if the options are valid; otherwise, one message per failed rule, in the order the rules are listed below.

**Exceptions:** Throws `ArgumentNullException` if `options` is `null`.

### `IsValid(this RenderOptions options)`

Determines whether the options pass all rules.

**Returns:** `true` if `Validate` returns no messages; otherwise, `false`.

**Exceptions:** Throws `ArgumentNullException` if `options` is `null`.

### `EnsureValid(this RenderOptions options)`

Throws if the options fail any rule.

**Exceptions:**
- `ArgumentNullException` if `options` is `null`.
- `ArgumentException` if any rule fails. The parameter name is `options`. The message is the prefix `RenderOptions is invalid. ` followed by all error messages joined with single spaces.

## Validation Rules

Rules are listed in the order they are checked. Error messages use the invariant culture, and `{0}` is replaced with the rejected value.

### 1. Width
- **Rule:** `Width` must be greater than zero and no greater than `16384` (`MaxWidth`).
- **Error when `Width <= 0`:** `Width must be positive, but was {0}.`
- **Error when `Width > 16384`:** `Width must be at most 16384, but was {0}.`
- **Note:** The two width checks are mutually exclusive, so at most one width message is reported.

### 2. RowHeight
- **Rule:** `RowHeight` must be greater than zero.
- **Error:** `RowHeight must be positive, but was {0}.`

### 3. MinLabelWidth
- **Rule:** `MinLabelWidth` must not be negative. Zero is accepted.
- **Error:** `MinLabelWidth must be non-negative, but was {0}.`

### 4. MinBoxWidth
- **Rule:** `MinBoxWidth` must not be negative. Zero is accepted.
- **Error:** `MinBoxWidth must be non-negative, but was {0}.`

### 5. MinSubtreeWidthPx
- **Rule:** `MinSubtreeWidthPx` must not be negative. Zero is accepted.
- **Error:** `MinSubtreeWidthPx must be non-negative, but was {0}.`

### 6. Padding
- **Rule:** `Padding` must not be negative. Zero is accepted.
- **Error:** `Padding must be non-negative, but was {0}.`

### 7. FontSize
- **Rule:** `FontSize` must be greater than zero.
- **Error:** `FontSize must be positive, but was {0}.`

### 8. FontFamily
- **Rule:** `FontFamily` is optional. `null` is accepted. When a value is set, it must not be empty or whitespace-only.
- **Error:** `FontFamily must be non-empty when specified, but was '{0}'.`

### 9. HighlightPattern
- **Rule:** `HighlightPattern` is optional. `null` or an empty string skips the check. Otherwise the value must be accepted by the .NET `Regex` constructor with default options.
- **Error:** `HighlightPattern must be a valid regular expression, but was invalid: {0}`
- **Note:** Only the pattern's syntax is checked. Matching behaviour is not validated.

## Behaviour Notes

- **Accumulated errors:** Validation does not stop at the first failure. All failing rules are reported together, except that the two `Width` checks are mutually exclusive.
- **Zero and NaN:** Zero is accepted only where the rule says "non-negative". Comparisons with `NaN` are always false, so `NaN` passes every float check (`RowHeight`, `MinLabelWidth`, `MinBoxWidth`, `MinSubtreeWidthPx`, `Padding`, `FontSize`). Infinite values also pass the non-negative and positive checks.
- **Not validated:** `Background`, `TextColor`, and `Inverted` have no validation rules.
- **Duplicated limit:** The `16384` in the width error message is written out as literal text. It is not built from `MaxWidth`, so if `MaxWidth` changes, the message text must be updated by hand.

## Examples

### Valid options
```csharp
var options = new RenderOptions
{
    Width = 1200,
    RowHeight = 18f,
    FontSize = 11f
};

bool ok = options.IsValid();                 // true
IReadOnlyList<string> errors = options.Validate(); // empty
```

### Collecting all errors
```csharp
var options = new RenderOptions
{
    Width = 0,
    RowHeight = -1f,
    HighlightPattern = "[unclosed"
};

IReadOnlyList<string> errors = options.Validate();
// [
//   "Width must be positive, but was 0.",
//   "RowHeight must be positive, but was -1.",
//   "HighlightPattern must be a valid regular expression, but was invalid: [unclosed"
// ]
```

### Throwing on invalid options
```csharp
try
{
    options.EnsureValid();
}
catch (ArgumentException ex)
{
    // ex.Message begins with "RenderOptions is invalid. " followed by the
    // error messages separated by spaces. ex.ParamName is "options".
}
```
