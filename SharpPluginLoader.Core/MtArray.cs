using System.Collections;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// This class is a wrapper for the MtArray object in the game.
    /// </summary>
    public class MtArray<T> : MtObject, IEnumerable<T> where T : MtObject, new()
    {
        /// <summary>
        /// Constructs a new MtArray instance from a native pointer.
        /// </summary>
        /// <param name="instance">The native pointer</param>
        public MtArray(nint instance) : base(instance) { }

        /// <summary>
        /// Constructs a new MtArray instance with a nullptr
        /// </summary>
        public MtArray() { }

        /// <summary>
        /// Gets the length of the array.
        /// </summary>
        public uint Length
        {
            get => Get<uint>(0x8);
            private set => Set(0x8, value);
        }

        /// <summary>
        /// Gets the capacity of the array.
        /// </summary>
        public uint Capacity => Get<uint>(0xC);

        /// <summary>
        /// Gets whether the array auto-deletes its contents when it is destroyed.
        /// </summary>
        public bool AutoDelete => Get<bool>(0x10);

        /// <summary>
        /// Gets whether the array is empty.
        /// </summary>
        public bool IsEmpty => Length == 0;


        private unsafe nint* Data => (nint*)Get<nint>(0x18);

        private unsafe ref nint ObjectAt(int index)
        {
            return ref Data[index];
        }

        /// <summary>
        /// Gets the value at the given index.
        /// </summary>
        /// <param name="index">The index of the object to retrieve</param>
        /// <returns>The object at the index</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if the provided index is out of bounds</exception>
        public T? this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException("Index was outside the bounds of the array.");

                var obj = ObjectAt(index);
                return obj != 0 ? new T { Instance = obj } : null;
            }
            set
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException("Index was outside the bounds of the array.");

                ObjectAt(index) = value?.Instance ?? 0;
            }
        }

        /// <summary>
        /// Adds an item to the end of the array.
        /// </summary>
        /// <param name="item">The item to add</param>
        public void Push(T? item)
        {
            if (Length == Capacity)
                Utility.ResizeArray(this, Capacity + Capacity / 2);

            this[(int)Length] = item;
        }

        /// <summary>
        /// Removes the last item from the array and returns it.
        /// </summary>
        /// <returns>The removed item</returns>
        /// <exception cref="InvalidOperationException">Thrown if the array is empty</exception>
        public T? Pop()
        {
            if (Length == 0)
                throw new InvalidOperationException("Array is empty.");

            var item = Last();
            this[(int)Length - 1] = null;
            Length--;

            return item;
        }

        /// <summary>
        /// Gets the first item in the array.
        /// </summary>
        /// <returns>The first item in the array</returns>
        /// <exception cref="InvalidOperationException">Thrown if the array is empty</exception>
        public T? First()
        {
            if (Length == 0)
                throw new InvalidOperationException("Array is empty.");

            return this[0];
        }

        /// <summary>
        /// Gets the last item in the array.
        /// </summary>
        /// <returns>The last item in the array</returns>
        /// <exception cref="InvalidOperationException">Thrown if the array is empty</exception>
        public T? Last()
        {
            if (Length == 0)
                throw new InvalidOperationException("Array is empty.");

            return this[(int)Length - 1];
        }

        /// <summary>
        /// Checks if the array contains the given item.
        /// </summary>
        /// <param name="item">The item to check for</param>
        /// <remarks>This function does a reference comparison.</remarks>
        /// <returns>True if the array contains the item</returns>
        public bool Contains(T? item)
        {
            return this.FirstOrDefault(x => x == item) != null;
        }

        /// <summary>
        /// Gets an enumerator for the array.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return new CustomEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class CustomEnumerator(MtArray<T> array) : IEnumerator<T>
        {
            private int _index = -1;

            public void Dispose() { }

            public bool MoveNext()
            {
                _index++;
                return _index < array.Length;
            }

            public void Reset()
            {
                _index = -1;
            }

            public T Current => array[_index]!;

            object IEnumerator.Current => Current;
        }
    }
}
