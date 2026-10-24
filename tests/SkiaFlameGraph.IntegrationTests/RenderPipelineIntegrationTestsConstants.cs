namespace SkiaFlameGraph.IntegrationTests;

/// <summary>
/// Magic values shared by the render pipeline integration tests.
/// </summary>
internal static class RenderPipelineIntegrationTestsConstants
{
    /// <summary>Fixture file copied to the test output's Fixtures folder.</summary>
    public const string FixtureFileName = "collapsed-stacks-sample.txt";

    /// <summary>Requested render width in pixels.</summary>
    public const int RenderWidth = 1600;

    /// <summary>Sum of all sample counts in the fixture; the root's value.</summary>
    public const double FixtureTotalSamples = 722;

    /// <summary>Deepest frame in the fixture (root is depth 0).</summary>
    public const int FixtureMaxDepth = 4;

    /// <summary>Number of non-root frames in the fixture tree.</summary>
    public const int FixtureFrameCount = 33;

    /// <summary>Background fill of <c>FlameGraphRenderer</c>'s synthetic root bar.</summary>
    public const byte RootBarRed = 0x3a;
    public const byte RootBarGreen = 0x3a;
    public const byte RootBarBlue = 0x44;

    /// <summary>
    /// Vertical offset, in pixels, from a row's top edge at which pixels are sampled. This sits above the
    /// label glyphs and inside the 1px outline drawn around each frame.
    /// </summary>
    public const int SampleInsetPx = 2;

    /// <summary>
    /// Allowed difference between a frame's measured and expected width. Each frame's 1px background-coloured
    /// outline consumes about 1px at each boundary, and the rest is rounding.
    /// </summary>
    public const float WidthTolerancePx = 3f;

    /// <summary>Byte offsets of width and height in the PNG IHDR chunk (big-endian).</summary>
    public const int PngWidthOffset = 16;
    public const int PngHeightOffset = 20;

    /// <summary>The 8-byte PNG file signature.</summary>
    public static readonly byte[] PngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
}
