using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Provides validation helpers for <see cref="SpeedscopeFile"/> instances.
/// Validates that all required fields are present and that values are within expected ranges.
/// </summary>
public static class SpeedscopeFileValidation
{
    /// <summary>
    /// Validates a <see cref="SpeedscopeFile"/> instance and returns a list of human-readable problems.
    /// </summary>
    /// <param name="value">The speedscope file to validate.</param>
    /// <returns>An enumerable of validation problems; empty if the file is valid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static IReadOnlyList<string> Validate(this SpeedscopeFile value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = new List<string>();

        // Validate Schema (optional)
        if (value.Schema is { Length: 0 })
        {
            problems.Add("SpeedscopeFile.Schema must not be an empty string.");
        }

        // Validate Shared (required)
        if (value.Shared is null)
        {
            problems.Add("SpeedscopeFile.Shared is required and cannot be null.");
        }
        else
        {
            problems.AddRange(ValidateSharedData(value.Shared));
        }

        // Validate Profiles (required)
        if (value.Profiles is null)
        {
            problems.Add("SpeedscopeFile.Profiles is required and cannot be null.");
        }
        else
        {
            problems.AddRange(ValidateProfiles(value.Profiles));
        }

        // Validate Name (optional)
        if (value.Name is { Length: 0 })
        {
            problems.Add("SpeedscopeFile.Name must not be an empty string.");
        }

        // Validate Exporter (optional)
        if (value.Exporter is { Length: 0 })
        {
            problems.Add("SpeedscopeFile.Exporter must not be an empty string.");
        }

