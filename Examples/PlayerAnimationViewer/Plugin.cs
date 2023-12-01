using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using ImGuiNET;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.IO;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Models;
using SharpPluginLoader.Core.Rendering;
using SharpPluginLoader.Core.Resources;
using SharpPluginLoader.Core.Resources.Animation;

namespace PlayerAnimationViewer
{
    public unsafe class Plugin : IPlugin
    {
        public string Name => "Player Animation Viewer";

        private delegate nint CreateShellDelegate(nint shellParam, nint source1, nint source2, nint data);
        private bool _speedLocked;

        #region Entity Picker
        private Model? _selectedModel;
        private MtDti? _selectedDti;
        private AnimationLayerComponent? _selectedAnimLayer;
        private MotionListPlayer? _lmtPlayer;
        private TimlPlayer? _timlPlayer;
        #endregion
        #region Animation Viewer
        private AnimationId _animationId = new(12, 0);
        private float _interFrame;
        private float _startFrame2;
        private float _startSpeed = 1f;

        private bool _lockInterFrame;
        private bool _lockStartFrame;
        private bool _lockAnimationId;
        private float _lockedInterFrame;
        private float _lockedStartFrame;
        private AnimationId _lockedAnimationId;
        #endregion
        #region LMT Editor
        private MotionList? _selectedLmt;
        private int _timelineFlags;
        private readonly Dictionary<uint, bool> _timelineGroupExpandedMap = [];
        private MetadataKeyframe* _selectedKeyframe = null;
        private PropType _selectedKeyframeType;
        private readonly float[] _keyframeBuffer = new float[50]; // More than 50 keyframes is unlikely
        private float _framePointer = 0f;
        #endregion

        public PluginData OnLoad()
        {
            return new PluginData
            {
                OnUpdate = true,
                OnRender = true,
                OnEntityAnimation = true
            };
        }

        public void OnUpdate(float deltaTime)
        {
            if (Monster.SingletonInstance == 0)
                return;

            if (Input.IsDown(Key.LeftControl) && Input.IsPressed(Key.L))
            {
                LogProperties(new MtObject(Monster.SingletonInstance));
            }

            var player = Player.MainPlayer;
            if (player == null)
                return;

            if (Input.IsDown(Key.LeftControl) && Input.IsPressed(Key.D))
                DumpLmtParams("nativePC/plugins/CSharp/PlayerAnimationViewer/LmtParams");
            
            if (!Input.IsDown(Button.L2)) 
                return;

            if (Input.IsPressed(Button.R3))
            {
                if (!TogglePause(player))
                    return;
            }

            if (Input.IsPressed(Button.Right))
                player.AnimationLayer!.CurrentFrame += 1;

            if (Input.IsPressed(Button.Left))
                player.AnimationLayer!.CurrentFrame -= 1;

            if (Input.IsPressed(Button.Up))
                player.AnimationLayer!.CurrentFrame += 10;

            if (Input.IsPressed(Button.Down))
                player.AnimationLayer!.CurrentFrame -= 10;

            var actionController = player.ActionController;
            if (Input.IsPressed(Button.Circle))
                Log.Info($"Action: {actionController.CurrentAction} Animation: {player.CurrentAnimation} Frame: {player.AnimationLayer!.CurrentFrame}");
        }

