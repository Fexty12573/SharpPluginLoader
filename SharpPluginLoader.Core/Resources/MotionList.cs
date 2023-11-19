using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using SharpPluginLoader.Core.Resources.Animation;

namespace SharpPluginLoader.Core.Resources
{
    /// <summary>
    /// Represents an instance of the rMotionList (lmt) class.
    /// </summary>
    public class MotionList : Resource
    {
        public MotionList(nint instance) : base(instance) { }
        public MotionList() { }

        /// <summary>
        /// The header of this motion list.
        /// </summary>
        public unsafe ref MotionListHeader Header => ref *(MotionListHeader*)Get<nint>(0xA8);
    }
}
