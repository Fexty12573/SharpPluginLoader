using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core.Entities
{
    public class Monster : Entity
    {
        /// <summary>
        /// The sEnemy singleton instance
        /// </summary>
        public static nint SingletonInstance => MemoryUtil.Read<nint>(0x14500ad00);

        /// <summary>
        /// Gets a list of all monsters in the game
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Monster> GetAllMonsters()
        {
            if (SingletonInstance == 0)
                yield break;

            var enemyList = new NativeArray<nint>(SingletonInstance + 0x38, 128);
            foreach (var aiData in enemyList)
            {
                if (aiData == 0)
                    continue;
                var monster = MemoryUtil.Read<nint>(aiData + 0x138);
                if (monster == 0)
                    continue;

                yield return new Monster(monster);
            }
        }

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
        public ref float Health => ref MemoryUtil.GetRef<float>(Get<nint>(0x7670) + 0x64);

        /// <summary>
        /// The maximum health of the monster
        /// </summary>
        public ref float MaxHealth => ref MemoryUtil.GetRef<float>(Get<nint>(0x7670) + 0x60);

        /// <summary>
        /// The speed of the monster (1.0 is normal speed)
        /// </summary>
        public ref float Speed => ref GetRef<float>(0x1D8A8);

        /// <summary>
        /// The despawn time of the monster (in seconds)
        /// </summary>
        public ref float DespawnTime => ref GetRef<float>(0x1C3D4);

        /// <summary>
        /// The index of the monster in the difficulty table (dtt_dif)
        /// </summary>
        public ref uint DifficultyIndex => ref MemoryUtil.GetRef<uint>(AiData + 0x8AC);

        /// <summary>
        /// Forces the monster to do a given action. This will interrupt the current action.
        /// </summary>
        /// <param name="id">The id of the action to execute</param>
        public unsafe void ForceAction(int id)
        {
            Set(0x18938, id);
            ForceActionFunc.Invoke(MemoryUtil.Read<nint>(AiData + 0x4B0), AiData);
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
            MemoryUtil.GetRef<nint>(MemoryUtil.Read<nint>(AiData + 0xAA8) + 0x5D0) = target;
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

        public override string ToString()
        {
            return $"{Name} @ 0x{Instance:X}";
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

        private static nint MonsterCtorHook(nint instance, MonsterType type, uint variant)
        {
            var monster = new Monster(_monsterCtorHook.Original(instance, type, variant));
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnMonsterCreate))
                plugin.OnMonsterCreate(monster);

            return monster.Instance;
        }

        private static void EntityInitializeHook(nint instance)
        {
            _entityInitializeHook.Original(instance);
            var monster = new Monster(instance);
            if (monster.Is("uEnemy"))
            {
                foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnMonsterInitialized))
                    plugin.OnMonsterInitialized(monster);
            }
        }

        private static void MonsterFlinchHook(nint instance, nint ai)
        {
            var monster = new Monster(MemoryUtil.Read<nint>(ai + 0x138));
            var shouldFlinch = true;
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnMonsterFlinch))
            {
                if (!plugin.OnMonsterFlinch(monster, ref monster.GetRef<int>(0x18938)))
                    shouldFlinch = false;
            }

            if (shouldFlinch)
                _monsterFlinchHook.Original(instance, ai);
        }

        private static bool EnrageHook(nint instance)
        {
            var monster = new Monster(MemoryUtil.Read<nint>(instance + 0x128));
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnMonsterEnrage))
                plugin.OnMonsterEnrage(monster);

            return _enrageHook.Original(instance);
        }

        private static void UnenrageHook(nint instance)
        {
            var monster = new Monster(MemoryUtil.Read<nint>(instance + 0x128));
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnMonsterUnenrage))
                plugin.OnMonsterUnenrage(monster);

            _unenrageHook.Original(instance);
        }

        private static void MonsterDieHook(nint instance, nint ai)
        {
            var monster = new Monster(MemoryUtil.Read<nint>(ai + 0x138));
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnMonsterDeath))
                plugin.OnMonsterDeath(monster);

            _monsterDieHook.Original(instance, ai);
        }

        private static void MonsterDtorHook(nint instance)
        {
            var monster = new Monster(instance);
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnMonsterDestroy))
                plugin.OnMonsterDestroy(monster);

            _monsterDtorHook.Original(instance);
        }

        internal static void Initialize()
        {
            _launchActionHook = Hook.Create<LaunchActionDelegate>(AddressRepository.Get("Monster:LaunchAction"), LaunchActionHook);
            _monsterCtorHook = Hook.Create<MonsterCtorDelegate>(AddressRepository.Get("Monster:Ctor"), MonsterCtorHook);
            _entityInitializeHook = Hook.Create<EntityInitializeDelegate>(AddressRepository.Get("Entity:Initialize"), EntityInitializeHook);
            _monsterFlinchHook = Hook.Create<MonsterFlinchDelegate>(AddressRepository.Get("Monster:Flinch"), MonsterFlinchHook);
            _enrageHook = Hook.Create<EnrageDelegate>(AddressRepository.Get("Monster:Enrage"), EnrageHook);
            _unenrageHook = Hook.Create<UnenrageDelegate>(AddressRepository.Get("Monster:Unenrage"), UnenrageHook);
            _monsterDieHook = Hook.Create<MonsterDieDelegate>(AddressRepository.Get("Monster:Die"), MonsterDieHook);
            _monsterDtorHook = Hook.Create<MonsterDtorDelegate>(AddressRepository.Get("Monster:Dtor"), MonsterDtorHook);
        }

        private delegate bool LaunchActionDelegate(nint monster, int actionId);
        private delegate nint MonsterCtorDelegate(nint instance, MonsterType type, uint variant);
        private delegate void EntityInitializeDelegate(nint instance);
        private delegate void MonsterFlinchDelegate(nint instnace, nint ai);
        private delegate bool EnrageDelegate(nint instance);
        private delegate void UnenrageDelegate(nint instance);
        private delegate void MonsterDieDelegate(nint instance, nint ai);
        private delegate void MonsterDtorDelegate(nint instance);
        private static readonly NativeAction<nint, nint> ForceActionFunc = new(AddressRepository.Get("Monster:ForceAction"));
        private static readonly NativeFunction<nint, bool> EnrageFunc = new(AddressRepository.Get("Monster:Enrage"));
        private static readonly NativeAction<nint> UnenrageFunc = new(AddressRepository.Get("Monster:Unenrage"));
        private static readonly Patch SpeedResetPatch1 = new(AddressRepository.Get("Monster:SpeedResetPatch1"), Enumerable.Repeat((byte)0x90, 10).ToArray());
        private static readonly Patch SpeedResetPatch2 = new(AddressRepository.Get("Monster:SpeedResetPatch2"), Enumerable.Repeat((byte)0x90, 6).ToArray());
        private static Hook<LaunchActionDelegate> _launchActionHook = null!;
        private static Hook<MonsterCtorDelegate> _monsterCtorHook = null!;
        private static Hook<EntityInitializeDelegate> _entityInitializeHook = null!;
        private static Hook<MonsterFlinchDelegate> _monsterFlinchHook = null!;
        private static Hook<EnrageDelegate> _enrageHook = null!;
        private static Hook<UnenrageDelegate> _unenrageHook = null!;
        private static Hook<MonsterDieDelegate> _monsterDieHook = null!;
        private static Hook<MonsterDtorDelegate> _monsterDtorHook = null!;
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
