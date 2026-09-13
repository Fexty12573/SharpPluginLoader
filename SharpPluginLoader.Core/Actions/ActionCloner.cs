using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Memory;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;

namespace SharpPluginLoader.Core.Actions;


/// <summary>
/// Allows for the creation of custom actions for monsters.
/// </summary>
public class ActionCloner
{
    /// <summary>
    /// Registers a custom action for a monster.
    /// </summary>
    /// <typeparam name="T">
    /// The <see cref="CustomAction"/> type to register. It must carry a <see cref="CustomActionAttribute"/>
    /// and have a public parameterless constructor.
    /// </typeparam>
    /// <param name="monster">The monster type to register the action for.</param>
    /// <param name="variant">The variant of the monster to register the action for.</param>
    /// <param name="virtualFunctions">
    /// Additional virtual functions to override, keyed by their index in the vtable. Only needed for
    /// functions that <see cref="CustomAction"/> does not already expose. The delegates must be kept
    /// alive by the caller for as long as the action is registered.
    /// </param>
    /// <remarks>
    /// One instance of <typeparamref name="T"/> is created per monster that receives the action, so
    /// instance state is never shared between monsters.
    /// </remarks>
    public static void RegisterAction<T>(MonsterType monster, uint variant,
        IReadOnlyDictionary<int, Delegate>? virtualFunctions = null) where T : CustomAction, new()
    {
        var attribute = typeof(T).GetCustomAttribute<CustomActionAttribute>()
            ?? throw new InvalidOperationException($"{typeof(T).Name} is missing a [CustomAction] attribute");

        MtDti? baseDti = null;
        if (attribute.BaseDti is not null)
        {
            baseDti = MtDti.Find(attribute.BaseDti)
                ?? throw new InvalidOperationException(
                    $"No DTI named {attribute.BaseDti} found (required by {typeof(T).Name})");
        }
        else if (attribute.BaseActionId < 0)
        {
            throw new InvalidOperationException(
                $"{typeof(T).Name} must specify either BaseActionId or BaseDti on its [CustomAction] attribute");
        }

        var registration = new ActionRegistration
        {
            Name = attribute.Name,
            Flags = attribute.Flags,
            BaseActionId = attribute.BaseActionId,
            BaseDti = baseDti,
            Factory = instance => new T { Instance = instance },
            HasOnInitialize = IsOverridden(typeof(T), nameof(CustomAction.OnInitialize)),
            HasOnExecute = IsOverridden(typeof(T), nameof(CustomAction.OnExecute)),
            HasOnUpdate = IsOverridden(typeof(T), nameof(CustomAction.OnUpdate)),
            HasOnEnd = IsOverridden(typeof(T), nameof(CustomAction.OnEnd))
        };

        if (virtualFunctions is not null)
        {
            foreach (var (index, func) in virtualFunctions)
                registration.VirtualFunctions[index] = func;
        }

        _instance.GetCustomActionList(monster, variant).Add(registration);
        _instance.BuildVTable(monster, registration);
    }

