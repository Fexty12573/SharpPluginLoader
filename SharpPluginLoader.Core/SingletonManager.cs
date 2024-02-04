
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core;

/// <summary>
/// Provides access to the game's singletons. (Anything that inherits from cSystem)
/// </summary>
public static class SingletonManager
{
    /// <summary>
    /// Gets a singleton by name.
    /// </summary>
    /// <param name="name">The name of the singleton</param>
    /// <returns>The singleton, or null if it doesn't exist</returns>
    /// <remarks>This method <b>can't</b> be called from <see cref="IPlugin.OnPreMain"/>, <see cref="IPlugin.OnWinMain"/>
    /// or <see cref="IPlugin.Initialize"/>, as the singletons aren't initialized yet at that point.
    /// </remarks>
    public static MtObject? GetSingleton(string name)
    {
        if (!_initialized)
        {
            Log.Warn("Singletons not initialized");
            return null;
        }

        if (!Singletons.TryGetValue(name, out var value))
        {
            Log.Warn($"Unknown Singleton '{name}'");
            return null;
        }

        return value;
    }

    /// <inheritdoc cref="GetSingleton(string)"/>
    /// <param name="dti">The DTI of the singleton</param>
    public static MtObject? GetSingleton(MtDti dti) => GetSingleton(dti.Name);

    internal static void MapSingletons()
    {
        if (_initialized) 
            return;

        foreach (var singleton in SingletonList)
        {
            var dti = singleton.GetDti();
            if (dti is null)
            {
                Log.Debug($"Singleton with no DTI found at 0x{singleton.Instance:X}");
                continue;
            }

            Singletons[dti.Name] = singleton;
        }

        SingletonList.Clear();
        _initialized = true;
    }

    internal static void Initialize()
    {
        _systemCtorHook = Hook.Create<SystemCtorDelegate>(AddressRepository.Get("cSystem:Ctor"), instance =>
        {
            SingletonList.Add(new MtObject(instance));
            return _systemCtorHook.Original(instance);
        });
    }

    private static readonly HashSet<MtObject> SingletonList = [];
    private static readonly Dictionary<string, MtObject> Singletons = [];
    private static bool _initialized = false;
    private static Hook<SystemCtorDelegate> _systemCtorHook = null!;

    private delegate nint SystemCtorDelegate(nint instance);
}
