/// <summary>
/// JSON property names used by FlameNodeJsonWriter when serializing a flame node.
/// </summary>
internal static class FlameNodeJsonWriterConstants
{
    /// <summary>
    /// JSON property name holding the node's display name (the frame or function name).
    /// </summary>
    public static readonly string NameKey = "name";

    /// <summary>
    /// JSON property name holding the node's numeric value (the sample weight or time attributed to the node).
    /// </summary>
    public static readonly string ValueKey = "value";
}
