using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts.Helpers
{
    [Serializable]
    public class BinaryTree<T> where T : IComparable<T>
    {
        private BinaryTreeNode<T> root;
        public int Count;

        public void Insert(T value)
        {
            BinaryTreeNode<T> node = new BinaryTreeNode<T>(value);

            Insert(node);
        }

        public void Insert(BinaryTreeNode<T> node)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node", "node cannot be null");
            }

            if (root == null)
            {
                root = node;
            }
            else
            {
                Insert(node, ref root);
            }

            Count++;
        }

        public void Delete(T value, bool rebalanceTree)
        {
            BinaryTreeNode<T> parentNode;
            BinaryTreeNode<T> foundNode = null;
            BinaryTreeNode<T> tree = parentNode = root;

            //Search for the node while keeping a reference to the parent
            while (tree != null)
            {
                if (value.CompareTo(tree.Data) == 0)
                {
                    foundNode = tree;
                    break;
                }
                else if (value.CompareTo(tree.Data) < 0)
                {
                    parentNode = tree;
                    tree = tree.Left;
                }
                else if (value.CompareTo(tree.Data) > 0)
                {
                    parentNode = tree;
                    tree = tree.Right;
                }
            }

            if (foundNode == null)
            {
                throw new Exception("Node not found in binary tree");
            }


            bool leftOrRightNode = false;

            if (foundNode != parentNode.Left)
            {
                leftOrRightNode = true;
            }

            if (foundNode == parentNode) //oh oh. We're trying to delete the root here.
            {
                if (rebalanceTree)
                {
                    //Let's just remove the parent and rebalance the tree to ensure our tree will be nice and balanced
                    //after the removal
                    IList<BinaryTreeNode<T>> listOfNodes = new List<BinaryTreeNode<T>>();

                    FillListInOrder(root, listOfNodes);
                    RemoveChildren(listOfNodes);
                    listOfNodes.Remove(parentNode);

                    root = null;
                    int count = Count - 1;
                    Count = 0;

                    BalanceTree(0, count - 1, listOfNodes);
                }
                else
                {
                    BinaryTreeNode<T> leftMost = FindLeftMost(parentNode.Right, true);

                    if (leftMost != null)
                    {
                        leftMost.Left = parentNode.Left;
                        leftMost.Right = parentNode.Right;
                        root = leftMost;
                    }
                }
            }
            else if (foundNode.Left == null && foundNode.Right == null) //This is a leaf node
            {
                //Just set it to null
                if (leftOrRightNode)
                {
                    parentNode.Right = null;
                }
                else
                {
                    parentNode.Left = null;
                }
            }
            else if (foundNode.Left != null && foundNode.Right != null) //This is a node with two children
            {
                if (leftOrRightNode)
                {
                    parentNode.Right = foundNode.Right;
                    parentNode.Right.Left = foundNode.Left;
                }
                else
                {
                    parentNode.Left = foundNode.Right;
                    parentNode.Left.Left = foundNode.Left;
                }
            }

            else if (foundNode.Left != null || foundNode.Right != null) //This is a node with a single child
            {
                if (foundNode.Left != null)
                {
                    if (leftOrRightNode)
                    {
                        parentNode.Right = foundNode.Left;
                    }
                    else
                    {
                        parentNode.Left = foundNode.Left;
                    }
                }
                else
                {
                    if (leftOrRightNode)
                    {
                        parentNode.Right = foundNode.Right;
                    }
                    else
                    {
                        parentNode.Left = foundNode.Right;
                    }
                }
            }
        }

        private BinaryTreeNode<T> FindLeftMost(BinaryTreeNode<T> node, bool setParentToNull)
        {
            BinaryTreeNode<T> leftMost = null;
            BinaryTreeNode<T> current = leftMost = node;
            BinaryTreeNode<T> parent = null;

            while (current != null)
            {
                if (current.Left != null)
                {
                    parent = current;
                    leftMost = current.Left;
                }

                current = current.Left;
            }

            if (parent != null && setParentToNull)
            {
                parent.Left = null;
            }

            return leftMost;
        }

        public BinaryTreeNode<T> Search(T value)
        {
            BinaryTreeNode<T> tree = root;

            while (tree != null)
            {
                if (value.CompareTo(tree.Data) == 0)
                {
                    return tree;
                }
                else if (value.CompareTo(tree.Data) < 0)
                {
                    tree = tree.Left;
                }
                else if (value.CompareTo(tree.Data) > 0)
                {
                    tree = tree.Right;
                }
            }

            return null;
        }

        public IEnumerable<BinaryTreeNode<T>> InOrder()
        {
            return InOrder(root);
        }

        private IEnumerable<BinaryTreeNode<T>> InOrder(BinaryTreeNode<T> node)
        {
            if (node != null)
            {
                foreach (BinaryTreeNode<T> left in InOrder(node.Left))
                {
                    yield return left;
                }

                yield return node;

                foreach (BinaryTreeNode<T> right in InOrder(node.Right))
                {
                    yield return right;
                }
            }
        }

        public IEnumerable<BinaryTreeNode<T>> PreOrder()
        {
            return PreOrder(root);
        }

        public IEnumerable<BinaryTreeNode<T>> PostOrder()
        {
            return PostOrder(root);
        }

        public IEnumerable<BinaryTreeNode<T>> BreadthFirstTraversal()
        {
            Queue<BinaryTreeNode<T>> queue = new Queue<BinaryTreeNode<T>>();

            queue.Enqueue(root);

            while (queue.Count != 0)
            {
                BinaryTreeNode<T> current = queue.Dequeue();

                if (current != null)
                {
                    queue.Enqueue(current.Left);
                    queue.Enqueue(current.Right);

                    yield return current;
                }
            }
        }

        public IEnumerable<BinaryTreeNode<T>> DepthFirstTraversal()
        {
            Stack<BinaryTreeNode<T>> queue = new Stack<BinaryTreeNode<T>>();

            BinaryTreeNode<T> current;

            queue.Push(root);

            while (queue.Count != 0)
            {
                current = queue.Pop();

                if (current != null)
                {
                    queue.Push(current.Right);
                    queue.Push(current.Left);

                    yield return current;
                }
            }
        }

        public void BalanceTree()
        {
            IList<BinaryTreeNode<T>> listOfNodes = new List<BinaryTreeNode<T>>();

            FillListInOrder(root, listOfNodes);
            RemoveChildren(listOfNodes);

            root = null;
            int count = Count;
            Count = 0;

            BalanceTree(0, count - 1, listOfNodes);
        }

        private void Insert(BinaryTreeNode<T> node, ref BinaryTreeNode<T> parent)
        {
            if (parent == null)
            {
                parent = node;
            }
            else
            {
                if (node.Data.CompareTo(parent.Data) < 0)
                {
                    Insert(node, ref parent.Left);
                }
                else if (node.Data.CompareTo(parent.Data) > 0)
                {
                    Insert(node, ref parent.Right);
                }
                else if (node.Data.CompareTo(parent.Data) == 0)
                {
                    throw new ArgumentException("Duplicate node");
                }
            }
        }

        private void BalanceTree(int min, int max, IList<BinaryTreeNode<T>> list)
        {
            if (min <= max)
            {
                int middleNode = (int) Math.Ceiling(((double) min + max)/2);

                Insert(list[middleNode]);

                BalanceTree(min, middleNode - 1, list);

                BalanceTree(middleNode + 1, max, list);
            }
        }

        private void FillListInOrder(BinaryTreeNode<T> node, ICollection<BinaryTreeNode<T>> list)
        {
            if (node != null)
            {
                FillListInOrder(node.Left, list);

                list.Add(node);

                FillListInOrder(node.Right, list);
            }
        }

        private void RemoveChildren(IEnumerable<BinaryTreeNode<T>> list)
        {
            foreach (BinaryTreeNode<T> node in list)
            {
                node.Left = null;
                node.Right = null;
            }
        }

        private IEnumerable<BinaryTreeNode<T>> PreOrder(BinaryTreeNode<T> node)
        {
            if (node != null)
            {
                yield return node;

                foreach (BinaryTreeNode<T> left in PreOrder(node.Left))
                {
                    yield return left;
                }

                foreach (BinaryTreeNode<T> right in PreOrder(node.Right))
                {
                    yield return right;
                }
            }
        }

        private IEnumerable<BinaryTreeNode<T>> PostOrder(BinaryTreeNode<T> node)
        {
            if (node != null)
            {
                foreach (BinaryTreeNode<T> left in PostOrder(node.Left))
                {
                    yield return left;
                }

                foreach (BinaryTreeNode<T> right in PostOrder(node.Right))
                {
                    yield return right;
                }

                yield return node;
            }
        }
    }

    [Serializable]
    public class BinaryTreeNode<T> where T : IComparable<T>
    {
        public BinaryTreeNode(T value)
        {
            Data = value;
        }

        public T Data { get; set; }

        public BinaryTreeNode<T> Left;

        public BinaryTreeNode<T> Right;
    }
}