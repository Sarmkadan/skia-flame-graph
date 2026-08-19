namespace SkiaFlameGraph.Tests
{
    public interface IFramePaletteTests
    {
        void ForFrame_ReturnsSameColor_ForSameName();
        void ForFrame_WithHighlightPattern_ReturnsHighlightColor_WhenMatched();
        void ForFrame_ThrowsArgumentException_WhenNameIsNullOrEmpty();
        void ForFrame_DistributesColorsAcrossManyDistinctNames();
    }
}