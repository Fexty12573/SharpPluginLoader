using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Models;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core;

/// <summary>
/// Represents an instance of uMhEpv. This class has the capability to
/// emit effects
/// </summary>
public class EffectEmitter : Unit
{
    public EffectEmitter(nint instance) : base(instance) { }
    public EffectEmitter() { }

    /// <summary>
    /// The EPV that this effect emitter is using. Updating this value
    /// will automatically call <see cref="Kill"/> and update the <see cref="Epvsp"/> property.
    /// </summary>
    public unsafe EffectProvider? EffectProvider
    {
        get => GetObject<EffectProvider>(0x260);
        set => SetEpvFunc.Invoke(Instance, value?.Instance ?? 0);
    }

    /// <summary>
    /// The model that this effect emitter is constrained to.
    /// </summary>
    public unsafe Model? ConstraintModel
    {
        get => GetObject<Model>(0x270);
        set => SetConstraintModelFunc.Invoke(Instance, value?.Instance ?? 0);
    }

    /// <summary>
    /// The EPV Group Id that this effect emitter is using.
    /// </summary>
    public ref int GroupId => ref GetRef<int>(0x268);

    /// <summary>
    /// The EPV Element Id that this effect emitter is using.
    /// </summary>
    public ref int ElementId => ref GetRef<int>(0x26C);

    /// <summary>
    /// Whether this effect emitter is playing. Do not modify this value,
    /// use the <see cref="Start"/> and <see cref="Finish"/> methods instead.
    /// </summary>
    public bool IsPlaying => GetRef<bool>(0x370);

    /// <summary>
    /// How the effect referenced by this emitter is played.
    /// Mostly unknown, 2 means the effect is looped infinitely.
    /// </summary>
    public ref int PlayType => ref GetRef<int>(0x374);

    /// <summary>
    /// The interval between each effect emission, if the effect is looped.
    /// (See <see cref="PlayType"/>)
    /// </summary>
    public ref float Interval => ref GetRef<float>(0x378);

    /// <summary>
    /// The EPVSP that this effect emitter is using. This is populated
    /// automatically when you set the EPV. (<see cref="EffectProvider"/>)
    /// </summary>
    public Resource? Epvsp => GetObject<Resource>(0xA00);

    /// <summary>
    /// Starts the emitter.
    /// </summary>
    public unsafe void Start() => StartFunction.Invoke(Instance);

    /// <summary>
    /// Ends the emitter.
    /// </summary>
    public unsafe void Finish() => FinishFunction.Invoke(Instance);

    /// <summary>
    /// Kills the emitter and all effects associated with it immediately.
    /// </summary>
    public unsafe void Kill() => KillFunction.Invoke(Instance);


    private NativeAction<nint> StartFunction => new(GetVirtualFunction(38));
    private NativeAction<nint> FinishFunction => new(GetVirtualFunction(37));
    private NativeAction<nint> KillFunction => new(GetVirtualFunction(25));

    private static readonly NativeAction<nint, nint> SetEpvFunc
        = new(AddressRepository.Get("EffectEmitter:SetEpv"));
    private static readonly NativeAction<nint, nint> SetConstraintModelFunc
        = new(AddressRepository.Get("EffectEmitter:SetConstraintModel"));
}
