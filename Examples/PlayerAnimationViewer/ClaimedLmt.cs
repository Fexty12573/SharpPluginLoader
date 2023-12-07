using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Resources;
using SharpPluginLoader.Core.Resources.Animation;

namespace PlayerAnimationViewer
{
    public class ClaimedLmt(MotionList lmt)
    {
        public MotionList Lmt { get; init; } = lmt;

        public readonly List<NativeArray<MetadataKeyframe>> ModifiedKeyframeLists = [];
        public readonly List<NativeArray<MetadataParamMember>> ModifiedParamMemberLists = [];
        public readonly List<NativeArray<MetadataParam>> ModifiedParamLists = [];

        ~ClaimedLmt()
        {
            foreach (var list in ModifiedKeyframeLists)
                list.Dispose();

            foreach (var list in ModifiedParamMemberLists)
                list.Dispose();
        }
    }
}
