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
            
            PrintAction("Insert", node);
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

                    if (target == Root)
                    {
                        Root = redLeaf;

                        redLeaf.Parent = null;
                        target.Left = null;
                        redLeaf.Color = Color.BLACK;
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

                // [Case 3] black leaf with 3-key right sibling:
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

                        // Attach parent as left child.
                        redSibling.Left = parent;
                        parent.Parent = redSibling;

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

                        // Attach parent as left child.
                        redSibling.Left = parent;
                        parent.Parent = redSibling;

                        // Attach black sibling as right child.
                        redSibling.Right = blackSibling;
                        blackSibling.Parent = redSibling;
                    }

                    // If parent is red.
                    else
                    {
                        // Connect with grandparent (the black one in the same 3-key as the red parent).
                        Node grandparent = parent.Parent;
                        grandparent.Left = redSibling;
                        redSibling.Parent = grandparent;

                        // Detach red sibling.
                        blackSibling.Left = null;

                        // Detach target.
                        target.Parent = null;
                        parent.Left = null;

                        // Attach parent as left child, and flip parent as black.
                        redSibling.Left = parent;
                        parent.Parent = redSibling;
                        parent.Color = Color.BLACK;

                        // Attach black sibling as right child.
                        redSibling.Right = blackSibling;
                        blackSibling.Parent = redSibling;
                    }
                }

                // [Case 4] black leaf with 3-key left sibling:


                // TODO: Test each case respectively before implementing more cases.
                // and merge the sub-cases if probably.
            }
        }

        // Adjust recursively to balance.
        private void _AdjustTree(Node newRed)
        {
            // Validate color.
            if (newRed.Color != Color.RED)
            {
                if (newRed != Root) Console.WriteLine($"Adjust non-red node with key {newRed.Key}");

                return;
            }

            Node parent = newRed.Parent;
            bool nodeIsLeft = parent.Left == newRed;

            // [Case 1] Red parent, left child: Spin clockwise & recurse.
            if (parent.Color == Color.RED && nodeIsLeft)
            {
                SpinClockwiseForConsecutiveReds(newRed);

                _AdjustTree(parent);
            }

            // [Case 2] Red parent, right child: spin reds counter-clockwise, then spin clockwise recursively.
            else if (parent.Color == Color.RED && !nodeIsLeft)
            {
                SpinCounterClockwiseAndSwapColorWithParent(newRed);
                SpinClockwiseForConsecutiveReds(newRed.Left);

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
        /// For two consecutive red nodes:
        /// 1. Flip leftmost-red as black;
        /// 2. Raise its red parent;
        /// 3. Attach sibling to grandparent.
        /// </summary>
        public void SpinClockwiseForConsecutiveReds(Node leftmostRed)
        {
            // Flip color.
            leftmostRed.Color = Color.BLACK;

            // Its parent is also red.
            Node redParent = leftmostRed.Parent;

            // Red node must have a black parent.
            Node blackGrandparent = redParent.Parent;

            // Nullable.
            Node greatGrandParent = blackGrandparent.Parent;
            
            // Nullable.
            Node sibling = redParent.Right;

            redParent.Right = blackGrandparent;
            blackGrandparent.Parent = redParent;

            blackGrandparent.Left = sibling;
            if (sibling != null) sibling.Parent = blackGrandparent;

            if (blackGrandparent == Root)
            {
                Root = redParent;
                redParent.Color = Color.BLACK;
            }

            // greatGrandParent != null && blackGrandparent is left child.
            else if (greatGrandParent.Left == blackGrandparent)
            {
                greatGrandParent.Left = redParent;
                redParent.Parent = greatGrandParent;
            }

            // greatGrandParent != null && blackGrandparent is right child.
            else
            {
                greatGrandParent.Right = redParent;
                redParent.Parent = greatGrandParent;
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

        public void PrintAction(string actionName, Node newNode)
        {
            Console.WriteLine($"\n==================== {actionName}({newNode.Key}) ====================");
            _PrintTree("", Root, ArrowRoot);
        }

        private const string Tab = "      ";
        private const string ArrowLeft = "  \\=> ";
        private const string ArrowRoot = "Tree> ";
        private const string ArrowRight = "  /=> ";

        /// <param name="leftRootRight">0: left child, 1: root, 2: right child.</param>
        private static void _PrintTree(string indent, Node node, string arrow)
        {
            if (node != null)
            {
                _PrintTree(indent + Tab, node.Right, ArrowRight);
                
                Console.Write(indent);
                Console.Write(arrow);
                Console.BackgroundColor = node.Color == Color.RED ? ConsoleColor.Red : ConsoleColor.Black;
                Console.WriteLine(node.Key.ToString("D4"));
                Console.ResetColor();

                _PrintTree(indent + Tab, node.Left, ArrowLeft);
            }
        }
    }
}
