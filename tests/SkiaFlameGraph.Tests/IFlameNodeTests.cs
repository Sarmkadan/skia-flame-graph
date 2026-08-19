using Xunit;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Tests
{
    /// <summary>
    /// Interface for FlameNode test contract.
    /// </summary>
    public interface IFlameNodeTests
    {
        void AddChild_CreatesChildWithCorrectDepth();
        void AddChild_WithSameName_CreatesDistinctNodes();
        void MaxDepth_LeafNode_ReturnsZero();
        void MaxDepth_ThreeLevelTree_ReturnsTwo();
        void AddChild_WithFileAndLine_SetsMetadata();
    }
}
