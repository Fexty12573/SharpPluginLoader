
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.SaveData;
using SharpPluginLoader.Core.UI;

namespace SharpPluginLoader.Core.Savedata;

public class UserData : MtObject
{
    public UserData(nint instance) : base(instance) { }
    public UserData() { }

    /// <summary>
    /// Gets the user data associated with the given save slot.
    /// </summary>
    /// <param name="saveSlot">The index of the save slot (0, 1, 2)</param>
    /// <returns>The requested user data</returns>
    public static UserData GetUserData(int saveSlot)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(saveSlot, 3);
        var instance = SingletonInstance.Get<nint>(0xA8) + saveSlot * UserDataSize;
        return new UserData(instance);
    }

    /// <summary>
    /// Gets the user data of the currently loaded save slot.
    /// </summary>
    /// <returns>The user data of the current save slot</returns>
    /// <remarks>
    /// Behavior is undefined if no save slot has been picked yet.
    /// </remarks>
    public static UserData GetCurrentUserData()
    {
        var slot = SingletonInstance.Get<int>(0xA0);
        return GetUserData(slot);
    }

    public ref int Zenny => ref GetRef<int>(0x94);
    public ref int MasterRank => ref GetRef<int>(0xD4);

    public EquipmentBox EquipmentBox => GetInlineObject<EquipmentBox>(0x41068);
    public unsafe Span<Item> ItemPouch => new(GetPtrInline<Item>(0x38080), 24);
    public unsafe Span<Item> AmmoPouch => new(GetPtrInline<Item>(0x38200), 24);
    public unsafe Span<Item> MaterialPouch => new(GetPtrInline<Item>(0x38380), 24);
    public unsafe Span<Item> SpecialItems => new(GetPtrInline<Item>(0x38500), 5);

    public static int ItemSetCount => 120;
    public ItemSet GetItemSet(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, 120);
        return GetInlineObject<ItemSet>(0x738 + index * 0x768);
    }

    public int GetItemSetIndex(int index) => Get<byte>(0x37FF8 + index);

    public static int EquipSetCount => 224;
    public EquipSet GetEquipSet(int index)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, 224);
        return GetInlineObject<EquipSet>(0x10CBE8 + index * 0x2B0);
    }

    public unsafe Span<uint> GuidingLandsRegionPoints => new(GetPtrInline(0x269B30), 6);
    public unsafe Span<uint> GuidingLandsMaxAchievedLevels => new(GetPtrInline(0x269C10), 6);

    internal static void Initialize()
    {
        _selectSaveSlotHook = Hook.Create<SelectSaveSlotDelegate>(AddressRepository.Get("SaveData:SelectSlot"), (userData, slot) =>
        {
            _selectSaveSlotHook.Original(userData, slot);

            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnSelectSaveSlot))
                plugin.OnSelectSaveSlot(slot);

            StartMenu.ApplyOptions();
        });
    }

    private static MtObject SingletonInstance
    {
        get
        {
            _singletonInstance ??= SingletonManager.GetSingleton("sUserData");
            Ensure.NotNull(_singletonInstance);

            return _singletonInstance;
        }
    }

    private static int UserDataSize
    {
        get
        {
            if (_userDataSize == 0)
                _userDataSize = (int)(MtDti.Find("cUserData")?.Size ?? 0);

            return _userDataSize;
        }
    }

    private delegate void SelectSaveSlotDelegate(nint userData, int slot);
    private static MtObject? _singletonInstance;
    private static int _userDataSize;
    private static Hook<SelectSaveSlotDelegate> _selectSaveSlotHook = null!;
}
