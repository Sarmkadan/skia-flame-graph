namespace SkiaFlameGraph.Tests;

/// <summary>
/// Shared constants used by <see cref="SpeedscopeFileTests"/>.
/// </summary>
internal static class SpeedscopeFileTestsConstants
{
    public const string SpeedscopeSchemaUrl = "https://www.speedscope.app/file-format-schema.json";
    public const string ExampleSchemaUrl = "https://example.com/schema.json";

    public const string TestProfileName = "Test Profile";
    public const string CpuProfileName = "CPU Profile";
    public const string TestFileName = "Test File";
    public const string MyApplicationProfileName = "My Application Profile";
    public const string Profile1Name = "Profile 1";
    public const string Profile2Name = "Profile 2";

    public const string DotnetTraceExporter = "dotnet-trace";
    public const string EventedProfileType = "evented";
    public const string SampledProfileType = "sampled";
    public const string MillisecondsUnit = "milliseconds";

    public const string MainFrameName = "Main";
    public const string MethodAFrameName = "MethodA";
    public const string ProgramCsFile = "Program.cs";
    public const string TestFrameName = "Test";
    public const string MyMethodFrameName = "MyMethod";
    public const string CompleteMethodFrameName = "CompleteMethod";
    public const string Frame1Name = "Frame 1";
    public const string Frame2Name = "Frame 2";
    public const string File1Name = "file1.cs";
    public const string File2Name = "file2.cs";

    public const string OpenEventType = "O";
    public const string CloseEventType = "C";

    public const string NameJsonProperty = "name";
    public const string FileJsonProperty = "file";
    public const string LineJsonProperty = "line";
    public const string ColJsonProperty = "col";

    public const int MainFrameLine = 10;
    public const int MainFrameCol = 5;
    public const int MethodAFrameLine = 20;
    public const int MethodAFrameCol = 15;
    public const int CompleteMethodLine = 42;
    public const int CompleteMethodCol = 10;
    public const int StartValue = 0;
    public const int EndValue = 100;
    public const int CloseEventAt = 50;
    public const int FrameCount = 2;
    public const int ProfileCount = 1;
}