internal static class FlameNodeValidationConstants
{
    // Repeated string literals (URLs, header names, config keys, format strings)
    public const string NameCannotBeNullOrWhitespace = "Name cannot be null or whitespace.";
    public const string ValueMustBePositive = "Value must be positive.";
    public const string DepthCannotBeNegative = "Depth cannot be negative.";
    public const string ChildrenCollectionCannotBeNull = "Children collection cannot be null.";
    public const string ChildrenCollectionContainsNullElement = "Children collection contains a null element.";
    public const string LineMustBePositiveInteger = "Line, if specified, must be a positive integer.";
    public const string FileCannotBeEmptyOrWhitespace = "File, if specified, cannot be empty or whitespace.";
    public const string ChildWeightInvariantViolated = "Child weight invariant violated at {0}: sum of children ({1}) exceeds parent value ({2}) by {3}.";
    public const string FlameNodeGraphContainsCycle = "FlameNode graph contains a cycle.";
}
