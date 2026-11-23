using System.Runtime.InteropServices;

namespace ImGuiNET
{
    public static unsafe class ImGuiPatches
    {
        public static ImGuiWindow* GetCurrentWindowRead()
        {
            return ImGuiNativePatches.igGetCurrentWindowRead();
        }

        public static void FocusWindow(ImGuiWindow* window, ImGuiFocusRequestFlags flags = 0)
        {
            ImGuiNativePatches.igFocusWindow(window, flags);
        }
    }

    public static unsafe class ImGuiNativePatches
    {
        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern ImGuiWindow* igGetCurrentWindowRead();

        [DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
        public static extern void igFocusWindow(ImGuiWindow* window, ImGuiFocusRequestFlags flags);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x480)]
    public unsafe struct ImGuiWindow;

    [Flags]
    public enum ImGuiFocusRequestFlags
    {
        None = 0,
        RestoreFocusedChild = 1 << 0,
        UnlessBelowModal = 1 << 1
    }
}
