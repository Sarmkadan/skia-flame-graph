namespace SkiaFlameGraph.Core.Models
{
    /// <summary>
    /// Constants used in FlameNodeExtensions.
    /// </summary>
    internal static class FlameNodeExtensionsConstants
    {
        /// <summary>
        /// Error message for negative depth in GetNodesAtDepth.
        /// </summary>
        public const string DepthCannotBeNegativeMessage = "Depth cannot be negative.";

        /// <summary>
        /// Zero value used in SumValuesWhere and GetLeafNodes.
        /// </summary>
        public const double ZeroValue = 0.0;

        /// <summary>
        /// Zero count used in GetLeafNodes.
        /// </summary>
        public const int ZeroCount = 0;
    }
}
