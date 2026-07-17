# RenderOptions

Configuration options that control the visual appearance and layout of flame graph renderings. This type is passed to renderer implementations to customize dimensions, colors, typography, and orientation without modifying rendering logic.

## API

### Width
```csharp
public int Width { get; set; }
```
Gets or sets the total canvas width in pixels. Must be greater than zero. Used to calculate horizontal scaling of stack frames.

### RowHeight
```csharp
public float RowHeight { get; set; }
```
Gets or sets the height of each call stack row in pixels. Must be positive. Determines vertical spacing between stack frames.

### MinLabelWidth
```csharp
public float MinLabelWidth { get; set; }
```
Gets or sets the minimum width in pixels required to render a frame label. Frames narrower than this threshold omit text. Must be non-negative.

### MinBoxWidth
```csharp
public float MinBoxWidth { get; set; }
```
Gets or sets the minimum width in pixels for a frame rectangle to be drawn. Frames narrower than this are not rendered. Must be non-negative.

### Padding
```csharp
public float Padding { get; set; }
```
Gets or sets the padding in pixels around the flame graph content. Applied uniformly on all sides. Must be non-negative.

### FontSize
```csharp
public float FontSize { get; set; }
```
Gets or sets the font size in points for frame labels. Must be positive.

### Background
```csharp
public SKColor Background { get; set; }
```
Gets or sets the background color of the rendered canvas. Defaults to white.

### TextColor
```csharp
public SKColor TextColor { get; set; }
```
Gets or sets the color used for frame labels. Defaults to black.

### Inverted
```csharp
public bool Inverted { get; set; }
```
Gets or sets whether the flame graph is rendered inverted (root at bottom, leaves at top). When `false`, root frames appear at the top.

## Usage

### Basic configuration for a standard flame graph
```csharp
var options = new RenderOptions
{
    Width = 1200,
    RowHeight = 18f,
    MinLabelWidth = 40f,
    MinBoxWidth = 2f,
    Padding = 10f,
    FontSize = 11f,
    Background = SKColors.White,
    TextColor = SKColors.Black,
    Inverted = false
};

using var surface = SKSurface.Create(new SKImageInfo(options.Width, CalculateHeight(data, options)));
var renderer = new FlameGraphRenderer(options);
renderer.Render(surface.Canvas, profileData);
```

### Inverted icicle graph with dark theme
```csharp
var options = new RenderOptions
{
    Width = 1600,
    RowHeight = 20f,
    MinLabelWidth = 50f,
    MinBoxWidth = 3f,
    Padding = 20f,
    FontSize = 12f,
    Background = new SKColor(0x1E, 0x1E, 0x1E),
    TextColor = new SKColor(0xD4, 0xD4, 0xD4),
    Inverted = true
};

using var bitmap = new SKBitmap(options.Width, CalculateHeight(data, options));
using var canvas = new SKCanvas(bitmap);
new FlameGraphRenderer(options).Render(canvas, profileData);
bitmap.Encode(SKEncodedImageFormat.Png, 100).SaveTo(File.OpenWrite("icicle.png"));
```

## Notes

- All dimension properties (`Width`, `RowHeight`, `MinLabelWidth`, `MinBoxWidth`, `Padding`, `FontSize`) should be validated by the caller; the type does not enforce constraints in setters.
- `SKColor` values are copied by value; mutating the original struct after assignment has no effect on the stored color.
- This type is immutable in practice once passed to a renderer. Concurrent reads from multiple threads are safe; concurrent writes to the same instance are not thread-safe.
- `Inverted` affects only the vertical layout direction; coordinate calculations in renderers should respect this flag when mapping stack depth to Y positions.
- `MinBoxWidth` and `MinLabelWidth` serve different culling purposes: the former hides the frame rectangle entirely, the latter hides only the text label while the rectangle may still render.
