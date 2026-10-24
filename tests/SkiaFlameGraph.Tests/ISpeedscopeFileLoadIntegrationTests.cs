namespace SkiaFlameGraph.Tests;

/// <summary>
/// Declares the integration test cases for loading speedscope files from disk.
/// </summary>
public interface ISpeedscopeFileLoadIntegrationTests
{
    void ValidProfile_LoadsExpectedTotals();
    void EmptyFile_ThrowsJsonException();
    void BomAndCrlf_LoadIdenticallyToLf();
    void LargeFile_ExceedingFiveMegabytes_LoadsWithinTimeBound();
    void MissingFile_ThrowsFileNotFoundWithPath();
    void LoadedProfile_RendersToImage();
    void LoadedProfile_RendersToPngOnDisk();
}