        return problems.AsReadOnly();
    }

    /// <summary>
    /// Determines whether a <see cref="SpeedscopeFile"/> instance is valid.
    /// </summary>
    /// <param name="value">The speedscope file to check.</param>
    /// <returns>True if the file is valid; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public static bool IsValid(this SpeedscopeFile value)
    {
        return value.Validate().Count == 0;
    }

    /// <summary>
    /// Ensures that a <see cref="SpeedscopeFile"/> instance is valid, throwing an exception if it is not.
    /// </summary>
    /// <param name="value">The speedscope file to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the file is invalid, containing a list of problems.</exception>
    public static void EnsureValid(this SpeedscopeFile value)
    {
        ArgumentNullException.ThrowIfNull(value);

        var problems = value.Validate();

        if (problems.Count > 0)
        {
            throw new ArgumentException(
                $"SpeedscopeFile is invalid. Problems:\n{string.Join("\n", problems)}");
        }
    }

    private static IEnumerable<string> ValidateSharedData(SharedData shared)
    {
        if (shared.Frames is null)
        {
            yield return "SharedData.Frames is required and cannot be null.";
        }
        else
        {
            foreach (var frameProblem in ValidateFrames(shared.Frames))
            {
                yield return frameProblem;
            }
        }
    }

    private static IEnumerable<string> ValidateFrames(IReadOnlyList<Frame> frames)
    {
        if (frames.Count == 0)
        {
            yield return "SharedData.Frames must contain at least one frame.";
        }

        for (var i = 0; i < frames.Count; i++)
        {
            var frame = frames[i];

            if (frame is null)
            {
                yield return $"Frame at index {i} is null.";
                continue;
            }

            if (frame.Name is { Length: 0 })
            {
                yield return $"Frame at index {i}.Name must not be an empty string.";
            }

            if (frame.Name is null)
            {
                yield return $"Frame at index {i}.Name is required and cannot be null.";
            }

            if (frame.File is { Length: 0 })
            {
                yield return $"Frame at index {i}.File must not be an empty string.";
            }

            if (frame.Line < 0)
            {
                yield return $"Frame at index {i}.Line must be non-negative, but was {frame.Line}.";
            }

            if (frame.Col < 0)
            {
                yield return $"Frame at index {i}.Col must be non-negative, but was {frame.Col}.";
            }
        }
    }

    private static IEnumerable<string> ValidateProfiles(IReadOnlyList<Profile> profiles)
    {
        if (profiles.Count == 0)
        {
            yield return "SpeedscopeFile.Profiles must contain at least one profile.";
        }

        for (var i = 0; i < profiles.Count; i++)
        {
            var profile = profiles[i];

            if (profile is null)
            {
                yield return $"Profile at index {i} is null.";
                continue;
            }

            if (profile.Type is { Length: 0 })
            {
                yield return $"Profile at index {i}.Type must not be an empty string.";
            }

            if (profile.Type is null)
            {
                yield return $"Profile at index {i}.Type is required and cannot be null.";
            }
            else if (profile.Type != "evented" && profile.Type != "sampled")
            {
                yield return $"Profile at index {i}.Type must be either 'evented' or 'sampled', but was '{profile.Type}'.";
            }

            if (profile.Unit is { Length: 0 })
            {
                yield return $"Profile at index {i}.Unit must not be an empty string.";
            }

            if (profile.Unit is null)
            {
                yield return $"Profile at index {i}.Unit is required and cannot be null.";
            }

            if (profile.StartValue < 0)
            {
                yield return $"Profile at index {i}.StartValue must be non-negative, but was {profile.StartValue}.";
            }

            if (profile.EndValue < profile.StartValue)
            {
                yield return $"Profile at index {i}.EndValue ({profile.EndValue}) must be greater than or equal to StartValue ({profile.StartValue}).";
            }

            if (profile.Events is not null)
            {
                foreach (var evtProblem in ValidateProfileEvents(profile.Events))
                {
                    yield return evtProblem;
                }
            }

            if (profile.Samples is not null)
            {
                foreach (var sampleProblem in ValidateProfileSamples(profile.Samples))
                {
                    yield return sampleProblem;
                }
            }

            if (profile.Weights is not null)
            {
                foreach (var weightProblem in ValidateProfileWeights(profile.Weights))
                {
                    yield return weightProblem;
                }
            }
        }
    }

    private static IEnumerable<string> ValidateProfileEvents(IReadOnlyList<ProfileEvent> events)
    {
        if (events.Count == 0)
        {
            yield return "Profile.Events must contain at least one event when not null.";
        }

        for (var i = 0; i < events.Count; i++)
        {
            var evt = events[i];

            if (evt is null)
            {
                yield return $"ProfileEvent at index {i} is null.";
                continue;
            }

            if (evt.Type is { Length: 0 })
            {
                yield return $"ProfileEvent at index {i}.Type must not be an empty string.";
            }

            if (evt.Type is null)
            {
                yield return $"ProfileEvent at index {i}.Type is required and cannot be null.";
            }
            else if (evt.Type != "O" && evt.Type != "C")
            {
                yield return $"ProfileEvent at index {i}.Type must be either 'O' or 'C', but was '{evt.Type}'.";
            }

            if (evt.Frame < 0)
            {
                yield return $"ProfileEvent at index {i}.Frame must be non-negative, but was {evt.Frame}.";
            }

            if (evt.At < 0)
            {
                yield return $"ProfileEvent at index {i}.At must be non-negative, but was {evt.At}.";
            }
        }
    }

    private static IEnumerable<string> ValidateProfileSamples(IReadOnlyList<List<int>> samples)
    {
        if (samples.Count == 0)
        {
            yield return "Profile.Samples must contain at least one sample when not null.";
        }

        for (var i = 0; i < samples.Count; i++)
        {
            var sample = samples[i];

            if (sample is null)
            {
                yield return $"Profile.Samples[{i}] is null.";
                continue;
            }

            if (sample.Count == 0)
            {
                yield return $"Profile.Samples[{i}] must contain at least one frame index.";
            }

            for (var j = 0; j < sample.Count; j++)
            {
                var frameIndex = sample[j];

                if (frameIndex < 0)
                {
                    yield return $"Profile.Samples[{i}][{j}] contains a negative frame index ({frameIndex}).";
                }
            }
        }
    }

    private static IEnumerable<string> ValidateProfileWeights(IReadOnlyList<double> weights)
    {
        if (weights.Count == 0)
        {
            yield return "Profile.Weights must contain at least one weight when not null.";
        }

        for (var i = 0; i < weights.Count; i++)
        {
            var weight = weights[i];

            if (weight < 0)
            {
                yield return $"Profile.Weights[{i}] must be non-negative, but was {weight}.";
            }
        }
    }
}
