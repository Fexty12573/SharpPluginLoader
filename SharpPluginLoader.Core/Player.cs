using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Memory;

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

        private static readonly NativeFunction<nint, nint> FindMasterPlayerFunc = new(0x141b41240);
    }
}
