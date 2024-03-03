using SharpPluginLoader.Core.Memory;

namespace SharpPluginLoader.Core;

/// <summary>
/// Exposes functionality related to the sMhArea singleton.
/// </summary>
public static class Area
{
    public static MtObject SingletonInstance => SingletonManager.GetSingleton("sMhArea")!;

    /// <summary>
    /// Gets the current area id.
    /// </summary>
    public static Stage CurrentStage => SingletonInstance.Get<Stage>(0xD328);
}

/// <summary>
/// Ids of the areas in the game.
/// </summary>
public enum Stage : uint
{
    InfinityOfNothingHandler = 100,
    AncientForest = 101,
    WildspireWaste = 102,
    CoralHighlands = 103,
    RottenVale = 104,
    ElderRecess = 105,
    GreatRavine = 106,
    GreatRavineStory = 107,
    HoarfrostReach = 108,
    GuidingLands = 109,
    InfinityOfNothing = 200,
    SpecialArena = 201,
    ChallengeArena = 202,
    Astera = 301,
    AsteraHub = 302,
    ResearchBase = 303,
    Seliana = 305,
    SelianaHub = 306,
    Everstream = 403,
    ConfluenceOfFates = 405,
    CharacterCreation = 407,
    DebugMap = 408,
    ElDorado = 409,
    SelianaSupplyCache = 411,
    OriginIsleNergigante = 412,
    OriginIsleSharaIshvalda = 413,
    SecludedValley = 415,
    AlatreonStage = 416,
    CastleSchrade = 417,
    LivingQuarters = 501,
    PrivateQuarters = 502,
    PrivateSuite = 503,
    TrainingCamp = 504,
    ChamberOfFive = 505,
    SelianaRoom = 506
}
