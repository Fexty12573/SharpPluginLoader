using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Resources;
using SharpPluginLoader.Core.Resources.Animation;

namespace PlayerAnimationViewer
{
    public class ClaimedLmt(MotionList lmt)
    {
        public MotionList Lmt { get; init; } = lmt;

        public List<NativeArray<MetadataKeyframe>> ModifiedKeyframeLists { get; } = [];
        public List<NativeArray<MetadataParamMember>> ModifiedParamMemberLists { get; } = [];
        public List<NativeArray<MetadataParam>> ModifiedParamLists { get; } = [];
        public NativeArray<Motion> CustomMotionsList { get; private set; } = default;
        private NativeArray<nint> MotionPointers { get; set; } = default;
        public bool HasCustomMotions => CustomMotionsList.Length > 0;

        private int _realMotionCount = 0;

        public void AddCustomMotion(ref Motion motion)
        {
            if (Unsafe.IsNullRef(ref motion))
                return;

            if (CustomMotionsList.Length == 0)
            {
                CustomMotionsList = NativeArray<Motion>.Create(Lmt.Header.MotionCount + 1);
                MotionPointers = NativeArray<nint>.Create(Lmt.Header.MotionCount + 1);

                for (int i = 0; i < Lmt.Header.MotionCount; i++)
                {
                    CustomMotionsList[i] = Lmt.Header.GetMotion(i);
                    MotionPointers[i] = MemoryUtil.AddressOf(ref CustomMotionsList[i]);
                }

                Motions(ref Lmt.Header) = MotionPointers.Address;
            }

            if (CustomMotionsList.Length >= _realMotionCount)
            {
                CustomMotionsList.Resize(CustomMotionsList.Length + 32);
            }

            CustomMotionsList[CustomMotionsList.Length - 1] = motion;
        }

        ~ClaimedLmt()
        {
            foreach (var list in ModifiedKeyframeLists)
                list.Dispose();

            foreach (var list in ModifiedParamMemberLists)
                list.Dispose();
        }

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_motions")]
        private static extern ref nint Motions(ref MotionListHeader header);
    }

    public class ExtensibleMotionList : MotionList
    {
        public ExtensibleMotionList(nint instance) : base(instance) { }
        public ExtensibleMotionList() { }



        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_motions")]
        private static extern ref nint Motions(ref MotionListHeader header);
    }
}
