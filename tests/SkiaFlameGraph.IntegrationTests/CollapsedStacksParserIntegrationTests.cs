using System.Text;
using SkiaFlameGraph.Core.Models;
using SkiaFlameGraph.Core.Parsing;
using Xunit;
using static SkiaFlameGraph.IntegrationTests.CollapsedStacksParserIntegrationTestsConstants;
using static SkiaFlameGraph.IntegrationTests.CollapsedStacksParserIntegrationTestsExtensions;

namespace SkiaFlameGraph.IntegrationTests;

/// <summary>
/// Integration tests for <see cref="CollapsedStacksParser"/> with realistic inputs: a generated 600-line profile
/// with spaces, generics and Unicode in frame names; a dump mixed with malformed lines; and repeated or
/// streamed parsing. Every node's total and depth is checked against an oracle computed independently of the parser.
/// </summary>
/// <remarks>
/// Malformed-line policy: <see cref="CollapsedStacksParser"/> skips them silently and never throws. A line is skipped
/// when it is blank, has no count, has a count that is not a number, is NaN or infinite, is zero or negative,
/// or has no frames. Skipped lines do not contribute to any node. There is no warning or error channel.
/// Trailing whitespace is ignored. The parser has no Stream overload, so stream tests supply lines lazily through
/// <c>TextReader</c>.
/// </remarks>
public class CollapsedStacksParserIntegrationTests : ICollapsedStacksParserIntegrationTests
{
    /// <summary>
    /// Verifies that a 600-line file produces exactly one node per distinct path prefix, that each node's value equals
    /// the oracle's subtree total, that each node's depth equals its path length, and that the root equals the grand total.
    /// </summary>
    [Fact]
    public void ParseFile_GeneratedProfile_ReproducesEveryNodeTotalAndDepth()
    {
        // Arrange
        var samples = GenerateProfile(GeneratedLineCount);
        var expected = ExpectedSubtreeTotals(samples);
        using var file = new TempProfileFile(samples.ToCollapsedLines());

        // Act
        var root = CollapsedStacksParser.ParseFile(file.FilePath);

        // Assert
        Assert.Equal(samples.Sum(s => (double)s.Count), root.Value);
        Assert.Equal(0, root.Depth);

        var nodes = IndexByPath(root);
        Assert.Equal(expected.Count, nodes.Count);
        Assert.Equal(expected.Count + 1, root.TotalNodeCount());

        foreach (var (path, node) in nodes)
        {
            Assert.Equal(expected[path], node.Value);
            Assert.Equal(path.Split(FrameSeparator).Length, node.Depth);
        }
    }

    /// <summary>
    /// Verifies that parsing the generated lines in memory produces the same tree as parsing the file.
    /// </summary>
    [Fact]
    public void Parse_GeneratedProfileLines_MatchesFileParse()
    {
        // Arrange
        var lines = GenerateProfile(GeneratedLineCount).ToCollapsedLines();
        using var file = new TempProfileFile(lines);

        // Act
        var fromLines = CollapsedStacksParser.Parse(lines);
        var fromFile = CollapsedStacksParser.ParseFile(file.FilePath);

        // Assert
        Assert.Equal(TreeSignature(fromFile), TreeSignature(fromLines));
    }

    /// <summary>
    /// Verifies that frames containing spaces, generic type arguments, or non-ASCII text (including an emoji surrogate pair)
    /// are kept as one frame each, and that the count after the last space is still read as the sample count.
    /// </summary>
    [Theory]
    [InlineData("Foo Bar")]
    [InlineData("List<T>.Add")]
    [InlineData("Dictionary<TKey, TValue>.TryGetValue")]
    [InlineData("Ünïcode 函数")]
    [InlineData("🔥 hot loop")]
    public void Parse_SpecialCharacterFrame_KeepsNameVerbatim(string frameName)
    {
        // Arrange
        var lines = new[] { $"main;{frameName} 4" };

        // Act
        var root = CollapsedStacksParser.Parse(lines);

        // Assert
        var main = Assert.Single(root.Children);
        var frame = Assert.Single(main.Children);
        Assert.Equal(frameName, frame.Name);
        Assert.Equal(2, frame.Depth);
        Assert.Equal(4, frame.Value);
    }

    /// <summary>
    /// Verifies that parsing lines lazily read from a UTF-8 stream produces the same tree as parsing the file,
    /// and that the reader is consumed to the end.
    /// </summary>
    [Fact]
    public void Parse_LazyReaderOverStream_MatchesFileParse()
    {
        // Arrange
        var lines = GenerateProfile(GeneratedLineCount).ToCollapsedLines();
        var bytes = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false).GetBytes(string.Join('\n', lines) + '\n');
        using var file = new TempProfileFile(lines);
        using var stream = new MemoryStream(bytes);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        // Act
        var fromStream = CollapsedStacksParser.Parse(ReadLines(reader));
        var fromFile = CollapsedStacksParser.ParseFile(file.FilePath);

