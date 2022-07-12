using System;
using System.Collections.Generic;
using KeyType = System.Int32;

namespace LLRB
{
    enum Color
    {
        RED = 1,
        BLACK = 0,
    }

    class Node
    {
        public KeyType Key;

        public Color Color = Color.RED;

        public Node Left = null;
        public Node Right = null;
        public Node Parent = null;

        public Node(KeyType key)
        {
            Key = key;
        }
    }

    class Tree
    {
        public Node Root = null;

        public void Insert(KeyType key)
        {
            Insert(new Node(key));

            //ValidateChildrensParent();
            //PrintAction("Insert", key);
        }

        public void Insert(Node node)
        {
            if (Root == null)
            {
                Root = node;
                Root.Color = Color.BLACK;
                Root.Parent = null;
                Root.Left = null;
                Root.Right = null;
            }
            else
            {
                // Search downward for leaf.
                Node pivot = Root;
                Node pivotParent = pivot;
                bool nodeIsLeft = true;
                while (pivot != null)
                {
                    // Left sub-tree.
                    if (node.Key <= pivot.Key)
                    {
                        pivotParent = pivot;
                        pivot = pivot.Left;
                        nodeIsLeft = true;
                    }

                    // Right sub-tree.
                    else
                    {
                        pivotParent = pivot;
                        pivot = pivot.Right;
                        nodeIsLeft = false;
                    }
                }

                // Insert node.
                node.Parent = pivotParent;
                node.Color = Color.RED;
                node.Left = null;
                node.Right = null;

                if (nodeIsLeft)
                {
                    pivotParent.Left = node;
                }
                else
                {
                    pivotParent.Right = node;
                }

                _AdjustTree(node);
            }
        }

        public Node Search(KeyType key)
        {
            Node pivot = Root;
            while (pivot != null)
            {
                // Left sub-tree.
                if (key < pivot.Key)
                {
                    pivot = pivot.Left;
                }

                // Right sub-tree.
                else if (key > pivot.Key)
                {
                    pivot = pivot.Right;
                }

                // Match.
                else
                {
                    return pivot;
                }
            }
            return null;
        }

