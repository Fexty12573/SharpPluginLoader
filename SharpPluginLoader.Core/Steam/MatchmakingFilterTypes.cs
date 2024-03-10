using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpPluginLoader.Core.Steam;

/// <summary>
/// The different comparison types for lobbies. See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#ELobbyComparison"/>
/// </summary>
public enum LobbyComparison
{
    /// <summary>
    /// The lobbies value must be equal to or less than this one.
    /// </summary>
    EqualToOrLessThan = -2,

    /// <summary>
    /// The lobbies value must be less than this one.
    /// </summary>
    LessThan = -1,

    /// <summary>
    /// The lobbies value must be equal to this one.
    /// </summary>
    Equal = 0,

    /// <summary>
    /// The lobbies value must be greater than this one.
    /// </summary>
    GreaterThan = 1,

    /// <summary>
    /// The lobbies value must be equal to or greater than this one.
    /// </summary>
    EqualToOrGreaterThan = 2,

    /// <summary>
    /// The lobbies value must not be equal to this one.
    /// </summary>
    NotEqual = 3
}

/// <summary>
/// The different distance filters for lobbies. See <see href="https://partner.steamgames.com/doc/api/ISteamMatchmaking#ELobbyDistanceFilter"/>
/// </summary>
public enum LobbyDistanceFilter
{
    /// <summary>
    /// Only lobbies in the same immediate region will be returned.
    /// </summary>
    Close,

    /// <summary>
    /// Only lobbies in the same region or nearby regions will be returned.
    /// </summary>
    Default,

    /// <summary>
    /// For games that don't have many latency requirements, will return lobbies about half-way around the globe.
    /// </summary>
    Far,

    /// <summary>
    /// No filtering, will match lobbies as far as India to NY (not recommended, expect multiple seconds of latency between the clients).
    /// </summary>
    WorldWide
}
