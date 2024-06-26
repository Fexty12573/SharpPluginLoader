﻿using System.Numerics;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;
using SharpPluginLoader.Core.Resources;
using SharpPluginLoader.Core.Weapons;

namespace SharpPluginLoader.Core.Entities
{
    public class Player : Entity
    {
        public Player(nint instance) : base(instance) { }
        public Player() { }

        /// <summary>
        /// The sPlayer singleton instance
        /// </summary>
        public static MtObject SingletonInstance => SingletonManager.GetSingleton("sPlayer")!;

        /// <summary>
        /// The main player
        /// </summary>
        public static unsafe Player? MainPlayer
        {
            get
            {
                var player = FindMasterPlayerFunc.InvokeUnsafe(SingletonInstance.Instance);
                return player == 0 ? null : new Player(player);
            }
        }

        /// <summary>
        /// The currently equipped weapon
        /// </summary>
        public Weapon? CurrentWeapon => GetObject<Weapon>(0x76B0);

        /// <summary>
        /// The currently equipped weapon type
        /// </summary>
        public unsafe WeaponType CurrentWeaponType => GetWeaponTypeFunc.InvokeUnsafe(Instance);

        /// <summary>
        /// The current health of the player
        /// </summary>
        public ref float Health => ref MemoryUtil.GetRef<float>(Get<nint>(0x7630) + 0x64);

        /// <summary>
        /// The maximum health of the player
        /// </summary>
        public ref float MaxHealth => ref MemoryUtil.GetRef<float>(Get<nint>(0x7630) + 0x60);

        /// <inheritdoc/>
        public override void CreateShell(uint index, Vector3 target, Vector3? origin = null)
        {
            var shll = CurrentWeaponType < WeaponType.Bow 
                ? CurrentWeapon?.GetObject<ShellParamList>(0x1D90) // Weapon shll
                : GetObject<ShellParamList>(0x56E8); // Ammo shll

            if (shll == null)
                throw new Exception("ShellParamList is null");

            CreateShell(shll, index, target, origin);
        }

        public override void CreateShell(ShellParamList shll, uint index, Vector3 target, Vector3? origin = null)
        {
            var shell = shll.GetShell(index);
            if (shell == null)
                throw new IndexOutOfRangeException("Shell index out of range");

            CreateShell(shell, target, origin);
        }

        public override unsafe void CreateShell(ShellParam shell, Vector3 target, Vector3? origin = null)
        {
            var shellParams = new ShellCreationParams(target, origin ?? Position);
            CreateShellFunc.Invoke(shell.Instance, Instance, Instance, (nint)(&shellParams));
        }

        public void RegisterMbd(Resource mbd, uint index)
        {
            if (index >= 16)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be less than 16");

            SetObject(0x7430 + (nint)index * 8, mbd);
        }

        internal static void Initialize()
        {
            _changeWeaponHook = Hook.Create<ChangeWeaponDelegate>(AddressRepository.Get("Player:ChangeWeapon"), ChangeWeaponHook);
        }

        private static void ChangeWeaponHook(nint player, WeaponType weaponType, int weaponId)
        {
            foreach (var plugin in PluginManager.Instance.GetPlugins(p => p.OnWeaponChange))
                plugin.OnWeaponChange(new Player(player), weaponType, weaponId);

            _changeWeaponHook.Original(player, weaponType, weaponId);
        }

        private delegate void ChangeWeaponDelegate(nint player, WeaponType weaponType, int weaponId);

        private static Hook<ChangeWeaponDelegate> _changeWeaponHook = null!;
        private static readonly NativeFunction<nint, nint> FindMasterPlayerFunc = new(AddressRepository.Get("Player:FindMasterPlayer"));
        private static readonly NativeFunction<nint, nint, nint, nint, nint> CreateShellFunc = new(AddressRepository.Get("Player:CreateShell"));
        private static readonly NativeFunction<nint, WeaponType> GetWeaponTypeFunc = new(AddressRepository.Get("Player:GetWeaponType"));
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
}
