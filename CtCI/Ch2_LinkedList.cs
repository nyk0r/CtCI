using System;
using System.Collections.Generic;
using System.Linq;
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

        private static Node<T> GetLastNode<T>(Node<T> list) {
            while (list != null && list.Next != null) {
                list = list.Next;
            }
            return list;
        }

        #region Remove Dups
        public static void RemoveDupsExternalBuffer<T>(Node<T> list) {
            var seen = new HashSet<T>();
            var target = new Node<T>();
            for (; list != null; list = list.Next) {
                if (!seen.Contains(list.Value)) {
                    target.Next = list;
                    target = target.Next;
                    seen.Add(list.Value);
                }
            }
            target.Next = null;
        }

        public static void RemoveDupsNoExternalBuffer<T>(Node<T> list) {
            var target = new Node<T>();
            for (var node = list; node != null; node = node.Next) {
                var was = false;
                for (var passedNode = list; passedNode != node; passedNode = passedNode.Next) {
                    if (node.Value.Equals(passedNode.Value)) {
                        was = true;
                        break;
                    }
                }
                if (!was) {
                    target.Next = node;
                    target = target.Next;
                }
            }
            target.Next = null;
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

            if (list == null) {
                throw new IndexOutOfRangeException("k");
            }

            Node<T> p1 = list, p2 = list;
            for (var i = 0; i < k; i++) {
                p1 = p1.Next;
                if (p1 == null) {
                    throw new IndexOutOfRangeException("k");
                }
            }

            while (p1.Next != null) {
                p1 = p1.Next;
                p2 = p2.Next;
            }

            return p2;
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
                    (byte) ((a != null ? a.Value : 0) +
                            (b != null ? b.Value : 0) +
                            reminder);

                reminder = 0;
                if (sum > 9) {
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
                Tuple.Create(Tuple.Create(new byte[] { 9 }, new byte[] { 9 }), new byte[] { 8, 1 }),
                Tuple.Create(Tuple.Create(new byte[] { 9, 9 }, new byte[] { 9 }), new byte[] { 8, 0, 1 }),
                Tuple.Create(Tuple.Create(new byte[] { 9 }, new byte[] { 9, 9 }), new byte[] { 8, 0, 1 }),
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
            Stack<byte> revA = new Stack<byte>(), revB = new Stack<byte>(), revC = new Stack<byte>();
            while (a != null || b != null) {
                if (a != null) {
                    revA.Push(a.Value);
                    a = a.Next;
                }
                if (b != null) {
                    revB.Push(b.Value);
                    b = b.Next;
                }
            }

            byte reminder = 0;
            while (revA.Any() || revB.Any()) {
                var sum =
                   (byte)((revA.Count != 0 ? revA.Peek() : 0) +
                           (revB.Count != 0 ? revB.Peek() : 0) +
                           reminder);
                reminder = 0;
                if (sum > 9) {
                    reminder = (byte)(sum / 10);
                    sum %= 10;
                }

                revC.Push(sum);

                if (revA.Any()) {
                    revA.Pop();
                }
                if (revB.Any()) {
                    revB.Pop();
                }
            }
            if (reminder != 0) {
                revC.Push(reminder);
            }

            Node<byte> c = new Node<byte>(), result = c;
            while (revC.Any()) {
                c.Next = new Node<byte>(revC.Pop());
                c = c.Next;
            }

            return result.Next;
        }

        [Test]
        public void TestSumListsBigEndian() {
            var fixures = new[] {
                Tuple.Create(Tuple.Create((byte[]) null, (byte[]) null), (byte[]) null),
                Tuple.Create(Tuple.Create(new byte[] { 6, 1, 7 }, (byte[]) null), new byte[] { 6, 1, 7 }),
                Tuple.Create(Tuple.Create(new byte[] { }, new byte[] { 2, 9, 5 }), new byte[] { 2, 9, 5 }),
                Tuple.Create(Tuple.Create(new byte[] { 9 }, new byte[] { 9 }), new byte[] { 1, 8 }),
                Tuple.Create(Tuple.Create(new byte[] { 9, 9 }, new byte[] { 9 }), new byte[] { 1, 0, 8 }),
                Tuple.Create(Tuple.Create(new byte[] { 9 }, new byte[] { 9, 9 }), new byte[] { 1, 0, 8 }),
                Tuple.Create(Tuple.Create(new byte[] { 6, 1, 7 }, new byte[] { 2, 9, 5 }), new byte[] { 9, 1, 2 })
            };

            foreach (var fixure in fixures) {
                Node<byte>
                    a = CreateListFromArray(fixure.Item1.Item1),
                    b = CreateListFromArray(fixure.Item1.Item2);
                Assert.True(IsListEquivalentToArray(SumListsBigEndian(a, b), fixure.Item2));
            }
        }
        #endregion

        #region Palidrome
        public static bool CheckPalidrome<T>(Node<T> list) {
            var firstPart = new Stack<T>();
            Node<T> p1 = new Node<T> { Next = list }, p2 = new Node<T> { Next = list };
            while (p2 != null && p2.Next != null) {
                p1 = p1.Next;
                p2 = p2.Next.Next;
                firstPart.Push(p1.Value);
            }

            if (p2 == null) {
                firstPart.Pop();
            }

            p1 = p1.Next;
            while (p1 != null) {
                if (!firstPart.Any()) {
                    return false;
                }
                if (!firstPart.Pop().Equals(p1.Value)) {
                    return false;
                }
                p1 = p1.Next;
            }

            return !firstPart.Any();
        }

        [Test]
        public static void TestCheckPalindrome() {
            var fixtures = new[] {
                Tuple.Create((char[]) null, true),
                Tuple.Create(new char[] { }, true),
                Tuple.Create(new [] { 'a' }, true),
                Tuple.Create(new [] { 'a', 'a' }, true),
                Tuple.Create(new [] { 'a', 'b' }, false),
                Tuple.Create(new [] { 'c', 'a', 't' }, false),
                Tuple.Create(new [] { 'b', 'o', 'b' }, true),
                Tuple.Create(new [] { 'a', 'b', 'b', 'a' }, true),
                Tuple.Create(new [] { '0', 'a', 'b', 'b', 'a', '0' }, true),
                Tuple.Create(new [] { 'p', 'u', 't', 'i', 't', 'u', 'p' }, true),
                Tuple.Create(new [] { 'p', 'u', 't', 'i', 't', 'u', 'z' }, false),
                Tuple.Create(new [] { '0', 'p', 'u', 't', 'i', 't', 'u', 'p', '0' }, true),
                Tuple.Create(new [] { '0', 'p', 'u', 't', 'i', 't', 'u', 'p', '1' }, false)
            };

            foreach (var fixture in fixtures) {
                Assert.AreEqual(fixture.Item2, CheckPalidrome(CreateListFromArray(fixture.Item1)));
            }
        }
        #endregion

        #region Intersection
        public static Node<T> GetIntersectionNode<T>(Node<T> list1, Node<T> list2) {
            Node<T> last1 = list1, last2 = list2;
            int list1Len = 0, list2Len = 0;
            while (last1 != null && last1.Next != null) {
                last1 = last1.Next;
                ++list1Len;
            }
            while (last2 != null && last2.Next != null) {
                last2 = last2.Next;
                ++list2Len;
            }

            if (last1 == last2) {
                var delta = Math.Max(list1Len, list2Len) - Math.Min(list1Len, list2Len);
                Node<T> smaller = list1, bigger = list2;
                if (list2Len < list1Len) {
                    smaller = list2;
                    bigger = list1;
                }
                while (delta > 0) {
                    bigger = bigger.Next;
                    --delta;
                }

                while (bigger != null && smaller != null) {
                    if (bigger == smaller) {
                        return bigger;
                    }
                    bigger = bigger.Next;
                    smaller = smaller.Next;
                }
            }

            return null;
        }

        [Test]
        public void TestGetIntersectionNode() {
            var nodeStart = CreateListFromArray(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var nodeEnd = nodeStart;
            while (nodeEnd.Next != null) {
                nodeEnd = nodeEnd.Next;
            }
            var nodeMiddle = nodeStart;
            while (nodeMiddle != null) {
                if (nodeMiddle.Value == 5) {
                    break;
                }
                nodeMiddle = nodeMiddle.Next;
            }

            var listStart = CreateListFromArray(new[] { 100, 101, 102 });
            GetLastNode(listStart).Next = nodeStart;
            var listEnd = CreateListFromArray(new[] { 100, 101, 102 });
            GetLastNode(listEnd).Next = nodeEnd;
            var listMiddle = CreateListFromArray(new[] { 100, 101, 102 });
            GetLastNode(listMiddle).Next = nodeMiddle;

            var fixtures = new[] {
                Tuple.Create(Tuple.Create((Node<int>) null, (Node<int>) null), (Node<int>) null),
                Tuple.Create(Tuple.Create((Node<int>) null, nodeStart), (Node<int>) null),
                Tuple.Create(Tuple.Create(nodeStart, (Node<int>) null), (Node<int>) null),

                Tuple.Create(Tuple.Create(CreateListFromArray(new[] { 1, 2, 3 }), nodeStart), (Node<int>) null),
                Tuple.Create(Tuple.Create(nodeStart, CreateListFromArray(new[] { 1, 2, 3 })), (Node<int>) null),

                Tuple.Create(Tuple.Create(listStart, nodeStart), nodeStart),
                Tuple.Create(Tuple.Create(listEnd, nodeStart), nodeEnd),
                Tuple.Create(Tuple.Create(listMiddle, nodeStart), nodeMiddle),

                Tuple.Create(Tuple.Create(nodeStart, listStart), nodeStart),
                Tuple.Create(Tuple.Create(nodeStart, listEnd), nodeEnd),
                Tuple.Create(Tuple.Create(nodeStart, listMiddle), nodeMiddle)
            };

            foreach (var fixture in fixtures) {
                Assert.AreEqual(fixture.Item2, GetIntersectionNode(fixture.Item1.Item1, fixture.Item1.Item2));
            }
        }
        #endregion

        #region Loop Detection
        public static Node<T> GetLoopingNode<T>(Node<T> list) {
            if (list == null) {
                return null;
            }

            Node<T> tortoise = list, hare = list;
            while (hare != null && hare.Next != null) {
                tortoise = tortoise.Next;
                hare = hare.Next.Next;
                if (tortoise == hare) {
                    break;
                }
            }

            if (tortoise != hare) {
                return null;
            }

            tortoise = list;
            while (tortoise != hare) {
                tortoise = tortoise.Next;
                hare = hare.Next;
            }

            return tortoise;
        }

        [Test]
        public void TestGetLoopNode() {
            var list = CreateListFromArray(new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 });
            var loopingNode = new Node<int>(101) { Next = list };
            GetLastNode(list).Next = loopingNode;

            var list2 = CreateListFromArray(new[] { -3, -2, -1 });
            GetLastNode(list2).Next = loopingNode;

            var list3 = CreateListFromArray(new[] { 1, 2, 3, 4, 5 });
            var last3 = GetLastNode(list3);
            last3.Next = last3;

            var fixtures = new[] {
                Tuple.Create((Node<int>) null, (Node<int>) null),
                Tuple.Create(CreateListFromArray(new[] { 0, 1, 2 }), (Node<int>) null),
                Tuple.Create(loopingNode, loopingNode),
                Tuple.Create(list2, loopingNode),
                Tuple.Create(list3, last3)
            };

            foreach (var fixture in fixtures) {
                Assert.AreEqual(fixture.Item2, GetLoopingNode(fixture.Item1));
            }
        }
        #endregion
    }
}
