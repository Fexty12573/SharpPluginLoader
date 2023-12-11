namespace SharpPluginLoader.Core.IO
{
    /// <summary>
    /// A hotkey, for either a keyboard or a controller.
    /// </summary>
    /// <typeparam name="T">The enum type</typeparam>
    /// <param name="key"></param>
    public readonly struct Keybind<T>(T key, params T[] modifiers) where T : Enum
    {
        public readonly T Key = key;
        public readonly T[] Modifiers = modifiers;
    }

    public static class KeyBindings
    {
        public static void AddKeybind(string name, Keybind<Button> keybind) => ControllerKeybinds[name] = keybind;
        public static void AddKeybind(string name, Keybind<Key> keybind) => KeyboardKeybinds[name] = keybind;

        public static bool IsPressed(string name)
        {
            if (ControllerKeybinds.TryGetValue(name, out var controllerKeybind))
                return Input.IsPressed(controllerKeybind.Key) && controllerKeybind.Modifiers.All(Input.IsDown);
            
            if (KeyboardKeybinds.TryGetValue(name, out var keyboardKeybind))
                return Input.IsPressed(keyboardKeybind.Key) && keyboardKeybind.Modifiers.All(Input.IsDown);

            return false;
        }

        private static readonly Dictionary<string, Keybind<Button>> ControllerKeybinds = [];
        private static readonly Dictionary<string, Keybind<Key>> KeyboardKeybinds = [];
    }
}
