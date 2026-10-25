# FlameNodeQueryExtensions

Static extension methods on `FlameNode` for walking the call tree: enumerating every node in depth-first order, counting nodes, and following the hottest path from a node down to a leaf. They return the tree's own nodes, so callers can filter or measure them without writing their own traversal code.

All methods are in the `SkiaFlameGraph.Core.Models` namespace. Each one throws `ArgumentNullException` when `node` is `null`.

> The two iterator methods, `DescendantsDepthFirst` and `HottestPath`, are `yield`-based. Their null check runs when the sequence is first enumerated, not when the method is called. `TotalNodeCount` runs eagerly.

## Example tree

The examples below use this tree:

```csharp
var root = new FlameNode("root") { Value = 100 };
var main = root.AddChild("main");
main.Value = 100;
var parse = main.AddChild("parse");
parse.Value = 60;
var render = main.AddChild("render");
render.Value = 30;
var io = parse.AddChild("io");
io.Value = 20;
```

```
root (100)
└── main (100)
    ├── parse (60)
    │   └── io (20)
    └── render (30)
```

## Methods

### DescendantsDepthFirst

```csharp
public static IEnumerable<FlameNode> DescendantsDepthFirst(this FlameNode node)
```

Returns `node` and every node below it in depth-first, pre-order sequence: a node is yielded before its children, and each child's subtree is finished before the next sibling starts. The result is lazy and reflects the tree as it is when enumerated.

**Throws:** `ArgumentNullException` if `node` is `null` (on enumeration).

**Example:**

```csharp
var names = root.DescendantsDepthFirst().Select(n => n.Name);
// root, main, parse, io, render

var leaves = root.DescendantsDepthFirst().Where(n => n.Children.Count == 0);
// io, render
```

### TotalNodeCount

```csharp
public static int TotalNodeCount(this FlameNode node)
```

Returns the number of nodes in the subtree, counting `node` itself. A leaf returns `1`. The count is computed recursively on each call, so for repeated queries on a large tree, consider caching the result or using `DescendantsDepthFirst().Count()`.

**Throws:** `ArgumentNullException` if `node` is `null`.

**Example:**

```csharp
int total = root.TotalNodeCount();   // 5
int inMain = main.TotalNodeCount();  // 4
```

### HottestPath

```csharp
public static IEnumerable<FlameNode> HottestPath(this FlameNode node)
```

Starts at `node` and repeatedly moves to the child with the largest `Value`, yielding each node along the way. It stops at the first node with no children, so the last element is a leaf. The result is lazy. When two siblings have the same `Value`, `MaxBy` picks the first one in the `Children` list.

Because the walk follows `Value` (total weight) rather than `SelfValue`, a child that carries a large total through its own children is preferred over a sibling with a larger self-time.

**Throws:** `ArgumentNullException` if `node` is `null` (on enumeration).

**Example:**

```csharp
string path = string.Join(" > ", root.HottestPath().Select(n => n.Name));
// root > main > parse > io

FlameNode hottestLeaf = root.HottestPath().Last();  // io
```

## Notes

- `DescendantsDepthFirst` walks the live tree. Adding or removing children while it is being enumerated throws `InvalidOperationException` from the underlying `List<T>` enumerator; call `.ToList()` first if the tree will change.
- These methods do not compute weights. Use `Value` as set by the parser or builder, and `SelfValue` where exclusive time is needed.
- For tree-wide measurements such as `SumValuesWhere` or `MaxSelfValue`, see [FlameNodeExtensions](FlameNodeExtensions.md).
