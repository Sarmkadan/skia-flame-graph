# HotFunctionsReport

Represents a collection of performance-critical functions identified during profiling, typically used to generate textual flame graph reports. This type aggregates hot function data for analysis and export purposes.

## API

### HotFunctionsReport()

Initializes a new instance of the HotFunctionsReport class. Constructs an empty report with no associated hot functions.

### ToText()

Converts the report contents into a formatted text representation suitable for output or logging.

**Returns:**  
`string` - A multi-line string containing the report data in a structured format.

### HotFunction

Gets the individual hot function entries contained within this report.

**Returns:**  
`HotFunction` - The hot function data structure (exact type definition not provided).

### Name

Gets the name identifier of the hot function.

**Returns:**  
`string` - The function name as recorded during profiling.

### Self

Gets the self-time metric for the hot function, representing time spent exclusively in this function excluding child calls.

**Returns:**  
`double` - The self-time value in seconds or milliseconds (unit context-dependent).

### Total

Gets the total-time metric for the hot function, representing cumulative time including all child function calls.

**Returns:**  
`double` - The total-time value in seconds or milliseconds (unit context-dependent).

## Usage

```csharp
// Create and populate a hot functions report
var report = new HotFunctionsReport();
report.Name = "MainLoop";
report.Self = 0.45;
report.Total = 1.23;

// Export to text format
Console.WriteLine(report.ToText());
```

```csharp
// Process multiple hot functions from a report
foreach (var function in report.HotFunction)
{
    Console.WriteLine($"{function.Name}: Self={function.Self}, Total={function.Total}");
}
```

## Notes

- Thread-safety: Instances of HotFunctionsReport are not thread-safe. Concurrent modifications to properties or the HotFunction collection may result in inconsistent state or exceptions.
- Edge cases: Calling ToText() on an uninitialized or partially constructed report may produce incomplete or malformed output. Ensure all required properties are set before export.
- The HotFunction property likely represents a collection or nested structure; exact behavior depends on its underlying type definition.
