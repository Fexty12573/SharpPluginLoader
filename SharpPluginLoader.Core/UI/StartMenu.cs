
using System.Runtime.InteropServices;
using System.Text;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core.UI;

/// <summary>
/// Exposes functionality related to the start menu
/// </summary>
public static unsafe class StartMenu
{
    /// <summary>
    /// Adds a new option to the start menu
    /// </summary>
    /// <param name="option">The option to add.</param>
    /// <returns>The ID of the added option.</returns>
    /// <remarks>
    /// This function must be called either in the <see cref="IPlugin.OnLoad"/> or <see cref="IPlugin.Initialize"/> methods.
    /// Calling it in any other context is invalid.
    /// </remarks>
    public static int AddOption(StartMenuOption option)
    {
        if (_optionsApplied)
            throw new InvalidOperationException("Cannot add start menu options after they have been applied");

        var id = Interlocked.Increment(ref _currentOptionId) - 1;
        Options[id] = option;
        return id;
    }

    /// <summary>
    /// Closes the start menu if it is currently open
    /// </summary>
    public static void Close()
    {
        var startMenu = Gui.FindObject("uGUIStartMenu");
        if (startMenu is not null)
            CloseStartMenu.Invoke(startMenu.Instance);
    }

    internal static void Initialize()
    {
        var accessPages = AddressRepository.Get("StartMenu:AccessPages");
        _questPages = new NativeArray<MenuPage>(accessPages + 4 + MemoryUtil.Read<int>(accessPages), NumPages);
        _hubPages = new NativeArray<MenuPage>(accessPages - 11 + 4 + MemoryUtil.Read<int>(accessPages - 11), NumPages);

        _isOptionVisibleHook = Hook.Create<IsOptionXDelegate>(
            AddressRepository.Get("StartMenu:IsOptionVisible"),
            (gui, id) =>
            {
                if (!Options.TryGetValue(id, out var opt))
                    return _isOptionVisibleHook.Original(gui, id);

                return opt.Visible ? True : False;
            }
        );

        _isOptionClickableHook = Hook.Create<IsOptionXDelegate>(
            AddressRepository.Get("StartMenu:IsOptionClickable"),
            (gui, id) =>
            {
                if (!Options.TryGetValue(id, out var opt))
                    return _isOptionClickableHook.Original(gui, id);

                return opt.Clickable ? True : False;
            }
        );

        _selectOptionHook = Hook.Create<SelectOptionDelegate>(
            AddressRepository.Get("StartMenu:SelectOption"),
            (gui, idx, obj) =>
            {
                var opts = new PointerArray<MenuOption>(gui + 0x29C8, 99);
                if (Options.TryGetValue(opts[idx].ActionId, out var opt))
                {
                    opt.OnClick(opts[idx].ActionId);
                    return;
                }

                _selectOptionHook.Original(gui, idx, obj);
            }
        );

        _hoverOptionHook = Hook.Create<HoverOptionDelegate>(
            AddressRepository.Get("StartMenu:HoverOption"),
            (gui, option) =>
            {
                if (option != null && Options.TryGetValue(option->ActionId, out var opt))
                {
                    var key = opt.Clickable ? option->DescKey : option->DisabledDescKey;

                    // GMD 11 is cm_start_menu_*
                    QueueCommonTextOp.Invoke(Gui.SingletonInstance.Instance, gui, key, 11, 0, false);
                    return;
                }

                _hoverOptionHook.Original(gui, option);
            }
        );

        var getLanguageStr =
            new NativeFunction<Language, nint>(AddressRepository.Get("Gui:GetLanguageIdentifierFromId"));

        foreach (var lang in Enum.GetValues<Language>())
        {
            LanguageIdentifiers[lang] = Marshal.PtrToStringUTF8(getLanguageStr.Invoke(lang))!;
        }
    }

    internal static void ApplyOptions()
    {
        if (_optionsApplied)
            return;

        var language = Gui.CurrentLanguage;
        var langStr = LanguageIdentifiers[language];

        var dti = MtDti.Find("rGUIMessage");
        Ensure.NotNull(dti);

        var gmd = ResourceManager.GetResource<Gmd>($@"common\text\cm_start_menu_{langStr}", dti);
        Ensure.NotNull(gmd);

        var allocator = dti.Allocator;
        var newCount = gmd.MessageCount + 3 * Options.Count; // 2x for name, desc and disabled-desc
        var messages = (byte**)allocator.Allocate(newCount * 8);
        MemoryUtil.Copy(gmd.Messages, messages, 8 * gmd.MessageCount);

        Dictionary<string, int> messageCache = [];

        var gmdIndex = gmd.MessageCount;

        var hubOptions = Options.Where(kv => kv.Value.AddTo.HasFlag(StartMenuType.Hub));
        var questOptions = Options.Where(kv => kv.Value.AddTo.HasFlag(StartMenuType.Quest));
        AddOptions(_hubPages, hubOptions, ref gmdIndex, language, messages, messageCache);
        AddOptions(_questPages, questOptions, ref gmdIndex, language, messages, messageCache);

        Ensure.IsTrue(gmdIndex <= newCount);

        gmd.Messages = messages;
        gmd.MessageCount = gmdIndex;

        _optionsApplied = true;
    }

