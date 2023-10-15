using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public class MtPropertyList : MtObject, IEnumerable<MtProperty>
    {
        private static readonly unsafe delegate* unmanaged[Fastcall]<nint, string, nint> InternalFindProperty = 
            (delegate* unmanaged[Fastcall]<nint, string, nint>)0x14218e740;
        private static readonly unsafe delegate* unmanaged[Fastcall]<nint, uint, string, nint> InternalFindPropertyOfType = 
            (delegate* unmanaged[Fastcall]<nint, uint, string, nint>)0x14218e6b0;
        private static readonly unsafe delegate* unmanaged[Fastcall]<nint, int, nint> InternalGetPropertyAt = 
            (delegate* unmanaged[Fastcall]<nint, int, nint>)0x01d6d290;

        internal Action<MtPropertyList>? Deleter = null;

        public MtPropertyList(nint instance) : base(instance) { }
        public MtPropertyList() { }

        ~MtPropertyList()
        {
            Deleter?.Invoke(this);
        }

        public MtProperty? First => GetObject<MtProperty>(0x8);

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

        public unsafe MtProperty? FindProperty(string name)
        {
            var prop = InternalFindProperty(Instance, name);
            return prop == 0 ? null : new MtProperty(prop);
        }

        public unsafe MtProperty? FindProperty(PropType type, string name)
        {
            var prop = InternalFindPropertyOfType(Instance, (uint)type, name);
            return prop == 0 ? null : new MtProperty(prop);
        }

        public unsafe MtProperty? this[int index]
        {
            get
            {
                var prop = InternalGetPropertyAt(Instance, index);
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
    }
}
