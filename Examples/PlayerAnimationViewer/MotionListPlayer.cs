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
    public class MotionListPlayer : MtObject
    {
        public MotionListPlayer(nint instance) : base(instance) { }
        public MotionListPlayer() { }

        public MotionList? ActiveMotionList => GetObject<MotionList>(0x8);

        public TimlPlayer TimlPlayer => GetInlineObject<TimlPlayer>(0x10);

        public AnimationId CurrentAnimationId => Get<AnimationId>(0x50);
    }

    public unsafe class TimlPlayer : MtObject
    {
        public TimlPlayer(nint instance) : base(instance) { }
        public TimlPlayer() { }

        public Metadata* ActiveTrack => GetPtr<Metadata>(0x8);

        public Span<LmtParamMember> MemberPool => new(GetPtr<nint>(0x10), Get<int>(0x18));

        public ObjectList? ObjectList => GetObject<ObjectList>(0x20);

        public Span<LmtParamMember> MemberBuffer => new(GetPtr<nint>(0x28), Get<int>(0x30));

        public float CurrentFrame => Get<float>(0x34);

        public float LoopStart => Get<float>(0x38);
    }

    public unsafe class ObjectList : MtObject
    {
        public ObjectList(nint instance) : base(instance) { }
        public ObjectList() { }

        public Span<nint> Objects => new(GetPtr<nint>(0x8), Get<int>(0x20));

        public ref LmtParamMemberDefPool MemberDefPool => ref GetRefInline<LmtParamMemberDefPool>(0x28);
    }

    public unsafe class TimelineObject : MtObject
    {
        public TimelineObject(nint instance) : base(instance) { }
        public TimelineObject() { }

        public Span<uint> Flags => new(GetPtrInline<uint>(0x8), 8);

        public Span<uint> Triggers => new(GetPtrInline<uint>(0x28), 8);

        public Span<LmtParamMemberDef> MemberList => new(GetPtr<nint>(0x50), Get<int>(0x58));
    }
}
