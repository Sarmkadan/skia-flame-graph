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
        /// Error message for a zero-valued total root in CalculatePercentageOfTotal.
        /// </summary>
        public const string TotalRootZeroValueMessage = "Total root has zero value, cannot calculate percentage.";

        /// <summary>
        /// Zero value used in SumValuesWhere.
        /// </summary>
        public const double ZeroValue = 0.0;

        /// <summary>
        /// Zero count used in GetLeafNodes.
        /// </summary>
        public const int ZeroCount = 0;

        /// <summary>
        /// Zero depth used in GetNodesAtDepth (direct children level and the lower bound for depth).
        /// </summary>
        public const int ZeroDepth = 0;

        /// <summary>
        /// Multiplier that converts a fraction into a percentage (0-100) in CalculatePercentageOfTotal.
        /// </summary>
        public const double PercentageMultiplier = 100.0;
    }
}
