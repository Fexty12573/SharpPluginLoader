
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

    public unsafe void OnInitialize() => ((delegate*<nint, void>)GetVirtualFunction(5))(Instance);

    public unsafe void OnExecute() => ((delegate*<nint, void>)GetVirtualFunction(6))(Instance);
    
    public unsafe bool OnUpdate() => ((delegate*<nint, byte>)GetVirtualFunction(7))(Instance) != 0;

    public unsafe void OnEnd() => ((delegate*<nint, void>)GetVirtualFunction(8))(Instance);
}
