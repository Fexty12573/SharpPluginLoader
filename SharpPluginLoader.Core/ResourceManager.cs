using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Resources;
using SharpPluginLoader.Core.Scripting;

namespace SharpPluginLoader.Core;

/// <summary>
/// Provides access to the game's resource manager (sMhResource)
/// </summary>
public static class ResourceManager
{
    /// <summary>
    /// The sMhResource singleton instance.
    /// </summary>
    public static MtObject SingletonInstance { get; private set; } = null!;

    /// <summary>
    /// Gets a resource from the game's resource manager.
    /// 
    /// If the resource is already loaded, it will return the existing instance.
    /// Otherwise, it will attempt to load the resource from the specified path.
    /// Files in nativePC take precedence over files in the chunks.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the resource to get. Use <see cref="Resource"/> if there is no dedicated type in the framework for the resource you're trying to load.
    /// </typeparam>
    /// <param name="path">The path to the file, relative to the chunk root and without the extension</param>
    /// <param name="dti">The dti of the file to load</param>
    /// <param name="flags">Flags to pass on to the function</param>
    /// <returns>The resource if it was found, or null</returns>
    public static unsafe T? GetResource<T>(string path, MtDti dti, LoadFlags flags = LoadFlags.Blocking) where T : Resource, new()
    {
        var resource = GetResourceFunc.Invoke(SingletonInstance.Instance, dti.Instance, path, flags);
        return resource == 0 ? null : new T { Instance = resource };
    }

    internal static void Initialize()
    {
        _getResourceHook = Hook.Create<GetResourceDelegate>(GetResourceFunc.NativePointer, GetResourceHook);

        // sMhResource gets special treatment because GetResource is called before the
        // constructor of sMhMain finishes, so we need to hook the sMhResource constructor directly
        // to obtain the singleton instance.
        _resourceManagerCtorHook = Hook.Create<CtorDelegate>(
            AddressRepository.Get("ResourceManager:Ctor"),
            (inst, unk1, unk2, unk3, unk4) =>
            {
                SingletonInstance = new MtObject(inst);
                return _resourceManagerCtorHook.Original(inst, unk1, unk2, unk3, unk4);
            }
        );
    }

    private static nint GetResourceHook(nint resourceMgr, nint dti, string path, LoadFlags flags)
    {
        var resource = _getResourceHook.Original(resourceMgr, dti, path, flags);
        var resObj = resource != 0 ? new Resource(resource) : null;
        var dtiObj = new MtDti(dti);

        foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnResourceLoad))
            plugin.OnResourceLoad(resObj, dtiObj, path, flags);

        ScriptContext.InvokeOnResourceLoad(resObj, dtiObj, path, flags);

        return resource;
    }

    private static Hook<GetResourceDelegate> _getResourceHook = null!;
    private static Hook<CtorDelegate> _resourceManagerCtorHook = null!;
    private static readonly NativeFunction<nint, nint, string, LoadFlags, nint> GetResourceFunc = new(AddressRepository.Get("ResourceManager:GetResource"));

    private delegate nint GetResourceDelegate(nint resourceMgr, nint dti, string path, LoadFlags flags);
    private delegate nint CtorDelegate(nint instance, nint unk1, uint unk2, nint unk3, int unk4);
}

/// <summary>
/// Flags to pass to the <see cref="ResourceManager.GetResource{T}"/> method.
/// </summary>
[Flags]
public enum LoadFlags
{
    /// <summary>
    /// No flags.
    /// </summary>
    None = 0x0,

    /// <summary>
    /// Load the resource in a blocking manner, i.e. wait for the resource to be loaded before returning.
    /// </summary>
    Blocking = 0x1,

    /// <summary>
    /// Load the resource asynchronously.
    /// </summary>
    Async = 0x2,

    /// <summary>
    /// Load the resource in the background. Exact behavior is unknown.
    /// </summary>
    Background = 0x4,

    /// <summary>
    /// Create the path if it doesn't exist.
    /// </summary>
    Create = 0x8,

    /// <summary>
    /// Only return the resource if it's already loaded, otherwise return null.
    /// Takes precedence over all other flags.
    /// </summary>
    NoLoad = 0x10,

    /// <summary>
    /// Load the resource from the nativePC folder.
    /// </summary>
    Stream = 0x20,

    /// <summary>
    /// Preload the resource. I.e. only create the resource object, but don't load the actual data.
    /// Incompatible with <see cref="Blocking"/> and <see cref="Async"/>. This operation is always synchronous.
    /// </summary>
    Preload = 0x40,

    /// <summary>
    /// Load with the lowest quality. Most likely used for textures.
    /// </summary>
    QualityLowest = 0x80,

    /// <summary>
    /// Load with low quality. Most likely used for textures.
    /// </summary>
    QualityLow = 0x100,

    /// <summary>
    /// Load with high quality. Most likely used for textures.
    /// </summary>
    QualityHigh = 0x200,

    /// <summary>
    /// Load with the highest quality. Most likely used for textures.
    /// </summary>
    QualityHighest = 0x400,

    /// <summary>
    /// Unknown flag, affects the behavior of file opening.
    /// </summary>
    Unknown = 0x800,

    /// <summary>
    /// Use the second load list. Exact behavior is unknown. Ignored unless <see cref="Async"/> is set.
    /// </summary>
    UseLoadList2 = 0x4000,
}
