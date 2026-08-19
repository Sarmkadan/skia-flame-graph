using Xunit;

namespace SkiaFlameGraph.Tests
{
    public interface IChromeTraceParserTests
    {
        void SimpleCompleteEvents_AreParsedCorrectly();
        void CompleteEvents_WithFileAndLine_AreParsedCorrectly();
        void NestedBeginEndEvents_BuildProperCallTree();
        void MultipleThreads_AreGroupedCorrectly();
        void EventsWithoutTid_AreIgnored();
        void NonCompleteBeginEndEvents_AreSkipped();
        void Events_AreSortedByTimestamp();
        void EmptyEventsArray_Throws();
        void NullDeserialization_Throws();
    }
}