        public void Remove(KeyType key)
        {
            Node target = Search(key);
            if (target != null)
            {
                // [Case 0] Root is leaf: Null it.
                if (target == Root && Root.Left == null)
                {
                    Root = null;
                }

                // [Case 1] 3-key leaf (red): Remove simply.
                else if (target.Color == Color.RED && target.Left == null && target.Right == null)
                {
                    // Red node is always a left child.
                    target.Parent.Left = null;
                    target.Parent = null;
                }

                // [Case 2] 3-key leaf (black): Replace it with its red leaf, and flip it as black.
                else if (target.Color == Color.BLACK && target.Left != null && target.Left.Color == Color.RED && target.Left.Left == null)
                {
                    Node redLeaf = target.Left;
                    redLeaf.Color = Color.BLACK;

                    if (target == Root)
                    {
                        Root = redLeaf;

                        redLeaf.Parent = null;
                        target.Left = null;
                    }
                    else
                    {
                        Node parent = target.Parent;

                        redLeaf.Parent = parent;
                        if (parent.Left == target) parent.Left = redLeaf;
                        else parent.Right = redLeaf;

                        target.Left = null;
                        target.Parent = null;
                    }
                }

                // [Case 3] 2-key black leaf with 3-key right sibling: Downgrade parent, and uplift red sibling as new black parent.
                else if (target.Color == Color.BLACK && target.Left == null && target.Right == null && target.Parent is Node parent &&
                    parent.Left == target && parent.Right is Node blackSibling && blackSibling.Left is Node redSibling)
                {
                    // If the parent is black root.
                    if (parent == Root)
                    {
                        // Set as root, and flip as black.
                        Root = redSibling;
                        redSibling.Color = Color.BLACK;

                        // Detach red sibling.
                        redSibling.Parent = null;
                        blackSibling.Left = null;

                        // Detach target.
                        target.Parent = null;
                        parent.Left = null;

                        // Attach parent as left leaf.
                        redSibling.Left = parent;
                        parent.Parent = redSibling;
                        parent.Right = null;

                        // Attach black sibling as right child.
                        redSibling.Right = blackSibling;
                        blackSibling.Parent = redSibling;
                    }

                    // If parent is black, but not root.
                    else if (parent.Color == Color.BLACK)
                    {
                        // Connect with grandparent, and flip as black.
                        Node grandparent = parent.Parent;
                        if (grandparent.Left == parent) grandparent.Left = redSibling;
                        else grandparent.Right = redSibling;
                        redSibling.Parent = grandparent;
                        redSibling.Color = Color.BLACK;

                        // Detach red sibling.
                        blackSibling.Left = null;

                        // Detach target.
                        target.Parent = null;
                        parent.Left = null;

                        // Attach parent as left leaf.
                        redSibling.Left = parent;
                        parent.Parent = redSibling;
                        parent.Right = null;

                        // Attach black sibling as right child.
                        redSibling.Right = blackSibling;
                        blackSibling.Parent = redSibling;
                    }

                    // If parent is red.
                    else
                    {
                        // Connect red sibling with grandparent (the black one in the same 3-key as the red parent).
                        Node grandparent = parent.Parent;
                        grandparent.Left = redSibling;
                        redSibling.Parent = grandparent;
                        blackSibling.Left = null;

                        // Detach target.
                        target.Parent = null;
                        parent.Left = null;

                        // Attach parent as left child, and flip parent as black.
                        redSibling.Left = parent;
                        parent.Parent = redSibling;
                        parent.Right = null;
                        parent.Color = Color.BLACK;

                        // Attach black sibling as right child.
                        redSibling.Right = blackSibling;
                        blackSibling.Parent = redSibling;
                    }
                }

                // [Case 4] 2-key black leaf with 3-key left sibling: Remove element, spin red sibling clockwise, and flip as black.
                else if (target.Color == Color.BLACK && target.Left == null && target.Right == null && target.Parent != null &&
                    target.Parent.Right == target && target.Parent.Left is Node leftBlackSibling && leftBlackSibling.Left is Node leftmostRedSibling)
                {
                    // Remove target.
                    Node parent0 = target.Parent;
                    parent0.Right = null;
                    target.Parent = null;

                    SpinClockwiseAndSyncColorWithGrandparent(leftmostRedSibling);

                    // Flip colors if both new children become red after spinning (or simply call `AdjustTree`.)
                    if (parent0.Color == Color.RED)
                    {
                        Node newParent = parent0.Parent;
                        newParent.Color = Color.RED;
                        newParent.Left.Color = Color.BLACK;
                        newParent.Right.Color = Color.BLACK;
                    }
                }

                // Done: Test each case respectively before implementing more cases.
                // TODO: Merge the sub-cases if probable.

                // TODO: Validate red-black traits (same black height of leaves, no consecutive red nodes, red always on left child) and binary-search trait.
            }

            PrintAction("Remove", key);
            ValidateChildrensParent();
        }

        // Adjust the tree recursively to balance.
        private void _AdjustTree(Node newRed)
        {
            // Validate color.
            if (newRed.Color != Color.RED)
            {
                if (newRed == Root) newRed.Parent = null;
                else Console.WriteLine($"Adjust non-red node with key {newRed.Key}");

                return;
            }

            Node parent = newRed.Parent;
            bool nodeIsLeft = parent.Left == newRed;

            // [Case 1] Red parent, left child: Spin clockwise & recurse.
            if (parent.Color == Color.RED && nodeIsLeft)
            {
                SpinClockwiseAndSyncColorWithGrandparent(newRed);

                _AdjustTree(parent);
            }

            // [Case 2] Red parent, right child: spin reds counter-clockwise, then spin clockwise recursively.
            else if (parent.Color == Color.RED && !nodeIsLeft)
            {
                SpinCounterClockwiseAndSwapColorWithParent(newRed);
                SpinClockwiseAndSyncColorWithGrandparent(newRed.Left);

                _AdjustTree(newRed);
            }

            // [Case 3] Black parent, left child: Do nothing.

            // [Case 4] Black parent, right child: Spin counter-clockwise, and swap colors.
            else if (parent.Color == Color.BLACK && !nodeIsLeft)
            {
                SpinCounterClockwiseAndSwapColorWithParent(newRed);

                // The black parent becomes red-left.
                // If its left sibling is red, recurse.
                if (parent.Left != null && parent.Left.Color == Color.RED)
                {
                    _AdjustTree(parent.Left);
                }
            }
        }

