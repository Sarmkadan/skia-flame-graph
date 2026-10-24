namespace SkiaFlameGraph.IntegrationTests;

/// <summary>
/// Magic values shared by the collapsed-stacks parser integration tests.
/// </summary>
internal static class CollapsedStacksParserIntegrationTestsConstants
{
    /// <summary>Number of stacks in the generated profile. Above 500 to exercise a realistic dump size.</summary>
    public const int GeneratedLineCount = 600;

    /// <summary>Deepest generated stack; stacks cycle through depths 1..MaxGeneratedDepth.</summary>
    public const int MaxGeneratedDepth = 5;

    /// <summary>Multiplier and modulus that spread generated sample counts over 1..CountModulo.</summary>
    public const int CountStride = 37;
    public const int CountModulo = 97;

    /// <summary>File name used for the temporary profile written by file-based tests.</summary>
    public const string ProfileFileName = "profile.collapsed.txt";

    /// <summary>Separator between frames in a collapsed-stack line.</summary>
    public const char FrameSeparator = ';';

    /// <summary>
    /// Malformed input mixed with valid lines. Valid lines are: "main;parse 5", "main;render 7   ",
    /// "main;render\t3\t ", "main;;leaf 2", "main;leaf 2  \t". Every other line must be skipped.
    /// </summary>
    public static readonly string[] MalformedLines =
    {
        "main;parse 5",
        "main;missingCount",
        "",
        "   ",
        "main;render 7   ",
        "main;encode x",
        "main;zero 0",
        "main;negative -4",
        " 9",
        ";; 2",
        "main;render\t3\t ",
        "main;;leaf 2",
        "main;leaf 2  \t",
    };

    /// <summary>Valid lines from <see cref="MalformedLines"/>, in the same order.</summary>
    public static readonly string[] MalformedLinesValidOnly =
    {
        "main;parse 5",
        "main;render 7   ",
        "main;render\t3\t ",
        "main;;leaf 2",
        "main;leaf 2  \t",
    };
}
