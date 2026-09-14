# Frame Palette Documentation

## Overview

The `FramePalette` class in `SkiaFlameGraph.Core.Rendering` provides deterministic color assignment for flame graph frames. The same frame name will always produce the same color across different renders, enabling visual comparison between profiles.

## Color Assignment Algorithm

### Deterministic Hashing

The algorithm uses an FNV-1a hash function (`StableHash`) to generate a consistent 32-bit hash value from the frame name. This avoids the randomized hash codes produced by `string.GetHashCode()` which would change between process runs.

### HSL Color Space

Colors are generated in the HSL (Hue, Saturation, Lightness) color space with the following ranges:

- **Hue**: 8° to 45° (reds → oranges → yellows)
  - Base hue: 8°
  - Variation: `hash % 37` (0-36)
  - Final hue: `8 + (hash % 37)`

- **Saturation**: 55% to 80%
  - Base saturation: 55%
  - Variation: `(hash >> 8) % 25` (0-24)
  - Final saturation: `55 + ((hash >> 8) % 25)`

- **Lightness**: 45% to 60%
  - Base lightness: 45%
  - Variation: `(hash >> 16) % 15` (0-14)
  - Final lightness: `45 + ((hash >> 16) % 15)`

### Highlight Frames

Frames matching a specified highlight pattern (regex) use a fixed highlight color:
- **Highlight Color**: `SKColor(0xff, 0x6b, 0x35)` (vibrant orange, #FF6B35)

## Usage

```csharp
// Get color for a frame name
SKColor color = FramePalette.ForFrame("my-frame-name");

// Get color with optional highlighting
SKColor color = FramePalette.ForFrame("my-frame-name", @"^highlight.*");
```

## Implementation Details

- The class is static and stateless
- All methods throw `ArgumentException` for null or empty frame names
- The hash computation is performed per-character for stability
- Color generation is purely functional with no side effects

## Design Rationale

1. **Deterministic Colors**: Essential for comparing flame graphs across different runs
2. **Warm Hue Band**: Reds/oranges/yellows provide good contrast and match traditional flame graph aesthetics
3. **Limited Saturation/Lightness Range**: Prevents overly bright or dull colors that could reduce readability
4. **FNV-1a Hash**: Simple, fast, and produces good distribution for string inputs
5. **Highlight Mechanism**: Allows visual emphasis of specific frames without affecting deterministic coloring of others