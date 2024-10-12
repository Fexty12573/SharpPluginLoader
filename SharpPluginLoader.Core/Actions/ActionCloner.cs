using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Actions;


/// <summary>
/// Allows for the creation of custom actions for monsters.
/// </summary>
public class ActionCloner
{
    /// <summary>
    /// Registers a custom action for a monster.
    /// </summary>
    /// <param name="monster">The monster type to register the action for.</param>
    /// <param name="variant">The variant of the monster to register the action for.</param>
    /// <param name="action">The custom action to register.</param>
    public static void RegisterAction(MonsterType monster, uint variant, CustomAction action)
    {
        _instance.GetCustomActionList(monster, variant).Add(action);
        _instance.BuildVTable(monster, action);
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

        _baseOnInitialize = Marshal.GetDelegateForFunctionPointer<OnActionInitialize>(baseActionVft[5]);
        _baseOnExecute = Marshal.GetDelegateForFunctionPointer<OnActionExecute>(baseActionVft[6]);
        _baseOnUpdate = Marshal.GetDelegateForFunctionPointer<OnActionUpdate>(baseActionVft[7]);
        _baseOnEnd = Marshal.GetDelegateForFunctionPointer<OnActionEnd>(baseActionVft[8]);

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

        _stringsToFree[monster] = [];
        Dictionary<int, CustomAction> actionMap = [];
        using var newTable = NativeArray<ActionTableEntry>.Create(actionCount + list.Actions.Count);
        MemoryUtil.Copy(actionTable, newTable.Address, actionCount * sizeof(ActionTableEntry));

        var index = actionCount;
        foreach (var action in list.Actions)
        {
            if (action.BaseDti is not null)
            {
                newTable[index].Dti = action.BaseDti;
                newTable[index].Flags = action.Flags;
            }
            else
            {
                newTable[index].Dti = newTable[action.BaseActionId].Dti;
                newTable[index].Flags = newTable[action.BaseActionId].Flags;
            }

            newTable[index].Name = action.Name ?? $"ACTION_{monster.Type}_{index}";
            newTable[index].Id = index;

            actionMap[index] = action;
            _stringsToFree[monster].Add(newTable[index].NamePtr);
            index++;
        }

        // Call the original function to create the action set
        _hook.Original(instance, set, newTable.Address, newTable.Length, controller);
        var actionController = monster.ActionController;
        var actionList = actionController.GetActionList(set);

        // Update the action list with the custom vtables
        foreach (var (idx, customAction) in actionMap)
        {
            var action = actionList[idx];
            if (action is null)
            {
                Log.Warn($"Failed to overwrite VTable for action {customAction.Name}, action was not created properly.");
                continue;
            }

            action.GetRef<nint>(0x0) = customAction.VTable.Address;
        }

        _applyActionParam.Invoke(instance);
    }

    private unsafe void BuildVTable(MonsterType monster, CustomAction action)
    {
        var dti = action.BaseDti;
        if (dti is null)
        {
            // Fixed length because we won't be going out of bounds anyway unless the user made a mistake, in which case
            // crashing is the desired behavior
            var actionTable = new NativeArray<ActionTableEntry>(_getActionTable.Invoke(monster), 400);
            dti = actionTable[action.BaseActionId].Dti;

            if (dti is null)
            {
                throw new InvalidOperationException($"No base dti found for action {action.BaseActionId}");
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

        action.ParentOnInitialize = Marshal.GetDelegateForFunctionPointer<OnActionInitialize>(dummyBaseAction.GetVirtualFunction(5));
        action.ParentOnExecute = Marshal.GetDelegateForFunctionPointer<OnActionExecute>(dummyBaseAction.GetVirtualFunction(6));
        action.ParentOnUpdate = Marshal.GetDelegateForFunctionPointer<OnActionUpdate>(dummyBaseAction.GetVirtualFunction(7));
        action.ParentOnEnd = Marshal.GetDelegateForFunctionPointer<OnActionEnd>(dummyBaseAction.GetVirtualFunction(8));

        action.OnInitializeWrapper = action.OnInitialize is null ? null : instance =>
        {
            action.OnInitialize(new Action(instance), action.ParentOnInitialize, _baseOnInitialize);
        };

        action.OnExecuteWrapper = action.OnExecute is null ? null : instance =>
        {
            action.OnExecute(new Action(instance), action.ParentOnExecute, _baseOnExecute);
        };

        action.OnUpdateWrapper = action.OnUpdate is null ? null : instance =>
        {
            return action.OnUpdate(new Action(instance), action.ParentOnUpdate, _baseOnUpdate);
        };

        action.OnEndWrapper = action.OnEnd is null ? null : instance =>
        {
            action.OnEnd(new Action(instance), action.ParentOnEnd, _baseOnEnd);
        };

        // Override action functions
        vtable[5] = action.OnInitialize != null 
            ? Marshal.GetFunctionPointerForDelegate(action.OnInitializeWrapper!)
            : dummyBaseAction.GetVirtualFunction(5);

        vtable[6] = action.OnExecute != null
            ? Marshal.GetFunctionPointerForDelegate(action.OnExecuteWrapper!)
            : dummyBaseAction.GetVirtualFunction(6);

        vtable[7] = action.OnUpdate != null
            ? Marshal.GetFunctionPointerForDelegate(action.OnUpdateWrapper!)
            : dummyBaseAction.GetVirtualFunction(7);

        vtable[8] = action.OnEnd != null
            ? Marshal.GetFunctionPointerForDelegate(action.OnEndWrapper!)
            : dummyBaseAction.GetVirtualFunction(8);

        // Explicit user overrides
        foreach (var (index, func) in action.VirtualFunctions)
        {
            vtable[index] = Marshal.GetFunctionPointerForDelegate(func);
        }

        action.VTable = vtable;
        dummyBaseAction.Destroy(true);
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

    internal static unsafe void OnMonsterDestroy(Monster monster)
    {
        if (_instance._stringsToFree.TryGetValue(monster, out var strings))
        {
            foreach (var ptr in strings)
            {
                Utf8StringMarshaller.Free((byte*)ptr);
            }

            _instance._stringsToFree.Remove(monster);
        }
    }

    internal static void Initialize()
    {
        _instance = new ActionCloner();
    }

    private static ActionCloner _instance = null!;

    private delegate void SetActionSetDelegate(nint instance, int set, nint actionTable, int actionCount, int controller);
    private readonly Dictionary<uint, int> _vtableSizes;
    private readonly List<CustomActionList> _customActions = [];
    private readonly Dictionary<Monster, List<nint>> _stringsToFree = [];
    private readonly NativeFunction<MonsterType, nint> _getActionTable;
    private readonly NativeAction<nint> _applyActionParam;
    private readonly Patch _patch;
    private readonly Hook<SetActionSetDelegate> _hook;

    private readonly OnActionInitialize _baseOnInitialize;
    private readonly OnActionExecute _baseOnExecute;
    private readonly OnActionUpdate _baseOnUpdate;
    private readonly OnActionEnd _baseOnEnd;
}

internal class CustomActionList(MonsterType type, uint variant)
{
    public MonsterType Monster { get; set; } = type;
    public uint Variant { get; set; } = variant;
    public List<CustomAction> Actions { get; } = [];

    public void Add(CustomAction action)
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
