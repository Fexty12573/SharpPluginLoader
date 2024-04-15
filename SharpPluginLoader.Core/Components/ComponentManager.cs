using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Components;

/// <summary>
/// Represents an instace of a cComponentManager.
/// </summary>
public class ComponentManager : MtObject
{
    public ComponentManager(nint instance) : base(instance) { }
    public ComponentManager() { }

    /// <summary>
    /// Finds a <see cref="Component"/> by its <see cref="MtDti"/>.
    /// </summary>
    /// <param name="dti">The <see cref="MtDti"/> of the component to find.</param>
    /// <param name="allowSubclass">Whether or not to allow subclasses of the specified DTI.</param>
    /// <returns>The <see cref="Component"/> if found, otherwise null.</returns>
    public unsafe Component? Find(MtDti dti, bool allowSubclass = false)
    {
        var component = FindComponentFunc.Invoke(Instance, dti.Instance, allowSubclass);
        return component == 0 ? null : new Component(component);
    }

    /// <inheritdoc cref="Find(MtDti,bool)"/>
    /// <param name="name">The name of the component to find.</param>
    public Component? Find(string name, bool allowSubclass = false)
    {
        var dti = MtDti.Find(name);
        return dti is null ? null : Find(dti, allowSubclass);
    }
        
    /// <summary>
    /// The root component of this component manager.
    /// </summary>
    public Component? Root => GetObject<Component>(0x8);


    private static readonly NativeFunction<nint, nint, bool, nint> FindComponentFunc =
        new(AddressRepository.Get("ComponentManager:Find"));
}