        // Assert
        Assert.Equal(TreeSignature(fromFile), TreeSignature(fromStream));
        Assert.Equal(-1, reader.Peek());
    }

    /// <summary>
    /// Verifies that blank, whitespace-only, count-less, non-numeric, zero, negative, frameless, and trailing-whitespace
    /// lines are handled as documented: invalid lines are skipped, and valid lines with trailing whitespace are counted.
    /// </summary>
    [Fact]
    public void Parse_MalformedLines_SkipsInvalidLinesAndCountsValidOnes()
    {
        // Act
        var root = CollapsedStacksParser.Parse(MalformedLines);

        // Assert
        Assert.Equal(19, root.Value);
        Assert.Equal(new[] { "main" }, root.Children.Select(c => c.Name));

        var main = root.Children[0];
        Assert.Equal(19, main.Value);
        Assert.Equal(new[] { "parse", "render", "leaf" }, main.Children.Select(c => c.Name));
        Assert.Equal(5, main.Children.Single(c => c.Name == "parse").Value);
        Assert.Equal(10, main.Children.Single(c => c.Name == "render").Value);
        Assert.Equal(4, main.Children.Single(c => c.Name == "leaf").Value);
    }

    /// <summary>
    /// Verifies that the malformed dump yields exactly the tree produced by its valid lines alone.
    /// </summary>
    [Fact]
    public void Parse_MalformedLines_TreeMatchesValidOnlyInput()
    {
        // Act
        var withMalformed = CollapsedStacksParser.Parse(MalformedLines);
        var validOnly = CollapsedStacksParser.Parse(MalformedLinesValidOnly);

        // Assert
        Assert.Equal(TreeSignature(validOnly), TreeSignature(withMalformed));
    }

    /// <summary>
    /// Verifies that the malformed dump written to a file parses to the same tree as the in-memory lines.
    /// </summary>
    [Fact]
    public void ParseFile_MalformedFile_MatchesStringParse()
    {
        // Arrange
        using var file = new TempProfileFile(MalformedLines);

        // Act
        var fromFile = CollapsedStacksParser.ParseFile(file.FilePath);
        var fromLines = CollapsedStacksParser.Parse(MalformedLines);

        // Assert
        Assert.Equal(TreeSignature(fromLines), TreeSignature(fromFile));
    }

    /// <summary>
    /// Verifies that a line whose count is NaN or infinity is skipped instead of poisoning the totals.
    /// </summary>
    [Theory]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    public void Parse_NonFiniteCount_SkipsLine(string count)
    {
        // Arrange
        var lines = new[] { "main;good 3", $"main;bad {count}" };

        // Act
        var root = CollapsedStacksParser.Parse(lines);

        // Assert
        Assert.Equal(3, root.Value);
        Assert.Single(root.Children[0].Children, c => c.Name == "good");
    }

    /// <summary>
    /// Verifies that feeding the same malformed input twice gives two equal trees, each with its own root object.
    /// </summary>
    [Fact]
    public void Parse_SameLinesFedTwice_ProducesIdenticalTrees()
    {
        // Act
        var first = CollapsedStacksParser.Parse(MalformedLines);
        var second = CollapsedStacksParser.Parse(MalformedLines);

        // Assert
        Assert.NotSame(first, second);
        Assert.Equal(TreeSignature(first), TreeSignature(second));
    }

    /// <summary>
    /// Verifies that parsing the same generated file twice gives identical trees.
    /// </summary>
    [Fact]
    public void ParseFile_SameFileParsedTwice_ProducesIdenticalTrees()
    {
        // Arrange
        using var file = new TempProfileFile(GenerateProfile(GeneratedLineCount).ToCollapsedLines());

        // Act
        var first = CollapsedStacksParser.ParseFile(file.FilePath);
        var second = CollapsedStacksParser.ParseFile(file.FilePath);

        // Assert
        Assert.Equal(TreeSignature(first), TreeSignature(second));
    }

    /// <summary>
    /// Verifies that concatenating the input with itself doubles every node's value, keeps the same node set and depths,
    /// and that each value is the sum of the two parses (counts are additive).
    /// </summary>
    [Fact]
    public void Parse_InputConcatenatedWithItself_DoublesEveryValueAndKeepsShape()
    {
        // Arrange
        var lines = GenerateProfile(GeneratedLineCount).ToCollapsedLines();
        var single = CollapsedStacksParser.Parse(lines);

        // Act
        var doubled = CollapsedStacksParser.Parse(lines.Concat(lines));

        // Assert
        var singleNodes = IndexByPath(single);
        var doubledNodes = IndexByPath(doubled);
        Assert.Equal(singleNodes.Keys.OrderBy(k => k, StringComparer.Ordinal), doubledNodes.Keys.OrderBy(k => k, StringComparer.Ordinal));
        Assert.Equal(single.Value * 2, doubled.Value);

        foreach (var (path, node) in singleNodes)
        {
            Assert.Equal(node.Value * 2, doubledNodes[path].Value);
            Assert.Equal(node.Depth, doubledNodes[path].Depth);
        }
    }

    /// <summary>
    /// Verifies that parsing a large input and then an unrelated small input returns only the small input's counts,
    /// so the parser keeps no state between calls.
    /// </summary>
    [Fact]
    public void Parse_LargeInputThenUnrelatedInput_DoesNotLeakState()
    {
        // Arrange
        var large = GenerateProfile(GeneratedLineCount).ToCollapsedLines();
        var small = new[] { "solo;leaf 2" };

        // Act
        _ = CollapsedStacksParser.Parse(large);
        var root = CollapsedStacksParser.Parse(small);

        // Assert
        Assert.Equal(2, root.Value);
        var solo = Assert.Single(root.Children);
        Assert.Equal("solo", solo.Name);
        Assert.Equal(2, solo.Value);
        Assert.Equal(2, Assert.Single(solo.Children).Value);
    }
}
