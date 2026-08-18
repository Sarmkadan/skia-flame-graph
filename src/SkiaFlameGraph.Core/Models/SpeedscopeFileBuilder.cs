using System;
using System.Collections.Generic;
using System.Linq;

namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Builder for <see cref="SpeedscopeFile"/> objects.
/// </summary>
public sealed class SpeedscopeFileBuilder
{
    private string? _schema;
    private SharedData? _shared;
    private List<Profile>? _profiles;
    private string? _name;
    private string? _exporter;

    /// <summary>
    /// Creates a new builder with default values.
    /// </summary>
    public SpeedscopeFileBuilder()
    {
    }

    /// <summary>
    /// Creates a builder initialized from an existing <see cref="SpeedscopeFile"/>.
    /// </summary>
    /// <param name="template">The template to copy values from.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="template"/> is <see langword="null"/>.</exception>
    /// <returns>A builder initialized with the template's values.</returns>
    public static SpeedscopeFileBuilder From(SpeedscopeFile template)
    {
        ArgumentNullException.ThrowIfNull(template);
        return new SpeedscopeFileBuilder
        {
            _schema = template.Schema,
            _shared = template.Shared,
            _profiles = template.Profiles?.ToList(),
            _name = template.Name,
            _exporter = template.Exporter
        };
    }

    /// <summary>
    /// Sets the `$schema` property.
    /// </summary>
    /// <param name="schema">The schema URI or identifier.</param>
    /// <returns>This builder instance.</returns>
    public SpeedscopeFileBuilder WithSchema(string? schema)
    {
        _schema = schema;
        return this;
    }

    /// <summary>
    /// Sets the <see cref="SharedData"/> object.
    /// </summary>
    /// <param name="shared">The shared data containing frames and other shared information.</param>
    /// <returns>This builder instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="shared"/> is <see langword="null"/>.</exception>
    public SpeedscopeFileBuilder WithShared(SharedData shared)
    {
        ArgumentNullException.ThrowIfNull(shared);
        _shared = shared;
        return this;
    }

    /// <summary>
    /// Sets the profiles collection.
    /// </summary>
    /// <param name="profiles">The list of profiles.</param>
    /// <returns>This builder instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="profiles"/> is <see langword="null"/>.</exception>
    public SpeedscopeFileBuilder WithProfiles(List<Profile> profiles)
    {
        ArgumentNullException.ThrowIfNull(profiles);
        _profiles = profiles;
        return this;
    }

    /// <summary>
    /// Sets the display name of the speedscope file.
    /// </summary>
    /// <param name="name">The display name.</param>
    /// <returns>This builder instance.</returns>
    public SpeedscopeFileBuilder WithName(string? name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the exporter string that identifies the tool that generated this file.
    /// </summary>
    /// <param name="exporter">The exporter identifier.</param>
    /// <returns>This builder instance.</returns>
    public SpeedscopeFileBuilder WithExporter(string? exporter)
    {
        _exporter = exporter;
        return this;
    }

    /// <summary>
    /// Sets the frames collection on the shared data.
    /// <para>If shared data has not been set, a new <see cref="SharedData"/> instance will be created.</para>
    /// </summary>
    /// <param name="frames">The list of frames.</param>
    /// <returns>This builder instance.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="frames"/> is <see langword="null"/>.</exception>
    public SpeedscopeFileBuilder WithFrames(List<Frame> frames)
    {
        ArgumentNullException.ThrowIfNull(frames);
        _shared ??= new SharedData();
        _shared.Frames = frames;
        return this;
    }

    /// <summary>
    /// Builds the <see cref="SpeedscopeFile"/> instance with the current property values.
    /// </summary>
    /// <returns>A new <see cref="SpeedscopeFile"/> instance.</returns>
    /// <exception cref="ArgumentException">If required properties are missing.</exception>
    public SpeedscopeFile Build()
    {
        // Validate required properties - none are strictly required as the class provides defaults
        // for Shared and Profiles, but we ensure they are never null.
        return new SpeedscopeFile
        {
            Schema = _schema,
            Shared = _shared ?? new SharedData(),
            Profiles = _profiles ?? new List<Profile>(),
            Name = _name,
            Exporter = _exporter
        };
    }
}