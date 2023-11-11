using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core
{
    public class Monster : Entity
    {
        /// <summary>
        /// Constructs a new Monster from a native pointer
        /// </summary>
        /// <param name="instance">The native pointer</param>
        public Monster(nint instance) : base(instance) { }
        /// <summary>
        /// Constructs a new Monster from a nullptr
        /// </summary>
        public Monster() { }

        /// <summary>
        /// The id of the monster
        /// </summary>
        public MonsterType Type => Get<MonsterType>(0x12280);

        /// <summary>
        /// The variant (sub id) of the monster
        /// </summary>
        public uint Variant => Get<uint>(0x12288);

        /// <summary>
        /// The name of the monster
        /// </summary>
        public string Name => Utility.GetMonsterName(Type);

        /// <summary>
        /// The current health of the monster
        /// </summary>
        public float Health
        {
            get => Get<nint>(0x7670).Read<float>(0x64);
            set => Get<nint>(0x7670).ReadRef<float>(0x64) = value;
        }

        /// <summary>
        /// The maximum health of the monster
        /// </summary>
        public float MaxHealth
        {
            get => Get<nint>(0x7670).Read<float>(0x60);
            set => Get<nint>(0x7670).ReadRef<float>(0x60) = value;
        }

        /// <summary>
        /// The speed of the monster (1.0 is normal speed)
        /// </summary>
        public float Speed
        {
            get => Get<float>(0x1D8A8);
            set => Set(0x1D8A8, value);
        }

        /// <summary>
        /// The despawn time of the monster (in seconds)
        /// </summary>
        public float DespawnTime
        {
            get => Get<float>(0x1C3D4);
            set => Set(0x1C3D4, value);
        }

        /// <summary>
        /// The index of the monster in the difficulty table (dtt_dif)
        /// </summary>
        public uint DifficultyIndex
        {
            get => AiData.Read<uint>(0x8AC);
            set => AiData.ReadRef<uint>(0x8AC) = value;
        }

        /// <summary>
        /// Freezes the monster and pauses all ai processing
        /// </summary>
        public bool Frozen
        {
            get => (Get<uint>(0x14) & 1) == 1;
            set => Set(0x14, (Get<uint>(0x14) & 0xFFFFFFFE) | (value ? 1u : 0u));
        }

        /// <summary>
        /// Forces the monster to do a given action. This will interrupt the current action.
        /// </summary>
        /// <param name="id">The id of the action to execute</param>
        public unsafe void ForceAction(int id)
        {
            Set(0x18938, id);
            ForceActionFunc.Invoke(AiData.Read<nint>(0x4B0), AiData);
        }

        /// <summary>
        /// Tries to enrage the monster
        /// </summary>
        public unsafe bool Enrage()
        {
            return EnrageFunc.Invoke((nint)GetPtrInline<nint>(0x1BD08));
        }

        /// <summary>
        /// Unenrages the monster
        /// </summary>
        public unsafe void Unenrage()
        {
            UnenrageFunc.Invoke((nint)GetPtrInline<nint>(0x1BD08));
        }

        /// <summary>
        /// Sets the monster's target
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(nint target)
        {
            AiData.Read<nint>(0xAA8).ReadRef<nint>(0x5D0) = target;
        }

        /// <summary>
        /// The AI data of the monster
        /// </summary>
        public nint AiData => Get<nint>(0x12278);

        /// <inheritdoc/>
        public override void CreateShell(uint index, MtVector3 target, MtVector3? origin = null)
        {
            base.CreateShell(index, target, origin);
        }

        /// <summary>
        /// Gets the human readable name of the given monster id
        /// </summary>
        /// <param name="type">The monster id</param>
        /// <returns>The human readable name of the monster</returns>
        /// <remarks>This is equivalent to the <see cref="Name"/> property on a monster with this <paramref name="type"/></remarks>
        public static string ToString(MonsterType type)
        {
            return Utility.GetMonsterName(type);
        }

        /// <summary>
        /// Disables the periodic speed reset that the game does
        /// </summary>
        /// <remarks>Call this before changing the <see cref="Speed"/> property on a monster. This setting applies globally</remarks>
        public static void DisableSpeedReset()
        {
            SpeedResetPatch1.Enable();
            SpeedResetPatch2.Enable();
        }

        /// <summary>
        /// Reenables the periodic speed reset that the game does
        /// </summary>
        public static void EnableSpeedReset()
        {
            SpeedResetPatch1.Disable();
            SpeedResetPatch2.Disable();
        }

        private static bool LaunchActionHook(nint instance, int actionId)
        {
            var monster = new Monster(instance);
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnMonsterAction))
                plugin.OnMonsterAction(monster, ref actionId);

            return _launchActionHook.Original(instance, actionId);
        }

        internal static void Initialize()
        {
            _launchActionHook = Hook.Create<LaunchActionDelegate>(LaunchActionHook, 0x1402a7f80);
        }

        private delegate bool LaunchActionDelegate(nint monster, int actionId);
        private static readonly NativeAction<nint, nint> ForceActionFunc = new(0x1413966e0);
        private static readonly NativeFunction<nint, bool> EnrageFunc = new(0x1402a8120);
        private static readonly NativeAction<nint> UnenrageFunc = new(0x1402a83b0);
        private static readonly Patch SpeedResetPatch1 = new(0x141cb08ab, Enumerable.Repeat((byte)0x90, 10).ToArray());
        private static readonly Patch SpeedResetPatch2 = new(0x140b00fff, Enumerable.Repeat((byte)0x90, 6).ToArray());
        private static Hook<LaunchActionDelegate> _launchActionHook = null!;
    }

    /// <summary>
    /// Represents a monster id
    /// </summary>
    public enum MonsterType : uint
    {
        Anjanath = 0x00,
        Rathalos = 0x01,
        Aptonoth = 0x02,
        Jagras = 0x03,
        ZorahMagdaros = 0x04,
        Mosswine = 0x05,
        Gajau = 0x06,
        GreatJagras = 0x07,
        KestodonM = 0x08,
        Rathian = 0x09,
        PinkRathian = 0x0A,
        AzureRathalos = 0x0B,
        Diablos = 0x0C,
        BlackDiablos = 0x0D,
        Kirin = 0x0E,
        Behemoth = 0x0F,
        KushalaDaora = 0x10,
        Lunastra = 0x11,
        Teostra = 0x12,
        Lavasioth = 0x13,
        Deviljho = 0x14,
        Barroth = 0x15,
        Uragaan = 0x16,
        Leshen = 0x17,
        Pukei = 0x18,
        Nergigante = 0x19,
        XenoJiiva = 0x1A,
        KuluYaKu = 0x1B,
        TzitziYaKu = 0x1C,
        Jyuratodus = 0x1D,
        TobiKadachi = 0x1E,
        Paolumu = 0x1F,
        Legiana = 0x20,
        GreatGirros = 0x21,
        Odogaron = 0x22,
        Radobaan = 0x23,
        VaalHazak = 0x24,
        Dodogama = 0x25,
        KulveTaroth = 0x26,
        Bazelgeuse = 0x27,
        Apceros = 0x28,
        KelbiM = 0x29,
        KelbiF = 0x2A,
        Hornetaur = 0x2B,
        Vespoid = 0x2C,
        Mernos = 0x2D,
        KestodonF = 0x2E,
        Raphinos = 0x2F,
        Shamos = 0x30,
        Barnos = 0x31,
        Girros = 0x32,
        AncientLeshen = 0x33,
        Gastodon = 0x34,
        Noios = 0x35,
        Magmacore = 0x36,
        Magmacore2 = 0x37,
        Gajalaka = 0x38,
        SmallBarrel = 0x39,
        LargeBarrel = 0x3A,
        TrainingPole = 0x3B,
        TrainingWagon = 0x3C,
        Tigrex = 0x3D,
        Nargacuga = 0x3E,
        Barioth = 0x3F,
        SavageDeviljho = 0x40,
        Brachydios = 0x41,
        Glavenus = 0x42,
        AcidicGlavenus = 0x43,
        FulgurAnjanath = 0x44,
        CoralPukei = 0x45,
        RuinerNergigante = 0x46,
        ViperTobi = 0x47,
        NightshadePaolumu = 0x48,
        ShriekingLegiana = 0x49,
        EbonyOdogaron = 0x4A,
        BlackveilVaal = 0x4B,
        SeethingBazelgeuse = 0x4C,
        Beotodus = 0x4D,
        Banbaro = 0x4E,
        Velkhana = 0x4F,
        Namielle = 0x50,
        Shara = 0x51,
        Popo = 0x52,
        Anteka = 0x53,
        Wulg = 0x54,
        Cortos = 0x55,
        Boaboa = 0x56,
        Alatreon = 0x57,
        GoldRathian = 0x58,
        SilverRathalos = 0x59,
        YianGaruga = 0x5A,
        Rajang = 0x5B,
        FuriousRajang = 0x5C,
        BruteTigrex = 0x5D,
        Zinogre = 0x5E,
        StygianZinogre = 0x5F,
        RagingBrachy = 0x60,
        SafiJiiva = 0x61,
        Unavaliable = 0x62,
        ScarredYianGaruga = 0x63,
        FrostfangBarioth = 0x64,
        Fatalis = 0x65
    }
}
