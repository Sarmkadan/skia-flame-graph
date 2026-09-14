# FlameNode Validation Rules

This document describes the validation rules implemented in the `FlameNodeValidation` partial class for `FlameNode` instances in the Skia Flame Graph library.

## Overview

The `FlameNodeValidation` class provides static extension methods to validate `FlameNode` objects, ensuring they meet the structural and semantic requirements of a flame graph node. Validation checks for null values, invalid ranges, collection integrity, and graph invariants.

## Validation Methods

### `Validate(this FlameNode value)`

Validates a `FlameNode` instance and returns a list of human-readable validation problems.

**Returns:** `IReadOnlyList<string>` - Empty list if the node is valid; otherwise, a list of error messages.

**Exceptions:** Throws `ArgumentNullException` if `value` is null.

### `Validate(this FlameNode value, IReadOnlyList<string>? parsingWarnings)`

Overload that combines structural validation with additional parsing warnings from event stream processing.

**Parameters:**
- `value`: The node to validate.
- `parsingWarnings`: Optional list of warnings from parsing the event stream.

**Returns:** `IReadOnlyList<string>` - Combined list of parsing warnings and structural validation problems.

**Exceptions:** Throws `ArgumentNullException` if `value` is null.

### `IsValid(this FlameNode? value)`

Determines whether a `FlameNode` instance is valid.

**Returns:** `true` if the node is valid (no validation problems); otherwise, `false`.

**Note:** Handles null input by returning `false`.

### `EnsureValid(this FlameNode? value)`

Ensures that a `FlameNode` instance is valid, throwing an exception if not.

**Exceptions:**
- `ArgumentNullException` if `value` is null.
- `ArgumentException` if the node is not valid, with a message detailing all validation problems.

### `EnsureValid(this FlameNode? value, IReadOnlyList<string>? parsingWarnings)`

Overload that includes parsing warnings in the validation check.

**Exceptions:**
- `ArgumentNullException` if `value` is null.
- `ArgumentException` if the node is not valid (including parsing warnings), with a message detailing all problems.

## Validation Rules

The following rules are checked during validation:

### 1. Name Validation
- **Rule:** `Name` cannot be null, empty, or consist only of whitespace.
- **Error Message:** `FlameNodeValidationConstants.NameCannotBeNullOrWhitespace`

### 2. Value Validation
- **Rule:** `Value` must be positive (greater than zero).
- **Error Message:** `FlameNodeValidationConstants.ValueMustBePositive`

### 3. Depth Validation
- **Rule:** `Depth` must be non-negative (greater than or equal to zero).
- **Error Message:** `FlameNodeValidationConstants.DepthCannotBeNegative`

### 4. Children Collection Validation
- **Rule:** The `Children` collection cannot be null.
- **Error Message:** `FlameNodeValidationConstants.ChildrenCollectionCannotBeNull`
- **Additional Rule:** The `Children` collection cannot contain null elements.
- **Error Message:** `FlameNodeValidationConstants.ChildrenCollectionContainsNullElement`

### 5. Line Validation (if present)
- **Rule:** If `Line` has a value (i.e., not null) and is an integer, it must be positive.
- **Error Message:** `FlameNodeValidationConstants.LineMustBePositiveInteger`

### 6. File Validation (if present)
- **Rule:** If `File` is provided and not null/whitespace, the trimmed string must not be empty.
- **Error Message:** `FlameNodeValidationConstants.FileCannotBeEmptyOrWhitespace`

### 7. Child Weight Invariant
- **Rule:** The sum of all children's `Value` properties must not exceed the parent node's `Value`.
- **Note:** Null children are excluded from the sum calculation.
- **Error Message:** 
  ```
  Child weight invariant violated at {path}: sum of children ({childSum}) exceeds parent value ({parentValue}) by {overage}.
  ```
  Where `{path}` is the node's path to root (using `GetPathToNode()`), `{childSum}` is the sum of children's values, `{parentValue}` is the parent's value, and `{overage}` is the difference.

### 8. Cycle Detection
- **Rule:** The node graph must not contain cycles (a node must not be its own ancestor).
- **Error Message:** `FlameNodeValidationConstants.FlameNodeGraphContainsCycle`

## Constants

Validation error messages are defined in `FlameNodeValidationConstants` (not shown in the provided files but referenced). Refer to the source code for the exact string values.

## Usage Examples

### Basic Validation
```csharp
var node = new FlameNode { Name = "Main", Value = 100, Depth = 0 };
var problems = node.Validate();
if (problems.Count > 0)
{
    // Handle validation problems
}
```

### Validation with Parsing Warnings
```csharp
var parsingWarnings = new List<string> { "Unexpected event type encountered" };
var problems = node.Validate(parsingWarnings);
// problems now contains both parsing warnings and structural validation issues
```

### Ensuring Validity
```csharp
try
{
    node.EnsureValid();
    // Node is valid, proceed with processing
}
catch (ArgumentException ex)
{
    // Log or display validation errors: ex.Message
}
```

## Implementation Notes

- The validation is implemented as a partial class, with additional methods in `FlameNodeValidation.Additions.cs`.
- The `Validate` method that takes `parsingWarnings` simply prepends those warnings to the list of structural validation problems.
- Cycle detection uses a depth-first search with a HashSet to track visited nodes on the current path.
- Child weight invariant validation skips null children when calculating the sum.
- All validation methods that take a `FlameNode` parameter throw `ArgumentNullException` if the node is null.