        public void OnRender()
        {
            if (ImGui.BeginCombo("Selected Entity", _selectedDti?.Name ?? "None"))
            {
                var entities = GetEntityList();
                var dtis = entities.Select(e => e.GetDti()).ToArray();

                for (var i = 0; i < dtis.Length; i++)
                {
                    if (ImGui.Selectable(dtis[i]?.Name ?? "N/A", dtis[i] == _selectedDti))
                    {
                        _selectedDti = dtis[i]!;
                        _selectedModel = entities[i];
                        _selectedAnimLayer = _selectedModel?.AnimationLayer;

                        Ensure.NotNull(_selectedAnimLayer);

                        _interFrame = 0f;
                        _startFrame2 = 0f;
                        _startSpeed = 1f;

                        _animationId = _selectedDti.InheritsFrom("uWeapon")
                            ? new AnimationId(0, 0) 
                            : new AnimationId(12, 0);

                        _lmtPlayer = GetLmtPlayer(_selectedAnimLayer!);
                        _timlPlayer = _lmtPlayer.TimlPlayer;
                        _timelineGroupExpandedMap.Clear();
                    }
                }

                ImGui.EndCombo();
            }

            if (_selectedModel == null || _selectedAnimLayer == null)
            {
                ImGui.TextColored(new Vector4(1, 1, 0, 1), "No Entity selected");
                return;
            }

            if (ImGui.Begin("Animation Viewer"))
            {
                var lmt = (int)_animationId.Lmt;
                var anim = (int)_animationId.Id;
                ImGui.InputInt("LMT", ref lmt);
                ImGui.InputInt("Animation", ref anim);
                ImGui.InputFloat("Inter Frame", ref _interFrame);
                ImGui.InputFloat("Start Frame", ref _startFrame2);
                ImGui.InputFloat("Start Speed", ref _startSpeed);
                _animationId = new AnimationId((uint)lmt, (uint)anim);
                if (ImGui.Button("Force Animation"))
                    _selectedAnimLayer.DoAnimation(_animationId, _interFrame, _startFrame2, _startSpeed);

                ImGui.NewLine();

                ImGui.Text($"Current Animation: {_selectedModel.CurrentAnimation}");
                ImGui.NewLine();

                if (ImGui.CollapsingHeader("Frame Viewer"))
                {
                    ImGui.Text($"Current Frame: {_selectedAnimLayer.CurrentFrame:.02}/{_selectedAnimLayer.MaxFrame}");
                    ImGui.SliderFloat("Frame", ref _selectedAnimLayer.CurrentFrame, 0f, _selectedAnimLayer.MaxFrame);

                    var paused = _selectedAnimLayer.Paused;
                    if (ImGui.Checkbox("Paused", ref paused))
                        _selectedAnimLayer.Paused = paused;

                    ImGui.Text($"Speed: {_selectedAnimLayer.Speed}");
                    ImGui.SliderFloat("Speed", ref _selectedAnimLayer.Speed, 0, 10);

                    if (ImGui.Checkbox("Lock Speed", ref _speedLocked))
                    {
                        if (_speedLocked)
                            _selectedAnimLayer.LockSpeed(_selectedAnimLayer.Speed);
                        else
                            _selectedAnimLayer.UnlockSpeed();
                    }
                }

                if (ImGui.CollapsingHeader("Utilities"))
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, new Vector4(1, 0.5f, 0.5f, 1));
                    ImGui.TextWrapped("Note: These features only work for Players/Monsters.");
                    ImGui.PopStyleColor();
                    ImGui.NewLine();

                    ImGui.Checkbox("##lock-animid", ref _lockAnimationId);
                    if (ImGui.BeginItemTooltip())
                    {
                        ImGui.Text("Lock Value");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    var animIdBuffer = stackalloc uint[2] { _lockedAnimationId.Lmt, _lockedAnimationId.Id };
                    if (ImGui.InputScalarN("Next Animation ID", ImGuiDataType.U32, (nint)animIdBuffer, 2))
                        _lockedAnimationId = new AnimationId(animIdBuffer[0], animIdBuffer[1]);

                    ImGui.Checkbox("##lock-interframe", ref _lockInterFrame);
                    if (ImGui.BeginTooltip())
                    {
                        ImGui.Text("Lock Value");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.InputFloat("Next InterFrame", ref _lockedInterFrame);

                    ImGui.Checkbox("##lock-startframe", ref _lockStartFrame);
                    if (ImGui.BeginTooltip())
                    {
                        ImGui.Text("Lock Value");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.InputFloat("Next StartFrame", ref _lockedStartFrame);
                }
                
                ImGui.End();
            }

            if (ImGui.TreeNode("LMT Editor"))
            {
                if (ImGui.TreeNode("Config"))
                {
                    ImGui.CheckboxFlags("Enable Pointer Snapping", ref _timelineFlags,
                        (int)ImGuiTimelineFlags.EnableFramePointerSnapping);
                    if (ImGui.BeginItemTooltip())
                    {
                        ImGui.Text("Always snaps the Frame Pointer to the nearest frame.");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.CheckboxFlags("Enable Keyframe Snapping", ref _timelineFlags,
                        (int)ImGuiTimelineFlags.EnableKeyframeSnapping);
                    if (ImGui.BeginItemTooltip())
                    {
                        ImGui.Text("Always snaps Keyframes to the nearest frame.");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.CheckboxFlags("Extend Frame Pointer", ref _timelineFlags,
                        (int)ImGuiTimelineFlags.ExtendFramePointer);
                    if (ImGui.BeginItemTooltip())
                    {
                        ImGui.Text("Extends the line of the Frame Pointer vertically across the entire timeline.");
                        ImGui.EndTooltip();
                    }
                    ImGui.CheckboxFlags("Extend Frame Markers", ref _timelineFlags,
                        (int)ImGuiTimelineFlags.ExtendFrameMarkers);
                    if (ImGui.BeginItemTooltip())
                    {
                        ImGui.Text("Extends the line of the Frame Markers vertically across the entire timeline.");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.CheckboxFlags("Show Selected Keyframe Markers", ref _timelineFlags,
                        (int)ImGuiTimelineFlags.ShowSelectedKeyframeMarkers);
                    if (ImGui.BeginItemTooltip())
                    {
                        ImGui.Text("Shows a marker on the currently selected Keyframe, spanning vertically across the entire timeline.");
                        ImGui.EndTooltip();
                    }

                    ImGui.TreePop();
                }

                if (_lmtPlayer == null || _timlPlayer == null)
                {
                    ImGui.TextColored(new Vector4(1, 1, 0, 1), "No LMT Player");
                    ImGui.TreePop();
                    return;
                }

                ImGui.Separator();

                if (ImGui.BeginCombo("LMT", _selectedLmt?.FilePath ?? "None"))
                {
                    foreach (var lmt in _selectedModel.MotionLists)
                    {
                        if (ImGui.Selectable(lmt.FilePath, lmt == _selectedLmt))
                            _selectedLmt = lmt;
                    }

                    ImGui.EndCombo();
                }

                if (_selectedLmt == null)
                {
                    ImGui.TextColored(new Vector4(1, 1, 0, 1), "No LMT selected");
                    ImGui.TreePop();
                    return;
                }

                ImGui.Separator();
                if (ImGui.Button("Save to File"))
                {
                    _selectedLmt.Serialize($"nativePC\\{_selectedLmt.FilePath}.modified.lmt");
                }

                if (ImGui.BeginItemTooltip())
                {
                    ImGui.Text($"Saves the LMT to 'nativePC\\{_selectedLmt.FilePath}.modified.lmt'");
                    ImGui.EndTooltip();
                }

                ref var header = ref _selectedLmt.Header;
                for (var i = 0; i < header.MotionCount; ++i)
                {
                    if (!header.HasMotion(i))
                        continue;

                    ref var motion = ref _selectedLmt.Header.GetMotion(i);
                    var name = $"Motion: Id:{i}, Bones: {motion.ParamNum}, Frames: {motion.FrameNum}";
                    if (ImGui.TreeNode(name))
                    {
                        var fp = _framePointer;
                        if (ImGuiExtensions.BeginTimeline(name, 0f, motion.FrameNum,
                                ref _selectedModel.CurrentAnimation.Id == (uint)i ? ref _selectedAnimLayer.CurrentFrame : ref _framePointer, 
                                (ImGuiTimelineFlags)_timelineFlags))
                        {
                            foreach (ref var param in motion.Metadata.Params)
                            {
                                var dti = param.Dti;
                                var objList = _timlPlayer.ObjectList;
                                if (objList == null)
                                    continue;

                                TimelineObject? obj = null;
                                foreach (var objPtr in objList.Objects)
                                {
                                    var obj2 = new TimelineObject(objPtr);
                                    if (obj2.GetDti() == dti)
                                    {
                                        obj = obj2;
                                        break;
                                    }
                                }

                                var props = obj?.GetProperties();
                                if (dti == null || props == null || obj == null)
                                    continue;

                                if (!_timelineGroupExpandedMap.TryGetValue(param.Hash, out var expanded))
                                {
                                    _timelineGroupExpandedMap[param.Hash] = false;
                                    expanded = false;
                                }

                                if (ImGuiExtensions.BeginTimelineGroup(dti.Name, ref expanded))
                                {
                                    foreach (ref var member in param.Members)
                                    {
                                        var prop = props.FindProperty(member.Hash);
                                        if (prop == null)
                                            continue;

                                        var memberName = prop.HashName;
                                        for (var k = 0; k < member.KeyframeNum; k++)
                                            _keyframeBuffer[k] = member.GetKeyframe(k).Frame;

                                        if (ImGuiExtensions.TimelineTrack(memberName, _keyframeBuffer,
                                                out var selectedKeyframe, member.KeyframeNum))
                                        {
                                            for (var k = 0; k < member.KeyframeNum; k++)
                                                member.GetKeyframe(k).Frame = _keyframeBuffer[k];
                                        }

                                        if (selectedKeyframe != -1)
                                        {
                                            _selectedKeyframe = MemoryUtil.AsPointer(ref member.GetKeyframe(selectedKeyframe));
                                            _selectedKeyframeType = prop.Type;
                                        }
                                    }
                                    
                                    ImGuiExtensions.EndTimelineGroup();
                                }

                                _timelineGroupExpandedMap[param.Hash] = expanded;
                            }

                            ImGuiExtensions.EndTimeline();
                            ImGui.NewLine();
                        }

                        if (_selectedKeyframe != null)
                        {
                            switch (_selectedKeyframeType)
                            {
                                case PropType.Bool:
                                    ImGui.Checkbox("Value", ref _selectedKeyframe->BoolValue);
                                    break;
                                case PropType.S32:
                                    ImGui.InputInt("Value", ref _selectedKeyframe->IntValue);
                                    break;
                                case PropType.U32:
                                    ImGuiExtensions.InputScalar("Value", ref _selectedKeyframe->UIntValue);
                                    break;
                                case PropType.Color:
                                {
                                    var vec4 = _selectedKeyframe->ColorValue.ToVector4();
                                    ImGui.ColorEdit4("Value", ref vec4);
                                    _selectedKeyframe->ColorValue = vec4;
                                    break;
                                }
                                case PropType.F32:
                                    ImGui.InputFloat("Value", ref _selectedKeyframe->FloatValue);
                                    break;
                                default:
                                    ImGui.Text($"Unsupported type: {_selectedKeyframeType}");
                                    break;
                            }

                            ImGui.Separator();
                            ImGui.InputFloat("Bounce Forward Limit", ref _selectedKeyframe->BounceForwardLimit);
                            ImGui.InputFloat("Bounce Back Limit", ref _selectedKeyframe->BounceBackLimit);
                            ImGui.Separator();
                            ImGui.InputFloat("Frame", ref _selectedKeyframe->Frame);
                            ImGuiExtensions.InputScalar("Flags Mask", ref _selectedKeyframe->FlagsMask);
                            ImGui.Separator();
                            var applyType = (int)_selectedKeyframe->ApplyType;
                            var interpType = (int)_selectedKeyframe->InterpolationType;
                            ImGui.Combo("Apply Type", ref applyType, Enum.GetNames(typeof(ApplyType)), (int)ApplyType.Count);
                            ImGui.Combo("Interpolation Type", ref interpType, Enum.GetNames(typeof(InterpType)), (int)InterpType.Count);
                            _selectedKeyframe->ApplyType = (ApplyType)applyType;
                            _selectedKeyframe->InterpolationType = (InterpType)interpType;
                        }

                        ImGui.TreePop();
                    }
                }

                ImGui.TreePop();
            }
        }

        public void OnEntityAnimation(Entity entity, ref AnimationId animId, ref float startFrame, ref float interFrame)
        {
            if (entity != _selectedModel)
                return;

            if (_lockAnimationId)
                animId = _lockedAnimationId;
            if (_lockInterFrame)
                interFrame = _lockedInterFrame;
            if (_lockStartFrame)
                startFrame = _lockedStartFrame;
        }

        private static bool TogglePause(Model model)
        {
            if (model.AnimationLayer == null)
                return false;

            return model.AnimationLayer.Paused = !model.AnimationLayer.Paused;
        }

        private static void DumpLmtParams(string dir)
        {
            Log.Info("Dumping LMT params");

            var memberDefPool = stackalloc LmtParamMemberDefPool[1];
            var populateParamMemberDefs = new NativeAction<nint, nint>(0x14231df70);
            var paramTypeCountAddr = (nint)0x1451c39e8;
            var paramTypesAddr = (nint)0x1451c39e0;
            var paramTypes = paramTypesAddr.Read<nint>()
                .ReadStructArray<LmtParamType>(0, paramTypeCountAddr.Read<int>());

            Directory.CreateDirectory(dir);

            var fullSb = new StringBuilder();

            foreach (var type in paramTypes)
            {
                var dti = type.Dti;
                var memberDefs = new LmtParamMemberDef[type.PropCount];
                var obj = dti.CreateInstance<MtObject>();
                var fullId = ((ulong)dti.Id << 32) | type.UniqueId;

                fixed (LmtParamMemberDef* memberDefsPtr = memberDefs)
                {
                    memberDefPool->Pool = memberDefsPtr;
                    memberDefPool->PoolSize = type.PropCount * (uint)sizeof(LmtParamMemberDef);
                    memberDefPool->UsedSize = 0;
                    memberDefPool->AllocatorIndex = 0;

                    populateParamMemberDefs.Invoke(obj.Instance, (nint)memberDefPool);
                }

                var sb = new StringBuilder();
                sb.AppendLine($"LMT Properties for {dti.Name}");
                sb.AppendLine($"Hash: 0x{dti.Id:X}, Unique ID: {type.UniqueId}, Full ID: {fullId}");
                foreach (var memberDef in memberDefs)
                {
                    var name = memberDef.Comment != "" ? memberDef.Comment : memberDef.Name;
                    sb.Append($"    [{memberDef.Type}] {name}, Hash: 0x{Utility.Crc32(name):X}");
                    if (memberDef.Comment != "")
                        sb.Append($", FieldName: {memberDef.Name}");
                    sb.AppendLine();
                }

                fullSb.AppendLine(sb.ToString());
                File.WriteAllText(Path.Combine(dir, $"{dti.Name.Replace("::", "_")}.txt"), sb.ToString());
            }
            
            File.WriteAllText(Path.Combine(dir, "LMT_Metadata.txt"), fullSb.ToString());
        }

        public void LogProperties(MtObject? obj)
        {
            if (obj == null)
                return;

            var props = obj.GetProperties();
            if (props == null)
                return;

            var dti = obj.GetDti();
            if (dti == null)
                return;

            Log.Info($"Properties for {dti.Name} (0x{dti.Id:X}):");
            foreach (var prop in props)
            {
                var get = prop.IsProperty ? prop.Get : prop.Get - obj.Instance;
                Log.Info($"    [{prop.Type}] {prop.Name}, Offset:0x{get:X}, CRC32: 0x{Utility.Crc32(prop.Name):X}");
            }
        }

        private static List<Model> GetEntityList()
        {
            var list = new List<Model>();
            var player = Player.MainPlayer;
            if (player != null)
            {
                list.Add(player);

                var weapon = player.CurrentWeapon;
                if (weapon != null)
                    list.Add(weapon);
            }
                
            list.AddRange(Monster.GetAllMonsters());
            return list;
        }

        private static MotionListPlayer GetLmtPlayer(AnimationLayerComponent animLayer)
        {
            // animLayer + 0x80 -> MotionLayer[4]
            // MotionLayer + 0x30 -> Motion[4]
            // Motion + 0x138 -> LmtPlayer
            return animLayer.GetInlineObject<MotionListPlayer>(0x80 + 0x30 + 0x138);
        }
    }
}
