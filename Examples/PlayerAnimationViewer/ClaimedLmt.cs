using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Resources;
using SharpPluginLoader.Core.Resources.Animation;

namespace PlayerAnimationViewer
{
    public unsafe class ClaimedLmt(MotionList lmt) : IDisposable
    {
        public MotionList Lmt { get; init; } = lmt;

        public List<NativeArray<MetadataKeyframe>> ModifiedKeyframeLists { get; } = [];
        public List<NativeArray<MetadataParamMember>> ModifiedParamMemberLists { get; } = [];
        public List<NativeArray<MetadataParam>> ModifiedParamLists { get; } = [];
        public NativeArray<Motion> CustomMotionsList { get; private set; }
        public bool HasCustomMotions => CustomMotionsList.Length > 0;

        private MotionListHeader* _customHeader = null;
        private NativeArray<nint> _motionPointers = default;
        private readonly HashSet<MotionList> _referencedMotionLists = [];

        public void AddCustomMotion(ref Motion motion, MotionList sourceList)
        {
            if (Unsafe.IsNullRef(ref motion))
                return;

            _referencedMotionLists.Add(sourceList);

            EnsureMotionCapacity();

            var motionCount = Lmt.Header.MotionCount;
            CustomMotionsList[motionCount] = motion;
            _motionPointers[motionCount] = MemoryUtil.AddressOf(ref CustomMotionsList[motionCount]);
            Lmt.Header.MotionCount = (ushort)(motionCount + 1);
        }

        public void OverrideMotion(ref Motion motion, int id, MotionList sourceList)
        {
            if (Unsafe.IsNullRef(ref motion))
                return;

            if (id >= Lmt.Header.MotionCount)
                return;

            _referencedMotionLists.Add(sourceList);
            if (motion.HasLazyOffsets(ref sourceList.Header))
                motion.ResolveLazyOffsets(ref sourceList.Header);

            EnsureMotionCapacity();

            CustomMotionsList[id] = motion;
            _motionPointers[id] = MemoryUtil.AddressOf(ref CustomMotionsList[id]);
        }

        public void AddCustomMotions(MotionList sourceList, ReadOnlySpan<int> ids)
        {
            foreach (var id in ids)
            {
                ref var motion = ref sourceList.Header.GetMotion(id);
                AddCustomMotion(ref motion, sourceList);
            }
        }

        private void EnsureMotionCapacity()
        {
            if (_customHeader == null)
            {
                _customHeader = (MotionListHeader*)MemoryUtil.Alloc(
                    // Not doing +1 here because sizeof(MotionListHeader) already includes the first motion pointer
                    sizeof(MotionListHeader) + Lmt.Header.MotionCount * sizeof(Motion*)
                );
                CustomMotionsList = NativeArray<Motion>.Create(Lmt.Header.MotionCount + 1);
                _motionPointers = new NativeArray<nint>((nint)_customHeader + 0x10, Lmt.Header.MotionCount);
                *_customHeader = Lmt.Header;

                for (var i = 0; i < Lmt.Header.MotionCount; i++)
                {
                    ref var motion = ref Lmt.Header.GetMotion(i);
                    if (Unsafe.IsNullRef(ref motion))
                    {
                        _motionPointers[i] = 0;
                        continue;
                    }

                    CustomMotionsList[i] = motion;
                    _motionPointers[i] = MemoryUtil.AddressOf(ref CustomMotionsList[i]);
                }

                Lmt.SetPtr(0xA8, _customHeader);
            }
            else if (Lmt.Header.MotionCount >= CustomMotionsList.Length)
            {
                CustomMotionsList.Resize(CustomMotionsList.Length + 32);
                _customHeader = (MotionListHeader*)MemoryUtil.Realloc(
                    (nint)_customHeader,
                    // -1 because sizeof(MotionListHeader) already includes the first motion pointer
                    sizeof(MotionListHeader) + (CustomMotionsList.Length - 1) * sizeof(Motion*)
                );
                _motionPointers = new NativeArray<nint>((nint)_customHeader + 0x10, CustomMotionsList.Length);

                for (var i = 0; i < CustomMotionsList.Length; i++)
                {
                    _motionPointers[i] = MemoryUtil.AddressOf(ref CustomMotionsList[i]);
                }

                Lmt.SetPtr(0xA8, _customHeader);
            }
        }

        ~ClaimedLmt()
        {
            ReleaseUnmanagedResources();
        }

        private void ReleaseUnmanagedResources()
        {
            foreach (var list in ModifiedKeyframeLists)
                list.Dispose();

            foreach (var list in ModifiedParamMemberLists)
                list.Dispose();

            CustomMotionsList.Dispose();
            _motionPointers.Dispose();

            _referencedMotionLists.Clear();
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_motions")]
        private static extern ref nint Motions(ref MotionListHeader header);
    }

    [StructLayout(LayoutKind.Explicit, Size = 0x18)]
    internal struct MotionListHeader2
    {
        [FieldOffset(0x00)] public uint Magic;
        [FieldOffset(0x04)] public ushort Version;
        [FieldOffset(0x06)] public ushort MotionCount;
        [FieldOffset(0x08)] public uint Unknown1;
        [FieldOffset(0x10)] internal nint _motions;
    }
}
