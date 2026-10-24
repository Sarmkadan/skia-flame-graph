namespace SkiaFlameGraph.IntegrationTests;

/// <summary>
/// Declares the scenarios covered by <see cref="CollapsedStacksParserIntegrationTests"/>.
/// </summary>
public interface ICollapsedStacksParserIntegrationTests
{
    void ParseFile_GeneratedProfile_ReproducesEveryNodeTotalAndDepth();
    void Parse_GeneratedProfileLines_MatchesFileParse();
    void Parse_SpecialCharacterFrame_KeepsNameVerbatim(string frameName);
    void Parse_LazyReaderOverStream_MatchesFileParse();
    void Parse_MalformedLines_SkipsInvalidLinesAndCountsValidOnes();
    void Parse_MalformedLines_TreeMatchesValidOnlyInput();
    void ParseFile_MalformedFile_MatchesStringParse();
    void Parse_NonFiniteCount_SkipsLine(string count);
    void Parse_SameLinesFedTwice_ProducesIdenticalTrees();
    void ParseFile_SameFileParsedTwice_ProducesIdenticalTrees();
    void Parse_InputConcatenatedWithItself_DoublesEveryValueAndKeepsShape();
    void Parse_LargeInputThenUnrelatedInput_DoesNotLeakState();
}
