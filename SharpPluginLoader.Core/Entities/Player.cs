using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core.Entities
{
    public class Player : Entity
    {
        public Player(nint instance) : base(instance) { }
        public Player() { }

        /// <summary>
        /// The sPlayer singleton instance
        /// </summary>
        public static nint SingletonInstance => MemoryUtil.Read<nint>(0x14500ca60);

        /// <summary>
        /// The main player
        /// </summary>
        public static unsafe Player? MainPlayer
        {
            get
            {
                var player = FindMasterPlayerFunc.InvokeUnsafe(SingletonInstance);
                return player == 0 ? null : new Player(player);
            }
        }

        /// <summary>
        /// The currently equipped weapon
        /// </summary>
        public MtObject? CurrentWeapon => GetObject<MtObject>(0x76B0);

        /// <summary>
        /// The currently equipped weapon type
        /// </summary>
        public unsafe WeaponType CurrentWeaponType => GetWeaponTypeFunc.InvokeUnsafe(Instance);

        /// <inheritdoc/>
        public override void CreateShell(uint index, MtVector3 target, MtVector3? origin = null)
        {
            var shll = CurrentWeaponType < WeaponType.Bow 
                ? CurrentWeapon?.GetObject<ShellParamList>(0x1D90) // Weapon shll
                : GetObject<ShellParamList>(0x56E8); // Ammo shll

            if (shll == null)
                throw new Exception("ShellParamList is null");

            CreateShell(shll, index, target, origin);
        }

        public override void CreateShell(ShellParamList shll, uint index, MtVector3 target, MtVector3? origin = null)
        {
            var shell = shll.GetShell(index);
            if (shell == null)
                throw new IndexOutOfRangeException("Shell index out of range");

            CreateShell(shell, target, origin);
        }

        public override unsafe void CreateShell(ShellParam shell, MtVector3 target, MtVector3? origin = null)
        {
            var shellParams = new ShellCreationParams(target, origin ?? Position);
            CreateShellFunc.Invoke(shell.Instance, Instance, Instance, (nint)(&shellParams));
        }

        private static void Initialize()
        {
            _changeWeaponHook = Hook.Create<ChangeWeaponDelegate>(ChangeWeaponHook, 0x141f59090);
        }

        private static void ChangeWeaponHook(nint player, WeaponType weaponType, int weaponId)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnWeaponChange))
                plugin.OnWeaponChange(new Player(player), weaponType, weaponId);

            _changeWeaponHook.Original(player, weaponType, weaponId);
        }

        private delegate void ChangeWeaponDelegate(nint player, WeaponType weaponType, int weaponId);

        private static Hook<ChangeWeaponDelegate> _changeWeaponHook = null!;
        private static readonly NativeFunction<nint, nint> FindMasterPlayerFunc = new(0x141b41240);
        private static readonly NativeFunction<nint, WeaponType> GetWeaponTypeFunc = new(0x141f61470);
        private static readonly NativeFunction<nint, nint, nint, nint, nint> CreateShellFunc = new(0x141aa67d0);
    }

    public enum WeaponType
    {
        GreatSword = 0,
        SwordAndShield = 1,
        DualBlades = 2,
        LongSword = 3,
        Hammer = 4,
        HuntingHorn = 5,
        Lance = 6,
        GunLance = 7,
        SwitchAxe = 8,
        ChargeBlade = 9,
        InsectGlaive = 10,
        Bow = 11,
        LightBowgun = 12,
        HeavyBowgun = 13,
        None = 0xFF
    }

    internal unsafe struct ShellCreationParams
    {
        public fixed float Values[74];

        private T* As<T>(long offset = 0) where T : unmanaged
        {
            fixed (float* ptr = Values)
                return (T*)((nint)ptr + offset);
        }

        public ShellCreationParams(MtVector3 target, MtVector3 origin)
        {
            Values[0] = origin.X;
            Values[1] = origin.Y;
            Values[2] = origin.Z;
            Values[3] = 0f;

            *As<bool>(4 * 4) = true;
            var values = As<float>(0x40);
            values[0] = target.X;
            values[1] = target.Y;
            values[2] = target.Z;
            values[3] = 0f;
            *As<bool>(0x50) = true;

            var ivals = As<int>(0xA0);
            ivals[0] = 0x12;
            ivals[1] = -1;
            ivals[2] = -1;
        }
    }
}
