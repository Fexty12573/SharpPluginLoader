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
    public class Weapon : Model
    {
        public Weapon(nint instance) : base(instance) { }
        public Weapon() { }

        public unsafe WeaponType Type => GetWeaponTypeFunc.InvokeUnsafe(Instance);

        public Entity? Holder => GetObject<Entity>(0x9B0);

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
