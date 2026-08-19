namespace SkiaFlameGraph.Tests
{
    public interface ICollapsedStacksParserTests
    {
        void Parse_NormalInput_BuildsCorrectTree();
        void Parse_EmptyInput_ReturnsEmptyRoot();
        void Parse_MalformedLines_AreSkipped();
        void Parse_DuplicateStacks_MergedCorrectly();
        void Parse_WhitespaceHandling_IgnoresExtraSpaces();
    }
}