using System;

namespace SkiaFlameGraph.Tests
{
    public interface IFlameGraphRendererTests
    {
        void Constructor_WithNullOptions_UsesDefaultOptions();
        void Constructor_WithValidOptions_UsesProvidedOptions();
        void Constructor_WithInvalidOptions_ThrowsValidationException();
        void Render_WithEmptyRoot_ThrowsArgumentException();
        void Render_WithSingleNode_CalculatesCorrectHeight();
        void Render_WithDeepTree_CalculatesCorrectHeight();
        void RenderToPng_WithNullRoot_ThrowsArgumentNullException();
        void RenderToPng_WithNullPath_ThrowsArgumentNullException();
        void RenderToPng_WithEmptyPath_ThrowsArgumentException();
        void Render_WithInvertedOption_CalculatesCorrectHeight();
        void Render_WithHighlightPattern_CreatesRendererWithHighlightPattern();
        void Render_WithCustomBackgroundColor_CreatesRendererWithCustomColor();
        void Render_WithCustomDimensions_CreatesRendererWithCustomWidth();
    }
}
