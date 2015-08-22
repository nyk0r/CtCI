using System;
using NUnit.Framework;

namespace CtCI {
    internal class Ch2_LinkedList
    {
        public class Node<T> {
            public T Value { get; set; }
            public Node<T> Next { get; set; }

            public Node(T value) {
                Value = value;
            }
        }

        #region Remove Dups
        public static void RemoveDups<T>(Node<T> list) {}

        [Test]
        public void TestRemoveDups() { }
        #endregion

        #region Return Kth to Last
        public Node<T> ReturnKthToLast<T>(Node<T> list) {
            throw new NotImplementedException();
        }

        [Test]
        public void TestReturnKthToLast() {
            
        }
        #endregion

        #region Delete Middle Node
        public static void DeleteMiddleNode<T>(Node<T> list) {}

        [Test]
        public static void TestDeleteMiddleNode() { }
        #endregion

        #region Partition
        public static void Partition<T>(Node<T> list, T pivot) where T : IComparable { }

        [Test]
        public void TestPartition() { }
        #endregion

        #region Sum Lists
        public static Node<byte> SumLists(Node<byte> list1, Node<byte> list2) {
            throw new NotImplementedException();
        }

        public void TestSumLists() { }
        #endregion

        #region Palidrome
        public static bool CheckPalidrome<T>(Node<T> list) {
            throw new NotImplementedException();
        }

        [Test]
        public static void TestCheckPalindrome() { }
        #endregion

        #region Intersection
        public static Node<T> GetIntersectionNode<T>(Node<T> list1, Node<T> list2) {
            throw new NotImplementedException();
        }

        [Test]
        public void TestGetIntersectionNode() { }
        #endregion

        #region Loop Detection
        public static Node<T> GetLoopNode<T>(Node<T> list) {
            throw new NotImplementedException();
        }

        [Test]
        public void TestGetLoopNode() { }
        #endregion
    }
}
