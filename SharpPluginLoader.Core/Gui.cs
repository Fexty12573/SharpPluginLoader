using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core
{
    public unsafe class Gui
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct Internal_sGUI
        {
            [FieldOffset(0xB8)] public nint mpFontFilterResource;
            [FieldOffset(0xC4)] public uint mEnableRubySpace;
            [FieldOffset(0x10C)] public bool mIsBufferCompactionAll;
            [FieldOffset(0x110)] public uint mVertexBufferSize;
        }

        private readonly Internal_sGUI* _instance;

        public nint Instance => (nint)_instance;

        public Gui(nint instance)
        {
            _instance = (Internal_sGUI*)instance;
        }

        public nint FontFilterResource
        {
            get => _instance->mpFontFilterResource;
            set => _instance->mpFontFilterResource = value;
        }

        public uint EnableRubySpace
        {
            get => _instance->mEnableRubySpace;
            set => _instance->mEnableRubySpace = value;
        }

        public bool IsBufferCompactionAll
        {
            get => _instance->mIsBufferCompactionAll;
            set => _instance->mIsBufferCompactionAll = value;
        }

        public uint VertexBufferSize
        {
            get => _instance->mVertexBufferSize;
            set => _instance->mVertexBufferSize = value;
        }
    }
}
