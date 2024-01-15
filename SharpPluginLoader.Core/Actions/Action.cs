
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
}
