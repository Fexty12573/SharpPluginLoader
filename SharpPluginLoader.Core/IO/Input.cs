using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.IO
{
    /// <summary>
    /// Provides a set of methods for checking the state of the controller, mouse and keyboard.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Checks if the specified button is currently pressed.
        /// </summary>
        public static bool IsDown(Button button)
        {
            return (padDown & (uint)button) == (uint)button;
        }

        /// <summary>
        /// Checks if the specified button was pressed in the last frame.
        /// </summary>
        public static bool IsPressed(Button button)
        {
            return (padDown & (uint)button) == (uint)button && (prevPadDown & (uint)button) != (uint)button;
        }

        /// <summary>
        /// Checks if the specified button was released in the last frame.
        /// </summary>
        public static bool IsReleased(Button button)
        {
            return (padDown & (uint)button) != (uint)button && (prevPadDown & (uint)button) == (uint)button;
        }

        /// <summary>
        /// Checks if the specified button was pressed or released in the last frame.
        /// </summary>
        public static bool IsChanged(Button button)
        {
            return (padDown & (uint)button) != (prevPadDown & (uint)button);
        }

        /// <summary>
        /// Checks if the specified key is currently pressed.
        /// </summary>
        public static unsafe bool IsDown(Key key)
        {
            var state = KbState;
            var vk = KbVkTable[(int)key];
            var vkMask = 1u << (vk & 0x1F);
            return (keyDown[vk >> 5] & vkMask) == vkMask;
        }

        /// <summary>
        /// Checks if the specified key was pressed in the last frame.
        /// </summary>
        public static unsafe bool IsPressed(Key key)
        {
            var state = KbState;
            var vk = KbVkTable[(int)key];
            var vkMask = 1u << (vk & 0x1F);
            return (keyDown[vk >> 5] & vkMask) == vkMask && (prevKeyDown[vk >> 5] & vkMask) != vkMask;
        }

        /// <summary>
        /// Checks if the specified key was released in the last frame.
        /// </summary>
        public static unsafe bool IsReleased(Key key)
        {
            var state = KbState;
            var vk = KbVkTable[(int)key];
            var vkMask = 1u << (vk & 0x1F);
            return (keyDown[vk >> 5] & vkMask) != vkMask && (prevKeyDown[vk >> 5] & vkMask) == vkMask;
        }

        /// <summary>
        /// Checks if the specified key was pressed or released in the last frame.
        /// </summary>
        public static unsafe bool IsChanged(Key key)
        {
            var state = KbState;
            var vk = KbVkTable[(int)key];
            var vkMask = 1u << (vk & 0x1F);
            return (keyDown[vk >> 5] & vkMask) != (prevKeyDown[vk >> 5] & vkMask);
        }

        /// <summary>
        /// Checks if the specified button is currently pressed.
        /// </summary>
        public static bool IsDown(Mouse button)
        {
            return (mouseDown & (uint)button) == (uint)button;
        }

        /// <summary>
        /// Checks if the specified button was pressed in the last frame.
        /// </summary>
        public static bool IsPressed(Mouse button)
        {
            return (mouseDown & (uint)button) == (uint)button && (prevMouseDown & (uint)button) != (uint)button;
        }

        /// <summary>
        /// Checks if the specified button was released in the last frame.
        /// </summary>
        public static bool IsReleased(Mouse button)
        {
            return (mouseDown & (uint)button) != (uint)button && (prevMouseDown & (uint)button) == (uint)button;
        }

        /// <summary>
        /// Checks if the specified button was pressed or released in the last frame.
        /// </summary>
        public static bool IsChanged(Mouse button)
        {
            return (mouseDown & (uint)button) != (prevMouseDown & (uint)button);
        }

        public static void UpdatePadState()
        {
            prevPadDown = padDown;
            padDown = PadDown;
            padRx = PadRx;
            padRy = PadRy;
            padLx = PadLx;
            padLy = PadLy;
            padRz = PadRz;
            padLz = PadLz;
            PadCaptured = false;
        }

        public static void UpdateMouseState()
        {
            prevMouseDown = mouseDown;
            mouseDown = MemoryUtil.Read<uint>(Mouse.Instance + 0x58);
            mouseDeltaX = MemoryUtil.Read<int>(Mouse.Instance + 0x4C);
            mouseDeltaY = MemoryUtil.Read<int>(Mouse.Instance + 0x50);
            scrollDeltaY = MemoryUtil.Read<int>(Mouse.Instance + 0x54);
            MouseCaptured = false;
        }

        public static unsafe void UpdateKeyboardState()
        {
            fixed (uint* lastPtr = keyDown, prevPtr = prevKeyDown)
            {
                MemoryUtil.Copy<uint>(lastPtr, prevPtr, 8);
                MemoryUtil.Copy<uint>(KbState->On, lastPtr, 8);
            }
            KeyboardCaptured = false;
        }

        public static bool PadCaptured = false;
        public static bool MouseCaptured = false;
        public static bool KeyboardCaptured = false;

        public static int GetPadRx() => padRx;
        public static int GetPadRy() => padRy;

        public static int GetPadLx() => padLx;
        public static int GetPadLy() => padLy;

        public static int SetPadRx(int Rx) => PadRx = Rx;
        public static int SetPadRy(int Ry) => PadRy = Ry;

        public static int SetPadLx(int Lx) => PadLx = Lx;
        public static int SetPadLy(int Ly) => PadLy = Ly;

        public static int GetMouseDeltaX() => mouseDeltaX;
        public static int GetMouseDeltaY() => mouseDeltaY;

        public static int GetScrollDeltaY() => scrollDeltaY;

        public static void Block(Button button)
        {
            PadDown &= ~(uint)button;
            PadOld &= ~(uint)button;
            PadTrg &= ~(uint)button;
            PadRel &= ~(uint)button;
            PadChg &= ~(uint)button;
            PadRepeat &= ~(uint)button;
        }

        public static void BlockPadButtons()
        {
            PadDown = 0;
            PadOld = 0;
            PadTrg = 0;
            PadRel = 0;
            PadChg = 0;
            PadRepeat = 0;
        }

        public static void BlockPadRx() => PadRx = 0;
        public static void BlockPadRy() => PadRy = 0;

        public static void BlockPadLx() => PadLx = 0;
        public static void BlockPadLy() => PadLy = 0;

        public static void BlockPadRz() => PadRz = 0;
        public static void BlockPadLz() => PadLz = 0;

        public static void BlockPad()
        {
            BlockPadButtons();
            BlockPadRx();
            BlockPadRy();
            BlockPadLx();
            BlockPadLy();
            BlockPadRz();
            BlockPadLz();
        }

        public static unsafe void Block(Key key)
        {
            var state = KbState;
            var vk = KbVkTable[(int)key];
            var vkMask = 1u << (vk & 0x1F);
            state->On[vk >> 5] &= ~vkMask;
        }

        public static unsafe void BlockAllKeys()
        {
            NativeMemory.Clear((byte*)KbState, (nuint)sizeof(KeyboardState));
        }

        public static void Block(Mouse button)
        {
            MemoryUtil.GetRef<uint>(Mouse.Instance + 0x58) &= ~(uint)button;
        }

        public static void BlockMouseClicks()
        {
            MemoryUtil.GetRef<uint>(Mouse.Instance + 0x58) = 0x0;
        }

        public static void BlockMouseDelta()
        {
            MemoryUtil.GetRef<ulong>(Mouse.Instance + 0x4C) = 0L; // dX(int), dY(int).
        }

        public static void BlockScrollWheel()
        {
            MemoryUtil.GetRef<int>(Mouse.Instance + 0x54) = 0;
        }

        private static MtObject Pad => SingletonManager.GetSingleton("sMhSteamController")!;
        private static MtObject Mouse => SingletonManager.GetSingleton("sMhMouse")!;
        private static MtObject Keyboard => SingletonManager.GetSingleton("sMhKeyboard")!;

        // sMhSteamController + (0x38 + (padIndex * 0x2D8)).
        private static ref uint PadDown => ref MemoryUtil.GetRef<uint>(Pad.Instance + 0x198);
        private static ref uint PadOld => ref MemoryUtil.GetRef<uint>(Pad.Instance + 0x19C);
        private static ref uint PadTrg => ref MemoryUtil.GetRef<uint>(Pad.Instance + 0x1A0);
        private static ref uint PadRel => ref MemoryUtil.GetRef<uint>(Pad.Instance + 0x1A4);
        private static ref uint PadChg => ref MemoryUtil.GetRef<uint>(Pad.Instance + 0x1A8);
        private static ref uint PadRepeat => ref MemoryUtil.GetRef<uint>(Pad.Instance + 0x1AC);

        private static ref int PadRx => ref MemoryUtil.GetRef<int>(Pad.Instance + 0x1B0);
        private static ref int PadRy => ref MemoryUtil.GetRef<int>(Pad.Instance + 0x1B4);
        private static ref int PadLx => ref MemoryUtil.GetRef<int>(Pad.Instance + 0x1B8);
        private static ref int PadLy => ref MemoryUtil.GetRef<int>(Pad.Instance + 0x1BC);

        private static ref byte PadRz => ref MemoryUtil.GetRef<byte>(Pad.Instance + 0x1C0);
        private static ref byte PadLz => ref MemoryUtil.GetRef<byte>(Pad.Instance + 0x1C1);

        // sMhMouse + (stateIndex * 0x78).
        private static uint MouseOn => MemoryUtil.Read<uint>(Mouse.Instance + 0x108);
        private static uint MouseOld => MemoryUtil.Read<uint>(Mouse.Instance + 0x10C);
        private static uint MouseTrg => MemoryUtil.Read<uint>(Mouse.Instance + 0x110);
        private static uint MouseRel => MemoryUtil.Read<uint>(Mouse.Instance + 0x114);
        private static uint MouseChg => MemoryUtil.Read<uint>(Mouse.Instance + 0x118);
        private static uint MouseRepeat => MemoryUtil.Read<uint>(Mouse.Instance + 0x11C);
        // MouseDeltaX: Mouse.Instance + 0xFC
        // MouseDeltaY: Mouse.Instance + 0x100
        // ScrollDeltaY: Mouse.Instance + 0x17C

        private static unsafe KeyboardState* KbState => (KeyboardState*)(Keyboard.Instance + 0x138);
        private static unsafe byte* KbVkTable => (byte*)(Keyboard.Instance + 0x38);

        private static uint padDown;
        private static uint prevPadDown;
        private static int padRx;
        private static int padRy;
        private static int padLx;
        private static int padLy;
        private static byte padRz;
        private static byte padLz;

        private static uint mouseDown;
        private static uint prevMouseDown;
        private static int mouseDeltaX;
        private static int mouseDeltaY;
        private static int scrollDeltaY;

        private static uint[] keyDown = new uint[8];
        private static uint[] prevKeyDown = new uint[8];
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
