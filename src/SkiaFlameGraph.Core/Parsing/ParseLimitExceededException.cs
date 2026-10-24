namespace SkiaFlameGraph.Core.Parsing;

/// <summary>
/// Thrown when a speedscope document exceeds one of the <see cref="ParseLimits"/>.
/// Derives from <see cref="FormatException"/> so existing handlers for malformed input still catch it.
/// </summary>
public sealed class ParseLimitExceededException : FormatException
{
    /// <summary>
    /// Initializes a new instance describing which limit was exceeded.
    /// </summary>
    /// <param name="limitName">The name of the <see cref="ParseLimits"/> property that was exceeded.</param>
    /// <param name="limit">The configured limit.</param>
    /// <param name="actual">The observed value that exceeded the limit.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="limitName"/> is null or empty.</exception>
    public ParseLimitExceededException(string limitName, long limit, long actual)
        : base(FormatMessage(limitName, limit, actual))
    {
        LimitName = limitName;
        Limit = limit;
        Actual = actual;
    }

    /// <summary>
    /// Gets the name of the <see cref="ParseLimits"/> property that was exceeded.
    /// </summary>
    public string LimitName { get; }

    /// <summary>
    /// Gets the configured limit.
    /// </summary>
    public long Limit { get; }

    /// <summary>
    /// Gets the observed value that exceeded the limit.
    /// </summary>
    public long Actual { get; }

    private static string FormatMessage(string limitName, long limit, long actual)
    {
        ArgumentException.ThrowIfNullOrEmpty(limitName);
        return $"speedscope document exceeds {limitName} ({actual} > {limit}).";
    }
}
