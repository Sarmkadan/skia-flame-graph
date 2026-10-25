namespace SkiaFlameGraph.Core.Models;

internal static class SpeedscopeFileConstants
{
    /// <summary>
    /// JSON property name of the top-level <c>$schema</c> URI that identifies the speedscope file format version.
    /// </summary>
    public const string JsonPropertyNameSchema = "$schema";

    /// <summary>
    /// JSON property name of the top-level <c>shared</c> object holding data (such as the frame table) shared by all profiles.
    /// </summary>
    public const string JsonPropertyNameShared = "shared";

    /// <summary>
    /// JSON property name of the top-level <c>profiles</c> array; each entry is one profile (evented or sampled) in the file.
    /// </summary>
    public const string JsonPropertyNameProfiles = "profiles";

    /// <summary>
    /// JSON property name of the <c>name</c> field, used for the file or an individual profile's display name.
    /// </summary>
    public const string JsonPropertyNameName = "name";

    /// <summary>
    /// JSON property name of the optional <c>exporter</c> field recording which tool produced the file.
    /// </summary>
    public const string JsonPropertyNameExporter = "exporter";

    /// <summary>
    /// JSON property name of the <c>frames</c> array inside <c>shared</c>; events and samples reference frames by index into it.
    /// </summary>
    public const string JsonPropertyNameFrames = "frames";

    /// <summary>
    /// JSON property name of a frame's <c>file</c> field, the source file path where the function is defined.
    /// </summary>
    public const string JsonPropertyNameFile = "file";

    /// <summary>
    /// JSON property name of a frame's <c>line</c> field, the source line number of the function.
    /// </summary>
    public const string JsonPropertyNameLine = "line";

    /// <summary>
    /// JSON property name of a frame's <c>col</c> field, the source column number of the function.
    /// </summary>
    public const string JsonPropertyNameCol = "col";

    /// <summary>
    /// JSON property name of a profile's <c>type</c> discriminator, which selects the evented or sampled layout.
    /// </summary>
    public const string JsonPropertyNameType = "type";

    /// <summary>
    /// JSON property name of a profile's <c>unit</c> field, the unit in which its values and timestamps are expressed.
    /// </summary>
    public const string JsonPropertyNameUnit = "unit";

    /// <summary>
    /// JSON property name of an evented profile's <c>startValue</c>, the timestamp at which the profile begins.
    /// </summary>
    public const string JsonPropertyNameStartValue = "startValue";

    /// <summary>
    /// JSON property name of an evented profile's <c>endValue</c>, the timestamp at which the profile ends.
    /// </summary>
    public const string JsonPropertyNameEndValue = "endValue";

    /// <summary>
    /// JSON property name of an evented profile's <c>events</c> array of open/close frame events.
    /// </summary>
    public const string JsonPropertyNameEvents = "events";

    /// <summary>
    /// JSON property name of a sampled profile's <c>samples</c> array; each sample is a stack of frame indices.
    /// </summary>
    public const string JsonPropertyNameSamples = "samples";

    /// <summary>
    /// JSON property name of a sampled profile's <c>weights</c> array, giving each sample's weight in the same order as <c>samples</c>.
    /// </summary>
    public const string JsonPropertyNameWeights = "weights";

    /// <summary>
    /// JSON property name of an event's <c>frame</c> field, the index of the frame being opened or closed.
    /// </summary>
    public const string JsonPropertyNameFrame = "frame";

    /// <summary>
    /// JSON property name of an event's <c>at</c> field, the timestamp at which the event occurs.
    /// </summary>
    public const string JsonPropertyNameAt = "at";
}
