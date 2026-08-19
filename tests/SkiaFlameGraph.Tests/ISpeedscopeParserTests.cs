using Xunit;
using SkiaFlameGraph.Core.Parsing;

namespace SkiaFlameGraph.Tests
{
    public interface ISpeedscopeParserTests
    {
        void SampledProfile_AggregatesWeightsUpTheStack();
        void RecursiveFrames_AreMergedIntoOneBox();
        void EventedProfile_AttributesElapsedTime();
        void EmptyDocument_Throws();
    }
}