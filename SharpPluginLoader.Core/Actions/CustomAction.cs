using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.Actions;


public delegate void OnActionInitialize(nint action);
public delegate void OnActionExecute(nint action);
[return: MarshalAs(UnmanagedType.I1)] public delegate bool OnActionUpdate(nint action);
public delegate void OnActionEnd(nint action);

/// <summary>
/// Declares the registration data of a <see cref="CustomAction"/>. Required on every
/// type passed to <see cref="ActionCloner.RegisterAction{T}"/>.
/// </summary>
/// <param name="name">The name of the action. Has no effect on functionality.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class CustomActionAttribute(string name) : Attribute
{
    /// <summary>
    /// The name of the action. Has no effect on functionality.
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// The flags of the action. -1 to inherit them from the base action.
    /// </summary>
    public int Flags { get; set; } = -1;

    /// <summary>
    /// The id of the action to use as a base for the custom action.
    /// Ignored if <see cref="BaseDti"/> is set.
    /// </summary>
    public int BaseActionId { get; set; } = -1;

    /// <summary>
    /// The name of the DTI of the action to use as a base for the custom action, e.g. "nActEm001::AdjustBite".
    /// Use this if the action you want to use as a base belongs to a different monster.
    /// If you set this, <see cref="BaseActionId"/> is ignored.
    /// </summary>
    public string? BaseDti { get; set; }
}

/// <summary>
/// The base class for custom monster actions. Inherit from it, decorate the type with
/// <see cref="CustomActionAttribute"/> and register it via <see cref="ActionCloner.RegisterAction{T}"/>.
/// </summary>
/// <remarks>
/// One instance of your type is created for every native action object, i.e. for every monster that
/// receives the action.
/// Do not create instances yourself, <see cref="ActionCloner"/> owns their lifetime.
/// </remarks>
public abstract class CustomAction : Action
{
    protected CustomAction() { }

    /// <summary>
    /// Called after the action object is created. The default implementation calls <see cref="ParentOnInitialize"/>.
    /// </summary>
    public override void OnInitialize() => ParentOnInitialize();

    /// <summary>
    /// Called each time the action is executed. The default implementation calls <see cref="ParentOnExecute"/>.
    /// </summary>
    public override void OnExecute() => ParentOnExecute();

    /// <summary>
    /// Called once per frame while the action is active. The default implementation calls <see cref="ParentOnUpdate"/>.
    /// </summary>
    /// <returns>False to end the action</returns>
    public override bool OnUpdate() => ParentOnUpdate();

    /// <summary>
    /// Called when the action ends. The default implementation calls <see cref="ParentOnEnd"/>.
    /// </summary>
    public override void OnEnd() => ParentOnEnd();

    /// <summary>
    /// Calls the implementation of the action this one is based on.
    /// </summary>
    protected unsafe void ParentOnInitialize() => Registration.ParentOnInitialize.Invoke(Instance);

    /// <inheritdoc cref="ParentOnInitialize"/>
    protected unsafe void ParentOnExecute() => Registration.ParentOnExecute.Invoke(Instance);

    /// <inheritdoc cref="ParentOnInitialize"/>
    protected unsafe bool ParentOnUpdate() => Registration.ParentOnUpdate.Invoke(Instance) != 0;

    /// <inheritdoc cref="ParentOnInitialize"/>
    protected unsafe void ParentOnEnd() => Registration.ParentOnEnd.Invoke(Instance);

    /// <summary>
    /// Calls the implementation of cActionBase itself, skipping the action this one is based on.
    /// </summary>
    protected unsafe void BaseOnInitialize() => ActionCloner.BaseOnInitialize.Invoke(Instance);

    /// <inheritdoc cref="BaseOnInitialize"/>
    protected unsafe void BaseOnExecute() => ActionCloner.BaseOnExecute.Invoke(Instance);

    /// <inheritdoc cref="BaseOnInitialize"/>
    protected unsafe bool BaseOnUpdate() => ActionCloner.BaseOnUpdate.Invoke(Instance) != 0;

    /// <inheritdoc cref="BaseOnInitialize"/>
    protected unsafe void BaseOnEnd() => ActionCloner.BaseOnEnd.Invoke(Instance);

    /// <summary>
    /// The registration this instance was created from. Holds the vtable and the base actions functions,
    /// all of which are shared by every instance of this action.
    /// </summary>
    internal ActionRegistration Registration { get; set; } = null!;
}
