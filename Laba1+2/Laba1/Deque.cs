using System;
using System.Collections.Generic;

namespace OwnCollections
{
    public sealed class Deque<T> : IList<T>, IReadOnlyList<T>, System.Collections.IList
    {
        #region Fields

        private const int DefaultCapacity = 8;

        private T[] _buffer;

        private int _offset;
        public bool IsEmpty
        {
            get { return Count == 0; }
        }

        private bool IsFull
        {
            get { return Count == Capacity; }
        }

        private bool IsSplit
        {
            get
            {
                return _offset > (Capacity - Count);
            }
        }

        public int Capacity
        {
            get
            {
                return _buffer.Length;
            }

            set
            {
                if (value < Count)
                    throw new ArgumentOutOfRangeException(nameof(value), "Capacity cannot be set to a value less than Count");

                if (value == _buffer.Length)
                    return;

                T[] newBuffer = new T[value];
                CopyToArray(newBuffer);

                _buffer = newBuffer;
                _offset = 0;
            }
        }

        public int Count { get; private set; }

        #endregion

        #region Methods

        public Deque(int capacity)
        {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity may not be negative.");
            _buffer = new T[capacity];
        }

        public Deque()
            : this(DefaultCapacity)
        {
        }

        public Deque(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            var source = CollectionHelpers.ReifyCollection(collection);
            var count = source.Count;
            if (count > 0)
            {
                _buffer = new T[count];
                DoInsertRange(0, source);
            }
            else
            {
                _buffer = new T[DefaultCapacity];
            }
        }

        private int DequeIndexToBufferIndex(int index)
        {
            return (index + _offset) % Capacity;
        }

        private T DoGetItem(int index)
        {
            return _buffer[DequeIndexToBufferIndex(index)];
        }

        private void DoSetItem(int index, T item)
        {
            _buffer[DequeIndexToBufferIndex(index)] = item;
        }

        private void DoInsert(int index, T item)
        {
            EnsureCapacityForOneElement();

            if (index == 0)
            {
                DoAddToFront(item);
                return;
            }
            else if (index == Count)
            {
                DoAddToBack(item);
                return;
            }

            DoInsertRange(index, new[] { item });
        }

        private void DoRemoveAt(int index)
        {
            if (index == 0)
            {
                DoRemoveFromFront();
                return;
            }
            else if (index == Count - 1)
            {
                DoRemoveFromBack();
                return;
            }

            DoRemoveRange(index, 1);
        }

        private int PostIncrement(int value)
        {
            int ret = _offset;
            _offset += value;
            _offset %= Capacity;
            return ret;
        }

        private int PreDecrement(int value)
        {
            _offset -= value;
            if (_offset < 0)
                _offset += Capacity;
            return _offset;
        }

        private void DoAddToBack(T value)
        {
            _buffer[DequeIndexToBufferIndex(Count)] = value;
            ++Count;
        }

        private void DoAddToFront(T value)
        {
            _buffer[PreDecrement(1)] = value;
            ++Count;
        }

        private T DoRemoveFromBack()
        {
            T ret = _buffer[DequeIndexToBufferIndex(Count - 1)];
            --Count;
            return ret;
        }

        private T DoRemoveFromFront()
        {
            --Count;
            return _buffer[PostIncrement(1)];
        }

        private void DoInsertRange(int index, IReadOnlyCollection<T> collection)
        {
            var collectionCount = collection.Count;
            if (index < Count / 2)
            {
                int copyCount = index;
                int writeIndex = Capacity - collectionCount;
                for (int j = 0; j != copyCount; ++j)
                    _buffer[DequeIndexToBufferIndex(writeIndex + j)] = _buffer[DequeIndexToBufferIndex(j)];

                PreDecrement(collectionCount);
            }
            else
            {
                int copyCount = Count - index;
                int writeIndex = index + collectionCount;
                for (int j = copyCount - 1; j != -1; --j)
                    _buffer[DequeIndexToBufferIndex(writeIndex + j)] = _buffer[DequeIndexToBufferIndex(index + j)];
            }

            int i = index;
            foreach (T item in collection)
            {
                _buffer[DequeIndexToBufferIndex(i)] = item;
                ++i;
            }

            Count += collectionCount;
        }

