using System.Collections.Generic;

namespace SkiaFlameGraph.Core.Reporting;

public interface IHotFunctionsReport
{
    IReadOnlyList<HotFunctionsReport.HotFunction> Functions { get; }
    double TotalSelfTime { get; }
    string ToText(int topN = 10);
}
