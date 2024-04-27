
using System.Runtime.InteropServices;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core.Fsm.Weapon;

/// <summary>
/// Allows registering custom weapon FSM conditions.
/// </summary>
public static unsafe class FsmExtender
{
    /// <summary>
    /// Registers a custom FSM condition for all weapon types.
    /// </summary>
    /// <param name="name">The name of the condition. This is the string that you will use in the .fsm files.</param>
    /// <param name="evaluator">The delegate that will be called to evaluate the condition.</param>
    public static void RegisterCondition(string name, FsmConditionDelegate evaluator)
    {
        RegisterCondition(WeaponType.None, name, evaluator);
    }

    /// <summary>
    /// Registers a custom FSM condition for the specified weapon type.
    /// </summary>
    /// <param name="type">The weapon type to register the condition for.</param>
    /// <param name="name">The name of the condition. This is the string that you will use in the .fsm files.</param>
    /// <param name="evaluator">The delegate that will be called to evaluate the condition.</param>
    public static void RegisterCondition(WeaponType type, string name, FsmConditionDelegate evaluator)
    {
        if (!TransitionMap.TryGetValue(type, out var transitions))
        {
            throw new ArgumentException($"Invalid weapon type: {type}", nameof(type));
        }

        var vtable = NativeArray<nint>.Create(BaseTransitionVirtualFunctionCount);

        for (var i = 0; i < BaseTransitionVirtualFunctionCount; i++)
        {
            vtable[i] = _baseTransitionVtable[i];
        }

        var evaluatorPtr = Marshal.GetFunctionPointerForDelegate(evaluator);
        vtable[6] = evaluatorPtr;
        transitions[name] = new CustomTransition
        {
            Name = name,
            Evaluator = evaluator,
            EvaluatorFptr = evaluatorPtr,
            Id = -1,
            Type = type,
            Vtable = vtable
        };
    }

    private static void LoadTransitionSetHook(nint as3Ptr, nint player, int set, TransitionMapping* map, int mapSize)
    {
        if (set != 1)
        {
            _loadTransitionSetHook.Original(as3Ptr, player, set, map, mapSize);
            return;
        }

        // Dispose the previous custom transition map if it exists.
        _customTransitionMap.Dispose();

        var totalCount = TransitionMap.Sum(kv => kv.Value.Count);
        _customTransitionMap = NativeArray<TransitionMapping>.Create(mapSize + totalCount);

        var nextId = -1;
        for (var i = 0; i < mapSize; i++)
        {
            _customTransitionMap[i] = map[i];
            nextId = Math.Max(nextId, map[i].TransitionId);
        }

        nextId++;

        foreach (var condition in TransitionMap.Values.SelectMany(x => x.Values))
        {
            condition.Id = nextId++;
            _customTransitionMap[condition.Id] = new TransitionMapping()
            {
                TransitionId = condition.Id,
                Dti = _baseTransitionDti
            };
        }

        _loadTransitionSetHook.Original(as3Ptr, player, set, _customTransitionMap.Pointer, mapSize + totalCount);

        var as3 = (As3*)as3Ptr;

        // Actual type of Transitions is 'cTransitionBase**'.
        var transitions = (nint**)as3->TransitionSet1.Transitions;

        for (var i = 0; i < totalCount; i++)
        {
            var condition = TransitionMap.Values.SelectMany(x => x.Values).First(x => x.Id == i);
            *transitions[i] = condition.Vtable.Address;
        }
    }

    private static int GetConditionIdHook(MtString** pstr)
    {
        if (pstr == null || *pstr == null || (*pstr)->Length == 0)
        {
            return _getConditionIdHook.Original(pstr);
        }

        return TransitionMap[WeaponType.None].TryGetValue((*pstr)->GetString(), out var transition) 
            ? transition.Id
            : _getConditionIdHook.Original(pstr);
    }

