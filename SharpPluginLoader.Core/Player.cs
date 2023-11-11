using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.MtTypes;

namespace SharpPluginLoader.Core
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

        public override void CreateShell(uint index, MtVector3 target, MtVector3? origin = null)
        {
            base.CreateShell(index, target, origin);
        }

        private static readonly NativeFunction<nint, nint> FindMasterPlayerFunc = new(0x141b41240);
    }
}
