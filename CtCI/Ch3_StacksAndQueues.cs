using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CtCI
{
    public class Ch3_StacksAndQueues {
        public interface IStack<T> {
            T Pop();
            void Push(T val);
            T Peek { get; }
            bool IsEmpty { get; }
            int Count { get; }
        }

        public interface IQueue<T> {
            T Dequeue();
            void Enqueue(T val);
            T Peek { get; }
            bool IsEmpty { get; }
            int Count { get; }
        }

        public class LinkedListStack<T> : IStack<T> {
            private class StackNode {
                public T Data { get; private set; }
                public StackNode Prev { get; private set; }

                public StackNode(T data, StackNode prev) {
                    Data = data;
                    Prev = prev;
                }
            }

            private StackNode _head;

            public LinkedListStack() {
                Count = 0;
            }

            public T Pop() {
                CheckUnderflow();
                --Count;
                var data = _head.Data;
                _head = _head.Prev;
                return data;
            }

            public void Push(T val) {
                ++Count;
                _head = new StackNode(val, _head);
            }

            public T Peek {
                get {
                    CheckUnderflow();
                    return _head.Data;
                }
            }

            public bool IsEmpty {
                get { return _head == null; }
            }

            public int Count { get; private set; }

            private void CheckUnderflow() {
                if (IsEmpty) {
                    throw new Exception("Stack underflow");
                }
            }
        }

        public class LinkedListQueue<T>: IQueue<T> {
            private class QueueNode {
                public T Data { get; private set; }
                public QueueNode Next { get; set; }

                public QueueNode(T data, QueueNode next) {
                    Data = data;
                    Next = next;
                }
            }

            private QueueNode _head, _tail;

            public T Dequeue() {
                CheckUnderflow();
                --Count;

                var result = _head.Data;
                _head = _head.Next;
                if (_head == null) {
                    _tail = null;
                }
                return result;
            }

            public void Enqueue(T val) {
                ++Count;

                var next = new QueueNode(val, null);
                if (_tail == null) {
                    _head = _tail = next;
                } else {
                    _tail.Next = next;
                    _tail = _tail.Next;
                }
            }

            public T Peek {
                get {
                    CheckUnderflow();
                    return _head.Data;
                }
            }

            public bool IsEmpty {
                get { return _head == null; }
            }

            public int Count { get; private set; }

            private void CheckUnderflow() {
                if (IsEmpty) {
                    throw new Exception("Queue underflow");
                }
            }
        }

        #region Three in One
        public class NStacksArray<T> {
            private T[] _data;
            private readonly int _buckets;
            private int _bucketSize;
            private readonly int[] _pointers;

            public NStacksArray(int buckets = 3, int bucketSize = 10) {
                if (buckets <= 0) {
                    throw new ArgumentOutOfRangeException("buckets");
                }

                if (bucketSize <= 1) {
                    throw new ArgumentOutOfRangeException("bucketSize");
                }

                _bucketSize = bucketSize;
                _buckets = buckets;
                _pointers = new int[_buckets];
                for (var idx = 0; idx < _pointers.Length; idx++) {
                    _pointers[idx] = -1;
                }
                _data = new T[_buckets*_bucketSize];
            }

            public T Pop(int bucket) {
                CheckBucketUnderflow(bucket);
                return _data[bucket*_bucketSize + _pointers[bucket]--];
            }

            public void Push(T val, int bucket) {
                CheckBucket(bucket);
                if (_pointers[bucket] + 1 == _bucketSize) {
                    var bucketSize = _bucketSize*2;
                    var data = new T[_buckets*bucketSize];
                    for (var bucketIdx = 0; bucketIdx < _buckets; bucketIdx++) {
                        for (var itemIdx = 0; itemIdx <= _pointers[bucketIdx]; itemIdx++) {
                            data[bucketIdx*_bucketSize + itemIdx] = _data[bucketIdx*_bucketSize + itemIdx];
                        }
                    }

                    _bucketSize = bucketSize;
                    _data = data;
                }
                _data[bucket*_bucketSize + ++_pointers[bucket]] = val;
            }

            public T Peek(int bucket) {
                CheckBucketUnderflow(bucket);
                return _data[bucket*_bucketSize + _pointers[bucket]];
            }

            public bool IsEmpty(int bucket) {
                CheckBucket(bucket);
                return _pointers[bucket] < 0;
            }

            private void CheckBucket(int bucket) {
                if (bucket < 0 || bucket >= _buckets) {
                    throw new ArgumentOutOfRangeException("bucket");
                }
            }

            private void CheckBucketUnderflow(int bucket) {
                CheckBucket(bucket);
                if (IsEmpty(bucket)) {
                    throw new Exception(String.Format("Bucket #{0} underflow", bucket));
                }
            }
        }

        public class ThreeStacksInOne<T>: NStacksArray<T> {
            public ThreeStacksInOne() : base(3, 10) {}
        }

        [Test]
        public void TestThreeInOne() {
            var stacks = new ThreeStacksInOne<int>();

            Assert.Throws<Exception>(() => { var _ = stacks.Peek(0); }, "Stack underflow");
            Assert.Throws<Exception>(() => { var _ = stacks.Pop(1); }, "Stack underflow");
            Assert.Throws<Exception>(() => { var _ = stacks.Peek(2); }, "Stack underflow");

            for (var stack = 0; stack < 3; stack++) {
                for (var val = 1; val <= 100; val++) {
                    stacks.Push(val, stack);
                }
            }

            for (var val = 100; val >= 1; val--) {
                for (var stack = 2; stack >= 0; stack--) {
                    Assert.AreEqual(val, stacks.Peek(stack));
                    Assert.AreEqual(val, stacks.Pop(stack));
                }
            }

            Assert.Throws<Exception>(() => { var _ = stacks.Peek(0); }, "Stack underflow");
            Assert.Throws<Exception>(() => { var _ = stacks.Pop(1); }, "Stack underflow");
            Assert.Throws<Exception>(() => { var _ = stacks.Peek(2); }, "Stack underflow");
        }
        #endregion

        #region Stack Min
        public class MinMaxStack<T> : IStack<T> where T: IComparable<T> {
            private class MinMaxStackItem {
                public T Min { get; private set; }
                public T Max { get; private set; }
                public T Data { get; private set; }

                public MinMaxStackItem(T min, T max, T data) {
                    Min = min;
                    Max = max;
                    Data = data;
                }
            }

            private readonly LinkedListStack<MinMaxStackItem> _stack = new LinkedListStack<MinMaxStackItem>(); 

            public T Pop() {
                return _stack.Pop().Data;
            }

            public void Push(T val) {
                T min = val, max = val;
                if (!IsEmpty) {
                    min = min.CompareTo(Min) < 0 ? min : Min;
                    max = max.CompareTo(Max) > 0 ? max : Max;
                }
                _stack.Push(new MinMaxStackItem(min, max, val));
            }

            public T Peek {
                get { return _stack.Peek.Data; }
            }

            public int Count {
                get { return _stack.Count; }
            }

            public bool IsEmpty {
                get { return _stack.IsEmpty; }
            }

            public T Min {
                get { return _stack.Peek.Min; }
            }

            public T Max {
                get { return _stack.Peek.Max; }
            }
        }

        [Test]
        public void TestStackMin() {
            var stack = new MinMaxStack<int>();

            Assert.Throws<Exception>(() => { var _ = stack.Peek; }, "Stack underflow");
            Assert.Throws<Exception>(() => { var _ = stack.Pop(); }, "Stack underflow");

            var fixtures = new[] {
                Tuple.Create(100, 100),
                Tuple.Create(200, 100),
                Tuple.Create(300, 100),
                Tuple.Create(10, 10),
                Tuple.Create(10, 10),
                Tuple.Create(30, 10),
                Tuple.Create(-1, -1)
            };

            for (var idx = 0; idx < fixtures.Length; idx++) {
                int val = fixtures[idx].Item1, min = fixtures[idx].Item2;
                stack.Push(val);
                Assert.AreEqual(min, stack.Min);
            }

            for (var idx = fixtures.Length - 1; idx >= 0; idx--) {
                var min = fixtures[idx].Item2;
                Assert.AreEqual(min, stack.Min);
                stack.Pop();
            }

            stack.Push(-1);
            Assert.AreEqual(-1, stack.Min);
            stack.Push(-2);
            Assert.AreEqual(-2, stack.Min);
            stack.Pop();
            Assert.AreEqual(-1, stack.Min);
            stack.Push(-3);
            Assert.AreEqual(-3, stack.Min);
            stack.Pop();
            stack.Pop();

            Assert.Throws<Exception>(() => { var _ = stack.Peek; }, "Stack underflow");
            Assert.Throws<Exception>(() => { var _ = stack.Pop(); }, "Stack underflow");
        }
        #endregion

        #region Stack of Plates
        public class SetOfStacks<T> : IStack<T> {
            private readonly List<LinkedListStack<T>> _stacks;
            public int Threshold { get; private set; }

            public SetOfStacks(int threshold) {
                _stacks = new List<LinkedListStack<T>>();
                Threshold = threshold;
            }

            public T Pop() {
                return PopAt(_stacks.Count - 1);
            }

            public T PopAt(int idx) {
                if (idx >= _stacks.Count) {
                    throw new Exception("Invalid stack index.");
                }
                CheckUnderflow();

                --Count;
                var stack = _stacks[idx];
                if (stack.Count == 1) {
                    _stacks.RemoveAt(idx);
                }
                return stack.Pop();
            }

            public void Push(T val) {
                ++Count;
                LinkedListStack<T> stack;
                if (_stacks.Count == 0 || _stacks[_stacks.Count - 1].Count == Threshold) {
                    stack = new LinkedListStack<T>();
                    _stacks.Add(stack);
                } else {
                    stack = _stacks[_stacks.Count - 1];
                }

                stack.Push(val);
            }

            public T Peek {
                get {
                    CheckUnderflow();
                    return _stacks[0].Peek;
                }
            }

            public bool IsEmpty {
                get { return _stacks.Count == 0; }
            }

            public int Count { get; private set; }

            public int StacksCount {
                get { return _stacks.Count; }
            }

            private void CheckUnderflow() {
                if (IsEmpty) {
                    throw new Exception("Stack underflow");
                }
            }
        }

        [Test]
        public void TesSetOfStacks() {
            var set = new SetOfStacks<int>(3);
            for (var num = 0; num < 30; num++) {
                set.Push(num);
            }
            Assert.AreEqual(10, set.StacksCount);
            for (var num = 29; num >= 0; num--) {
                Assert.AreEqual(num, set.Pop());
            }

            for (var num = 0; num < 30; num++) {
                set.Push(num);
            }
            Assert.AreEqual(10, set.StacksCount);

            Assert.AreEqual(29, set.PopAt(9));
            Assert.AreEqual(28, set.PopAt(9));

            Assert.AreEqual(5, set.PopAt(1));
            Assert.AreEqual(4, set.PopAt(1));
            Assert.AreEqual(3, set.PopAt(1));

            Assert.AreEqual(8, set.PopAt(1));
        }
        #endregion

        #region Queue via Stacks
        public class TwoStacksQueue<T> : IQueue<T> {
            private readonly LinkedListStack<T> _inStack = new LinkedListStack<T>(), _ouStack = new LinkedListStack<T>(); 

            public T Dequeue() {
                PrepareOutStack();
                --Count;
                return _ouStack.Pop();
            }

            public void Enqueue(T val) {
                ++Count;
                _inStack.Push(val);
            }

            public T Peek {
                get {
                    PrepareOutStack();
                    return _ouStack.Peek;
                }
            }

            public bool IsEmpty {
                get { return _inStack.IsEmpty && _ouStack.IsEmpty; }
            }

            public int Count { get; private set; }

            private void CheckUnderflow() {
                if (IsEmpty) {
                    throw new Exception("Queue underflow");
                }
            }

            private void PrepareOutStack() {
                CheckUnderflow();
                if (_ouStack.IsEmpty) {
                    while (!_inStack.IsEmpty) {
                        _ouStack.Push(_inStack.Pop());
                    }
                }
            }
        }

        [Test]
        public void TestTwoStacksQueue() {
            var queue = new TwoStacksQueue<int>();
            Assert.Throws<Exception>(() => { var _ = queue.Peek; }, "Queue underflow");
            Assert.Throws<Exception>(() => { var _ = queue.Dequeue(); }, "Queue underflow");

            var fixtures = new[] { 1, 2, 3, 4, 5 };
            for (var idx = 0; idx < fixtures.Length; idx++) {
                queue.Enqueue(fixtures[idx]);
            }
            for (var idx = 0; idx < fixtures.Length; idx++) {
                Assert.AreEqual(fixtures[idx], queue.Peek);
                Assert.AreEqual(fixtures[idx], queue.Dequeue());
            }

            Assert.Throws<Exception>(() => { var _ = queue.Peek; }, "Queue underflow");
            Assert.Throws<Exception>(() => { var _ = queue.Dequeue(); }, "Queue underflow");

            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            Assert.AreEqual(1, queue.Dequeue());
            queue.Enqueue(4);
            queue.Enqueue(5);
            Assert.AreEqual(2, queue.Dequeue());
            Assert.AreEqual(3, queue.Dequeue());
            Assert.AreEqual(4, queue.Dequeue());
            Assert.AreEqual(5, queue.Dequeue());
        }
        #endregion

        #region Sort Stack
        public void SortStack<T>(Stack<T> sorted) where T: IComparable<T> {
            // a variation of insertion sort 
            // O(n^2)
            var buffer = new Stack<T>(sorted.Count);
            while (sorted.Any()) {
                buffer.Push(sorted.Pop());
            }
            while (buffer.Any()) {
                var item = buffer.Pop();
                while (sorted.Any() && sorted.Peek().CompareTo(item) < 0) {
                    buffer.Push(sorted.Pop());
                }
                sorted.Push(item);
            }
        }

        [Test]
        public void TestSortStack() {
            var fixture = new[] { 1, 10, 15, -1, 100, 300, -1, -2, 100, 150, 3, 3, 2, 1 };
            var stack = new Stack<int>(fixture);
            SortStack(stack);

            var prev = int.MinValue;
            while (stack.Any()) {
                Assert.LessOrEqual(prev, stack.Peek());
                prev = stack.Pop();
            }
        }
        #endregion

        #region Animal Shelter
        public class Animal {
            public string Name { get; private set; }

            public Animal(string name) {
                Name = name;
            }
        }

        public class Cat : Animal {
            public Cat(string name) : base(name) {}
        }

        public class Dog : Animal {
            public Dog(string name) : base(name) {}
        }

        public class AnimalShelter {
            private int _counter = 0;

            private readonly LinkedListQueue<Tuple<Cat, int>> _cats = new LinkedListQueue<Tuple<Cat, int>>();
            private readonly LinkedListQueue<Tuple<Dog, int>> _dogs = new LinkedListQueue<Tuple<Dog, int>>();

            public void AddCat(Cat cat) {
                _cats.Enqueue(Tuple.Create(cat, ++_counter));
            }

            public void AddDog(Dog dog) {
                _dogs.Enqueue(Tuple.Create(dog, ++_counter));
            }

            public Cat TakeCat() {
                if (_cats.IsEmpty) {
                    throw new Exception("No more cats.");
                }
                return _cats.Dequeue().Item1;
            }

            public Dog TakeDog() {
                if (_dogs.IsEmpty) {
                    throw new Exception("No more dogs.");
                }
                return _dogs.Dequeue().Item1;
            }

            public Animal TakeAnimal() {
                if (_cats.IsEmpty && _dogs.IsEmpty) {
                    throw new Exception("No more animal.");
                }

                if (_cats.IsEmpty) {
                    return _dogs.Dequeue().Item1;
                } else if (_dogs.IsEmpty) {
                    return _cats.Dequeue().Item1;
                } else {
                    return _cats.Peek.Item2 < _dogs.Peek.Item2
                        ? (Animal)_cats.Dequeue().Item1
                        : _dogs.Dequeue().Item1;
                }
            }
        }

        [Test]
        public void TestAnimalShelter() {
            var shelter = new AnimalShelter();
            Assert.Throws<Exception>(() => shelter.TakeCat(), "No more cats.");
            Assert.Throws<Exception>(() => shelter.TakeDog(), "No more dogs.");
            Assert.Throws<Exception>(() => shelter.TakeAnimal(), "No more animal.");

            shelter.AddCat(new Cat("cat1"));
            shelter.AddCat(new Cat("cat2"));
            shelter.AddDog(new Dog("dog1"));
            shelter.AddDog(new Dog("dog2"));
            shelter.AddCat(new Cat("cat3"));

            Assert.AreEqual("cat1", shelter.TakeCat().Name);
            Assert.AreEqual("dog1", shelter.TakeDog().Name);
            Assert.AreEqual("cat2", shelter.TakeAnimal().Name);
            Assert.AreEqual("dog2", shelter.TakeAnimal().Name);
            Assert.AreEqual("cat3", shelter.TakeCat().Name);
        }
        #endregion
    }
}