    private static int GetConditionIdWpHook(WeaponType type, MtString** pstr)
    {
        if (pstr == null || *pstr == null || (*pstr)->Length == 0 || 
            !TransitionMap.TryGetValue(type, out var idMap)) // Should never happen
        {
            return _getConditionIdWpHook.Original(type, pstr);
        }

        return idMap.TryGetValue((*pstr)->GetString(), out var transition) 
            ? transition.Id
            : _getConditionIdWpHook.Original(type, pstr);
    }

    internal static void Initialize()
    {
        _loadTransitionSetHook = Hook.Create<LoadTransitionSetDelegate>(
            AddressRepository.Get("WeaponFsm:LoadTransitionSet"),
            LoadTransitionSetHook
        );

        _getConditionIdHook = Hook.Create<GetConditionIdDelegate>(
            AddressRepository.Get("WeaponFsm:GetConditionIdForName"),
            GetConditionIdHook
        );

        _getConditionIdWpHook = Hook.Create<GetConditionIdWpDelegate>(
            AddressRepository.Get("WeaponFsm:GetConditionIdForNameWp"),
            GetConditionIdWpHook
        );

        _baseTransitionDti = MtDti.Find("cHumanTransitionBase")!;
        Ensure.NotNull(_baseTransitionDti);

        var dummyObj = _baseTransitionDti.CreateInstance<MtObject>();
        _baseTransitionVtable = new NativeArray<nint>(
            dummyObj.Get<nint>(0),
            BaseTransitionVirtualFunctionCount
        );

        dummyObj.Destroy(true);
    }

    private static readonly Dictionary<WeaponType, Dictionary<string, CustomTransition>> TransitionMap = new()
    {
        { WeaponType.GreatSword, [] },
        { WeaponType.SwordAndShield, [] },
        { WeaponType.DualBlades, [] },
        { WeaponType.LongSword, [] },
        { WeaponType.Hammer, [] },
        { WeaponType.HuntingHorn, [] },
        { WeaponType.Lance, [] },
        { WeaponType.GunLance, [] },
        { WeaponType.SwitchAxe, [] },
        { WeaponType.ChargeBlade, [] },
        { WeaponType.InsectGlaive, [] },
        { WeaponType.Bow, [] },
        { WeaponType.LightBowgun, [] },
        { WeaponType.HeavyBowgun, [] },
        { WeaponType.None, [] },
    };

    private static Hook<LoadTransitionSetDelegate> _loadTransitionSetHook = null!;
    private static Hook<GetConditionIdDelegate> _getConditionIdHook = null!;
    private static Hook<GetConditionIdWpDelegate> _getConditionIdWpHook = null!;

    private static NativeArray<TransitionMapping> _customTransitionMap;

    private const int BaseTransitionVirtualFunctionCount = 9;
    private static MtDti _baseTransitionDti = null!;
    private static NativeArray<nint> _baseTransitionVtable;

    private delegate int GetConditionIdDelegate(MtString** pstr);
    private delegate int GetConditionIdWpDelegate(WeaponType type, MtString** pstr);
    private delegate void LoadTransitionSetDelegate(nint as3, nint player, int set, 
        TransitionMapping* map, int mapSize);
}


[StructLayout(LayoutKind.Explicit)]
file struct As3
{
    [FieldOffset(0xC0)] internal TransitionSet TransitionSet0;
    [FieldOffset(0xD0)] internal TransitionSet TransitionSet1;
    [FieldOffset(0xE0)] internal TransitionSet TransitionSet2;
    [FieldOffset(0xF0)] internal TransitionSet TransitionSet3;

    [StructLayout(LayoutKind.Explicit)]
    internal struct TransitionSet
    {
        [FieldOffset(0x00)] public nint Transitions;
        [FieldOffset(0x08)] public int TransitionCount;
    }
}

internal class CustomTransition
{
    public required string Name { get; init; }
    public required FsmConditionDelegate Evaluator { get; init; }
    public required nint EvaluatorFptr { get; init; }
    public required NativeArray<nint> Vtable { get; init; }
    public required int Id { get; set; }
    public required WeaponType Type { get; init; }
}
