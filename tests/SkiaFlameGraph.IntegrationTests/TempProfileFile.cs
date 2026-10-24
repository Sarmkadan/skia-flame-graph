using System.Text;
using static SkiaFlameGraph.IntegrationTests.CollapsedStacksParserIntegrationTestsConstants;

namespace SkiaFlameGraph.IntegrationTests;

/// <summary>
/// Writes lines to a UTF-8 file (no BOM) in its own temporary directory and deletes that directory on disposal.
/// </summary>
internal sealed class TempProfileFile : IDisposable
{
    private readonly string _directory;

    /// <summary>
    /// Creates the file with the given lines.
    /// </summary>
    /// <param name="lines">Lines to write, one per line.</param>
    public TempProfileFile(IEnumerable<string> lines)
    {
        _directory = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "skia-flame-collapsed-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_directory);
        FilePath = System.IO.Path.Combine(_directory, ProfileFileName);
        File.WriteAllLines(FilePath, lines, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    /// <summary>
    /// Full path of the written file.
    /// </summary>
    public string FilePath { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
