# FlameNodeExtensions

Static extension methods on `FlameNode` for querying and measuring the call tree: searching by name or depth, collecting leaves, summing filtered weights, computing percentages, and walking up to the root. They let callers work with a parsed tree without writing their own traversal code.

All methods are in the `SkiaFlameGraph.Core.Models` namespace. Each one throws `ArgumentNullException` when `node` is `null`.

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
root (100, depth 0)
└── main (100, depth 1)
    ├── parse (60, depth 2)
    │   └── io (20, depth 3)
    └── render (30, depth 2)
```

## Methods

### FindByName

```csharp
public static FlameNode? FindByName(this FlameNode? node, string name)
```

Returns the first node whose `Name` equals `name` (case-sensitive), checking `node` itself before its descendants. Returns `null` if nothing matches.

**Throws:** `ArgumentNullException` if `node` is `null`; `ArgumentException` if `name` is `null` or empty.

**Example:**

```csharp
var found = root.FindByName("io");        // io node
var missing = root.FindByName("IO");      // null (case-sensitive)
```

### GetNodesAtDepth

```csharp
public static IEnumerable<FlameNode> GetNodesAtDepth(this FlameNode node, int depth)
```

Returns the descendants that are `depth` levels below `node`. Depth `0` means direct children. The result is lazy.

**Throws:** `ArgumentNullException` if `node` is `null`; `ArgumentOutOfRangeException` if `depth` is negative.

**Example:**

```csharp
var direct = root.GetNodesAtDepth(0).Select(n => n.Name);  // main
var grand  = root.GetNodesAtDepth(1).Select(n => n.Name);  // parse, render
```

### SumValuesWhere

```csharp
public static double SumValuesWhere(this FlameNode node, Func<FlameNode, bool> predicate)
```

Adds up the `Value` of every node in the subtree (including `node` itself) for which `predicate` returns `true`. Matching nodes contribute their full `Value`, not their `SelfValue`, so a matching parent also includes its children's weight.

**Throws:** `ArgumentNullException` if `node` or `predicate` is `null`.

**Example:**

```csharp
// Sum of all leaf frames' weights: io (20) + render (30)
double leafTotal = root.SumValuesWhere(n => n.Children.Count == 0);  // 50
```

### GetLeafNodes

```csharp
public static IEnumerable<FlameNode> GetLeafNodes(this FlameNode node)
```

Returns every node in the subtree that has no children, in depth-first order. If `node` itself has no children, the result is just `node`. The result is lazy.

**Throws:** `ArgumentNullException` if `node` is `null`.

**Example:**

```csharp
var leaves = root.GetLeafNodes().Select(n => n.Name);  // io, render
```

### CalculatePercentageOfTotal

```csharp
public static double CalculatePercentageOfTotal(this FlameNode node, FlameNode totalRoot)
```

Returns `node.Value / totalRoot.Value * 100`. The result is on a 0–100 scale. `totalRoot` is passed in separately so that the percentage can be taken against any reference tree, such as the full profile when `node` is a sub-tree.

**Throws:** `ArgumentNullException` if `node` or `totalRoot` is `null`; `ArgumentException` if `totalRoot.Value` is zero.

**Example:**

```csharp
double pct = parse.CalculatePercentageOfTotal(root);  // 60
```

### GetPathToNode

```csharp
public static IEnumerable<string> GetPathToNode(this FlameNode node)
```

Returns the names from the root down to `node`, inclusive, using `Parent` links. The first element is the root, and the last is `node`. The method relies on `Parent` being set correctly, which `AddChild` does for you.

**Throws:** `ArgumentNullException` if `node` is `null`.

**Example:**

```csharp
string path = string.Join(" > ", io.GetPathToNode());  // root > main > parse > io
```

### GetDeepestNode

```csharp
public static FlameNode GetDeepestNode(this FlameNode node)
```

Returns the node in the subtree with the greatest `Depth`. When several nodes share the maximum depth, the first one found in depth-first order is returned. If `node` is a leaf, it returns `node`.

**Throws:** `ArgumentNullException` if `node` is `null`.

**Example:**

```csharp
string deepest = root.GetDeepestNode().Name;  // io
```

### MaxSelfValue

```csharp
public static double MaxSelfValue(this FlameNode node)
```

Returns the largest `SelfValue` among `node` and all of its descendants. A node with no children returns its own `SelfValue`.

**Throws:** `ArgumentNullException` if `node` is `null`.

**Example:**

```csharp
double hottest = root.MaxSelfValue();  // weight of the single most expensive frame
```

### GetCumulativeValueToRoot

```csharp
public static double GetCumulativeValueToRoot(this FlameNode node)
```

Returns the sum of `SelfValue` over `node` and all of its ancestors. This is the weight that the call stack from the root to `node` accounts for in its own frames.

**Throws:** `ArgumentNullException` if `node` is `null`.

**Example:**

```csharp
// io (20) + parse (40) + main (10) + root (0)
double cumulative = io.GetCumulativeValueToRoot();  // 70
```