        private void DoRemoveRange(int index, int collectionCount)
        {
            if (index == 0)
            {
                PostIncrement(collectionCount);
                Count -= collectionCount;
                return;
            }
            else if (index == Count - collectionCount)
            {
                Count -= collectionCount;
                return;
            }

            if ((index + (collectionCount / 2)) < Count / 2)
            {
                int copyCount = index;
                int writeIndex = collectionCount;
                for (int j = copyCount - 1; j != -1; --j)
                    _buffer[DequeIndexToBufferIndex(writeIndex + j)] = _buffer[DequeIndexToBufferIndex(j)];

                PostIncrement(collectionCount);
            }
            else
            {
                int copyCount = Count - collectionCount - index;
                int readIndex = index + collectionCount;
                for (int j = 0; j != copyCount; ++j)
                    _buffer[DequeIndexToBufferIndex(index + j)] = _buffer[DequeIndexToBufferIndex(readIndex + j)];
            }

            Count -= collectionCount;
        }

        private void EnsureCapacityForOneElement()
        {
            if (IsFull)
            {
                Capacity = (Capacity == 0) ? 1 : Capacity * 2;
            }
        }

        public void AddToBack(T value)
        {
            EnsureCapacityForOneElement();
            DoAddToBack(value);
        }

        public void AddToFront(T value)
        {
            EnsureCapacityForOneElement();
            DoAddToFront(value);
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            CheckNewIndexArgument(Count, index);
            var source = CollectionHelpers.ReifyCollection(collection);
            int collectionCount = source.Count;

            if (collectionCount > Capacity - Count)
            {
                Capacity = checked(Count + collectionCount);
            }

            if (collectionCount == 0)
            {
                return;
            }

            DoInsertRange(index, source);
        }

        public void RemoveRange(int offset, int count)
        {
            CheckRangeArguments(Count, offset, count);

            if (count == 0)
            {
                return;
            }

            DoRemoveRange(offset, count);
        }

        public T RemoveFromBack()
        {
            if (IsEmpty)
                throw new InvalidOperationException("The deque is empty.");

            return DoRemoveFromBack();
        }

        public T RemoveFromFront()
        {
            if (IsEmpty)
                throw new InvalidOperationException("The deque is empty.");

            return DoRemoveFromFront();
        }

        public void Clear()
        {
            _offset = 0;
            Count = 0;
        }

        public T[] ToArray()
        {
            var result = new T[Count];
            ((ICollection<T>)this).CopyTo(result, 0);
            return result;
        }

        public void PrintDeque()
        {
            Console.Write("[ ");

            foreach (T element in this)
            {
                Console.Write(element + " ");
            }

            Console.Write("]");
            Console.WriteLine();
        }

        #endregion

        #region GenericListImplementations

        bool ICollection<T>.IsReadOnly => false;

        public T this[int index]
        {
            get
            {
                CheckExistingIndexArgument(Count, index);
                return DoGetItem(index);
            }

            set
            {
                CheckExistingIndexArgument(Count, index);
                DoSetItem(index, value);
            }
        }

        public void Insert(int index, T item)
        {
            CheckNewIndexArgument(Count, index);
            DoInsert(index, item);
        }

        public void RemoveAt(int index)
        {
            CheckExistingIndexArgument(Count, index);
            DoRemoveAt(index);
        }

        public int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            int ret = 0;
            foreach (var sourceItem in this)
            {
                if (comparer.Equals(item, sourceItem))
                    return ret;
                ++ret;
            }

            return -1;
        }