    /// <summary>
    /// Checks whether <paramref name="type"/> provides its own implementation of one of the
    /// <see cref="CustomAction"/> hooks, rather than inheriting the default that forwards to the parent action.
    /// </summary>
    private static bool IsOverridden(Type type, string name)
    {
        var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance);
        return method is not null && method.DeclaringType != typeof(CustomAction);
    }

    private unsafe ActionCloner()
    {
        _vtableSizes = [];
        var chunk = InternalCalls.GetDefaultChunk();
        var vtableSizesFile = InternalCalls.ChunkGetFile(chunk, "/Resources/VTableSizes.bin");
        var vtableSizesData = InternalCalls.FileGetContents(vtableSizesFile);

        var count = *(int*)vtableSizesData;
        var entries = (VTableEntry*)(vtableSizesData + sizeof(int));
        for (var i = 0; i < count; i++)
        {
            _vtableSizes[entries[i].DtiId] = entries[i].Size;

            var dti = MtDti.Find(entries[i].DtiId);
            if ((dti?.InheritsFrom("cEmAction") ?? false) &&
                entries[i].Size < 9)
            {
                Log.Error($"VTable for {dti.Name} has less than 9 entries");
            }
        }

        // Create hooks and patches
        var getActionTableCall = AddressRepository.Get("Monster:GetActionTableCall");
        _getActionTable = new NativeFunction<MonsterType, nint>(GetRel32Address(getActionTableCall));
        var setActionSetFixup = AddressRepository.Get("Entity:SetActionSetFixup");
        _applyActionParam = new NativeAction<nint>(GetRel32Address(setActionSetFixup));
        _patch = new Patch(setActionSetFixup, [0xC3, 0xCC, 0xCC, 0xCC, 0xCC], true);
        _hook = Hook.Create<SetActionSetDelegate>(AddressRepository.Get("Entity:SetActionSet"), SetActionSetHook);

        var vtableAssignment = AddressRepository.Get("EmAction:VTableAssignment");
        var baseActionVft = (nint*)(MemoryUtil.Read<int>(vtableAssignment) + vtableAssignment + 4);

        BaseOnInitialize = new NativeAction<nint>(baseActionVft[5]);
        BaseOnExecute = new NativeAction<nint>(baseActionVft[6]);
        BaseOnUpdate = new NativeFunction<nint, byte>(baseActionVft[7]);
        BaseOnEnd = new NativeAction<nint>(baseActionVft[8]);

        return;

        // Resolves the destination address of a relative call/jmp instruction
        static nint GetRel32Address(nint address)
        {
            var offset = MemoryUtil.Read<int>(address + 1);
            return address + 5 + offset;
        }
    }

    private unsafe void SetActionSetHook(nint instance, int set, nint actionTable, int actionCount, int controller)
    {
        var entity = new Entity(instance);
        if (!entity.Is("uEnemy") || set != 1) // Only hook the main action set
        {
            _hook.Original(instance, set, actionTable, actionCount, controller);
            _applyActionParam.Invoke(instance);
            return;
        }

        var monster = entity.As<Monster>();

        var list = GetCustomActionList(monster.Type, monster.Variant);
        if (list.Actions.Count == 0 || actionTable != _getActionTable.Invoke(list.Monster))
        {
            _hook.Original(instance, set, actionTable, actionCount, controller);
            _applyActionParam.Invoke(instance);
            return;
        }

        // The action set can be built more than once for the same monster. The managed actions from a
        // previous pass refer to native objects that are about to be replaced, so drop them.
        ReleaseActions(monster);

        // The name strings are not freed here: the action objects from the previous pass hold pointers
        // to them and are only torn down by the call to the original function further below. They stay
        // alive until the monster is destroyed.
        if (!_stringsToFree.TryGetValue(monster, out var strings))
            _stringsToFree[monster] = strings = [];

        List<nint> actionInstances = [];
        _actionsByMonster[monster] = actionInstances;

        Dictionary<int, ActionRegistration> actionMap = [];
        using var newTable = NativeArray<ActionTableEntry>.Create(actionCount + list.Actions.Count);
        MemoryUtil.Copy(actionTable, newTable.Address, actionCount * sizeof(ActionTableEntry));

        var index = actionCount;
        foreach (var registration in list.Actions)
        {
            if (registration.BaseDti is not null)
            {
                newTable[index].Dti = registration.BaseDti;
                newTable[index].Flags = registration.Flags >= 0 ? registration.Flags : 0;
            }
            else
            {
                newTable[index].Dti = newTable[registration.BaseActionId].Dti;
                newTable[index].Flags = registration.Flags >= 0
                    ? registration.Flags
                    : newTable[registration.BaseActionId].Flags;
            }

            newTable[index].Name = string.IsNullOrEmpty(registration.Name)
                ? $"ACTION_{monster.Type}_{index}"
                : registration.Name;
            newTable[index].Id = index;

            actionMap[index] = registration;
            strings.Add(newTable[index].NamePtr);

            Log.Debug($"[{monster.Name}] Action {newTable[index].Name} registered with ID {index}");

            index++;
        }

        // Call the original function to create the action set
        _hook.Original(instance, set, newTable.Address, newTable.Length, controller);
        var actionController = monster.ActionController;
        var actionList = actionController.GetActionList(set);

        // Update the action list with the custom vtables, and give every native action object
        // its own managed instance to dispatch to
        foreach (var (idx, registration) in actionMap)
        {
            var action = actionList[idx];
            if (action is null)
            {
                Log.Warn($"Failed to overwrite VTable for action {registration.Name}, action was not created properly.");
                continue;
            }

            var customAction = registration.Factory(action.Instance);
            customAction.Registration = registration;

            _actions[action.Instance] = customAction;
            actionInstances.Add(action.Instance);

            action.GetRef<nint>(0x0) = registration.VTable.Address;
        }

        _applyActionParam.Invoke(instance);
    }

    private unsafe void BuildVTable(MonsterType monster, ActionRegistration registration)
    {
        var dti = registration.BaseDti;
        if (dti is null)
        {
            // Fixed length because we won't be going out of bounds anyway unless the user made a mistake, in which case
            // crashing is the desired behavior
            var actionTable = new NativeArray<ActionTableEntry>(_getActionTable.Invoke(monster), 400);
            dti = actionTable[registration.BaseActionId].Dti;

            if (dti is null)
            {
                throw new InvalidOperationException($"No base dti found for action {registration.BaseActionId}");
            }
        }

        var vtable = NativeArray<nint>.Create(GetVTableSize(dti));
        Ensure.IsTrue(vtable.Length >= 9); // Minimum size for action vtables

        var dummyBaseAction = dti.CreateInstance<Action>();
        if (dummyBaseAction is null)
        {
            throw new InvalidOperationException($"Failed to create dummy base action for {dti}");
        }

        // Copy the vtable from the base action
        for (var i = 0; i < vtable.Length; i++)
        {
            vtable[i] = dummyBaseAction.GetVirtualFunction(i);
        }

        registration.ParentOnInitialize = new NativeAction<nint>(vtable[5]);
        registration.ParentOnExecute = new NativeAction<nint>(vtable[6]);
        registration.ParentOnUpdate = new NativeFunction<nint, byte>(vtable[7]);
        registration.ParentOnEnd = new NativeAction<nint>(vtable[8]);

        // Only redirect the slots the action actually implements. The rest keep pointing at the base.
        if (registration.HasOnInitialize)
        {
            registration.OnInitializeWrapper = nativeAction =>
            {
                var action = Resolve(nativeAction, registration);
                if (action is null)
                {
                    registration.ParentOnInitialize.Invoke(nativeAction);
                    return;
                }

                try
                {
                    action.OnInitialize();
                }
                catch (Exception e)
                {
                    LogCallbackException(registration, nameof(CustomAction.OnInitialize), e);
                }
            };

            vtable[5] = Marshal.GetFunctionPointerForDelegate(registration.OnInitializeWrapper);
        }

        if (registration.HasOnExecute)
        {
            registration.OnExecuteWrapper = nativeAction =>
            {
                var action = Resolve(nativeAction, registration);
                if (action is null)
                {
                    registration.ParentOnExecute.Invoke(nativeAction);
                    return;
                }

                try
                {
                    action.OnExecute();
                }
                catch (Exception e)
                {
                    LogCallbackException(registration, nameof(CustomAction.OnExecute), e);
                }
            };

            vtable[6] = Marshal.GetFunctionPointerForDelegate(registration.OnExecuteWrapper);
        }

        if (registration.HasOnUpdate)
        {
            registration.OnUpdateWrapper = nativeAction =>
            {
                var action = Resolve(nativeAction, registration);
                if (action is null)
                    return registration.ParentOnUpdate.Invoke(nativeAction) != 0;

                try
                {
                    return action.OnUpdate();
                }
                catch (Exception e)
                {
                    LogCallbackException(registration, nameof(CustomAction.OnUpdate), e);
                    return true; // Keep the action alive
                }
            };

            vtable[7] = Marshal.GetFunctionPointerForDelegate(registration.OnUpdateWrapper);
        }

        if (registration.HasOnEnd)
        {
            registration.OnEndWrapper = nativeAction =>
            {
                var action = Resolve(nativeAction, registration);
                if (action is null)
                {
                    registration.ParentOnEnd.Invoke(nativeAction);
                    return;
                }

                try
                {
                    action.OnEnd();
                }
                catch (Exception e)
                {
                    LogCallbackException(registration, nameof(CustomAction.OnEnd), e);
                }
            };

            vtable[8] = Marshal.GetFunctionPointerForDelegate(registration.OnEndWrapper);
        }

        // Explicit user overrides
        foreach (var (index, func) in registration.VirtualFunctions)
        {
            vtable[index] = Marshal.GetFunctionPointerForDelegate(func);
        }

        registration.VTable = vtable;
        dummyBaseAction.Destroy(true);
    }

    /// <summary>
    /// Maps a native action object back to the managed <see cref="CustomAction"/> driving it.
    /// </summary>
    /// <remarks>
    /// Every action that uses one of our vtables gets its managed counterpart created in
    /// <see cref="SetActionSetHook"/>, so a miss means something went wrong. Callers fall back to
    /// the behavior of the base action in that case.
    /// </remarks>
    private CustomAction? Resolve(nint nativeAction, ActionRegistration registration)
    {
        if (_actions.TryGetValue(nativeAction, out var action))
            return action;

        if (!registration.WarnedAboutUntrackedInstance)
        {
            registration.WarnedAboutUntrackedInstance = true;
            Log.Error($"No managed instance for action {registration.Name} at 0x{nativeAction:X}, falling back " +
                      $"to the base action. This is only logged once per registered action.");
        }

        return null;
    }

    private static void LogCallbackException(ActionRegistration registration, string callback, Exception e)
    {
        Log.Error($"Unhandled exception in {callback} of action {registration.Name}: {e}");
    }

    private CustomActionList GetCustomActionList(MonsterType monster, uint variant)
    {
        var list = _customActions.FirstOrDefault(x => x.Monster == monster && x.Variant == variant);
        if (list is null)
        {
            list = new CustomActionList(monster, variant);
            _customActions.Add(list);
        }

        return list;
    }

    private int GetVTableSize(MtDti dti)
    {
        if (!_vtableSizes.TryGetValue(dti.Id, out var size))
        {
            throw new InvalidOperationException($"No vtable size found for type {dti}");
        }

        return size;
    }

    /// <summary>
    /// Drops the managed actions of a monster. Called both when the monster is destroyed and when its
    /// action set is rebuilt, since either one invalidates the native action objects behind them.
    /// </summary>
    private void ReleaseActions(Monster monster)
    {
        if (!_actionsByMonster.Remove(monster, out var actions))
            return;

        foreach (var ptr in actions)
        {
            _actions.Remove(ptr);
        }
    }

    internal static unsafe void OnMonsterDestroy(Monster monster)
    {
        if (_instance._stringsToFree.Remove(monster, out var strings))
        {
            foreach (var ptr in strings)
            {
                Utf8StringMarshaller.Free((byte*)ptr);
            }
        }

        _instance.ReleaseActions(monster);
    }

    internal static void Initialize()
    {
        _instance = new ActionCloner();
    }

    /// <summary>
    /// The cActionBase implementations, shared by every custom action regardless of what it is based on.
    /// </summary>
    internal static NativeAction<nint> BaseOnInitialize { get; private set; }
    internal static NativeAction<nint> BaseOnExecute { get; private set; }
    internal static NativeFunction<nint, byte> BaseOnUpdate { get; private set; }
    internal static NativeAction<nint> BaseOnEnd { get; private set; }

    private static ActionCloner _instance = null!;

    private delegate void SetActionSetDelegate(nint instance, int set, nint actionTable, int actionCount, int controller);
    private readonly Dictionary<uint, int> _vtableSizes;
    private readonly List<CustomActionList> _customActions = [];
    private readonly Dictionary<Monster, List<nint>> _stringsToFree = [];

    // Native action object -> the managed instance driving it, plus the reverse index used to clean it up
    private readonly Dictionary<nint, CustomAction> _actions = [];
    private readonly Dictionary<Monster, List<nint>> _actionsByMonster = [];

    private readonly NativeFunction<MonsterType, nint> _getActionTable;
    private readonly NativeAction<nint> _applyActionParam;
    private readonly Patch _patch;
    private readonly Hook<SetActionSetDelegate> _hook;
}

