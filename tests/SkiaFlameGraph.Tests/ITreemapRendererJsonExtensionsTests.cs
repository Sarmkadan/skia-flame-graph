using System;
using System.Text.Json;
using SkiaFlameGraph.Core.Rendering;
using Xunit;

namespace SkiaFlameGraph.Tests
{
    /// <summary>
    /// Interface for <see cref="TreemapRendererJsonExtensionsTests"/>
    /// </summary>
    public interface ITreemapRendererJsonExtensionsTests
    {
        void ToJson_NullValue_ThrowsArgumentNullException();
        void ToJson_IndentsFalse_ReturnsCompactJson();
        void FromJson_NullJson_ThrowsArgumentNullException();
        void FromJson_EmptyOrWhitespaceJson_ThrowsJsonException();
        void TryFromJson_NullJson_ThrowsArgumentNullException();
        void TryFromJson_InvalidJson_ThrowsJsonException();
    }
}
