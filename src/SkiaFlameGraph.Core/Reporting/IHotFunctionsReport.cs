using System.Collections.Generic;

namespace SkiaFlameGraph.Core.Reporting;

public interface IHotFunctionsReport
{
    /// <summary>
    /// Gets the list of hot functions sorted by self-time in descending order.
    /// </summary>
    IReadOnlyList<HotFunctionsReport.HotFunction> Functions { get; }

    /// <summary>
    /// Gets the total self-time across all functions.
    /// </summary>
    double TotalSelfTime { get; }

    /// <summary>
    /// Renders the hot functions report as a text table.
    /// </summary>
    /// <param name="topN">The number of top functions to include, or a non-positive value to include all functions.</param>
    /// <returns>A formatted text representation of the report.</returns>
    string ToText(int topN = 10);
}
