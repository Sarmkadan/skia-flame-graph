using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SkiaFlameGraph.Core.Models;

/// <summary>
/// Provides extension methods for working with <see cref="SpeedscopeFile"/> instances.
/// These methods offer practical utilities for analyzing, transforming, and querying
/// speedscope profile data.
/// </summary>
public static class SpeedscopeFileExtensions
{
    /// <summary>
    /// Gets the total duration of all profiles in the speedscope file.
    /// For evented profiles, this calculates the duration from startValue to endValue.
    /// For sampled profiles, this also uses the duration between startValue and endValue.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <returns>The total duration across all profiles, or 0 if no profiles exist or durations are invalid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    public static double GetTotalDuration(this SpeedscopeFile speedscopeFile)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);

        return speedscopeFile.Profiles
            .Where(p => p.StartValue >= 0 && p.EndValue >= p.StartValue)
            .Select(p => Math.Max(0, p.EndValue - p.StartValue))
            .Sum();
    }

    /// <summary>
    /// Gets the total number of events across all profiles in the speedscope file.
    /// For evented profiles, this counts the events. For sampled profiles, this counts the samples.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <returns>The total number of events/samples across all profiles.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    public static int GetTotalEventCount(this SpeedscopeFile speedscopeFile)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);

        return speedscopeFile.Profiles
            .Sum(p => (p.Events?.Count ?? 0) + (p.Samples?.Count ?? 0));
    }

    /// <summary>
    /// Gets the number of unique frames referenced across all profiles.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <returns>The count of unique frame indices used across all profiles.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    public static int GetUniqueFrameCount(this SpeedscopeFile speedscopeFile)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);

        return speedscopeFile.Profiles
            .SelectMany(p => p.Events?.Select(e => e.Frame) ?? Enumerable.Empty<int>())
            .Concat(speedscopeFile.Profiles.SelectMany(p => p.Samples?.SelectMany(s => s) ?? Enumerable.Empty<int>()))
            .Distinct()
            .Count();
    }

    /// <summary>
    /// Gets the frame with the specified index from the shared frames collection.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <param name="frameIndex">The index of the frame to retrieve.</param>
    /// <returns>The frame at the specified index, or null if the index is out of range.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="frameIndex"/> is negative.</exception>
    public static Frame? GetFrame(this SpeedscopeFile speedscopeFile, int frameIndex)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);
        ArgumentOutOfRangeException.ThrowIfNegative(frameIndex);

        return frameIndex < speedscopeFile.Shared.Frames.Count
            ? speedscopeFile.Shared.Frames[frameIndex]
            : null;
    }

    /// <summary>
    /// Gets all profile names from the speedscope file.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <returns>An enumerable of profile names. Returns empty if no profiles exist.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    public static IEnumerable<string> GetProfileNames(this SpeedscopeFile speedscopeFile)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);

        return speedscopeFile.Profiles
            .Select(p => p.Name ?? "Unnamed Profile")
            .ToList();
    }

    /// <summary>
    /// Determines whether the speedscope file contains any evented profiles.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <returns>True if any profile has events; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    public static bool HasEventedProfiles(this SpeedscopeFile speedscopeFile)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);

        return speedscopeFile.Profiles.Any(p => p.Events is not null && p.Events.Count > 0);
    }

    /// <summary>
    /// Determines whether the speedscope file contains any sampled profiles.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <returns>True if any profile has samples; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    public static bool HasSampledProfiles(this SpeedscopeFile speedscopeFile)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);

        return speedscopeFile.Profiles.Any(p => p.Samples is not null && p.Samples.Count > 0);
    }

    /// <summary>
    /// Gets the average duration per event/sample across all profiles.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <returns>The average duration per event, or 0 if no events exist.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    public static double GetAverageDurationPerEvent(this SpeedscopeFile speedscopeFile)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);

        var totalDuration = speedscopeFile.GetTotalDuration();
        var totalEvents = speedscopeFile.GetTotalEventCount();

        return totalEvents > 0
            ? totalDuration / totalEvents
            : 0;
    }

    /// <summary>
    /// Gets the frame with the highest cumulative time across all profiles.
    /// For evented profiles, this calculates the total time spent in each frame by summing
    /// the duration between open and close events. For sampled profiles, each sample counts as 1.0.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <returns>A tuple containing the frame index and its cumulative time, or null if no frames exist.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    public static (int FrameIndex, double CumulativeTime)? GetHottestFrame(this SpeedscopeFile speedscopeFile)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);

        if (speedscopeFile.Shared.Frames.Count == 0)
        {
            return null;
        }

        var frameTimes = new double[speedscopeFile.Shared.Frames.Count];

        foreach (var profile in speedscopeFile.Profiles)
        {
            if (profile.Events is not null)
            {
                // For evented profiles, calculate duration between open and close events
                var frameStack = new Stack<int>();
                var frameStartTimes = new Dictionary<int, double>();

                foreach (var evt in profile.Events)
                {
                    if (evt.Type == "O") // Open event
                    {
                        frameStack.Push(evt.Frame);
                        frameStartTimes[evt.Frame] = evt.At;
                    }
                    else if (evt.Type == "C" && frameStack.Count > 0) // Close event
                    {
                        var frameIndex = frameStack.Pop();
                        if (frameStartTimes.TryGetValue(frameIndex, out var startTime))
                        {
                            frameTimes[frameIndex] += evt.At - startTime;
                            frameStartTimes.Remove(frameIndex);
                        }
                    }
                }
            }

            if (profile.Samples is not null)
            {
                foreach (var sample in profile.Samples)
                {
                    foreach (var frameIndex in sample)
                    {
                        frameTimes[frameIndex] += 1.0;
                    }
                }
            }
        }

        if (frameTimes.Length == 0)
        {
            return null;
        }

        var maxIndex = 0;
        var maxTime = frameTimes[0];

        for (var i = 1; i < frameTimes.Length; i++)
        {
            if (frameTimes[i] > maxTime)
            {
                maxTime = frameTimes[i];
                maxIndex = i;
            }
        }

        return maxTime > 0 ? (maxIndex, maxTime) : null;
    }

    /// <summary>
    /// Gets the duration of the longest profile in the speedscope file.
    /// </summary>
    /// <param name="speedscopeFile">The speedscope file instance.</param>
    /// <returns>The duration of the longest profile, or 0 if no profiles exist or durations are invalid.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="speedscopeFile"/> is null.</exception>
    public static double GetMaxProfileDuration(this SpeedscopeFile speedscopeFile)
    {
        ArgumentNullException.ThrowIfNull(speedscopeFile);

        return speedscopeFile.Profiles
            .Where(p => p.StartValue >= 0 && p.EndValue >= p.StartValue)
            .Select(p => Math.Max(0, p.EndValue - p.StartValue))
            .DefaultIfEmpty()
            .Max();
    }
}