using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace CtCI {
    internal class Ch2_LinkedList
    {
        public class Node<T> {
            public T Value { get; set; }
            public Node<T> Next { get; set; }

            public Node(T value) {
                Value = value;
            }

            public Node() : this(default(T)) { }
        }

        private static bool IsListEquivalentToArray<T>(Node<T> list, T[] array) {
            var node = list;
            if (node == null) {
                return array == null || array.Length == 0;
            }
            var nodesCounter = 0;
            while (node != null) {
                if (nodesCounter >= array.Length) {
                    return false;
                }
                if (!Equals(node.Value, array[nodesCounter])) {
                    return false;
                }
                ++nodesCounter;
                node = node.Next;
            }
            return nodesCounter == array.Length;
        }

        private static Node<T> CreateListFromArray<T>(T[] array) {
            if (array == null || array.Length == 0) {
                return null;
            }

            var list = new Node<T>(array[0]);
            var node = list;
            for (var idx = 1; idx < array.Length; idx++) {
                node.Next = new Node<T>(array[idx]);
                node = node.Next;
            }

            return list;
        }

        #region Remove Dups
        public static void RemoveDupsExternalBuffer<T>(Node<T> list) {
            var seen = new HashSet<T>();
            for (; list != null; list = list.Next) {
                seen.Add(list.Value);
                while (list.Next != null && seen.Contains(list.Next.Value)) {
                    list.Next = list.Next.Next;
                }
            }
        }

        public static void RemoveDupsNoExternalBuffer<T>(Node<T> list) {
            for (; list != null; list = list.Next) {
                var mark = list;
                while (mark != null) {
                    var nextNode = mark.Next;
                    while (nextNode != null && list.Value.Equals(nextNode.Value)) {
                        nextNode = nextNode.Next;
                    }
                    mark.Next = nextNode;
                    mark = mark.Next;
                }
            }
        }

        [Test]
        public void TestRemoveDups() {
            var fixures = new[] {
                Tuple.Create(new int[0], new int[0]),
                Tuple.Create(new[] { 0 }, new[] { 0 }),
                Tuple.Create(new[] { 1, 2, 3 }, new[] { 1, 2, 3 }),

                Tuple.Create(new[] { 1, 1, 2, 2, 3, 3, 1 }, new[] { 1, 2, 3 }),
                Tuple.Create(new[] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3 }, new[] { 1, 2, 3 }),
                Tuple.Create(new[] { 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 1, 2, 3, 1, 1, 2, 2, 2 }, new[] { 1, 2, 3 }),
                Tuple.Create(new[] { 1, 2, 3, 4, 5, 2, 1, 4, 4, 3, 1, 6, 1 }, new[] { 1, 2, 3, 4, 5, 6 }),
                Tuple.Create(new[] { 1, 2, 3, 4, 5, 2, 1, 4, 4, 3, 1, 6, 1, 7 }, new[] { 1, 2, 3, 4, 5, 6, 7 })
            };

            Assert.DoesNotThrow(() => { RemoveDupsExternalBuffer<int>(null); });
            Assert.DoesNotThrow(() => { RemoveDupsNoExternalBuffer<int>(null); });

            foreach (var fixure in fixures) {
                var list = CreateListFromArray(fixure.Item1);
                RemoveDupsExternalBuffer(list);
                Assert.True(IsListEquivalentToArray(list, fixure.Item2));

                list = CreateListFromArray(fixure.Item1);
                RemoveDupsNoExternalBuffer(list);
                Assert.True(IsListEquivalentToArray(list, fixure.Item2));
            }
        }
        #endregion

        #region Return Kth to Last
        public Node<T> ReturnKthToLast<T>(Node<T> list, int k) {
            if (k < 0) {
                throw new ArgumentOutOfRangeException("k");
            }

            for (; list != null; list = list.Next) {
                var steps = 0;
                var forward = list.Next;
                while (steps < k && forward != null) {
                    steps++;
                    forward = forward.Next;
                }
                if (forward == null) {
                    if (steps == k) {
                        return list;
                    }
                    throw new IndexOutOfRangeException("k");
                }
            }

            throw new IndexOutOfRangeException("k");
        }

        [Test]
        public void TestReturnKthToLast() {
            Assert.Throws<ArgumentOutOfRangeException>(() => { ReturnKthToLast(CreateListFromArray(new int[] { 1 }), -1); }, "k");
            Assert.Throws<ArgumentOutOfRangeException>(() => { ReturnKthToLast(CreateListFromArray(new int[] { }), -1); }, "k");

            Assert.Throws<IndexOutOfRangeException>(() => { ReturnKthToLast<int>(null, 1); }, "k");
            Assert.Throws<IndexOutOfRangeException>(() => { ReturnKthToLast(CreateListFromArray(new int[] { }), 1); }, "k");
            Assert.Throws<IndexOutOfRangeException>(() => { ReturnKthToLast(CreateListFromArray(new int[] { 1, 2 }), 3); }, "k");

            var arr = new[] { 1, 2, 3, 4, 5, 6, 7 };
            var list = CreateListFromArray(arr);
            for (var k = 0; k < arr.Length; k++) {
                Assert.AreEqual(arr[arr.Length - 1 - k], ReturnKthToLast(list, k).Value);
            }
        }
        #endregion

        #region Delete Middle Node
        public static void DeleteMiddleNode<T>(Node<T> node) {
            if (node == null) {
                throw new ArgumentNullException("node");
            }
            if (node.Next == null) {
                throw new InvalidOperationException("Cannot delete the last node");
            }

            node.Value = node.Next.Value;
            node.Next = node.Next.Next;
        }

        [Test]
        public static void TestDeleteMiddleNode() {
            var fixture = CreateListFromArray(new [] { 'a', 'b', 'c', 'd', 'e' });
            Assert.Throws<ArgumentNullException>(() => { DeleteMiddleNode<char>(null); });
            var last = fixture;
            while (last.Next != null) {
                last = last.Next;
            }
            Assert.Throws<InvalidOperationException>(() => { DeleteMiddleNode(last); });
            
            DeleteMiddleNode(fixture);
            Assert.True(IsListEquivalentToArray(fixture, new[] { 'b', 'c', 'd', 'e' }));

            DeleteMiddleNode(fixture.Next);
            Assert.True(IsListEquivalentToArray(fixture, new[] { 'b', 'd', 'e' }));
        }
        #endregion

        #region Partition

        private delegate Node<T> PartitioningMethod<T>(Node<T> list, T pivot);

        public static Node<T> PartitionReorderNode<T>(Node<T> list, T pivot) where T : IComparable<T> {
            /*if (list == null) {
                throw new ArgumentNullException("list");
            }*/
            
            Node<T> lList = new Node<T>(),
                geList = new Node<T>(),
                cLList = lList,
                cGeList = geList;

            for (; list != null; list = list.Next) {
                if (list.Value.CompareTo(pivot) < 0) {
                    cLList.Next = list;
                    cLList = cLList.Next;
                } else {
                    cGeList.Next = list;
                    cGeList = cGeList.Next;
                }
            }

            cLList.Next = geList.Next;
            cGeList.Next = null;
            return lList.Next;
        }

        public static Node<T> PatitionReorderValues<T>(Node<T> list, T pivot) where T : IComparable<T> {
            // Lomuto method
            var less = list;
            var start = list;
            for (; list != null; list = list.Next) {
                if (list.Value.CompareTo(pivot) < 0) {
                    var tmp = less.Value;
                    less.Value = list.Value;
                    list.Value = tmp;
                    less = less.Next;
                }
            }

            return start;
        }

        [Test]
        public void TestPartition() {
            var fixtures = new [] {
                Tuple.Create(new int[] { }, 1),
                Tuple.Create((int[])null, 1),

                Tuple.Create(new[] { 5, 5, 5, 5, 5, 5, 5 }, 5),
                Tuple.Create(new[] { 3, 5, 8, 5, 10, 2, 1 }, 5),
                Tuple.Create(new[] { 7, 5, 8, 5, 10, 6, 11 }, 5),
                Tuple.Create(new[] { 5, 5, 10, 15, 7, 3, 5, 5, 2, 1, 9, 9, 0, 3, 5 }, 5),
            };

            var methods = new PartitioningMethod<int>[] { PartitionReorderNode, PatitionReorderValues };

            foreach (var method in methods) {
                foreach (var fixture in fixtures) {
                    var pivot = fixture.Item2;
                    var result = method(CreateListFromArray(fixture.Item1), pivot);

                    var greaterFound = false;
                    for (var node = result; node != null; node = node.Next) {
                        if (node.Value.CompareTo(pivot) < 0) {
                            if (greaterFound) {
                                Assert.Fail();
                            }
                        } else {
                            greaterFound = true;
                        }
                    }
                }
            }
        }
        #endregion

        #region Sum Lists
        public static Node<byte> SumListsLittleEndian(Node<byte> a, Node<byte> b) {
            var c = new Node<byte>();
            var result = c;
            byte reminder = 0;
            while (a != null || b != null) {
                var sum =
                    (byte)((a != null ? a.Value : 0) +
                    (b != null ? b.Value : 0) +
                    reminder);

                reminder = 0;
                if (sum >= 10) {
                    reminder = (byte) (sum/10);
                    sum %= 10;
                }

                c.Next = new Node<byte>(sum);
                c = c.Next;

                if (a != null) {
                    a = a.Next;
                }
                if (b != null) {
                    b = b.Next;
                }
            }

            if (reminder != 0) {
                c.Next = new Node<byte>(reminder);
            }

            return result.Next;
        }

        [Test]
        public void TestSumListsLittleEndian() {
            var fixures = new[] {
                Tuple.Create(Tuple.Create((byte[]) null, (byte[]) null), (byte[]) null),
                Tuple.Create(Tuple.Create(new byte[] { 7, 1, 6 }, (byte[]) null), new byte[] { 7, 1, 6 }),
                Tuple.Create(Tuple.Create(new byte[] { }, new byte[] { 5, 9, 2 }), new byte[] { 5, 9, 2 }),
                Tuple.Create(Tuple.Create(new byte[] { 7, 1, 6 }, new byte[] { 5, 9, 2 }), new byte[] { 2, 1, 9 })
            };

            foreach (var fixure in fixures) {
                Node<byte>
                    a = CreateListFromArray(fixure.Item1.Item1),
                    b = CreateListFromArray(fixure.Item1.Item2);
                Assert.True(IsListEquivalentToArray(SumListsLittleEndian(a, b), fixure.Item2));
            }
        }

        public static Node<byte> SumListsBigEndian(Node<byte> a, Node<byte> b) {
            throw new NotImplementedException();
        }

        [Test]
        public void TestSumListsBigEndian() { }
        #endregion

        #region Palidrome
        public static bool CheckPalidrome<T>(Node<T> list) {
            throw new NotImplementedException();
        }

        [Test]
        public static void TestCheckPalindrome() {
            
        }
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
