namespace SkiaFlameGraph.IntegrationTests;

/// <summary>
/// Declares the end-to-end scenarios covered by <see cref="RenderPipelineIntegrationTests"/>.
/// </summary>
public interface IRenderPipelineIntegrationTests
{
    void ParseFile_Fixture_BuildsTreeWithCorrectDepthsAndTotals();
    void Render_Fixture_ReturnsBitmapWithRequestedWidthAndDerivedHeight();
    void Render_Fixture_RootFrameSpansFullPlotWidthOnBottomRow();
    void Render_Fixture_FrameWidthsAreProportionalToSampleCounts();
    void Render_Fixture_KnownFramePositionsHavePaletteColor(string name, int depth);
    void RenderToPng_Fixture_WritesValidPngFileToTempDirectory();
}
