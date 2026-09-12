namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Interface for the speedscope file model.
/// </summary>
public interface ISpeedscopeFile
{
    /// <summary>
    /// Gets or sets the schema version.
    /// </summary>
    string? Schema { get; set; }

    /// <summary>
    /// Gets or sets the shared data.
    /// </summary>
    SharedData Shared { get; set; }

    /// <summary>
    /// Gets or sets the profiles.
    /// </summary>
    List<Profile> Profiles { get; set; }

    /// <summary>
    /// Gets or sets the name of the file.
    /// </summary>
    string? Name { get; set; }

    /// <summary>
    /// Gets or sets the exporter.
    /// </summary>
    string? Exporter { get; set; }
}