    private static void AddOptions(
        NativeArray<MenuPage> pages,
        IEnumerable<KeyValuePair<int, StartMenuOption>> options,
        ref int gmdIndex,
        Language language,
        byte** messages,
        Dictionary<string, int> messageCache)
    {
        foreach (var group in options.GroupBy(kv => kv.Value.Page))
        {
            var originalCount = PageOptionCounts[group.Key];

            // +1 for the terminator option
            // Intentionally not 'using' the NativeArray here because we want to keep the memory alive
            var newOptions = NativeArray<MenuOption>.Create(originalCount + group.Count() + 1);
            for (var i = 0; i < originalCount; i++)
            {
                newOptions[i] = _questPages[(int)group.Key].Options[i];
            }

            var optIndex = originalCount;
            foreach (var (id, option) in group)
            {
                var name = option.GetName(language);
                var desc = option.GetDesc(language);
                var disa = option.GetDisabledDesc?.Invoke(language);

                if (!messageCache.TryGetValue(name, out var nameIdx))
                {
                    nameIdx = gmdIndex++;
                    messages[nameIdx] = MemoryUtil.CreateNullterminated(name).Pointer;
                    messageCache[name] = nameIdx;
                }

                if (!messageCache.TryGetValue(desc, out var descIdx))
                {
                    descIdx = gmdIndex++;
                    messages[descIdx] = MemoryUtil.CreateNullterminated(desc).Pointer;
                    messageCache[desc] = descIdx;
                }

                var disaIdx = descIdx;
                if (disa is not null && !messageCache.TryGetValue(disa, out disaIdx))
                {
                    disaIdx = gmdIndex++;
                    messages[disaIdx] = MemoryUtil.CreateNullterminated(disa).Pointer;
                    messageCache[disa] = disaIdx;
                }

                newOptions[optIndex++] = new MenuOption
                {
                    ActionId = id,
                    NameKey = nameIdx,
                    DescKey = descIdx,
                    DisabledDescKey = disaIdx,
                };
            }

            // Add a terminator option to the end of the list
            newOptions[^1] = new MenuOption
            {
                ActionId = 0x4C,
                NameKey = 0,
                DescKey = 0,
                DisabledDescKey = 0,
            };

            MemoryUtil.WithRwx(pages.Address, pages.ByteSize, _ =>
            {
                pages[(int)group.Key].Options = newOptions.Pointer;
            });
        }
    }

    private const int NumPages = 5;
    private static int _currentOptionId = 0x50;
    private static bool _optionsApplied = false;
    private static NativeArray<MenuPage> _questPages;
    private static NativeArray<MenuPage> _hubPages;

    private static readonly NativeAction<nint, nint, int, int, int, bool> QueueCommonTextOp = new(
        AddressRepository.Get("Gui:QueueCommonTextOperation")
    );
    private static readonly NativeAction<nint> CloseStartMenu = new(AddressRepository.Get("StartMenu:Close"));
    private static readonly Dictionary<int, StartMenuOption> Options = [];
    private static readonly Dictionary<Language, string> LanguageIdentifiers = [];
    private static readonly Dictionary<StartMenuPage, int> PageOptionCounts = new()
    {
        { StartMenuPage.ItemsAndEquipment, 5 },
        { StartMenuPage.Quest, 8 },
        { StartMenuPage.Info, 11 },
        { StartMenuPage.Communication, 11 },
        { StartMenuPage.System, 6 },
    };

    private static Hook<IsOptionXDelegate> _isOptionVisibleHook = null!;
    private static Hook<IsOptionXDelegate> _isOptionClickableHook = null!;
    private static Hook<SelectOptionDelegate> _selectOptionHook = null!;
    private static Hook<HoverOptionDelegate> _hoverOptionHook = null!;

    private const byte True = 1;
    private const byte False = 0;

    private delegate void SelectOptionDelegate(nint gui, int idx, nint obj);
    private delegate byte IsOptionXDelegate(nint gui, int id);
    private delegate void HoverOptionDelegate(nint gui, MenuOption* option);
}

/// <summary>
/// Defines a custom start menu option
/// </summary>
public class StartMenuOption
{
    /// <summary>
    /// The page of the start menu to add this option to
    /// </summary>
    public required StartMenuPage Page { get; init; }

    /// <summary>
    /// Which start menu(s) to add this option to
    /// </summary>
    public required StartMenuType AddTo { get; init; }

    /// <summary>
    /// This action is called when the option is clicked. The parameter is the ID of the option
    /// </summary>
    public required Action<int> OnClick { get; init; }

    /// <summary>
    /// Gets the name of the option for the specified language
    /// </summary>
    public required Func<Language, string> GetName { get; init; }

    /// <summary>
    /// Gets the description of the option for the specified language
    /// </summary>
    public required Func<Language, string> GetDesc { get; init; }

    /// <summary>
    /// Gets the alternate description of the option for the specified language, if the option is disabled (not clickable).
    /// </summary>
    public Func<Language, string>? GetDisabledDesc { get; init; }

    /// <summary>
    /// Returns true if the option is visible at the current time, false otherwise. If null, the option is always visible
    /// </summary>
    public Func<bool>? IsVisible { get; init; }

    /// <summary>
    /// Returns true if the option is clickable at the current time, false otherwise. If null, the option is always clickable
    /// </summary>
    public Func<bool>? IsClickable { get; init; }

    internal bool Visible => IsVisible?.Invoke() ?? true;
    internal bool Clickable => IsClickable?.Invoke() ?? true;
}

/// <summary>
/// The different pages of the start menu.
/// </summary>
public enum StartMenuPage
{
    ItemsAndEquipment = 0,
    Quest = 1,
    Info = 2,
    Communication = 3,
    System = 4,
}

/// <summary>
/// The different types of start menu options.
/// </summary>
[Flags]
public enum StartMenuType
{
    Quest = 1,
    Hub = 2,
    Both = Quest | Hub,
}

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
internal unsafe struct MenuPage
{
    public MenuOption* Options;
    public int Unk;
}

[StructLayout(LayoutKind.Sequential, Size = 0x10)]
internal struct MenuOption
{
    public int ActionId;
    public int NameKey;
    public int DescKey;
    public int DisabledDescKey;
}
