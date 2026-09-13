
using SharpPluginLoader.Core.Entities;

namespace SharpPluginLoader.Core.Actions;

/// <summary>
/// Represents an instance of the cActionBase class.
/// </summary>
public class Action : MtObject
{
    public Action(nint instance) : base(instance) { }
    public Action() { }

    /// <summary>
    /// The amount of time the action has been active.
    /// </summary>
    public ref float ActiveTime => ref GetRef<float>(0x8);

    /// <summary>
    /// The actions delta time.
    /// </summary>
    public ref float DeltaSec => ref GetRef<float>(0xC);

    /// <summary>
    /// The flags of the action.
    /// </summary>
    public ref ulong Flags => ref GetRef<ulong>(0x10);

    /// <summary>
    /// The name of the action.
    /// </summary>
    public unsafe string Name => new(GetPtr<sbyte>(0x20));

    /// <summary>
    /// The entity that the action is attached to.
    /// </summary>
    public Entity? Parent => GetObject<Entity>(0x30);

    /// <summary>
    /// Called after the action object is created. Dispatches through the objects vtable.
    /// </summary>
    /// <remarks><see cref="CustomAction"/> overrides this to run managed code instead.</remarks>
    public virtual unsafe void OnInitialize() => ((delegate* unmanaged<nint, void>)GetVirtualFunction(5))(Instance);

    /// <summary>
    /// Called each time the action is executed. Dispatches through the objects vtable.
    /// </summary>
    /// <remarks><see cref="CustomAction"/> overrides this to run managed code instead.</remarks>
    public virtual unsafe void OnExecute() => ((delegate* unmanaged<nint, void>)GetVirtualFunction(6))(Instance);

    /// <summary>
    /// Called once per frame while the action is active. Dispatches through the objects vtable.
    /// </summary>
    /// <returns>False to end the action</returns>
    /// <remarks><see cref="CustomAction"/> overrides this to run managed code instead.</remarks>
    public virtual unsafe bool OnUpdate() => ((delegate* unmanaged<nint, byte>)GetVirtualFunction(7))(Instance) != 0;

    /// <summary>
    /// Called when the action ends. Dispatches through the objects vtable.
    /// </summary>
    /// <remarks><see cref="CustomAction"/> overrides this to run managed code instead.</remarks>
    public virtual unsafe void OnEnd() => ((delegate* unmanaged<nint, void>)GetVirtualFunction(8))(Instance);
}