        /// <summary>
        /// 1. Flip the color of leftmost-red as its grandparent;<br/>
        /// 2. Raise its parent;<br/>
        /// 3. Attach right sibling to grandparent.<br/>
        /// </summary>
        /// <remarks>
        /// This can be applied for two consecutive red nodes after insertion,<br/>
        /// or for the 3-key left leaf after the removal of its right 2-key sibling.
        /// </remarks>
        public void SpinClockwiseAndSyncColorWithGrandparent(Node leftmostRed)
        {
            Node parent = leftmostRed.Parent;
            Node grandparent = parent.Parent;

            // Flip color as grandparent's. For consecutive reds, grandparent must be black.
            leftmostRed.Color = grandparent.Color;

            // Nullable.
            Node greatGrandParent = grandparent.Parent;
            
            // Nullable.
            Node sibling = parent.Right;

            parent.Right = grandparent;
            grandparent.Parent = parent;

            grandparent.Left = sibling;
            if (sibling != null) sibling.Parent = grandparent;

            if (grandparent == Root)
            {
                Root = parent;
                parent.Color = Color.BLACK;
                parent.Parent = null;
            }

            // greatGrandParent != null && grandparent is left child.
            else if (greatGrandParent.Left == grandparent)
            {
                greatGrandParent.Left = parent;
                parent.Parent = greatGrandParent;
            }

            // greatGrandParent != null && grandparent is right child.
            else
            {
                greatGrandParent.Right = parent;
                parent.Parent = greatGrandParent;
            }
        }

        public void SpinCounterClockwiseAndSwapColorWithParent(Node rightRed)
        {
            Node parent = rightRed.Parent;

            if (parent == Root)
            {
                Root = rightRed;
                rightRed.Parent = null;
            }
            else
            { 
                Node grandparent = parent.Parent;

                if (grandparent.Left == parent) grandparent.Left = rightRed;
                else grandparent.Right = rightRed;

                rightRed.Parent = grandparent;
            }

            // Cousin.
            parent.Right = rightRed.Left;
            if (rightRed.Left != null) rightRed.Left.Parent = parent;

            rightRed.Left = parent;
            parent.Parent = rightRed;

            // Swap colors.
            Color tmp = rightRed.Color;
            rightRed.Color = parent.Color;
            parent.Color = tmp;
        }

        public void PrintAction(string actionName, KeyType key)
        {
            Console.WriteLine($"\n==================== {actionName}({key}) ====================");
            _PrintTree("", Root, ArrowForRoot);
        }

        private const string IndentSpaces = "      ";
        private const string ArrowForLeft = "  \\=> ";
        private const string ArrowForRoot = "Tree> ";
        private const string ArrowForRight = "  /=> ";

        /// <param name="leftRootRight">0: left child, 1: root, 2: right child.</param>
        private static void _PrintTree(string indent, Node node, string arrow)
        {
            if (node != null)
            {
                _PrintTree(indent + IndentSpaces, node.Right, ArrowForRight);
                
                Console.Write(indent);
                Console.Write(arrow);
                Console.BackgroundColor = node.Color == Color.RED ? ConsoleColor.Red : ConsoleColor.Black;
                Console.WriteLine(node.Key.ToString("D4"));
                Console.ResetColor();

                _PrintTree(indent + IndentSpaces, node.Left, ArrowForLeft);
            }
        }

        public void ValidateChildrensParent()
        {
            if (Root != null)
            {
                if (Root.Parent != null) throw new Exception($"Root {Root.Key} should not have parent {Root.Parent.Key}!");
                _ValidateChildrensParent(Root);
            }
        }

        private void _ValidateChildrensParent(Node node)
        {
            if (node.Left != null)
            {
                if (node.Left.Parent != node) throw new Exception($"Node {node.Key} left {node.Left.Key} has wrong parent {node.Left.Parent.Key}!");
                _ValidateChildrensParent(node.Left);
            }

            if (node.Right != null)
            {
                if (node.Right.Parent != node) throw new Exception($"Node {node.Key} right {node.Right.Key} has wrong parent {node.Right.Parent.Key}!");
                _ValidateChildrensParent(node.Right);
            }
        }
    }
}
