using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core
{
    public delegate void DialogCallback(DialogResult result);

    public static class Gui
    {
        /// <summary>
        /// The singleton instance of the sMhGUI class.
        /// </summary>
        public static MtObject SingletonInstance => SingletonManager.GetSingleton("sMhGUI")!;

        /// <summary>
        /// Displays a popup message on the screen.
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="duration">The duration for which the popup should be displayed</param>
        /// <param name="delay">The delay before the popup is displayed</param>
        /// <param name="xOff">The horizontal offset from the center of the screen</param>
        /// <param name="yOff">The vertical offset from the center of the screen</param>
        public static unsafe void DisplayPopup(string message, TimeSpan duration, TimeSpan delay, float xOff = 0.0f, float yOff = 0.0f)
        {
            var durationSeconds = (float)duration.TotalSeconds;
            var delaySeconds = (float)delay.TotalSeconds;

            var messagePtr = Marshal.StringToHGlobalAnsi(message);
            CachedMessages.Enqueue(messagePtr);

            if (CachedMessages.Count > 30)
                Marshal.FreeHGlobal(CachedMessages.Dequeue());

            DisplayPopupFunc.Invoke(SingletonInstance.Instance, messagePtr, durationSeconds, delaySeconds, false, xOff, yOff);
        }

        /// <inheritdoc cref="DisplayPopup(string,TimeSpan,TimeSpan,float,float)"/>
        public static void DisplayPopup(string message, TimeSpan duration, float xOff = 0, float yOff = 0)
        {
            DisplayPopup(message, duration, TimeSpan.Zero, xOff, yOff);
        }

        /// <inheritdoc cref="DisplayPopup(string,TimeSpan,TimeSpan,float,float)"/>
        public static void DisplayPopup(string message, float xOff = 0, float yOff = 0)
        {
            DisplayPopup(message, TimeSpan.FromSeconds(3), xOff, yOff);
        }

        /// <summary>
        /// Displays a blue/purple message in the chat box.
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="delay">The delay before the message is displayed</param>
        /// <param name="isImportant">Whether the message should be displayed in purple</param>
        public static unsafe void DisplayMessage(string message, TimeSpan delay, bool isImportant = false)
        {
            var delaySeconds = (float)delay.TotalSeconds;
            DisplayMessageFunc.Invoke(SingletonManager.GetSingleton("sChat")!.Instance, message, delaySeconds, 0, isImportant);
        }

        /// <inheritdoc cref="DisplayMessage(string,TimeSpan,bool)"/>
        public static void DisplayMessage(string message, bool isImportant = false)
        {
            DisplayMessage(message, TimeSpan.Zero, isImportant);
        }

        /// <summary>
        /// Displays a prompt with a yes/no/cancel button. The callback will be called when the user clicks a button.
        /// Note: The user is responsible for keeping the callback alive.
        /// </summary>
        /// <param name="message">The message to display</param>
        /// <param name="callback">The callback to call when the user clicks a button</param>
        public static void DisplayYesNoDialog(string message, DialogCallback callback)
        {
            var msgPtr = Marshal.StringToHGlobalAnsi(message);
            DialogCallbacks.Enqueue(callback);
            CachedMessages.Enqueue(msgPtr);

            if (CachedMessages.Count > 30)
                Marshal.FreeHGlobal(CachedMessages.Dequeue());
            
            InternalCalls.QueueYesNoDialog(msgPtr);
        }

        public static unsafe void DisplayMessageWindow(string message, Vector2 offset = new())
        {
            var msgPtr = Marshal.StringToHGlobalAnsi(message);
            var offsetPtr = &offset;

            CachedMessages.Enqueue(msgPtr);
            if (CachedMessages.Count > 30)
                Marshal.FreeHGlobal(CachedMessages.Dequeue());

            DisplayMessageWindowFunc.Invoke(SingletonInstance.Instance, msgPtr, 0, (nint)offsetPtr, false);
        }

        public static unsafe void DisplayAlert(string message)
        {
            DisplayAlertFunc.Invoke(message);
        }

        [UnmanagedCallersOnly]
        private static unsafe void PropagateDialogResult(nint popup, nint unknown, DialogResult* result)
        {
            Debug.Assert(DialogCallbacks.Count > 0);

            var callback = DialogCallbacks.Dequeue();
            callback(*result);
        }

        private static void ChatMessageSentHook(string message)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnChatMessageSent))
                plugin.OnChatMessageSent(message);

            _chatMessageSentHook.Original(message);
        }

        internal static void Initialize()
        {
            _chatMessageSentHook = Hook.Create<ChatMessageSentDelegate>(AddressRepository.Get("Chat:MessageSent"), ChatMessageSentHook);
        }

        private static readonly Queue<DialogCallback> DialogCallbacks = new();
        private static readonly Queue<nint> CachedMessages = new();
        private static readonly NativeAction<nint, nint, float, float, bool, float, float> DisplayPopupFunc = new(AddressRepository.Get("Gui:DisplayPopup"));
        private static readonly NativeAction<nint, string, float, uint, bool> DisplayMessageFunc = new(AddressRepository.Get("Gui:DisplayMessage"));
        private static readonly NativeAction<nint, nint, nint, nint, bool> DisplayMessageWindowFunc = new(AddressRepository.Get("Gui:DisplayMessageWindow"));
        private static readonly NativeAction<string> DisplayAlertFunc = new(AddressRepository.Get("Gui:DisplayAlert"));
        private static Hook<ChatMessageSentDelegate> _chatMessageSentHook = null!;

        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode)]
        private delegate void ChatMessageSentDelegate(string message);
    }

    public enum DialogResult
    {
        Yes,
        No,
        Cancel
    }

    /// <summary>
    /// This class provides constants for the game's color codes.
    /// </summary>
    public static class TextColor
    {
#pragma warning disable CS1591
        public static string WhiteDefault => "<STYL MOJI_WHITE_DEFAULT>";
        public static string WhiteSelected => "<STYL MOJI_WHITE_SELECTED>";
        public static string WhiteSelected2 => "<STYL MOJI_WHITE_SELECTED2>";
        public static string WhiteDisable => "<STYL MOJI_WHITE_DISABLE>";
        public static string WhiteDefault2 => "<STYL MOJI_WHITE_DEFAULT2>";
        public static string BlackDefault => "<STYL MOJI_BLACK_DEFAULT>";
        public static string RedDefault => "<STYL MOJI_RED_DEFAULT>";
        public static string RedSelected => "<STYL MOJI_RED_SELECTED>";
        public static string RedSelected2 => "<STYL MOJI_RED_SELECTED2>";
        public static string RedDisable => "<STYL MOJI_RED_DISABLE>";
        public static string YellowDefault => "<STYL MOJI_YELLOW_DEFAULT>";
        public static string YellowSelected => "<STYL MOJI_YELLOW_SELECTED>";
        public static string YellowSelected2 => "<STYL MOJI_YELLOW_SELECTED2>";
        public static string YellowDisable => "<STYL MOJI_YELLOW_DISABLE>";
        public static string OrangeDefault => "<STYL MOJI_ORANGE_DEFAULT>";
        public static string OrangeSelected => "<STYL MOJI_ORANGE_SELECTED>";
        public static string OrangeSelected2 => "<STYL MOJI_ORANGE_SELECTED2>";
        public static string OrangeDisable => "<STYL MOJI_ORANGE_DISABLE>";
        public static string GreenDefault => "<STYL MOJI_GREEN_DEFAULT>";
        public static string GreenSelected => "<STYL MOJI_GREEN_SELECTED>";
        public static string GreenSelected2 => "<STYL MOJI_GREEN_SELECTED2>";
        public static string GreenDisable => "<STYL MOJI_GREEN_DISABLE>";
        public static string SlgreenDefault => "<STYL MOJI_SLGREEN_DEFAULT>";
        public static string SlgreenSelected => "<STYL MOJI_SLGREEN_SELECTED>";
        public static string SlgreenSelected2 => "<STYL MOJI_SLGREEN_SELECTED2>";
        public static string SlgreenDisable => "<STYL MOJI_SLGREEN_DISABLE>";
        public static string Slgreen2Default => "<STYL MOJI_SLGREEN2_DEFAULT>";
        public static string Slgreen2Selected => "<STYL MOJI_SLGREEN2_SELECTED>";
        public static string Slgreen2Selected2 => "<STYL MOJI_SLGREEN2_SELECTED2>";
        public static string Slgreen2Disable => "<STYL MOJI_SLGREEN2_DISABLE>";
        public static string LightblueDefault => "<STYL MOJI_LIGHTBLUE_DEFAULT>";
        public static string LightblueSelected2 => "<STYL MOJI_LIGHTBLUE_SELECTED2>";
        public static string LightblueSelected => "<STYL MOJI_LIGHTBLUE_SELECTED>";
        public static string LightblueDisable => "<STYL MOJI_LIGHTBLUE_DISABLE>";
        public static string Lightblue2Default => "<STYL MOJI_LIGHTBLUE2_DEFAULT>";
        public static string Lightblue2Selected2 => "<STYL MOJI_LIGHTBLUE2_SELECTED2>";
        public static string Lightblue2Selected => "<STYL MOJI_LIGHTBLUE2_SELECTED>";
        public static string Lightblue2Disable => "<STYL MOJI_LIGHTBLUE2_DISABLE>";
        public static string LightgreenDefault => "<STYL MOJI_LIGHTGREEN_DEFAULT>";
        public static string LightgreenSelected => "<STYL MOJI_LIGHTGREEN_SELECTED>";
        public static string LightgreenSelected2 => "<STYL MOJI_LIGHTGREEN_SELECTED2>";
        public static string LightgreenDisable => "<STYL MOJI_LIGHTGREEN_DISABLE>";
        public static string LightyellowDefault => "<STYL MOJI_LIGHTYELLOW_DEFAULT>";
        public static string LightyellowSelected => "<STYL MOJI_LIGHTYELLOW_SELECTED>";
        public static string LightyellowSelected2 => "<STYL MOJI_LIGHTYELLOW_SELECTED2>";
        public static string LightyellowDisable => "<STYL MOJI_LIGHTYELLOW_DISABLE>";
        public static string Lightyellow2Default => "<STYL MOJI_LIGHTYELLOW2_DEFAULT>";
        public static string Lightyellow2Selected => "<STYL MOJI_LIGHTYELLOW2_SELECTED>";
        public static string Lightyellow2Selected2 => "<STYL MOJI_LIGHTYELLOW2_SELECTED2>";
        public static string Lightyellow2Disable => "<STYL MOJI_LIGHTYELLOW2_DISABLE>";
        public static string BrownDefault => "<STYL MOJI_BROWN_DEFAULT>";
        public static string BrownSelected2 => "<STYL MOJI_BROWN_SELECTED2>";
        public static string BrownSelected => "<STYL MOJI_BROWN_SELECTED>";
        public static string BrownDisable => "<STYL MOJI_BROWN_DISABLE>";
        public static string Yellow2Default => "<STYL MOJI_YELLOW2_DEFAULT>";
        public static string Yellow2Selected => "<STYL MOJI_YELLOW2_SELECTED>";
        public static string Yellow2Selected2 => "<STYL MOJI_YELLOW2_SELECTED2>";
        public static string Yellow2Disable => "<STYL MOJI_YELLOW2_DISABLE>";
        public static string Orenge2Default => "<STYL MOJI_ORENGE2_DEFAULT>";
        public static string Orenge2Selected => "<STYL MOJI_ORENGE2_SELECTED>";
        public static string Orenge2Selected2 => "<STYL MOJI_ORENGE2_SELECTED2>";
        public static string Orenge2Disable => "<STYL MOJI_ORENGE2_DISABLE>";
        public static string PurpleDefault => "<STYL MOJI_PURPLE_DEFAULT>";
        public static string PurpleSelected => "<STYL MOJI_PURPLE_SELECTED>";
        public static string PurpleSelected2 => "<STYL MOJI_PURPLE_SELECTED2>";
        public static string PurpleDisable => "<STYL MOJI_PURPLE_DISABLE>";
        public static string Red2Default => "<STYL MOJI_RED2_DEFAULT>";
        public static string Red2Selected => "<STYL MOJI_RED2_SELECTED>";
        public static string Red2Selected2 => "<STYL MOJI_RED2_SELECTED2>";
        public static string Red2Disable => "<STYL MOJI_RED2_DISABLE>";
        public static string BlueDefault => "<STYL MOJI_BLUE_DEFAULT>";
        public static string BlueSelected => "<STYL MOJI_BLUE_SELECTED>";
        public static string BlueSelected2 => "<STYL MOJI_BLUE_SELECTED2>";
        public static string BlueDisable => "<STYL MOJI_BLUE_DISABLE>";
        public static string PaleblueDefault => "<STYL MOJI_PALEBLUE_DEFAULT>";
        public static string PaleblueSelected => "<STYL MOJI_PALEBLUE_SELECTED>";
        public static string PaleblueSelected2 => "<STYL MOJI_PALEBLUE_SELECTED2>";
        public static string PaleblueDisable => "<STYL MOJI_PALEBLUE_DISABLE>";
        public static string None => "</STYL>";
#pragma warning restore CS1591
    }
}
