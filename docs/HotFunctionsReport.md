# Hot Functions Report

The `HotFunctionsReport` class (in `src/SkiaFlameGraph.Core/Reporting/HotFunctionsReport.cs`) 
identifies hot functions by aggregating the self-time (time spent in a function excluding its children) 
for each unique function name across the entire flame graph tree.

## How it works

1. The report is constructed by traversing the flame graph tree (starting at the root node).
2. For each node, if the node's self-value (self-time) is greater than zero, it is added to the aggregation for that function name.
3. The aggregation sums the self-time and total-time (self-time plus children's time) for each function.
4. After traversal, the functions are sorted by self-time in descending order.
5. Each function's percentage is calculated as (function self-time / total self-time of all functions) * 100.

## Output format

The report can be rendered as a text table via the `ToText(int topN = 10)` method.

The table includes:

   - Rank
   - Function name (truncated to 29 characters)
   - Self time (time spent in the function itself)
   - Total time (time spent in the function and its children)
   - Percentage of total self-time

Example output:

```
Hot Functions Report
===================

#   Name                          Self      Total     %
--  ----------------------------- --------- --------- --------
 1  my_function                   120.50    150.00    60.25%
 2  another_function              80.00     90.00     40.00%
... (5 more functions)

Total: 200.50 units across 7 functions
```

If `topN` is specified (positive), only the top N functions are shown, with a note indicating how many more are omitted.
If `topN` is zero or negative, all functions are shown.

## Implementation details

The class implements `IHotFunctionsReport` and provides:
   - `Functions`: read-only list of hot functions sorted by self-time (descending)
   - `TotalSelfTime`: the sum of self-time across all functions

Each hot function is represented by a `HotFunction` object with:
   - `Name`: the function name
   - `Self`: self-time (updated during aggregation)
   - `Total`: total-time (updated during aggregation)
   - `Percent`: calculated percentage (read-only, depends on the report's total self-time)

Note: The `HotFunction.Percent` property uses the report's total self-time (set during construction) 
        to calculate the percentage.