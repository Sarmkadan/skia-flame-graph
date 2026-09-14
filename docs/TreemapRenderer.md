# TreemapRenderer

The `TreemapRenderer` class implements a squarified treemap layout algorithm for visualizing hierarchical data as a space-filling approximation of a tree structure. This renderer is particularly useful when the aggregate cost of nodes is more important than the call ordering.

## Algorithm Overview

The treemap layout follows the squarified treemap algorithm by Bruls, Huizing, and van Wijk (2000). The algorithm recursively partitions a rectangle into smaller rectangles representing nodes, aiming to maintain aspect ratios close to 1 (square) for better readability.

### Key Steps

1. **Sorting**: Child nodes are sorted in descending order by their value (typically representing size or weight).

2. **Row-based Packing**: 
   - The algorithm processes nodes in rows, filling the available rectangle from one side.
   - For each row, it calculates how many nodes can be added before the aspect ratio of the row's rectangles starts to degrade.
   - The aspect ratio is defined as the ratio of the longer side to the shorter side of a rectangle (closer to 1 is better).

3. **Placement**:
   - Rows are placed along the shorter dimension of the remaining space.
   - If the width is greater than the height, nodes are stacked vertically (horizontal layout).
   - If the height is greater than or equal to the width, nodes are stacked horizontally (vertical layout).

4. **Recursion**: 
   - Each node's rectangle is then processed recursively for its children.
   - Recursion stops when:
     - The node has no children (leaf node).
     - The maximum treemap depth (12) is reached.
     - The allocated rectangle becomes too small (< 2x2 pixels).

### Visual Properties

- **Leaf Nodes**: Filled with a color from the frame palette, bordered, and labeled if space permits.
- **Internal Nodes**: Not filled (only their children are visible), but still participate in the layout.
- **Labels**: Drawn with 3px left padding and baseline aligned to the font size. Labels are omitted if the rectangle is too narrow (< 34px) or too short (< font size + 2px).
- **Padding**: Configured padding is applied to the overall drawing area.
- **Strokes**: 1px antialiased strokes in the background color separate cells.

### Constants

- `DefaultHeightRatio = 0.62f`: Default height as 62% of width when height is not explicitly specified.
- `MinCellSize = 2`: Minimum width/height in pixels for a cell to be rendered.
- `MaxTreemapDepth = 12`: Maximum recursion depth to prevent excessive fragmentation.
- `MinLabelWidth = 34`: Minimum width in pixels required to attempt label rendering.
- `LabelMinHeightExtra = 2`: Additional height in pixels required beyond font size for label rendering.

### Algorithm Details

The core logic resides in the `Squarify` method:

1. **Initialization**: 
   - Calculate total value of all children.
   - Start with the full available rectangle.

2. **Row Construction**:
   - For each potential row, calculate the area per value unit.
   - Greedily add children to the current row while the aspect ratio improves or stays acceptable.
   - The aspect ratio calculation considers the worst-case ratio among all rectangles that would be in the row.

3. **Row Placement**:
   - Determine orientation based on whether width >= height.
   - Calculate the dimension of the row (width for horizontal layout, height for vertical).
   - Place each child's rectangle proportionally to its value within the row.
   - Recursively layout each child within its allocated rectangle (inset by 1px for visual separation).

4. **Termination Conditions**:
   - When all children are placed.
   - When a row would have zero or negative remaining space.

### Mathematical Formulation

For a row of nodes with values `v₁, v₂, ..., vₙ`:
- Total row value: `V = Σvᵢ`
- Area per value: `A = (width × height) / V` (for the current remaining space)
- Each node's area: `aᵢ = vᵢ × A`
- In horizontal layout (width ≥ height):
  - Row height = `height` (full available height)
  - Row width = `V × A / height`
  - Each node's height = `vᵢ × height / V`
- In vertical layout (height > width):
  - Row width = `width` (full available width)
  - Row height = `V × A / width`
  - Each node's width = `vᵢ × width / V`

The algorithm seeks to minimize the maximum aspect ratio (max(width/height, height/width)) across all nodes in the row.

## Usage

The renderer is used via the `Render` or `RenderToPng` methods inherited from `BaseFlameNodeRenderer`. It requires a root `FlameNode` and optional render options (width, padding, colors, etc.).

## Implementation Notes

- The algorithm modifies the drawing area by removing a 1px inset (`Deflate`) between cells to create visual separation.
- Colors are determined by `FramePalette.ForFrame(node.Name)` for leaf nodes.
- Text rendering uses antialiasing and is clipped to cell boundaries.
- The implementation includes argument null checks for public methods.

## References

- Bruls, M., Huizing, K., & van Wijk, J. J. (2000). "Squarified Treemaps." In Proceedings of the Joint Eurographics and IEEE TCVG Symposium on Visualization (pp. 33-42).