namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Interface for the speedscope file model.
/// </summary>
public interface ISpeedscopeFile
{
    string? Schema { get; set; }
    SharedData Shared { get; set; }
    List<Profile> Profiles { get; set; }
    string? Name { get; set; }
    string? Exporter { get; set; }
}