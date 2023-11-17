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
    public class MotionList : Resource
    {
        public MotionList(nint instance) : base(instance) { }
        public MotionList() { }

        public unsafe ref MotionListHeader Header => ref *(MotionListHeader*)Get<nint>(0xA8);
    }
}
