using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Models;
using SharpPluginLoader.Core.Resources;

namespace SharpPluginLoader.Core.Weapons
{
    /// <summary>
    /// Represents an instance of a uWeapon class
    /// </summary>
    public class Weapon : Model
    {
        public Weapon(nint instance) : base(instance) { }
        public Weapon() { }

        /// <summary>
        /// Gets the type of the weapon
        /// </summary>
        public unsafe WeaponType Type => GetWeaponTypeFunc.InvokeUnsafe(Instance);

        /// <summary>
        /// Gets the holder of the weapon if it has one
        /// </summary>
        public Entity? Holder => GetObject<Entity>(0x9B0);

        /// <summary>
        /// Registers the given rObjCollision (.col file) for the weapon. This col can then be called by an lmt.
        /// </summary>
        /// <param name="objCollision">The col to register</param>
        /// <param name="index">The index in which to register the col file. Must be less than 8</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public unsafe void RegisterObjCollision(Resource objCollision, uint index)
        {
            if (index >= 8)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be less than 8");

            RegisterObjCollisionFunc.Invoke(ObjCollisionComponent, objCollision.Instance, index, true);
        }

        internal nint ObjCollisionComponent => Get<nint>(0x6A8);

        private static readonly NativeFunction<nint, WeaponType> GetWeaponTypeFunc = new(0x141f61470);
        private static readonly NativeAction<nint, nint, uint, bool> RegisterObjCollisionFunc = new(0x1412dddf0);
    }
}
