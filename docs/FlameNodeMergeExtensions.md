# FlameNodeMergeExtensions

Static extension methods on `FlameNode` that combine or compare two call trees: `Merge` adds their weights together, and `Diff` subtracts a baseline from a current tree. Both return a new tree and leave their inputs unchanged.

Both methods are in the `SkiaFlameGraph.Core.Models` namespace and throw `ArgumentNullException` when either argument is `null`.

> `Diff` here is the tree-level extension. The differential renderer and the public `FlameDiff.Diff` API use a separate implementation in `SkiaFlameGraph.Core.FlameDiff` (see [FlameDiff](FlameDiff.md)). The two share the same name-matching approach, but they are not the same code.

## Shared matching rule

Both algorithms pair children by `Name` only. `File` and `Line` are not part of the match, unlike `FlameNode.AddChild`, which also compares them. Two sibling frames with the same name are treated as the same frame, even if they come from different source locations.

Matching happens one level at a time. A child is matched only against children of the matched parent. Two nodes are matched only when their parents were matched (the roots are always matched with each other).

## Example trees

The examples below use these two trees:

```csharp
var a = new FlameNode("root") { Value = 10 };
var aMain = a.AddChild("main");
aMain.Value = 10;
var parse = aMain.AddChild("parse");
parse.Value = 6;

var b = new FlameNode("root") { Value = 5 };
var bMain = b.AddChild("main");
bMain.Value = 5;
var render = bMain.AddChild("render");
render.Value = 5;
```

```
a                         b
root (10)                 root (5)
└── main (10)             └── main (5)
    └── parse (6)             └── render (5)
```

## Merge

```csharp
public static FlameNode Merge(this FlameNode a, FlameNode b)
```

Builds a new tree whose weights are the sum of `a` and `b`.

### Algorithm

1. Create the result node from `a`: `Name`, `File`, `Line`, and `Depth` are copied from `a`, and `Value` is `a.Value + b.Value`. The values are summed as stored, not recalculated from the children.
2. Index `b.Children` by name in a dictionary.
3. For each child of `a`:
   - If `b` has a child with the same name, recurse with `Merge(aChild, bChild)`.
   - Otherwise, add a deep copy of `aChild` (`CloneSubtree`).
4. For each child of `b`:
   - Skip it if `a` also has a child with that name. It was already handled in step 3.
   - Otherwise, add a deep copy of `bChild` (`CloneSubtree`).
5. Set `Parent` on every child added to the result. The returned root's `Parent` stays `null`.

The result lists `a`'s children first, in their original order, followed by `b`-only children in `b`'s order.

**Throws:** `ArgumentNullException` if `a` or `b` is `null`.

**Example:**

```csharp
var merged = a.Merge(b);

// merged: root (15)
//         ├── main (15)
//         │   ├── parse (6)     <- only in a
//         │   └── render (5)    <- only in b
```

Here `root` and `main` are matched, so their values are summed. `parse` and `render` each appear in only one input, so they are copied with their original values.

### Notes

- **Metadata comes from `a`.** For matched nodes, `File`, `Line`, and `Depth` are taken from `a`. Values in `b` for those fields are ignored.
- **Duplicate sibling names.** The name index keeps only the last child of `b` with a given name. If `a` has several children with that name, each of them is merged with that same `b` child, so `b`'s weight is counted once per `a` duplicate. If `b` has duplicates whose name is not in `a`, each one is copied. Duplicate names are therefore not handled as a clean sum. This is based on reading the code, not on a test.
- **Redundant check.** In step 4, `bChildrenByName.ContainsKey(...)` is always `true` for names taken from `b.Children`. The effective condition is just "`a` has a child with this name".

## Diff

```csharp
public static FlameNode Diff(this FlameNode baseline, FlameNode current)
```

Builds a new tree whose value at each node is `current - baseline`. Nodes that exist in only one tree keep their full value, or the negated value when they exist only in the baseline.

### Algorithm

1. Create the result node from `current`: `Name`, `File`, `Line`, and `Depth` are copied from `current`, and `Value` is `current.Value - baseline.Value`.
2. Index `baseline.Children` by name in a dictionary.
3. For each child of `current`:
   - If the baseline has a child with the same name, recurse with `Diff(baselineChild, currentChild)`.
   - Otherwise, add a deep copy of `currentChild` with its values unchanged (`CloneSubtree`). This is a frame that only appears in the current profile.
4. For each child of `baseline`:
   - Skip it if `current` also has a child with that name. It was already handled in step 3.
   - Otherwise, add a deep copy of `baselineChild` with every `Value` negated (`NegateSubtree`). This is a frame that only appears in the baseline profile.
5. Set `Parent` on every child added to the result. The returned root's `Parent` stays `null`.

The result lists `current`-only and matched children first, in `current`'s order, followed by the negated baseline-only children.

**Throws:** `ArgumentNullException` if `baseline` or `current` is `null`.

**Example:**

```csharp
var delta = a.Diff(b);

// delta: root (-5)
//        ├── main (-5)
//        │   ├── render (5)    <- only in current, copied as is
//        │   └── parse (-6)    <- only in baseline, negated
```

`root` and `main` are matched, so their values are `5 - 10 = -5`. `render` only exists in `b`, so it keeps `5`. `parse` only exists in `a`, so it becomes `-6`.

### Properties

- **Reversal.** Swapping the arguments negates every value: `a.Diff(b)` and `b.Diff(a)` have the same structure and opposite values. Names, files, and lines still come from the `current` argument, so they differ between the two calls.
- **Mixed signs.** A matched node can end up with a positive or negative value, depending on whether it got heavier or lighter. Unmatched nodes keep their own sign: positive for current-only frames, negative for baseline-only frames.

### Notes

- **Duplicate sibling names.** The same rules apply as for `Merge`: the last baseline child with a given name is used for matching, and each `current` duplicate with that name is compared against it.
- **Redundant check.** In step 4, `baselineChildrenByName.ContainsKey(...)` is always `true`, so the effective condition is again "`current` has a child with this name".

## Shared behaviour

| Property | Behaviour |
|----------|-----------|
| Mutation | Inputs are never modified. Every node in the result is a new `FlameNode`. |
| Values | Matched nodes are combined as `a + b` (`Merge`) or `current - baseline` (`Diff`). Values are used as stored, not recalculated from children. |
| Consistency | If both inputs satisfy `Value = self + sum(children)`, the output does too. Unmatched subtrees are copied unchanged or negated, so they stay consistent. |
| Recursion | Both methods recurse once per tree level. Very deep trees use more stack. |
| Cost | Building each name index is linear in the child count of that node. The `Any` check in step 4 is linear for each `b` (or baseline) child, so a node with many children costs about `k²` comparisons, where `k` is the child count. |
| Usage | Neither method is called anywhere else in `src/` or `tests/` at the time of writing. The renderer uses `FlameDiff.Diff`. |

## Related

- [FlameNode](FlameNode.md): the node type these methods operate on.
- [FlameDiff](FlameDiff.md): the diff used by `DifferentialFlameGraphRenderer`.
- [FlameNodeExtensions](FlameNodeExtensions.md): read-only queries on the same type.
