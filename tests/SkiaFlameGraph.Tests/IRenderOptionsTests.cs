public interface IRenderOptionsTests
{
    void DefaultValues_ShouldMatchExpected();
    void CanSetWidthAndRowHeight();
    void CanSetMinWidthsAndPadding();
    void CanSetColors();
    void CanToggleInvertedAndHighlightPattern();
    void SettingNegativeValues_ShouldNotThrow();
}
