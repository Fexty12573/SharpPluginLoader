using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Resources;
using SharpPluginLoader.Core.Resources.Animation;

namespace PlayerAnimationViewer
{
    public static unsafe class MotionListExtension
    {
        /// <summary>
        /// Serializes a MotionList to a file.
        /// The basic ayout of the file is as follows:
        /// <code>
        /// - Header
        /// - Motion Offsets[]
        /// - Motion[0]
        ///     - Motion Header
        ///     - Motion Params[]
        ///     - Motion Param Bounds[]
        ///     - Motion Param Buffers[]
        ///     - Motion Metadata
        ///     - Motion Metadata Params[]
        ///     - Motion Metadata Param Members[]
        ///     - Motion Metadata Keyframes[]
        /// - Motion[1] ...
        /// </code>
        /// </summary>
        /// <param name="lmt"></param>
        /// <param name="path"></param>
        public static void Serialize(this MotionList lmt, string path)
        {
            var dir = Path.GetDirectoryName(path);
            if (dir != null && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            var ms = new FileStream(path, FileMode.Create, FileAccess.ReadWrite);
            var w = new BinaryWriter(ms);

            w.Write(lmt.Header.Magic);
            w.Write(lmt.Header.Version);
            w.Write(lmt.Header.MotionCount);
            w.Write(lmt.Header.Unknown1);
            w.Write(0u); // Padding
            
            var motionOffsets = new List<long>();
            var offsetsOffset = ms.Position;
            for (var i = 0; i < lmt.Header.MotionCount; ++i)
                 w.Write(0ul); // Placeholder for motion offsets

            // Serialize Motion Headers
            for (var i = 0; i < lmt.Header.MotionCount; ++i)
            {
                if (!lmt.Header.HasMotion(i))
                {
                    motionOffsets.Add(0);
                    continue;
                }
                
                motionOffsets.Add(ms.Position);
                SerializeMotion(ref lmt.Header.GetMotion(i), w);
            }

            ms.Position = offsetsOffset;
            foreach (var offset in motionOffsets)
                w.Write(offset);
        }

        private static void SerializeMotion(ref Motion motion, BinaryWriter w)
        {
            var startOffset = w.BaseStream.Position;
            w.Write(0ul); // Placeholder for param offset
            w.Write(motion.ParamNum);
            w.Write(motion.FrameNum);
            w.Write(motion.LoopFrame);
            w.Write(Zeros, 0, 12); // Padding
            w.Write(motion.BaseTransform.X);
            w.Write(motion.BaseTransform.Y);
            w.Write(motion.BaseTransform.Z);
            w.Write(0u); // Padding
            w.Write(motion.BaseQuat.X);
            w.Write(motion.BaseQuat.Y);
            w.Write(motion.BaseQuat.Z);
            w.Write(motion.BaseQuat.W);
            w.Write(motion.Flags);
            w.Write(Zeros, 0, 20); // Padding
            w.Write(0ul); // Placeholder for metadata offset

            w.AlignTo(16);

            // Serialize Params
            var motParams = new Span<MotionParam>((void*)motion.Params, (int)motion.ParamNum);
            var motionParamOffset = w.BaseStream.Position;
            var paramOffsets = new List<long>();
            foreach (ref var param in motParams)
            {
                paramOffsets.Add(w.BaseStream.Position);
                SerializeMotionParam(ref param, w);
            }

            w.AlignTo(16);

            // Serialize Param Bounds
            var boundsOffsets = new List<long>();
            for (var i = 0; i < motParams.Length; ++i)
            {
                ref var param = ref motParams[i];
                if (param.Bounds == null)
                {
                    boundsOffsets.Add(0);
                    continue;
                }

                boundsOffsets.Add(w.BaseStream.Position);
                w.Write(param.Bounds->Addin[0]);
                w.Write(param.Bounds->Addin[1]);
                w.Write(param.Bounds->Addin[2]);
                w.Write(param.Bounds->Addin[3]);
                w.Write(param.Bounds->Offset[0]);
                w.Write(param.Bounds->Offset[1]);
                w.Write(param.Bounds->Offset[2]);
                w.Write(param.Bounds->Offset[3]);
            }

            w.AlignTo(16);

            // Serialize Param Buffers
            var bufferOffsets = new List<long>();
            for (var i = 0; i < motParams.Length; ++i)
            {
                ref var param = ref motParams[i];
                if (param.Buffer == null)
                {
                    Log.Warn($"Null Motion Param Buffer for Motion @ 0x{startOffset:X}, Param[{i}]");
                    bufferOffsets.Add(0);
                    continue;
                }

                bufferOffsets.Add(w.BaseStream.Position);
                w.Write(new ReadOnlySpan<byte>(param.Buffer, param.BufferSize));
            }

            w.AlignTo(16);
            var endOffset = w.BaseStream.Position;

            if (!Unsafe.IsNullRef(ref motion.Metadata))
            {
                // Serialize Metadata
                var metadataOffset = w.BaseStream.Position;
                SerializeMetadata(ref motion.Metadata, w);
                w.AlignTo(16);

                var metadataParamOffset = w.BaseStream.Position;

                // Serialize Metadata Params
                var metadataParamOffsets = new List<long>();
                for (var i = 0; i < motion.Metadata.ParamNum; ++i)
                {
                    metadataParamOffsets.Add(w.BaseStream.Position);
                    SerializeMetadataParam(ref motion.Metadata.Params[i], w);
                }

                w.AlignTo(16);

                // Serialize Metadata Param Members
                var metadataParamMemberOffsets = new List<List<long>>();
                for (var i = 0; i < motion.Metadata.ParamNum; ++i)
                {
                    metadataParamMemberOffsets.Add([]);
                    ref var param = ref motion.Metadata.Params[i];
                    if (param.MemberNum == 0)
                        continue;

                    foreach (ref var member in param.Members)
                    {
                        metadataParamMemberOffsets[i].Add(w.BaseStream.Position);
                        SerializeMetadataParamMember(ref member, w);
                    }
                }

                w.AlignTo(16);

                // Serialize Metadata Param Member Keyframes
                var metadataParamMemberKeyframeOffsets = new List<List<long>>();
                for (var i = 0; i < motion.Metadata.ParamNum; ++i)
                {
                    metadataParamMemberKeyframeOffsets.Add([]);
                    ref var param = ref motion.Metadata.Params[i];

                    for (var j = 0; j < param.MemberNum; ++j)
                    {
                        ref var member = ref param.Members[j];
                        if (member.KeyframeNum == 0)
                        {
                            metadataParamMemberKeyframeOffsets[i].Add(0);
                            continue;
                        }

                        metadataParamMemberKeyframeOffsets[i].Add(w.BaseStream.Position);

                        foreach (ref var keyframe in member.Keyframes)
                        {
                            w.Write(keyframe.UIntValue);
                            w.Write(keyframe.BounceForwardLimit);
                            w.Write(keyframe.BounceBackLimit);
                            w.Write(keyframe.Frame);
                            w.Write((ushort)keyframe.ApplyType);
                            w.Write((ushort)keyframe.InterpolationType);
                        }
                    }
                }

                w.AlignTo(16);
                endOffset = w.BaseStream.Position;

                // Metadata offset
                w.DoAt(startOffset + 0x58, _ => w.Write(metadataOffset));
                w.DoAt(metadataOffset, _ => w.Write(metadataParamOffset));

                // Metadata param offsets
                for (var i = 0; i < metadataParamOffsets.Count; ++i)
                {
                    w.BaseStream.Position = metadataParamOffsets[i];
                    w.Write(metadataParamMemberOffsets[i].Count > 0 ? metadataParamMemberOffsets[i][0] : 0L);

                    for (var j = 0; j < metadataParamMemberOffsets[i].Count; ++j)
                    {
                        w.BaseStream.Position = metadataParamMemberOffsets[i][j];
                        w.Write(metadataParamMemberKeyframeOffsets[i][j]);
                    }
                }
            }

            // Write offsets
            // First motion param offset
            w.DoAt(startOffset + 0x00, _ => w.Write(motionParamOffset));

            // Offsets for each motion param (buffers, bounds)
            for (var i = 0; i < paramOffsets.Count; ++i)
            {
                w.BaseStream.Position = paramOffsets[i] + 0x10;
                w.Write(bufferOffsets[i]);
                w.BaseStream.Position = paramOffsets[i] + 0x28;
                w.Write(boundsOffsets[i]);
            }

            w.BaseStream.Position = endOffset; // Go back to end of motion to continue writing
        }

        private static void SerializeMotionParam(ref MotionParam param, BinaryWriter w)
        {
            w.Write(param.Type);
            w.Write(param.Usage);
            w.Write(param.JointType);
            w.Write(param.JointCount);
            w.Write(param.BoneId);
            w.Write(param.Weight);
            w.Write(param.BufferSize);
            w.Write(0ul); // Placeholder for buffer offset
            w.Write(param.ReferenceFrame[0]);
            w.Write(param.ReferenceFrame[1]);
            w.Write(param.ReferenceFrame[2]);
            w.Write(param.ReferenceFrame[3]);
            w.Write(0ul); // Placeholder for bounds offset
        }

        private static void SerializeMetadata(ref Metadata metadata, BinaryWriter w)
        {
            w.Write(0ul); // Placeholder for param offset
            w.Write(metadata.ParamNum);
            w.Write(Zeros, 0, 12); // Padding
            w.Write(metadata.FrameCount);
            w.Write(metadata.LoopStart);
            w.Write(metadata.LoopValue);
            w.Write(metadata.Hash);
        }

        private static void SerializeMetadataParam(ref MetadataParam param, BinaryWriter w)
        {
            w.Write(0ul); // Placeholder for member offset
            w.Write(param.MemberNum);
            w.Write(0u); // Padding
            w.Write(param.Hash);
            w.Write(param.UniqueId);
        }

        private static void SerializeMetadataParamMember(ref MetadataParamMember member, BinaryWriter w)
        {
            w.Write(0ul); // Placeholder for keyframe offset
            w.Write(member.KeyframeNum);
            w.Write(0u); // Padding
            w.Write(member.Hash);
            w.Write(member.KeyframeType);
        }

        private static void AlignTo(this BinaryWriter w, int alignment)
        {
            var pad = alignment - (w.BaseStream.Position % alignment);
            if (pad != alignment)
                w.Write(Zeros, 0, (int)pad);
        }

        private static void DoAt(this BinaryWriter w, long offset, Action<BinaryWriter> action)
        {
            var pos = w.BaseStream.Position;
            w.BaseStream.Position = offset;
            action(w);
            w.BaseStream.Position = pos;
        }

        private static readonly byte[] Zeros = new byte[128];
    }
}
