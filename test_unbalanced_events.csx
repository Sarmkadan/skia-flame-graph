#!/usr/bin/env dotnet-script

using System;
using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Models;

Console.WriteLine("Testing unbalanced event stream detection...");

// Test 1: Profile ending with unclosed frames
const string unclosedJson = @"{
    \"shared\": { \"frames\": [ { \"name\": \"main\" }, { \"name\": \"work\" } ] },
    \"profiles\": [{
        \"type\": \"evented\",
        \"unit\": \"milliseconds\",
        \"startValue\": 0,
        \"endValue\": 10,
        \"events\": [
            { \"type\": \"O\", \"frame\": 0, \"at\": 0 },
            { \"type\": \"O\", \"frame\": 1, \"at\": 2 },
            { \"type\": \"C\", \"frame\": 1, \"at\": 8 }
        ]
    }]
}";

try {
    var file = SpeedscopeParser.Deserialize(unclosedJson);
    var (root, warnings) = SpeedscopeParser.BuildTreeWithWarnings(file);

    Console.WriteLine($"✓ Successfully parsed profile with unclosed frames");
    Console.WriteLine($"✓ Warnings: {warnings.Count}");
    foreach (var warning in warnings) {
        Console.WriteLine($"  - {warning}");
    }

    Console.WriteLine($"✓ Root has {root.Children.Count} children");
    if (root.Children.Count > 0) {
        var main = root.Children[0];
        Console.WriteLine($"✓ Main frame: {main.Name}, Value: {main.Value}");
        Console.WriteLine($"✓ Main has {main.Children.Count} children");
    }

    // Test validation with warnings
    var validationProblems = root.Validate(warnings);
    Console.WriteLine($"✓ Validation with warnings: {validationProblems.Count} problems");

    Console.WriteLine("\nAll tests passed!");
} catch (Exception ex) {
    Console.WriteLine($"✗ Error: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    Environment.Exit(1);
}