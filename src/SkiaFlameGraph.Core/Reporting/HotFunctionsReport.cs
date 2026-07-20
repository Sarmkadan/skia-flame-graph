using System.Text;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Core.Reporting;

/// <summary>
/// Aggregates FlameNode self-time by frame name across the tree and produces a top-N list.
/// </summary>
public sealed class HotFunctionsReport
{
    private readonly List<HotFunction> _functions = new();
    private double _totalSelfTime;

    /// <summary>
    /// Creates a hot functions report by aggregating self-time across all nodes in the tree.
    /// </summary>
    /// <param name="root">The root node of the flame graph tree.</param>
    public HotFunctionsReport(FlameNode root)
    {
        if (root == null)
            throw new ArgumentNullException(nameof(root));

        AggregateSelfTimes(root);
        foreach (var func in _functions)
        {
            func.TotalSelfTime = _totalSelfTime;
        }
        _functions.Sort((a, b) => b.Self.CompareTo(a.Self));
    }

    /// <summary>
    /// Gets the list of hot functions sorted by self-time (descending).
    /// </summary>
    public IReadOnlyList<HotFunction> Functions => _functions.AsReadOnly();

    /// <summary>
    /// Gets the total self-time across all functions.
    /// </summary>
    public double TotalSelfTime => _totalSelfTime;

    private void AggregateSelfTimes(FlameNode node)
    {
        if (node == null)
            return;

        // Add this node's self-time to the aggregation
        var selfTime = node.SelfValue;
        if (selfTime > 0)
        {
            var function = _functions.FirstOrDefault(f => f.Name == node.Name);
            if (function == null)
            {
                function = new HotFunction(node.Name);
                _functions.Add(function);
            }

            function.Self += selfTime;
            function.Total += node.Value;
            _totalSelfTime += selfTime;
        }

        // Recursively process children
        foreach (var child in node.Children)
        {
            AggregateSelfTimes(child);
        }
    }

    /// <summary>
    /// Renders the hot functions report as a text table.
    /// </summary>
    /// <param name="topN">Number of top functions to include. If 0 or negative, include all.</param>
    /// <returns>A formatted text representation of the report.</returns>
    public string ToText(int topN = 10)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Hot Functions Report");
        sb.AppendLine("===================");
        sb.AppendLine();

        if (_functions.Count == 0)
        {
            sb.AppendLine("No hot functions found.");
            return sb.ToString();
        }

        // Header
        sb.AppendLine("#   Name                          Self      Total     %");
        sb.AppendLine("--  ----------------------------- --------- --------- --------");

        // Data rows
        var count = Math.Min(topN > 0 ? topN : _functions.Count, _functions.Count);
        for (var i = 0; i < count; i++)
        {
            var func = _functions[i];
            var percent = _totalSelfTime > 0 ? (func.Self / _totalSelfTime) * 100 : 0;

            sb.AppendLine($"{i + 1,2}  {func.Name,-29} {func.Self,8:N2} {func.Total,9:N2} {percent,6:N2}%");
        }

        if (topN > 0 && _functions.Count > topN)
        {
            sb.AppendLine($"... ({_functions.Count - topN} more functions)");
        }

        sb.AppendLine();
        sb.AppendLine($"Total: {_totalSelfTime:N2} units across {_functions.Count} functions");

        return sb.ToString();
    }

    /// <summary>
    /// A single hot function entry with self-time and total-time metrics.
    /// </summary>
    public sealed class HotFunction
    {
        /// <summary>
        /// Creates a new HotFunction entry.
        /// </summary>
        /// <param name="name">The function name.</param>
        public HotFunction(string name)
        {
            Name = name;
        }

        /// <summary>Gets the function name.</summary>
        public string Name { get; }

        /// <summary>Gets or sets the self-time (time spent in this function excluding children).</summary>
        public double Self { get; set; }

        /// <summary>Gets or sets the total-time (time spent in this function and all children).</summary>
        public double Total { get; set; }

        /// <summary>Gets the percentage of total self-time.</summary>
        public double Percent => TotalSelfTime > 0 ? (Self / TotalSelfTime) * 100 : 0;

        /// <summary>Gets or sets the total self-time across all functions (for percentage calculation).</summary>
        internal double TotalSelfTime { get; set; }
    }
}
