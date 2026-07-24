using System;
using System.Collections.Generic;
using Xunit;
using SkiaFlameGraph.Core.Models;

namespace SkiaFlameGraph.Tests;

/// <summary>
/// Unit tests for <see cref="SpeedscopeFileValidation"/> extension methods.
/// </summary>
public class SpeedscopeFileValidationTests
{
    private static SpeedscopeFile CreateValidFile()
    {
        return new SpeedscopeFile
        {
            Shared = new SharedData
            {
                Frames = new List<Frame>
                {
                    new Frame { Name = "ValidFrame", File = "Program.cs", Line = 1 }
                }
            },
            Profiles = new List<Profile>
            {
                CreateValidProfile()
            }
        };
    }

    private static Profile CreateValidProfile()
    {
        return new Profile
        {
            Type = "evented",
            Unit = "milliseconds",
            StartValue = 0,
            EndValue = 10,
            Events = new List<ProfileEvent>
            {
                new ProfileEvent { Type = "O", Frame = 0, At = 0 },
                new ProfileEvent { Type = "C", Frame = 0, At = 10 }
            }
        };
    }

    [Fact]
    public void Validate_ValidFile_ReturnsEmptyList()
    {
        var file = CreateValidFile();

        var problems = file.Validate();

        Assert.Empty(problems);
    }

    [Fact]
    public void IsValid_ValidFile_ReturnsTrue()
    {
        var file = CreateValidFile();

        Assert.True(file.IsValid());
    }

    [Fact]
    public void EnsureValid_ValidFile_DoesNotThrow()
    {
        var file = CreateValidFile();

        var ex = Record.Exception(() => file.EnsureValid());

        Assert.Null(ex);
    }

    [Fact]
    public void Validate_NullFile_ThrowsArgumentNullException()
    {
        SpeedscopeFile? file = null;

        Assert.Throws<ArgumentNullException>(() => file!.Validate());
    }

    [Fact]
    public void IsValid_NullFile_ThrowsArgumentNullException()
    {
        SpeedscopeFile? file = null;

        Assert.Throws<ArgumentNullException>(() => file!.IsValid());
    }

    [Fact]
    public void EnsureValid_NullFile_ThrowsArgumentNullException()
    {
        SpeedscopeFile? file = null;

        Assert.Throws<ArgumentNullException>(() => file!.EnsureValid());
    }

    [Fact]
    public void Validate_MissingShared_ReturnsProblem()
    {
        var file = new SpeedscopeFile
        {
            Shared = null,
            Profiles = new List<Profile> { CreateValidProfile() }
        };

        var problems = file.Validate();

        Assert.Contains("SpeedscopeFile.Shared is required and cannot be null.", problems);
    }

    [Fact]
    public void Validate_EmptyFrames_ReturnsProblem()
    {
        var file = new SpeedscopeFile
        {
            Shared = new SharedData { Frames = new List<Frame>() },
            Profiles = new List<Profile> { CreateValidProfile() }
        };

        var problems = file.Validate();

        Assert.Contains("SharedData.Frames must contain at least one frame.", problems);
    }

    [Fact]
    public void Validate_InvalidProfileType_ReturnsProblem()
    {
        var profile = CreateValidProfile();
        profile.Type = "invalid";

        var file = new SpeedscopeFile
        {
            Shared = new SharedData
            {
                Frames = new List<Frame> { new Frame { Name = "F" } }
            },
            Profiles = new List<Profile> { profile }
        };

        var problems = file.Validate();

        Assert.Contains("Profile at index 0.Type must be either 'evented' or 'sampled', but was 'invalid'.", problems);
    }

    [Fact]
    public void EnsureValid_InvalidFile_ThrowsArgumentException()
    {
        var file = new SpeedscopeFile
        {
            Shared = null,
            Profiles = null
        };

        var ex = Assert.Throws<ArgumentException>(() => file.EnsureValid());

        Assert.Contains("SpeedscopeFile.Shared is required", ex.Message);
    }
}
