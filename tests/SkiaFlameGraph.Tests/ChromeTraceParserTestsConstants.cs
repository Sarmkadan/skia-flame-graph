namespace SkiaFlameGraph.Tests;

internal static class ChromeTraceParserTestsConstants
{
    public const string SimpleCompleteEventsJson = """
    [
      { "name": "main", "ph": "X", "ts": 0, "dur": 100, "tid": 1 },
      { "name": "work", "ph": "X", "ts": 10, "dur": 50, "tid": 1 },
      { "name": "helper", "ph": "X", "ts": 20, "dur": 30, "tid": 1 }
    ]
    """;

    public const string NestedEventsJson = """
    [
      { "name": "main", "ph": "B", "ts": 0, "tid": 1 },
      { "name": "a", "ph": "B", "ts": 5, "tid": 1 },
      { "name": "b", "ph": "X", "ts": 10, "dur": 20, "tid": 1 },
      { "name": "a", "ph": "E", "ts": 30, "tid": 1 },
      { "name": "main", "ph": "E", "ts": 40, "tid": 1 }
    ]
    """;

    public const string MultipleThreadsJson = """
    [
      { "name": "thread1-work", "ph": "X", "ts": 0, "dur": 50, "tid": 1 },
      { "name": "thread2-work", "ph": "X", "ts": 0, "dur": 30, "tid": 2 }
    ]
    """;

    public const string EventsWithArgsJson = """
    [
      { "name": "func", "ph": "X", "ts": 0, "dur": 100, "tid": 1, "args": { "arg1": "value1" } }
    ]
    """;

    public const int ThreadId1 = 1;
    public const int ThreadId2 = 2;
    public const string EventPhaseComplete = "X";
    public const string EventPhaseBegin = "B";
    public const string EventPhaseEnd = "E";
    public const string MainEventName = "main";
    public const string ThreadNamePrefix = "thread";
}