/// <summary>
/// The per-registration state of a custom action. Everything in here is shared by every instance of a
/// single <see cref="CustomAction"/> type registered for one monster.
/// </summary>
internal sealed class ActionRegistration
{
    public required string Name { get; init; }
    public required int Flags { get; init; }
    public required int BaseActionId { get; init; }
    public required MtDti? BaseDti { get; init; }

    /// <summary>
    /// Creates a managed action bound to the given native action object.
    /// </summary>
    public required Func<nint, CustomAction> Factory { get; init; }

    public required bool HasOnInitialize { get; init; }
    public required bool HasOnExecute { get; init; }
    public required bool HasOnUpdate { get; init; }
    public required bool HasOnEnd { get; init; }

    /// <summary>
    /// Extra virtual functions to override, keyed by their index in the vtable.
    /// </summary>
    public Dictionary<int, Delegate> VirtualFunctions { get; } = [];

    public NativeArray<nint> VTable;

    // The implementations of the action this one is based on
    public NativeAction<nint> ParentOnInitialize;
    public NativeAction<nint> ParentOnExecute;
    public NativeFunction<nint, byte> ParentOnUpdate;
    public NativeAction<nint> ParentOnEnd;

    // Rooted here so the reverse P/Invoke stubs the vtable points at stay alive
    public OnActionInitialize? OnInitializeWrapper;
    public OnActionExecute? OnExecuteWrapper;
    public OnActionUpdate? OnUpdateWrapper;
    public OnActionEnd? OnEndWrapper;

