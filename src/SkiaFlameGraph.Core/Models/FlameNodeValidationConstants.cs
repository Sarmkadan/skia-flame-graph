internal static class FlameNodeValidationConstants
{
    // Repeated string literals (URLs, header names, config keys, format strings)
    /// <summary>
    /// Problem message reported when <c>FlameNode.Name</c> is null, empty, or whitespace-only.
    /// </summary>
    public const string NameCannotBeNullOrWhitespace = "Name cannot be null or whitespace.";

    /// <summary>
    /// Problem message reported when <c>FlameNode.Value</c> is negative. Zero is allowed.
    /// </summary>
    public const string ValueCannotBeNegative = "Value cannot be negative.";

    /// <summary>
    /// Problem message reported when <c>FlameNode.Depth</c> is negative.
    /// </summary>
    public const string DepthCannotBeNegative = "Depth cannot be negative.";

    /// <summary>
    /// Problem message reported when <c>FlameNode.Children</c> is null. The collection itself must always be present, even if empty.
    /// </summary>
    public const string ChildrenCollectionCannotBeNull = "Children collection cannot be null.";

    /// <summary>
    /// Problem message reported when the <c>FlameNode.Children</c> collection contains a null entry. Validation stops scanning at the first null element.
    /// </summary>
    public const string ChildrenCollectionContainsNullElement = "Children collection contains a null element.";

    /// <summary>
    /// Problem message reported when <c>FlameNode.Line</c> is specified but is zero or negative. Line numbers are 1-based, so the value must be at least 1 when present.
    /// </summary>
    public const string LineMustBePositiveInteger = "Line, if specified, must be a positive integer.";

    /// <summary>
    /// Problem message reported when <c>FlameNode.File</c> is non-null but consists only of whitespace. A null value is allowed and means "no file".
    /// </summary>
    public const string FileCannotBeEmptyOrWhitespace = "File, if specified, cannot be empty or whitespace.";

    /// <summary>
    /// Format string for the child weight invariant message: the sum of child values must not exceed the parent's <c>FlameNode.Value</c>.
    /// Placeholders: {0} = node path, {1} = children sum, {2} = parent value, {3} = overage.
    /// Note: the validator currently builds this message with string interpolation, so this constant is not referenced by <c>FlameNodeValidation</c>.
    /// </summary>
    public const string ChildWeightInvariantViolated = "Child weight invariant violated at {0}: sum of children ({1}) exceeds parent value ({2}) by {3}.";

    /// <summary>
    /// Problem message reported when the node graph reachable from a <c>FlameNode</c> contains a reference cycle (a node is its own ancestor).
    /// </summary>
    public const string FlameNodeGraphContainsCycle = "FlameNode graph contains a cycle.";
}
