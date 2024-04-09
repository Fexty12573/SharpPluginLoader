using System.Collections;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core
{
    /// <summary>
    /// Represents a list of properties of an MtObject.
    /// </summary>
    public class MtPropertyList : MtObject, IEnumerable<MtProperty>
    {
        public MtPropertyList(nint instance) : base(instance) { }
        public MtPropertyList() { }

        ~MtPropertyList()
        {
            Deleter?.Invoke(this);
        }

        /// <summary>
        /// The first property in the list.
        /// </summary>
        public MtProperty? First => GetObject<MtProperty>(0x8);

        /// <summary>
        /// The number of properties in the list.
        /// </summary>
        /// <remarks>This property is not cached, and will iterate through the list to count the properties.</remarks>
        public uint Count
        {
            get
            {
                var current = First;

                uint count = 0;
                while (current != null)
                {
                    count++;
                    current = current.Next;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets all properties in the list.
        /// </summary>
        public MtProperty[] Properties
        {
            get
            {
                var properties = new List<MtProperty>();
                var current = First;
                while (current != null)
                {
                    properties.Add(current);
                    current = current.Next;
                }

                return properties.ToArray();
            }
        }

        /// <summary>
        /// Finds a property by name.
        /// </summary>
        /// <param name="name">The name of the property to find</param>
        /// <returns>The property, or null if it was not found</returns>
        public unsafe MtProperty? FindProperty(string name)
        {
            var prop = FindPropertyFunc.Invoke(Instance, name);
            return prop == 0 ? null : new MtProperty(prop);
        }

        /// <summary>
        /// Finds a property by type and name.
        /// </summary>
        /// <param name="type">The type of the property</param>
        /// <param name="name">The name of the property</param>
        /// <returns>The property, or null if it was not found</returns>
        public unsafe MtProperty? FindProperty(PropType type, string name)
        {
            var prop = FindPropertyOfTypeFunc.Invoke(Instance, (uint)type, name);
            return prop == 0 ? null : new MtProperty(prop);
        }

        /// <summary>
        /// Finds a property by its hash.
        /// </summary>
        /// <param name="hash">The hash of the property to find</param>
        /// <returns>The property, or null if it was not found</returns>
        public unsafe MtProperty? FindProperty(uint hash)
        {
            var prop = FindPropertyByHashFunc.Invoke(Instance, hash);
            return prop == 0 ? null : new MtProperty(prop);
        }

        /// <summary>
        /// Gets the property at the specified index.
        /// </summary>
        public unsafe MtProperty? this[int index]
        {
            get
            {
                var prop = GetPropertyAtFunc.Invoke(Instance, index);
                return prop == 0 ? null : new MtProperty(prop);
            }
        }

        public IEnumerator<MtProperty> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private class Enumerator : IEnumerator<MtProperty>
        {
            private MtPropertyList _list;
            private MtProperty? _current;

            public Enumerator(MtPropertyList list)
            {
                _list = list;
                _current = null;
            }

            public void Dispose()
            {
                _list = null!;
                _current = null;
            }

            public bool MoveNext()
            {
                if (_current == null)
                {
                    _current = _list.First;
                    return _current != null;
                }

                _current = _current.Next;
                return _current != null;
            }

            public void Reset()
            {
                _current = null;
            }

            public MtProperty Current => _current!;

            object IEnumerator.Current => Current;
        }

        private static readonly NativeFunction<nint, string, nint> FindPropertyFunc = new(AddressRepository.Get("MtPropertyList:Find"));
        private static readonly NativeFunction<nint, uint, string, nint> FindPropertyOfTypeFunc = new(AddressRepository.Get("MtPropertyList:FindByType"));
        private static readonly NativeFunction<nint, uint, nint> FindPropertyByHashFunc = new(AddressRepository.Get("MtPropertyList:FindByHash"));
        private static readonly NativeFunction<nint, int, nint> GetPropertyAtFunc = new(AddressRepository.Get("MtPropertyList:operator[]"));

        internal static readonly MtDti Dti = MtDti.Find("MtPropertyList")!;

        internal Action<MtPropertyList>? Deleter = null;
    }
}
