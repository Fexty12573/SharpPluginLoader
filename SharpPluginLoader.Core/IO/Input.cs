using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.IO
{
    /// <summary>
    /// Provides a set of methods for checking the state of the controller and keyboard.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Checks if the specified button is currently pressed.
        /// </summary>
        public static bool IsDown(Button button) => (PadDown & (uint)button) != 0;

        /// <summary>
        /// Checks if the specified button was pressed in the last frame.
        /// </summary>
        public static bool IsPressed(Button button) => (PadTrg & (uint)button) != 0;

        /// <summary>
        /// Checks if the specified button was released in the last frame.
        /// </summary>
        public static bool IsReleased(Button button) => (PadRel & (uint)button) != 0;

        /// <summary>
        /// Checks if the specified button was pressed or released in the last frame.
        /// </summary>
        public static bool IsChanged(Button button) => (PadChg & (uint)button) != 0;

        /// <summary>
        /// Checks if the specified button is currently pressed.
        /// </summary>
        private static unsafe bool IsDown(VirtualKey key)
        {
            var state = KbState;
            var vk = KbVkTable[(int)key];
            return (state->On[vk >> 5] & 1u << (vk & 0x1F)) != 0;
        }

        /// <summary>
        /// Checks if the specified button was pressed in the last frame.
        /// </summary>
        private static unsafe bool IsPressed(VirtualKey key)
        {
            var state = KbState;
            var vk = KbVkTable[(int)key];
            return (state->Trg[vk >> 5] & 1u << (vk & 0x1F)) != 0;
        }

        /// <summary>
        /// Checks if the specified button was released in the last frame.
        /// </summary>
        private static unsafe bool IsReleased(VirtualKey key)
        {
            var state = KbState;
            var vk = KbVkTable[(int)key];
            return (state->Rel[vk >> 5] & 1u << (vk & 0x1F)) != 0;
        }

        /// <summary>
        /// Checks if the specified button was pressed or released in the last frame.
        /// </summary>
        private static unsafe bool IsChanged(VirtualKey key)
        {
            var state = KbState;
            var vk = KbVkTable[(int)key];
            return (state->Chg[vk >> 5] & 1u << (vk & 0x1F)) != 0;
        }
        
        
        private static nint Pad => MemoryUtil.Read<nint>(0x1451c2318);
        private static nint Keyboard => MemoryUtil.Read<nint>(0x1451c3170);

        private static uint PadDown => (Pad + 0x198).Read<uint>();
        private static uint PadOld => (Pad + 0x19C).Read<uint>();
        private static uint PadTrg => (Pad + 0x1A0).Read<uint>();
        private static uint PadRel => (Pad + 0x1A4).Read<uint>();
        private static uint PadChg => (Pad + 0x1A8).Read<uint>();

        private static unsafe KeyboardState* KbState => (KeyboardState*)(Keyboard + 0x138);
        private static unsafe byte* KbVkTable => (byte*)(Keyboard + 0x38);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal unsafe struct KeyboardState
    {
        public fixed uint On[8];
        public fixed uint Old[8];
        public fixed uint Trg[8];
        public fixed uint Rel[8];
        public fixed uint Chg[8];
        public fixed uint Repeat[8];
        public fixed ulong RepeatTime[256];
    }
}
