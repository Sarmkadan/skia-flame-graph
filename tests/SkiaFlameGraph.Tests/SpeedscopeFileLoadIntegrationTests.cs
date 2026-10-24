using System.Diagnostics;
using System.Text;
using System.Text.Json;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Parsing;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Integration tests for the on-disk load pipeline: a real file is read through
/// <see cref="SpeedscopeParser.ParseFile(string, CancellationToken)"/>, aggregated into a
/// <see cref="FlameNode"/> tree, and handed to <see cref="FlameGraphRenderer"/>.
/// </summary>
[Collection("ProcessMemory")]
public class SpeedscopeFileLoadIntegrationTests : ISpeedscopeFileLoadIntegrationTests, IDisposable
{
    private const int LargeFileMinBytes = 5 * 1024 * 1024;
    private const int LargeFileSampleCount = 500_000;
    private const int LargeFileFrameCount = 50;
    private const int LargeFileTimeBoundMs = 5_000;

    private const string ValidJson = """
    {
      "shared": { "frames": [
        { "name": "a" }, { "name": "b" }, { "name": "c" }
      ]},
      "profiles": [{
        "type": "sampled",
        "unit": "milliseconds",
        "startValue": 0, "endValue": 10,
        "samples": [ [0, 1], [0, 1], [0, 2] ],
        "weights": [3, 2, 5]
      }]
    }
    """;

    private readonly string _tempDir;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpeedscopeFileLoadIntegrationTests"/> class
    /// and creates an isolated temporary directory for the test files.
    /// </summary>
    public SpeedscopeFileLoadIntegrationTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "skia-flame-load-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    /// <summary>
    /// Removes the temporary directory created for the test.
    /// </summary>
    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    /// <summary>
    /// Verifies that a valid profile loaded from disk yields the expected root total and per-frame totals.
    /// </summary>
    [Fact]
    public void ValidProfile_LoadsExpectedTotals()
    {
        var path = WriteBytes("valid.speedscope.json", Encoding.UTF8.GetBytes(ValidJson));

        var root = SpeedscopeParser.ParseFile(path);

        Assert.Equal(10, root.Value);
        var a = Assert.Single(root.Children);
        Assert.Equal("a", a.Name);
        Assert.Equal(10, a.Value);
        Assert.Equal(2, a.Children.Count);
        Assert.Equal(5, a.Children.Find(n => n.Name == "b")!.Value);
        Assert.Equal(5, a.Children.Find(n => n.Name == "c")!.Value);
    }

    /// <summary>
    /// Verifies that an empty file is rejected with a JSON parse error rather than producing an empty tree.
    /// </summary>
    [Fact]
    public void EmptyFile_ThrowsJsonException()
    {
        var path = WriteBytes("empty.speedscope.json", Array.Empty<byte>());

        Assert.ThrowsAny<JsonException>(() => SpeedscopeParser.ParseFile(path));
    }

    /// <summary>
    /// Verifies that a UTF-8 BOM and CRLF line endings produce exactly the same tree as the LF-only file.
    /// </summary>
    [Fact]
    public void BomAndCrlf_LoadIdenticallyToLf()
    {
        var lfPath = WriteBytes("lf.speedscope.json", Encoding.UTF8.GetBytes(ValidJson.Replace("\r\n", "\n")));

        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var crlfBody = Encoding.UTF8.GetBytes(ValidJson.Replace("\r\n", "\n").Replace("\n", "\r\n"));
        var bomCrlfPath = WriteBytes("bom-crlf.speedscope.json", bom.Concat(crlfBody).ToArray());

        var lfRoot = SpeedscopeParser.ParseFile(lfPath);
        var bomCrlfRoot = SpeedscopeParser.ParseFile(bomCrlfPath);

        AssertSameTree(lfRoot, bomCrlfRoot);
        Assert.Equal(10, bomCrlfRoot.Value);
    }

