using SharpPluginLoader.Core.Memory;
using System.Runtime.InteropServices;

namespace SharpPluginLoader.Core.Steam;

public static unsafe partial class Matchmaking
{
    /// <summary>
    /// Sets the physical distance for which we should search for lobbies, this is based on the users IP address
    /// and a IP location map on the Steam backed.
    /// </summary>
    /// <param name="filter">Specifies the maximum distance.</param>
    public static void AddRequestLobbyListDistanceFilter(LobbyDistanceFilter filter)
    {
        var iface = GetSteamMatchmakingInterface();
        new NativeAction<nint, LobbyDistanceFilter>(
            GetVirtualFunction(iface, VirtualFunctionIndex.AddRequestLobbyListDistanceFilter)
        ).Invoke(iface, filter);
    }

    /// <summary>
    /// Filters to only return lobbies with the specified number of open slots available.
    /// </summary>
    /// <param name="slotsAvailable">The number of open slots that must be open.</param>
    public static void AddRequestLobbyListFilterSlotsAvailable(int slotsAvailable)
    {
        var iface = GetSteamMatchmakingInterface();
        new NativeAction<nint, int>(
            GetVirtualFunction(iface, VirtualFunctionIndex.AddRequestLobbyListFilterSlotsAvailable)
        ).Invoke(iface, slotsAvailable);
    }

    /// <summary>
    /// Sorts the results closest to the specified value.
    /// 
    /// Near filters don't actually filter out values, they just influence how the results are sorted. 
    /// You can specify multiple near filters, with the first near filter influencing the most, 
    /// and the last near filter influencing the least.
    /// </summary>
    /// <param name="keyToMatch">The filter key name to match. This can not be longer than 255 bytes</param>
    /// <param name="valueToBeCloseTo">The value that lobbies will be sorted on.</param>
    public static void AddRequestLobbyListNearValueFilter(string keyToMatch, int valueToBeCloseTo)
    {
        var iface = GetSteamMatchmakingInterface();
        new NativeAction<nint, string, int>(
            GetVirtualFunction(iface, VirtualFunctionIndex.AddRequestLobbyListNearValueFilter)
        ).Invoke(iface, keyToMatch, valueToBeCloseTo);
    }

    /// <summary>
    /// Adds a numerical comparison filter to the next RequestLobbyList call.
    /// </summary>
    /// <param name="keyToMatch">The filter key name to match. This can not be longer than 255 bytes</param>
    /// <param name="valueToMatch">The number to match.</param>
    /// <param name="comparison">The comparison type to use.</param>
    public static void AddRequestLobbyListNumericalFilter(string keyToMatch, int valueToMatch, LobbyComparison comparison)
    {
        var iface = GetSteamMatchmakingInterface();
        new NativeAction<nint, string, int, LobbyComparison>(
            GetVirtualFunction(iface, VirtualFunctionIndex.AddRequestLobbyListNumericalFilter)
        ).Invoke(iface, keyToMatch, valueToMatch, comparison);
    }

    /// <summary>
    /// Adds a string comparison filter to the next RequestLobbyList call.
    /// </summary>
    /// <param name="keyToMatch">The filter key name to match. This can not be longer than 255 bytes</param>
    /// <param name="valueToMatch">The string to match.</param>
    /// <param name="comparison">The comparison type to use.</param>
    public static void AddRequestLobbyListStringFilter(string keyToMatch, string valueToMatch, LobbyComparison comparison)
    {
        var iface = GetSteamMatchmakingInterface();
        new NativeAction<nint, string, string, LobbyComparison>(
            GetVirtualFunction(iface, VirtualFunctionIndex.AddRequestLobbyListStringFilter)
        ).Invoke(iface, keyToMatch, valueToMatch, comparison);
    }

    #region Internal

    private static nint GetVirtualFunction(nint iface, VirtualFunctionIndex func)
    {
        var vtable = *(nint*)iface;
        return MemoryUtil.Read<nint>(vtable + (int)func * nint.Size);
    }