    public bool WarnedAboutUntrackedInstance;

    ~ActionRegistration()
    {
        if (VTable.Address != 0)
        {
            VTable.Dispose();
        }
    }
}

internal class CustomActionList(MonsterType type, uint variant)
{
    public MonsterType Monster { get; set; } = type;
    public uint Variant { get; set; } = variant;
    public List<ActionRegistration> Actions { get; } = [];

    public void Add(ActionRegistration action)
    {
        Actions.Add(action);
    }
}

[StructLayout(LayoutKind.Sequential, Size = 0x6)]
file readonly struct VTableEntry
{
    public readonly uint DtiId;
    public readonly short Size;
}

[StructLayout(LayoutKind.Explicit, Size = 0x20)]
file unsafe struct ActionTableEntry
{
    [FieldOffset(0x00)] public int Id;
    [FieldOffset(0x08)] private nint _dti;
    [FieldOffset(0x10)] public int Flags;
    [FieldOffset(0x18)] private sbyte* _name;

    public MtDti? Dti
    {
        get => _dti == 0 ? null : new MtDti(_dti);
        set => _dti = value?.Instance ?? 0;
    }

    public string Name
    {
        get => _name == null ? string.Empty : new string(_name);
        set => _name = (sbyte*)Utf8StringMarshaller.ConvertToUnmanaged(value);
    }

    public nint NamePtr => (nint)_name;
}