        void ICollection<T>.Add(T item)
        {
            DoInsert(Count, item);
        }

        bool ICollection<T>.Contains(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            foreach (var entry in this)
            {
                if (comparer.Equals(item, entry))
                    return true;
            }
            return false;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            int count = Count;
            CheckRangeArguments(array.Length, arrayIndex, count);
            CopyToArray(array, arrayIndex);
        }

        private void CopyToArray(Array array, int arrayIndex = 0)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (IsSplit)
            {
                int length = Capacity - _offset;
                Array.Copy(_buffer, _offset, array, arrayIndex, length);
                Array.Copy(_buffer, 0, array, arrayIndex + length, Count - length);
            }
            else
            {
                Array.Copy(_buffer, _offset, array, arrayIndex, Count);
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
                return false;

            DoRemoveAt(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            int count = Count;
            for (int i = 0; i != count; ++i)
            {
                yield return DoGetItem(i);
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region ObjectListImplementations

        private static bool IsT(object value)
        {
            if (value is T)
                return true;
            if (value != null)
                return false;
            return default(T) == null;
        }

        int System.Collections.IList.Add(object value)
        {
            if (value == null && default(T) != null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            if (!IsT(value))
                throw new ArgumentException("Value is of incorrect type.", nameof(value));
            AddToBack((T)value);
            return Count - 1;
        }

        bool System.Collections.IList.Contains(object value)
        {
            return IsT(value) ? ((ICollection<T>)this).Contains((T)value) : false;
        }

        int System.Collections.IList.IndexOf(object value)
        {
            return IsT(value) ? IndexOf((T)value) : -1;
        }

        void System.Collections.IList.Insert(int index, object value)
        {
            if (value == null && default(T) != null)
                throw new ArgumentNullException(nameof(value), "Value cannot be null.");
            if (!IsT(value))
                throw new ArgumentException("Value is of incorrect type.", nameof(value));
            Insert(index, (T)value);
        }

        bool System.Collections.IList.IsFixedSize
        {
            get { return false; }
        }

        bool System.Collections.IList.IsReadOnly
        {
            get { return false; }
        }

        void System.Collections.IList.Remove(object value)
        {
            if (IsT(value))
                Remove((T)value);
        }

        object System.Collections.IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                if (value == null && default(T) != null)
                    throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                if (!IsT(value))
                    throw new ArgumentException("Value is of incorrect type.", nameof(value));
                this[index] = (T)value;
            }
        }

        void System.Collections.ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array), "Destination array cannot be null.");
            CheckRangeArguments(array.Length, index, Count);

            try
            {
                CopyToArray(array, index);
            }
            catch (ArrayTypeMismatchException ex)
            {
                throw new ArgumentException("Destination array is of incorrect type.", nameof(array), ex);
            }
            catch (RankException ex)
            {
                throw new ArgumentException("Destination array must be single dimensional.", nameof(array), ex);
            }
        }

        bool System.Collections.ICollection.IsSynchronized
        {
            get { return false; }
        }

        object System.Collections.ICollection.SyncRoot
        {
            get { return this; }
        }

        #endregion

        #region GenericListHelpers

        private static void CheckNewIndexArgument(int sourceLength, int index)
        {
            if (index < 0 || index > sourceLength)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Invalid new index " + index + " for source length " + sourceLength);
            }
        }

        private static void CheckExistingIndexArgument(int sourceLength, int index)
        {
            if (index < 0 || index >= sourceLength)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Invalid existing index " + index + " for source length " + sourceLength);
            }
        }

        private static void CheckRangeArguments(int sourceLength, int offset, int count)
        {
            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Invalid offset " + offset);
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Invalid count " + count);
            }

            if (sourceLength - offset < count)
            {
                throw new ArgumentException("Invalid offset (" + offset + ") or count + (" + count + ") for source length " + sourceLength);
            }
        }

        #endregion

    }
}