    private static nint GetSteamMatchmakingInterface()
    {
        return MemoryUtil.Read<nint>(SteamInternal_ContextInit(_steamMatchmakingInterfaceGetter));
    }

    internal static void Initialize()
    {
        _searchLobbiesHook = Hook.Create<SearchLobbiesDelegate>(
            AddressRepository.Get("Matchmaking:StartRequest"), (netCore, netRequest) =>
        {
            var phase = MemoryUtil.Read<int>(netRequest + 0xE0);
            if (phase != 0)
                return _searchLobbiesHook!.Original(netCore, netRequest);

            ref int maxResults = ref MemoryUtil.GetRef<int>(netRequest + 0x60);
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnLobbySearch))
                plugin.OnLobbySearch(ref maxResults);

            return _searchLobbiesHook!.Original(netCore, netRequest);
        });

        _resultCountSanityCheckPath = new Patch(
            AddressRepository.Get("Matchmaking:StartRequest") + 212,
            [0xEB, 0x10],
            true
        );

        var leaInstruction = PatternScanner.FindFirst(Pattern.FromString("48 8B D6 48 8B 08 48 8B 01 FF 90 88 00 00 00")) - 13;
        Log.Info($"[Matchmaking] Found lea instruction at 0x{leaInstruction:X}");
        var afterLeaInstruction = leaInstruction + 4;
        var offset = MemoryUtil.Read<int>(leaInstruction);
        Log.Info($"[Matchmaking] Found offset {offset:X}");
        _steamMatchmakingInterfaceGetter = afterLeaInstruction + offset;
        Log.Info($"[Matchmaking] Found SteamMatchmaking interface getter at 0x{_steamMatchmakingInterfaceGetter:X}");
    }

    [LibraryImport("steam_api64.dll")]
    private static partial nint SteamInternal_ContextInit(nint request);

    private delegate int SearchLobbiesDelegate(nint netCore, nint netRequest);
    private static Hook<SearchLobbiesDelegate> _searchLobbiesHook = null!;
    private static Patch _resultCountSanityCheckPath;

    private static nint _steamMatchmakingInterfaceGetter;

    private enum VirtualFunctionIndex
    {
        GetFavoriteGameCount = 0,
        GetFavoriteGame = 1,
        AddFavoriteGame = 2,
        RemoveFavoriteGame = 3,
        RequestLobbyList = 4,
        AddRequestLobbyListStringFilter = 5,
        AddRequestLobbyListNumericalFilter = 6,
        AddRequestLobbyListNearValueFilter = 7,
        AddRequestLobbyListFilterSlotsAvailable = 8,
        AddRequestLobbyListDistanceFilter = 9,
        AddRequestLobbyListResultCountFilter = 10,
        AddRequestLobbyListCompatibleMembersFilter = 11,
        GetLobbyByIndex = 12,
        CreateLobby = 13,
        JoinLobby = 14,
        LeaveLobby = 15,
        InviteUserToLobby = 16,
        GetNumLobbyMembers = 17,
        GetLobbyMemberByIndex = 18,
        GetLobbyData = 19,
        SetLobbyData = 20,
        GetLobbyDataCount = 21,
        GetLobbyDataByIndex = 22,
        DeleteLobbyData = 23,
        GetLobbyMemberData = 24,
        SetLobbyMemberData = 25,
        SendLobbyChatMessage = 26,
        GetLobbyChatEntry = 27,
        RequestLobbyData = 28,
        SetLobbyGameServer = 29,
        GetLobbyGameServer = 30,
        SetLobbyMemberLimit = 31,
        GetLobbyMemberLimit = 32,
        SetLobbyType = 33,
        SetLobbyJoinable = 34,
        GetLobbyOwner = 35,
        SetLobbyOwner = 36,
        SetLinkedLobby = 37,
    }

    #endregion
}
