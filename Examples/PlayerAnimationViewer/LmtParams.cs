using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;

namespace PlayerAnimationViewer
{
    [StructLayout(LayoutKind.Sequential, Size = 0x10)]
    public readonly struct LmtParamType
    {
        private readonly nint _dti;
        public readonly uint PropCount;
        public readonly uint UniqueId;

        public MtDti Dti => new(_dti);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x60)]
    public readonly unsafe struct LmtParamMemberDef
    {
        [FieldOffset(0x00)] private readonly sbyte* _name;
        [FieldOffset(0x08)] private readonly sbyte* _comment;
        [FieldOffset(0x10)] public readonly uint Attr;

        [FieldOffset(0x18)] private readonly nint _object;
        [FieldOffset(0x20)] private readonly nint _get;
        [FieldOffset(0x28)] private readonly nint _getCount;
        [FieldOffset(0x30)] private readonly nint _set;
        [FieldOffset(0x38)] private readonly nint _setCount;
        
        [FieldOffset(0x58)] public readonly uint Hash;

        public string Name => _name != null ? new string(_name) : "";
        public string Comment => _comment != null ? new string(_comment) : "";
        public PropType Type => (PropType)(Attr & 0xFFF);
        public MtObject Object => new(_object);
    }
}
