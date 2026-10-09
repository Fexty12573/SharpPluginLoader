using System.Drawing;
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
            return (padDown & (uint)button) != 0;
        }

        /// <summary>
        /// Checks if the specified button was pressed in the last frame.
        /// </summary>
        public static bool IsPressed(Button button)
        {
            return (padDown & (uint)button) != 0 && (prevPadDown & (uint)button) == 0;
        }

        /// <summary>
        /// Checks if the specified button was released in the last frame.
        /// </summary>
        public static bool IsReleased(Button button)
        {
            return (padDown & (uint)button) == 0 && (prevPadDown & (uint)button) != 0;
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
            (int index, uint vkMask) = GetVk(key);
            return (keyDown[index] & vkMask) != 0;
        }

        /// <summary>
        /// Checks if the specified key was pressed in the last frame.
        /// </summary>
        public static unsafe bool IsPressed(Key key)
        {
            (int index, uint vkMask) = GetVk(key);
            return (keyDown[index] & vkMask) != 0 && (prevKeyDown[index] & vkMask) == 0;
        }

        /// <summary>
        /// Checks if the specified key was released in the last frame.
        /// </summary>
        public static unsafe bool IsReleased(Key key)
        {
            (int index, uint vkMask) = GetVk(key);
            return (keyDown[index] & vkMask) == 0 && (prevKeyDown[index] & vkMask) != 0;
        }

        /// <summary>
        /// Checks if the specified key was pressed or released in the last frame.
        /// </summary>
        public static unsafe bool IsChanged(Key key)
        {
            (int index, uint vkMask) = GetVk(key);
            return (keyDown[index] & vkMask) != (prevKeyDown[index] & vkMask);
        }

        /// <summary>
        /// Checks if the specified button is currently pressed.
        /// </summary>
        public static bool IsDown(Mouse button)
        {
            return (mouseDown & (uint)button) != 0;
        }

        /// <summary>
        /// Checks if the specified button was pressed in the last frame.
        /// </summary>
        public static bool IsPressed(Mouse button)
        {
            return (mouseDown & (uint)button) != 0 && (prevMouseDown & (uint)button) == 0;
        }

        /// <summary>
        /// Checks if the specified button was released in the last frame.
        /// </summary>
        public static bool IsReleased(Mouse button)
        {
            return (mouseDown & (uint)button) == 0 && (prevMouseDown & (uint)button) != 0;
        }

        /// <summary>
        /// Checks if the specified button was pressed or released in the last frame.
        /// </summary>
        public static bool IsChanged(Mouse button)
        {
            return (mouseDown & (uint)button) != (prevMouseDown & (uint)button);
        }

        public static bool PadCaptured = false;
        public static bool MouseCaptured = false;
        public static bool KeyboardCaptured = false;

        public static int GetPadRx() => padRx;
        public static int GetPadRy() => padRy;

        public static int GetPadLx() => padLx;
        public static int GetPadLy() => padLy;

        public static int GetPadRz() => padRz;
        public static int GetPadLz() => padLz;

        public static Point GetMouseDelta() => mouseDelta;
        public static int GetMouseWheel() => mouseWheel;

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

        public static void Block(Mouse button)
        {
            Mouse.GetRef<uint>(0x58) &= ~(uint)button;
        }

        public static void BlockMouseClicks()
        {
            Mouse.GetRef<uint>(0x58) = 0;
        }

        public static void BlockMouseDelta()
        {
            Mouse.GetRef<ulong>(0x4C) = 0L; // dX(int), dY(int).
        }

        public static void BlockMouseWheel()
        {
            Mouse.GetRef<int>(0x54) = 0;
        }

        public static void BlockMouse()
        {
            BlockMouseClicks();
            BlockMouseDelta();
            BlockMouseWheel();
        }

        public static unsafe void Block(Key key)
        {
            (int index, uint vkMask) = GetVk(key);
            KbState->On[index] &= ~vkMask;
        }

        public static unsafe void BlockKeyboard()
        {
            NativeMemory.Clear((byte*)KbState, (nuint)(sizeof(uint) * 8 * 6));
        }

        private static MtObject Pad => SingletonManager.GetSingleton("sMhSteamController")!;
        private static MtObject Mouse => SingletonManager.GetSingleton("sMhMouse")!;
        private static MtObject Keyboard => SingletonManager.GetSingleton("sMhKeyboard")!;

        // sMhSteamController + (0x38 + (padIndex * 0x2D8)).
        private static ref uint PadDown => ref Pad.GetRef<uint>(0x198);
        private static ref uint PadOld => ref Pad.GetRef<uint>(0x19C);
        private static ref uint PadTrg => ref Pad.GetRef<uint>(0x1A0);
        private static ref uint PadRel => ref Pad.GetRef<uint>(0x1A4);
        private static ref uint PadChg => ref Pad.GetRef<uint>(0x1A8);
        private static ref uint PadRepeat => ref Pad.GetRef<uint>(0x1AC);

        private static ref int PadRx => ref Pad.GetRef<int>(0x1B0);
        private static ref int PadRy => ref Pad.GetRef<int>(0x1B4);

        private static ref int PadLx => ref Pad.GetRef<int>(0x1B8);
        private static ref int PadLy => ref Pad.GetRef<int>(0x1BC);

        private static ref byte PadRz => ref Pad.GetRef<byte>(0x1C0);
        private static ref byte PadLz => ref Pad.GetRef<byte>(0x1C1);

        // sMhMouse + (stateIndex * 0x78).
        private static uint MouseOn => Mouse.Get<uint>(0x108);
        private static uint MouseOld => Mouse.Get<uint>(0x10C);
        private static uint MouseTrg => Mouse.Get<uint>(0x110);
        private static uint MouseRel => Mouse.Get<uint>(0x114);
        private static uint MouseChg => Mouse.Get<uint>(0x118);
        private static uint MouseRepeat => Mouse.Get<uint>(0x11C);

        private static Point MouseDelta => Mouse.Get<Point>(0xFC);
        private static int MouseWheel => Mouse.Get<int>(0x17C);

        private static byte MetaKeyVk(Key key)
        {
            switch (key)
            {
                case Key.LeftMouse:   return 0x1;
                case Key.RightMouse:  return 0x2;
                case Key.MiddleMouse: return 0x4;
                case Key.Mouse4:      return 0x5;
                case Key.Mouse5:      return 0x6;
                case Key.Shift:       return 0x10;
                case Key.Control:     return 0x11;
                case Key.Alt:         return 0x12;
            }
            return 0x0; // 0 indexes a bit that's never set. Equals, Apostrophe, and Grave are 0 in KbVkTable.
        }

        private static unsafe (int, uint) GetVk(Key key)
        {
            byte vk = (key > Key.MediaSelect) ? MetaKeyVk(key) : KbVkTable[(int)key];
            return (vk >> 5, 1u << (vk & 0x1F));
        }

        private static unsafe KeyboardState* KbState => (KeyboardState*)(Keyboard.Instance + 0x138);
        private static unsafe byte* KbVkTable => (byte*)(Keyboard.Instance + 0x38);

        internal static void UpdatePadState()
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

        internal static void UpdateMouseState()
        {
            prevMouseDown = mouseDown;
            mouseDown = Mouse.Get<uint>(0x58);
            mouseDelta = Mouse.Get<Point>(0x4C);
            mouseWheel = Mouse.Get<int>(0x54);
            MouseCaptured = false;
        }

        internal static unsafe void UpdateKeyboardState()
        {
            fixed (uint* lastPtr = keyDown, prevPtr = prevKeyDown)
            {
                MemoryUtil.Copy<uint>(lastPtr, prevPtr, 8);
                MemoryUtil.Copy<uint>(KbState->On, lastPtr, 8);
            }
            KeyboardCaptured = false;
        }

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
        private static Point mouseDelta;
        private static int mouseWheel;

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
