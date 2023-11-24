using System.Collections;

namespace SharpPluginLoader.Core
{
    public readonly unsafe struct NativeArray<T> : IEnumerable<T> where T : unmanaged
    {
        public int Length { get; }

        public nint Pointer { get; }

        public NativeArray(nint pointer, int length)
        {
            Pointer = pointer;
            Length = length;
        }

        public T this[int index]
        {
            get => *(T*)(Pointer + (index * sizeof(T)));
            set => *(T*)(Pointer + (index * sizeof(T))) = value;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (var i = 0; i < Length; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