    /// <summary>
    /// Verifies that a file larger than 5 MB loads within the time bound and produces the expected total.
    /// </summary>
    [Fact]
    [Trait("Category", "Slow")]
    public void LargeFile_ExceedingFiveMegabytes_LoadsWithinTimeBound()
    {
        var path = WriteLargeProfile("large.speedscope.json");
        Assert.True(new FileInfo(path).Length > LargeFileMinBytes,
            $"generated file is {new FileInfo(path).Length} bytes; expected more than {LargeFileMinBytes}");

        var stopwatch = Stopwatch.StartNew();
        var root = SpeedscopeParser.ParseFile(path);
        stopwatch.Stop();

        Assert.True(stopwatch.ElapsedMilliseconds < LargeFileTimeBoundMs,
            $"loading took {stopwatch.ElapsedMilliseconds} ms; bound is {LargeFileTimeBoundMs} ms");
        Assert.Equal(LargeFileSampleCount, root.Value);
    }

    /// <summary>
    /// Verifies that a missing path raises <see cref="FileNotFoundException"/> naming the path,
    /// not an empty tree or a parse error.
    /// </summary>
    [Fact]
    public void MissingFile_ThrowsFileNotFoundWithPath()
    {
        var path = Path.Combine(_tempDir, "does-not-exist.speedscope.json");

        var ex = Assert.Throws<FileNotFoundException>(() => SpeedscopeParser.ParseFile(path));

        Assert.Equal(path, ex.FileName);
    }

    /// <summary>
    /// Verifies that the loaded profile can be rendered to an in-memory image with non-zero dimensions.
    /// </summary>
    [Fact]
    public void LoadedProfile_RendersToImage()
    {
        var path = WriteBytes("render.speedscope.json", Encoding.UTF8.GetBytes(ValidJson));
        var root = SpeedscopeParser.ParseFile(path);

        using var image = new FlameGraphRenderer().Render(root);

        Assert.NotNull(image);
        Assert.True(image.Width > 0);
        Assert.True(image.Height > 0);
    }

    /// <summary>
    /// Verifies that the loaded profile can be written to a PNG file on disk.
    /// </summary>
    [Fact]
    public void LoadedProfile_RendersToPngOnDisk()
    {
        var path = WriteBytes("png.speedscope.json", Encoding.UTF8.GetBytes(ValidJson));
        var root = SpeedscopeParser.ParseFile(path);
        var pngPath = Path.Combine(_tempDir, "flame.png");

        new FlameGraphRenderer().RenderToPng(root, pngPath);

        Assert.True(File.Exists(pngPath));
        Assert.True(new FileInfo(pngPath).Length > 0);
    }

    private string WriteBytes(string fileName, byte[] bytes)
    {
        var path = Path.Combine(_tempDir, fileName);
        File.WriteAllBytes(path, bytes);
        return path;
    }

    /// <summary>
    /// Writes a sampled profile whose serialized form exceeds <see cref="LargeFileMinBytes"/>.
    /// Every sample has a three-frame stack with valid frame indices and weight 1.
    /// </summary>
    private string WriteLargeProfile(string fileName)
    {
        var sb = new StringBuilder(LargeFileMinBytes * 2);
        sb.Append("{\"shared\":{\"frames\":[");
        for (var f = 0; f < LargeFileFrameCount; f++)
        {
            if (f > 0) sb.Append(',');
            sb.Append("{\"name\":\"frame").Append(f).Append("\"}");
        }
        sb.Append("]},\"profiles\":[{\"type\":\"sampled\",\"unit\":\"none\",\"startValue\":0,\"endValue\":")
          .Append(LargeFileSampleCount)
          .Append(",\"samples\":[");

        for (var i = 0; i < LargeFileSampleCount; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append('[')
              .Append(i % LargeFileFrameCount).Append(',')
              .Append((i / LargeFileFrameCount) % LargeFileFrameCount).Append(',')
              .Append((i / (LargeFileFrameCount * LargeFileFrameCount)) % LargeFileFrameCount)
              .Append(']');
        }

        sb.Append("],\"weights\":[");
        for (var i = 0; i < LargeFileSampleCount; i++)
        {
            if (i > 0) sb.Append(',');
            sb.Append('1');
        }
        sb.Append("]}]}");

        return WriteBytes(fileName, Encoding.UTF8.GetBytes(sb.ToString()));
    }

    private static void AssertSameTree(FlameNode expected, FlameNode actual)
    {
        Assert.Equal(expected.Name, actual.Name);
        Assert.Equal(expected.Value, actual.Value);
        Assert.Equal(expected.Children.Count, actual.Children.Count);
        for (var i = 0; i < expected.Children.Count; i++)
            AssertSameTree(expected.Children[i], actual.Children[i]);
    }
}
