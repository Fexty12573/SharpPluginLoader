using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public class MtProperty : NativeWrapper
    {
        public MtProperty(nint instance) : base(instance) { }
        public MtProperty() { }

        public string Name => Marshal.PtrToStringAnsi(Get<nint>(0x0))!;

        public string Comment => Marshal.PtrToStringAnsi(Get<nint>(0x8))!;

        public PropType Type => (PropType)(Get<uint>(0x10) & 0xFFF);

        public uint Attr => Get<uint>(0x10) >> 12;

        public MtObject? Owner => GetObject<MtObject>(0x18);

        public nint Get => Get<nint>(0x20);

        public nint GetCount => Get<nint>(0x28);

        public nint Set => Get<nint>(0x30);

        public nint Realloc => Get<nint>(0x38);

        public uint Index => Get<uint>(0x40);

        public MtProperty? Previous => GetObject<MtProperty>(0x48);

        public MtProperty? Next => GetObject<MtProperty>(0x50);
    }
}
