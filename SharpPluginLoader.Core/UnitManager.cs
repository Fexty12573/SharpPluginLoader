using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core;

/// <summary>
/// The core unit manager of the game. See <see cref="Unit"/> for more information about units.
/// </summary>
public static class UnitManager
{
    public static MtObject? SingletonInstance => _singleton ??= SingletonManager.GetSingleton("sMhUnit");

    /// <summary>
    /// The number of lines in use.
    /// </summary>
    public static uint LineCount => SingletonInstance?.Get<uint>(0x3E38) ?? 0;

    /// <summary>
    /// The number of unit groups in use.
    /// </summary>
    public static uint UnitGroupCount => SingletonInstance?.Get<uint>(0x4A40) ?? 0;

    /// <summary>
    /// Gets a line by its index.
    /// </summary>
    /// <param name="index">The index of the line. Can be between 0 and <see cref="LineCount"/> - 1.</param>
    /// <returns>The line</returns>
    public static MoveLine? GetLine(int index)
    {
        if (index >= LineCount)
            return null;

        return MoveLines[index] ??= SingletonInstance?.GetInlineObject<MoveLine>(0x38 + index * 0xF8);
    }

    /// <summary>
    /// Creates a new instance of a unit.
    /// </summary>
    /// <typeparam name="T">The type of the unit.</typeparam>
    /// <param name="dti">The dti of the unit.</param>
    /// <returns>The new unit.</returns>
    /// <remarks>
    /// This function does not register the unit in a line. You must do that manually.
    /// 
    /// </remarks>
    public static T? NewUnit<T>(MtDti dti) where T : Unit
    {
        var instance = dti.CreateInstance<Unit>();
        if (instance.Instance == 0)
            return null;

        return instance as T;
    }

    /// <inheritdoc cref="NewUnit{T}(MtDti)"/>
    public static Unit? NewUnit(MtDti dti) => NewUnit<Unit>(dti);

    /// <summary>
    /// Registers at the top of a line.
    /// </summary>
    /// <param name="unit">The unit to register.</param>
    /// <param name="line">The line to register the unit in.</param>
    /// <param name="groupBit">The group bit of the unit.</param>
    /// <returns>True if the unit was registered, otherwise false.</returns>
    /// <remarks>
    /// Generally only monsters and players should be registered using this function.
    /// For other units, use <see cref="AddBottom"/>.
    /// </remarks>
    public static unsafe bool AddTop(Unit unit, int line, ulong groupBit)
    {
        if (SingletonInstance is null)
            return false;

        return AddTopFunc.Invoke(SingletonInstance.Instance, line, unit.Instance, groupBit);
    }

    /// <summary>
    /// Registers at the bottom of a line. Prefer this function over <see cref="AddTop"/> for non-monster and non-player units.
    /// </summary>
    /// <param name="unit">The unit to register.</param>
    /// <param name="line">The line to register the unit in.</param>
    /// <param name="groupBit">The group bit of the unit.</param>
    /// <returns>True if the unit was registered, otherwise false.</returns>
    public static unsafe bool AddBottom(Unit unit, int line, ulong groupBit)
    {
        if (SingletonInstance is null)
            return false;

        return AddBottomFunc.Invoke(SingletonInstance.Instance, line, unit.Instance, groupBit);
    }


    private static MtObject? _singleton;
    private static readonly MoveLine?[] MoveLines = new MoveLine?[64];

    private static readonly NativeFunction<nint, int, nint, ulong, bool> AddTopFunc = new(AddressRepository.Get("UnitManager:AddTop"));
    private static readonly NativeFunction<nint, int, nint, ulong, bool> AddBottomFunc = new(AddressRepository.Get("UnitManager:AddBottom"));
}

/// <summary>
/// Represents an instance of a sUnit::MoveLine.
/// </summary>
/// <remarks>
/// A 'line' is a list of units that are updated together. For example, all monsters in the game are in the same line.
/// Individual lines are updated in parallel.
/// </remarks>
public class MoveLine : MtObject
{
    public MoveLine(nint instance) : base(instance) { }
    public MoveLine() { }

    /// <summary>
    /// The first unit in the line.
    /// </summary>
    public Unit? Top => GetObject<Unit>(0x48);

    /// <summary>
    /// The last unit in the line.
    /// </summary>
    public Unit? Bottom => GetObject<Unit>(0x50);

    /// <summary>
    /// Gets all units in the line.
    /// </summary>
    public IEnumerable<Unit> Units
    {
        get
        {
            for (var unit = Top; unit is not null; unit = unit.Next)
                yield return unit;
        }
    }

    /// <summary>
    /// The name of the line.
    /// </summary>
    public unsafe string Name => new(GetPtr<sbyte>(0x58));

    /// <summary>
    /// The flags of the line.
    /// </summary>
    public ref ushort Flags => ref GetRef<ushort>(0x60);

    /// <summary>
    /// The number of units in the line.
    /// </summary>
    public ref ushort UnitCount => ref GetRef<ushort>(0x62);

    /// <summary>
    /// The delta time of the line.
    /// </summary>
    public ref float DeltaTime => ref GetRef<float>(0x64);

    /// <summary>
    /// A list of units, purpose is unknown.
    /// </summary>
    public MtArray<Unit> ExtraList => GetInlineObject<MtArray<Unit>>(0x78);

    /// <summary>
    /// The list of units that will be initialized during the next frame.
    /// </summary>
    public MtArray<Unit> SetupList => GetInlineObject<MtArray<Unit>>(0x98);

    /// <summary>
    /// The list of units that will be updated during the next frame.
    /// </summary>
    public MtArray<Unit> UnitList => GetInlineObject<MtArray<Unit>>(0xB8);

    /// <summary>
    /// A list of uGUI units, purpose is unknown.
    /// </summary>
    public MtArray<Unit> GuiList => GetInlineObject<MtArray<Unit>>(0xD8);
}
