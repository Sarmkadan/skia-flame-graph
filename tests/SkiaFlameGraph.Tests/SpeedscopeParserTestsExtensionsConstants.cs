using System;

namespace SkiaFlameGraph.Tests
{
    /// <summary>
    /// Constant values used in <see cref="SpeedscopeParserTestsExtensions"/>.
    /// </summary>
    internal static class SpeedscopeParserTestsExtensionsConstants
    {
        public const string FrameNameFormat = "{\"name\": \"{0}\"}";
        public const string SampleIndicesFormat = "[{0}]";
        public const string EventFormat = "{\"type\": \"{0}\", \"frame\": {1}, \"at\": {2}";
        public const string SharedFramesStart = "\"shared\": {\"frames\": [";
        public const string ProfilesArrayStart = "],\"profiles\": [{";
        public const string UnitMilliseconds = "\"unit\": \"milliseconds\",";
        public const string StartValueZero = "\"startValue\": 0,";
        public const string EndValuePrefix = "\"endValue\": ";
        public const string SamplesArrayStart = "\"samples\": [";
        public const string WeightsArrayStart = "\"weights\": [";
        public const string EventsArrayStart = "\"events\": [";
        public const string ProfilesArrayAndObjectEnd = "}]";
    }
}