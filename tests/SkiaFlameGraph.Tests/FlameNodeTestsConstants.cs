namespace SkiaFlameGraph.Tests;

/// <summary>
/// Shared constants used by <see cref="FlameNodeTests"/>.
/// </summary>
internal static class FlameNodeTestsConstants
{
    public const string RootNodeName = "root";
    public const string ChildNodeName = "child";
    public const string DuplicateNodeName = "dup";
    public const string LeafNodeName = "leaf";
    public const string GrandchildNodeName = "grandchild";
    public const string TestFileName = "Test.cs";

    public const int LeafMaxDepth = 0;
    public const int ChildMaxDepth = 1;
    public const int ThreeLevelTreeMaxDepth = 2;
    public const int DuplicateChildCount = 2;
    public const int TestFileLine = 42;
}