using System;
using System.Collections.Generic;
using System.Globalization;

namespace SkiaFlameGraph.Core.Parsing;

/// <summary>
/// Builder for creating <see cref="ChromeTraceEvent"/> instances with fluent interface.
/// </summary>
public sealed class ChromeTraceParserBuilder
{
    private string? _ph;
    private string? _name;
    private double _ts;
    private double? _dur;
    private int? _tid;
    private int? _pid;
    private string? _file;
    private int? _line;
    private string? _category;
    private Dictionary<string, object>? _args;

    /// <summary>
    /// Sets the event phase/type (e.g., "X" for complete, "B" for begin, "E" for end).
    /// </summary>
    /// <param name="ph">The event phase.</param>
    /// <returns>This builder instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="ph"/> is null.</exception>
    public ChromeTraceParserBuilder WithPh(string ph)
    {
        if (ph == null) throw new ArgumentNullException(nameof(ph));
        _ph = ph;
        return this;
    }

    /// <summary>
    /// Sets the event name/function being profiled.
    /// </summary>
    /// <param name="name">The event name.</param>
    /// <returns>This builder instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="name"/> is null.</exception>
    public ChromeTraceParserBuilder WithName(string name)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the timestamp in microseconds.
    /// </summary>
    /// <param name="ts">The timestamp.</param>
    /// <returns>This builder instance.</returns>
    public ChromeTraceParserBuilder WithTs(double ts)
    {
        _ts = ts;
        return this;
    }

    /// <summary>
    /// Sets the duration in microseconds (for complete events).
    /// </summary>
    /// <param name="dur">The duration.</param>
    /// <returns>This builder instance.</returns>
    public ChromeTraceParserBuilder WithDur(double? dur)
    {
        _dur = dur;
        return this;
    }

    /// <summary>
    /// Sets the thread ID.
    /// </summary>
    /// <param name="tid">The thread ID.</param>
    /// <returns>This builder instance.</returns>
    public ChromeTraceParserBuilder WithTid(int? tid)
    {
        _tid = tid;
        return this;
    }

    /// <summary>
    /// Sets the process ID.
    /// </summary>
    /// <param name="pid">The process ID.</param>
    /// <returns>This builder instance.</returns>
    public ChromeTraceParserBuilder WithPid(int? pid)
    {
        _pid = pid;
        return this;
    }

    /// <summary>
    /// Sets the optional source file.
    /// </summary>
    /// <param name="file">The source file.</param>
    /// <returns>This builder instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="file"/> is null.</exception>
    public ChromeTraceParserBuilder WithFile(string file)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));
        _file = file;
        return this;
    }

    /// <summary>
    /// Sets the optional line number.
    /// </summary>
    /// <param name="line">The line number.</param>
    /// <returns>This builder instance.</returns>
    public ChromeTraceParserBuilder WithLine(int? line)
    {
        _line = line;
        return this;
    }

    /// <summary>
    /// Sets the optional category.
    /// </summary>
    /// <param name="category">The category.</param>
    /// <returns>This builder instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="category"/> is null.</exception>
    public ChromeTraceParserBuilder WithCategory(string category)
    {
        if (category == null) throw new ArgumentNullException(nameof(category));
        _category = category;
        return this;
    }

    /// <summary>
    /// Sets the optional arguments/attributes.
    /// </summary>
    /// <param name="args">The arguments dictionary.</param>
    /// <returns>This builder instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="args"/> is null.</exception>
    public ChromeTraceParserBuilder WithArgs(Dictionary<string, object>? args)
    {
        if (args == null) throw new ArgumentNullException(nameof(args));
        _args = args;
        return this;
    }

    /// <summary>
    /// Creates a new builder pre-filled with values from an existing ChromeTraceEvent.
    /// </summary>
    /// <param name="template">The template event to copy values from.</param>
    /// <returns>A new builder instance with values from the template.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="template"/> is null.</exception>
    public static ChromeTraceParserBuilder From(ChromeTraceEvent template)
    {
        ArgumentNullException.ThrowIfNull(template);

        return new ChromeTraceParserBuilder
        {
            _ph = template.Ph,
            _name = template.Name,
            _ts = template.Ts,
            _dur = template.Dur,
            _tid = template.Tid,
            _pid = template.Pid,
            _file = template.File,
            _line = template.Line,
            _category = template.Category,
            _args = template.Args
        };
    }

    /// <summary>
    /// Builds the ChromeTraceEvent instance with the configured values.
    /// </summary>
    /// <returns>A configured ChromeTraceEvent instance.</returns>
    /// <exception cref="ArgumentException">If required properties are missing.</exception>
    public ChromeTraceEvent Build()
    {
        // Validate required properties
        if (_ph == null)
            throw new ArgumentException("Event phase (Ph) is required.", nameof(_ph));

        if (_name == null)
            throw new ArgumentException("Event name (Name) is required.", nameof(_name));

        // Ts is required but has a default value, so we just use what was set
        // (it's set to 0 by default if not explicitly set, but that's a valid timestamp)

        return new ChromeTraceEvent
        {
            Ph = _ph,
            Name = _name,
            Ts = _ts,
            Dur = _dur,
            Tid = _tid,
            Pid = _pid,
            File = _file,
            Line = _line,
            Category = _category,
            Args = _args
        };
    }
}