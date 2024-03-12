using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ImGuiNET;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Components;
using SharpPluginLoader.Core.Configuration;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.Geometry;
using SharpPluginLoader.Core.IO;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Models;
using SharpPluginLoader.Core.MtTypes;
using SharpPluginLoader.Core.Rendering;
using SharpPluginLoader.Core.Resources;
using SharpPluginLoader.Core.Resources.Animation;

namespace PlayerAnimationViewer
{
    public unsafe class Plugin : IPlugin
    {
        public string Name => "Player Animation Viewer";
        public string Author => "Fexty";

        #region Entity Picker
        private Model? _selectedModel;
        private MtDti? _selectedDti;
        private AnimationLayerComponent? _selectedAnimLayer;
        private MotionListPlayer? _lmtPlayer;
        private TimlPlayer? _timlPlayer;
        private bool _lockActions;
        private int _selectedAction = 1;
        #endregion
        #region Animation Viewer
        private bool _speedLocked;
        private float _lockedSpeed;
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
        private LmtBitMapping _lmtBitMapping = null!;
        private MotionList? _selectedLmt;
        private int _timelineFlags = (int)ImGuiTimelineFlags.EnableFramePointerSnapping
                                     | (int)ImGuiTimelineFlags.EnableKeyframeSnapping
                                     | (int)ImGuiTimelineFlags.ExtendFramePointer;
        private readonly Dictionary<uint, bool> _timelineGroupExpandedMap = [];
        private Motion* _selectedMotion = null;
        private MetadataParam* _selectedParam = null;
        private MetadataParamMember* _selectedParamMember = null;
        private MetadataKeyframe* _selectedKeyframe = null;
        private string _selectedParamName = string.Empty;
        private string _selectedParamMemberName = string.Empty;
        private PropType _selectedKeyframeType;
        private readonly float[] _keyframeBuffer = new float[50]; // More than 50 keyframes is unlikely
        private float _framePointer;

        private bool _sortKeyframes = true;
        private readonly List<ClaimedLmt> _claimedLmts = [];
        private ImFontPtr _supplementalFont;
        private int _fontLoadStatus = 0;

        private MotionList? _pendingLmt;

        private static string LmtBitMappingFile => "nativePC/plugins/CSharp/PlayerAnimationViewer/FlagMapping.json";
        private static string SupplementalFontFile => "nativePC/plugins/CSharp/PlayerAnimationViewer/CascadiaCode.ttf";

        #region Adding Keyframes
        private Motion* _addKeyframeMotion = null;
        private MetadataParam* _addKeyframeParam = null;
        private MetadataParamMember* _addKeyframeParamMember = null;
        private float _addKeyframeFrame;
        private ApplyType _addKeyframeApplyType;
        private InterpType _addKeyframeInterpType;
        #endregion

        #region Adding Params
        private readonly List<MtDti> _timlObjectDtiList = [];
        private MtDti? _addParamDti;
        private LmtParamMemberDef* _addParamDef;
        private Motion* _addParamMotion;
        private TimelineObject? _addParamTimlObject;

        private readonly NativeArray<LmtParamMemberDef> _paramMemberDefBuffer = NativeArray<LmtParamMemberDef>.Create(0x200);
        private LmtParamMemberDefPool* _paramMemberDefPool;
        private NativeArray<LmtParamType> LmtParamTypes => new(
            MemoryUtil.Read<nint>(0x1451c39e0),
            MemoryUtil.Read<int>(0x1451c39e8)
        );
        #endregion
        #endregion

        public void OnLoad()
        {
            var timlObjDti = MtDti.Find("nTimeline::Object");
            Ensure.NotNull(timlObjDti);

            _timlObjectDtiList.AddRange(timlObjDti.Children);
            _paramMemberDefPool = MemoryUtil.Alloc<LmtParamMemberDefPool>();
            _paramMemberDefPool->Pool = _paramMemberDefBuffer.Pointer;
            _paramMemberDefPool->PoolSize = _paramMemberDefBuffer.Length * sizeof(LmtParamMemberDef);
            _paramMemberDefPool->UsedSize = 0;
            _paramMemberDefPool->AllocatorIndex = 0;

            if (!File.Exists(LmtBitMappingFile))
            {
                var bitMapping = new LmtBitMapping();
                LmtBitMapping.SaveTo(LmtBitMappingFile, bitMapping);

                Log.Debug($"Created new LMT Bit Mapping file at {LmtBitMappingFile}");
            }

            _lmtBitMapping = LmtBitMapping.LoadFrom(LmtBitMappingFile);
        }

        public void OnUpdate(float deltaTime)
        {
            var player = Player.MainPlayer;
            if (player is null)
                return;
            
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

        public void OnImGuiFreeRender()
        {
            if (!Renderer.MenuShown)
                return;

            if (_selectedModel is null || _selectedAnimLayer is null)
                return;

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

                var currentAnim = _selectedModel.CurrentAnimation;
                ImGui.Text($"Current Animation: {currentAnim}");
                if (_selectedAnimLayer is not null)
                {
                    var motionLists = new NativeArray<nint>(_selectedAnimLayer.Instance + 0xE120, 16);
                    var lmtPtr = motionLists[(int)currentAnim.Lmt];
                    ImGui.Text($"LMT {currentAnim.Lmt} <-> {MemoryUtil.ReadString(lmtPtr + 0xC)}");
                }

                if (_selectedModel.Is("uCharacterModel"))
                {
                    var entity = _selectedModel.As<Entity>();
                    ImGui.Text($"Current Action: {entity.ActionController.CurrentAction}");
                }

                if (_selectedModel.Is("uPlayer") && ImGui.CollapsingHeader("Flags & Triggers"))
                {
                    if (ImGui.CollapsingHeader("PlMotionInput"))
                    {
                        var plMotion = _selectedModel.GetObject<MtObject>(0xE480);
                        if (plMotion is not null)
                        {

                            var flags = new NativeArray<uint>(plMotion.Instance + 0x8, 8);
                            var triggers = new NativeArray<uint>(plMotion.Instance + 0x28, 8);
                            ImGui.Text($"Flags");
                            for (var i = 0; i < 8; i++)
                                ImGui.Text($"    {i}: {flags[i]:b32}");
                            ImGui.Text($"Triggers");
                            for (var i = 0; i < 8; i++)
                                ImGui.Text($"    {i}: {triggers[i]:b32}");
                        }
                    }

                    if (ImGui.CollapsingHeader("PlMotionCommon"))
                    {
                        var plCommon = _selectedModel.GetObject<MtObject>(0xE470);
                        if (plCommon is not null)
                        {

                            var flags = new NativeArray<uint>(plCommon.Instance + 0x8, 8);
                            var triggers = new NativeArray<uint>(plCommon.Instance + 0x28, 8);
                            ImGui.Text($"Flags");
                            for (var i = 0; i < 8; i++)
                                ImGui.Text($"    {i}: {flags[i]:b32}");
                            ImGui.Text($"Triggers");
                            for (var i = 0; i < 8; i++)
                                ImGui.Text($"    {i}: {triggers[i]:b32}");
                        }
                    }
                }

                ImGui.NewLine();

                if (ImGui.CollapsingHeader("Frame Viewer"))
                {
                    ImGui.Text($"Current Frame: {_selectedAnimLayer!.CurrentFrame:.02}/{_selectedAnimLayer.MaxFrame}");
                    ImGui.SliderFloat("Frame", ref _selectedAnimLayer.CurrentFrame, 0f, _selectedAnimLayer.MaxFrame);

                    var paused = _selectedAnimLayer.Paused;
                    if (ImGui.Checkbox("Paused", ref paused))
                        _selectedAnimLayer.Paused = paused;

                    ImGui.Text($"Current Animation Speed: {_selectedAnimLayer.Speed}");
                    ImGui.SliderFloat("Speed", ref _lockedSpeed, 0, 10);

                    if (ImGui.Checkbox("Lock Speed", ref _speedLocked))
                    {
                        if (_speedLocked)
                            _selectedAnimLayer.LockSpeed(_lockedSpeed);
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
                    if (ImGui.BeginItemTooltip())
                    {
                        ImGui.Text("Lock Value");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.InputFloat("Next InterFrame", ref _lockedInterFrame);

                    ImGui.Checkbox("##lock-startframe", ref _lockStartFrame);
                    if (ImGui.BeginItemTooltip())
                    {
                        ImGui.Text("Lock Value");
                        ImGui.EndTooltip();
                    }
                    ImGui.SameLine();
                    ImGui.InputFloat("Next StartFrame", ref _lockedStartFrame);
                }
            }

            ImGui.End();
        }

        public void OnImGuiRender()
        {
            var entities = GetEntityList();
            if (_selectedModel is not null && !entities.Contains(_selectedModel))
            {
                _selectedModel = null;
                _selectedAnimLayer = null;
                _selectedDti = null;
                _selectedAnimLayer = null;
                _lmtPlayer = null;
                _timlPlayer = null;
                _timelineGroupExpandedMap.Clear();
            }

            ImGui.BeginGroup();

            if (ImGui.BeginCombo("Selected Entity", _selectedDti?.Name ?? "None"))
            {
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

            if (_selectedModel is null || _selectedAnimLayer is null)
            {
                ImGui.TextColored(new Vector4(1, 1, 0, 1), "No Entity selected");
                ImGui.EndGroup();
                return;
            }

            if (_selectedModel.Is("uEnemy"))
            {
                ImGui.Checkbox("Lock Actions", ref _lockActions);
                ImGui.InputInt("Action", ref _selectedAction);
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

                if (_lmtPlayer is null || _timlPlayer is null)
                {
                    ImGui.TextColored(new Vector4(1, 1, 0, 1), "No LMT Player");
                    ImGui.TreePop();
                    ImGui.EndGroup();

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

                if (_selectedLmt is null)
                {
                    ImGui.TextColored(new Vector4(1, 1, 0, 1), "No LMT selected");
                    ImGui.TreePop();
                    ImGui.EndGroup();

                    return;
                }
                
                ImGui.Separator();
                if (ImGui.Button("Save to File"))
                {
                    if (_sortKeyframes)
                        _selectedLmt.SortKeyframes();
                    _selectedLmt.Serialize($"nativePC\\{_selectedLmt.FilePath}.modified.lmt");
                }

                if (ImGui.BeginItemTooltip())
                {
                    ImGui.Text($"Saves the LMT to 'nativePC\\{_selectedLmt.FilePath}.modified.lmt'");
                    ImGui.EndTooltip();
                }

                ImGui.SameLine();
                ImGui.Checkbox("Sort Keyframes", ref _sortKeyframes);

                ImGui.SameLine();
                if (ImGui.Button("Sort All Keyframes"))
                    _selectedLmt.SortKeyframes();

                ImGui.SameLine();
                if (ImGui.Button("Reload Flag Mappings"))
                {
                    _lmtBitMapping = LmtBitMapping.LoadFrom(LmtBitMappingFile);
                }

                ImGui.BeginChild("##timelines", new Vector2(), ImGuiChildFlags.Border | ImGuiChildFlags.ResizeY);

                ref var header = ref _selectedLmt.Header;
                for (var i = 0; i < header.MotionCount; ++i)
                {
                    if (!header.HasMotion(i))
                        continue;

                    ref var motion = ref _selectedLmt.Header.GetMotion(i);
                    if (!motion.HasMetadata)
                        continue;

                    var name = $"Motion: Id:{i}, Bones: {motion.ParamNum}, Frames: {motion.FrameNum}";
                    if (ImGui.TreeNode(name))
                    {
                        if (ImGui.Button("Add New Keyframe"))
                        {
                            _addKeyframeMotion = MemoryUtil.AsPointer(ref motion);

                            Ensure.IsTrue(_addKeyframeMotion->Metadata.ParamNum > 0);
                            _addKeyframeParam = MemoryUtil.AsPointer(ref _addKeyframeMotion->Metadata.Params[0]);

                            Ensure.IsTrue(_addKeyframeMotion->Metadata.Params[0].MemberNum > 0);
                            _addKeyframeParamMember = MemoryUtil.AsPointer(ref _addKeyframeParam->Members[0]);

                            ImGui.OpenPopup("Add New Keyframe##popup");
                        }

                        ImGui.SameLine();

                        if (ImGui.Button("Add New Param"))
                        {
                            Ensure.IsTrue(motion.HasMetadata);
                            _addParamMotion = MemoryUtil.AsPointer(ref motion);

                            ImGui.OpenPopup("Add New Param##popup");
                        }

                        ImGui.SameLine();

                        if (ImGui.Button("Sort Keyframes"))
                        {
                            foreach (ref var param in motion.Metadata.Params)
                            {
                                foreach (ref var member in param.Members)
                                    member.SortKeyframes();
                            }
                        }

                        if (ImGui.BeginItemTooltip())
                        {
                            ImGui.Text("Sorts all Keyframes in this Entry by their frame.");
                            ImGui.NewLine();
                            ImGui.TextWrapped("When you drag a keyframe with a higher frame number to the left of a "
                                              + "different keyframe with a lower frame number, the keyframes will be "
                                              + "out of order. This will cause weird behavior in-game, so you should "
                                              + "sort the keyframes after you're done editing them.");
                            ImGui.EndTooltip();
                        }

                        if (ImGuiExtensions.BeginTimeline(name, 0f, motion.FrameNum,
                                ref _selectedModel.CurrentAnimation.Id == (uint)i ? ref _selectedAnimLayer.CurrentFrame : ref _framePointer, 
                                (ImGuiTimelineFlags)_timelineFlags))
                        {
                            foreach (ref var param in motion.Metadata.Params)
                            {
                                var dti = param.Dti;
                                if (dti is null)
                                    continue;

                                var obj = _timlPlayer.ObjectList?.FindObject(dti);
                                var props = obj?.GetProperties();
                                if (props is null || obj is null)
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
                                        var defIndex = FindMemberDef(obj.MemberList, member.Hash);
                                        if (defIndex == -1)
                                            continue;
                                        ref var def = ref obj.MemberList[defIndex];

                                        var memberName = def.HashName;
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
                                            _selectedMotion = MemoryUtil.AsPointer(ref motion);
                                            _selectedParam = MemoryUtil.AsPointer(ref param);
                                            _selectedParamMember = MemoryUtil.AsPointer(ref member);
                                            _selectedKeyframe = MemoryUtil.AsPointer(ref member.GetKeyframe(selectedKeyframe));
                                            _selectedKeyframeType = def.Type;
                                            _selectedParamName = dti.Name;
                                            _selectedParamMemberName = memberName;
                                        }
                                    }
                                    
                                    ImGuiExtensions.EndTimelineGroup();
                                }

                                _timelineGroupExpandedMap[param.Hash] = expanded;
                            }

                            ImGuiExtensions.EndTimeline();
                            ImGui.NewLine();
                        }

                        if (ImGui.BeginPopup("Add New Keyframe##popup"))
                        {
                            var kfName = _addKeyframeParam != null ? _addKeyframeParam->Dti?.Name : "None";
                            if (ImGui.BeginCombo("Param", kfName))
                            {
                                foreach (ref var param in _addKeyframeMotion->Metadata.Params)
                                {
                                    if (param.Dti is null)
                                        continue;

                                    var props = _timlPlayer.ObjectList?.FindObject(param.Dti)?.GetProperties();
                                    if (props is null)
                                        continue;

                                    if (ImGui.Selectable(param.Dti.Name, MemoryUtil.AsPointer(ref param) == _addKeyframeParam))
                                        _addKeyframeParam = MemoryUtil.AsPointer(ref param);
                                }

                                ImGui.EndCombo();
                            }

                            if (_addKeyframeParam is not null)
                            {
                                var props = _timlPlayer.ObjectList?.FindObject(_addKeyframeParam->Dti!)?.GetProperties();
                                Ensure.NotNull(props);

                                if (ImGui.BeginCombo("Member", props.FindProperty(_addKeyframeParamMember->Hash)?.HashName ?? "None"))
                                {
                                    foreach (ref var member in _addKeyframeParam->Members)
                                    {
                                        var prop = props.FindProperty(member.Hash);
                                        if (prop is null)
                                            continue;

                                        if (ImGui.Selectable(prop.HashName, MemoryUtil.AsPointer(ref member) == _addKeyframeParamMember))
                                            _addKeyframeParamMember = MemoryUtil.AsPointer(ref member);
                                    }

                                    ImGui.EndCombo();
                                }
                            }

                            var applyType = (int)_addKeyframeApplyType;
                            var interpType = (int)_addKeyframeInterpType;
                            ImGui.DragFloat("Frame", ref _addKeyframeFrame, 0.2f);
                            ImGui.Combo("Apply Type", ref applyType, Enum.GetNames(typeof(ApplyType)), (int)ApplyType.Count);
                            ImGui.Combo("Interpolation Type", ref interpType, Enum.GetNames(typeof(InterpType)), (int)InterpType.Count);
                            _addKeyframeApplyType = (ApplyType)applyType;
                            _addKeyframeInterpType = (InterpType)interpType;

                            if (_addKeyframeParamMember is null)
                                ImGui.BeginDisabled();

                            if (ImGui.Button("Add Keyframe"))
                            {
                                AddKeyframe();
                                ImGui.CloseCurrentPopup();
                            }

                            if (_addKeyframeParamMember is null)
                                ImGui.EndDisabled();

                            ImGui.EndPopup();
                        }

                        if (ImGui.BeginPopup("Add New Param##popup"))
                        {
                            if (ImGui.BeginCombo("Param", _addParamDti?.Name ?? "None"))
                            {
                                foreach (var dti in _timlObjectDtiList)
                                {
                                    if (ImGui.Selectable(dti.Name, dti == _addParamDti))
                                    {
                                        if (_addParamTimlObject is not null)
                                        {
                                            _addParamTimlObject.Destroy(true);
                                            _addParamTimlObject = null;
                                        }

                                        _paramMemberDefPool->UsedSize = 0;
                                        _addParamDef = null;
                                        _addParamDti = dti;
                                        _addParamTimlObject = dti.CreateInstance<TimelineObject>();
                                        GetMemberDefs(_addParamTimlObject, _paramMemberDefPool);
                                    }
                                }

                                ImGui.EndCombo();
                            }

                            if (_addParamTimlObject is not null)
                            {
                                var selectedMemberName = _addParamDef is null ? "None" : _addParamDef->HashName;
                                if (ImGui.BeginCombo("Member", selectedMemberName))
                                {
                                    foreach (ref var def in _addParamTimlObject.MemberList)
                                    {
                                        var pdef = MemoryUtil.AsPointer(ref def);
                                        if (ImGui.Selectable(pdef->HashName, pdef == _addParamDef))
                                            _addParamDef = pdef;
                                    }

                                    ImGui.EndCombo();
                                }
                            }

                            if (_addParamDef is null)
                                ImGui.BeginDisabled();

                            if (ImGui.Button("Add Param"))
                            {
                                AddParamMember();
                                ImGui.CloseCurrentPopup();
                            }

                            if (_addParamDef is null)
                                ImGui.EndDisabled();
                            
                            ImGui.EndPopup();
                        }

                        ImGui.TreePop();
                    }
                }

                ImGui.EndChild();

                ImGui.BeginChild("##keyframe editor");

                if (_selectedKeyframe is not null)
                {
                    if (ImGui.Button("Duplicate Keyframe") ||
                        ImGui.IsKeyDown(ImGuiKey.LeftCtrl) && ImGui.IsKeyPressed(ImGuiKey.D))
                    {
                        DuplicateSelectedKeyframe();
                        ImGuiExtensions.NotificationInfo("Duplicated Keyframe");
                    }

                    ImGui.SameLine();
                    if (ImGui.Button("Delete Keyframe") || ImGui.IsKeyPressed(ImGuiKey.Delete))
                    {
                        if (_selectedParamMember->KeyframeNum > 1)
                        {
                            DeleteSelectedKeyframe();
                            ImGuiExtensions.NotificationInfo("Deleted Keyframe");
                        }
                        else
                        {
                            ImGuiExtensions.NotificationError("Cannot delete last Keyframe");
                        }
                    }

                    switch (_selectedKeyframeType)
                    {
                        case PropType.Bool:
                            ImGui.Checkbox("Value", ref _selectedKeyframe->BoolValue);
                            break;
                        case PropType.S32:
                            ImGui.InputInt("Value", ref _selectedKeyframe->IntValue);
                            break;
                        case PropType.U32:
                            ImGuiExtensions.InputScalar("Value", ref _selectedKeyframe->UIntValue,
                                format: _selectedKeyframe->ApplyType == ApplyType.Flags ? "%X" : "%u");
                            break;
                        case PropType.Color:
                            {
                                var vec4 = _selectedKeyframe->ColorValue.ToVector4();
                                ImGui.ColorEdit4("Value", ref vec4);
                                _selectedKeyframe->ColorValue = MtColor.FromVector4(vec4);
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

                    if (_selectedKeyframe->ApplyType == ApplyType.Flags)
                        ImGuiExtensions.InputScalar("Flags Mask", ref _selectedKeyframe->FlagsMask, format: "%X");

                    ImGui.Separator();
                    var applyType = (int)_selectedKeyframe->ApplyType;
                    var interpType = (int)_selectedKeyframe->InterpolationType;
                    ImGui.Combo("Apply Type", ref applyType, Enum.GetNames(typeof(ApplyType)), (int)ApplyType.Count);
                    ImGui.Combo("Interpolation Type", ref interpType, Enum.GetNames(typeof(InterpType)), (int)InterpType.Count);
                    _selectedKeyframe->ApplyType = (ApplyType)applyType;
                    _selectedKeyframe->InterpolationType = (InterpType)interpType;

                    DisplayBitMappings();
                }

                ImGui.EndChild();

                ImGui.TreePop();
            }

            ImGui.EndGroup();

            if (ImGui.BeginDragDropTarget())
            {
                var payload = ImGui.AcceptDragDropPayload("AssetBrowser_Item");
                if (payload.NativePtr != null)
                {
                    var path = Encoding.UTF8.GetString((byte*)payload.Data, payload.DataSize);
                    Log.Info($"Dropped {path}");
                    var lmt = ResourceManager.GetResource<MotionList>(path, MtDti.Find("rMotionList")!);
                    if (lmt is not null && lmt.Header.MotionCount > 0)
                    {
                        _pendingLmt = lmt;
                        ImGui.OpenPopup("LMT Without Object");
                    }
                }

                ImGui.EndDragDropTarget();
            }

            ImGui.SetNextWindowSize(new Vector2(500, 200), ImGuiCond.Appearing);
            if (ImGui.BeginPopupModal("LMT Without Object"))
            {
                ImGui.TextWrapped("Warning: This LMT does not have an associated object. " +
                           "It is possible that certain LMT Parameters will " +
                           "not be shown accordingly.");
                ImGui.Text("Do you want to load it anyway?");

                ImGui.BeginHorizontal("##drag-drop-popup");

                ImGui.Spring(-1f);

                if (ImGui.Button("Yes"))
                {
                    _selectedLmt = _pendingLmt;
                    _pendingLmt = null;
                    ImGui.CloseCurrentPopup();
                }

                if (ImGui.Button("No"))
                {
                    _pendingLmt = null;
                    ImGui.CloseCurrentPopup();
                }

                ImGui.EndHorizontal();

                ImGui.EndPopup();
            }
            else
            {
                _pendingLmt = null;
            }
        }

        private void DisplayBitMappings()
        {
            if (_selectedKeyframe->ApplyType is not (ApplyType.Flags or ApplyType.Trigger)) 
                return;

            var mappings = _lmtBitMapping.GetBitMapping(
                _selectedParamName,
                _selectedParamMemberName
            );

            if (mappings.Count == 0)
                return;

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(6, 7));

            var hoveredBit = -1;
            ImGuiExtensions.Bitfield("Value", ref _selectedKeyframe->UIntValue, ref hoveredBit);

            ImGui.PopStyleVar();

            if (hoveredBit != -1)
            {
                var mapping = mappings.Find(mapping => mapping.Bit == hoveredBit);
                if (mapping is not null)
                {
                    ImGui.BeginTooltip();
                    ImGui.Text(mapping.Name);
                    ImGui.Text($"Affects Bit {mapping.Bit} (0x{(1u << mapping.Bit):X})");
                    ImGui.Text(mapping.Description);
                    ImGui.EndTooltip();
                }
                else
                {
                    ImGui.BeginTooltip();
                    ImGui.Text("No Mapping");
                    ImGui.Text($"Affects Bit {hoveredBit} (0x{(1u << hoveredBit):X})");
                    ImGui.EndTooltip();
                }
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
            if (model.AnimationLayer is null)
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
            var paramTypes = MemoryUtil.ReadStructArray<LmtParamType>(
                MemoryUtil.Read<nint>(paramTypesAddr),
                MemoryUtil.Read<int>(paramTypeCountAddr)
            );

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
                    memberDefPool->PoolSize = (int)type.PropCount * sizeof(LmtParamMemberDef);
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

        private static void DumpDtiHashMap(string path)
        {
            Log.Info("Dumping DTI HashMap");
            var dtiHashMap = new NativeArray<nint>((nint)0x145030970, 256);
            var sb = new StringBuilder();

            for (var i = 0; i < 256; i++)
            {
                var dti = new MtDti(dtiHashMap[i]);
                while (dti is not null)
                {
                    sb.AppendLine($"{{ 0x{dti.Id:X}, \"{dti.Name}\" }},");
                    dti = dti.Next;
                }
            }

            File.WriteAllText(path, sb.ToString());
        }

        public void OnMonsterAction(Monster monster, ref int action)
        {
            if (monster != _selectedModel)
                return;

            if (_lockActions)
                action = _selectedAction;
        }

        public void LogProperties(MtObject? obj, string indent = "    ")
        {
            var mtArrayDti = MtDti.Find("MtArray");
            if (obj is null)
                return;

            var props = obj.GetProperties();
            var dti = obj.GetDti();
            if (dti is null)
                return;

            Log.Info($"{indent[4..]}Properties for {dti.Name} (0x{dti.Id:X}):");
            foreach (var prop in props)
            {
                var get = prop.IsProperty ? prop.Get : prop.Get - obj.Instance;
                Log.Info($"{indent}[{prop.Type}] {prop.Name}, Offset:0x{get:X}, CRC32: 0x{Utility.Crc32(prop.Name):X}");

                if (prop.Type is not PropType.Class and not PropType.ClassRef)
                    continue;
                if (prop.IsArray && dti != mtArrayDti)
                    continue;

                if (dti == mtArrayDti)
                {
                    Log.Info($"{indent}Entries:");
                    var arr = obj.As<MtArray<MtObject>>();
                    foreach (var item in arr)
                        LogProperties(item, indent + "    ");
                    continue;
                }

                var getter = prop.Type switch
                {
                    PropType.Class => new NativeFunction<nint, nint>(0x14218c020),
                    PropType.ClassRef => new NativeFunction<nint, nint>(0x14218d780),
                    _ => new NativeFunction<nint, nint>()
                };

                var cls = new MtObject(getter.Invoke(prop.Instance));
                LogProperties(cls, indent + "    ");
            }
        }

        private static List<Model> GetEntityList()
        {
            var list = new List<Model>();
            var player = Player.MainPlayer;
            if (player is not null)
            {
                list.Add(player);

                var weapon = player.CurrentWeapon;
                if (weapon is not null)
                    list.Add(weapon);

                var claw = player.GetObject<Model>(0x8918);
                if (claw is not null)
                    list.Add(claw);
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

        private void DeleteSelectedKeyframe()
        {
            if (_selectedLmt is null)
                return;

            var kfList = _selectedParamMember->Keyframes;
            var newKfList = NativeArray<MetadataKeyframe>.Create(_selectedParamMember->KeyframeNum - 1);
            var newLen = 0;
            for (var k = 0; k < _selectedParamMember->KeyframeNum; k++)
            {
                if (Unsafe.AreSame(ref kfList[k], ref *_selectedKeyframe))
                    continue;

                newKfList[newLen++] = kfList[k];
            }

            _selectedParamMember->KeyframeNum = newLen;
            _selectedParamMember->KeyframePtr = newKfList.Pointer;
            var claimedLmt = _claimedLmts.Find(lmt => lmt.Lmt == _selectedLmt);
            if (claimedLmt is null)
            {
                claimedLmt = new ClaimedLmt(_selectedLmt);
                _claimedLmts.Add(claimedLmt);
            }
            else
            {
                claimedLmt.ModifiedKeyframeLists.Add(newKfList);
            }
        }

        private void DuplicateSelectedKeyframe()
        {
            Ensure.NotNull(_selectedLmt);
            Ensure.NotNull(_selectedMotion);
            Ensure.NotNull(_selectedParamMember);
            Ensure.NotNull(_selectedKeyframe);

            var existingKfList = _selectedParamMember->Keyframes;
            NativeArray<MetadataKeyframe> newKfList;
            var claimedLmt = _claimedLmts.Find(lmt => lmt.Lmt == _selectedLmt);
            var kfListIndex = claimedLmt?.ModifiedKeyframeLists.FindIndex(
                list => list.Pointer == _selectedParamMember->KeyframePtr) ?? -1;
            if (claimedLmt is null || kfListIndex == -1)
            {
                newKfList = NativeArray<MetadataKeyframe>.Create(_selectedParamMember->KeyframeNum + 1);
            }
            else
            {
                newKfList = claimedLmt.ModifiedKeyframeLists[kfListIndex];
                newKfList.Resize(_selectedParamMember->KeyframeNum + 1);
            }

            for (var k = 0; k < _selectedParamMember->KeyframeNum; k++)
                newKfList[k] = existingKfList[k];

            var newFrame = Math.Clamp(_selectedKeyframe->Frame + 5, 0, _selectedMotion->FrameNum - 1);
            var newKf = new MetadataKeyframe
            {
                Frame = newFrame,
                ApplyType = _selectedKeyframe->ApplyType,
                InterpolationType = _selectedKeyframe->InterpolationType,
                BounceBackLimit = _selectedKeyframe->BounceBackLimit,
                BounceForwardLimit = _selectedKeyframe->BounceForwardLimit,
                UIntValue = _selectedKeyframe->UIntValue
            };

            newKfList[_selectedParamMember->KeyframeNum] = newKf;
            _selectedParamMember->KeyframeNum += 1;
            _selectedParamMember->KeyframePtr = newKfList.Pointer;

            if (claimedLmt is null)
            {
                claimedLmt = new ClaimedLmt(_selectedLmt);
                _claimedLmts.Add(claimedLmt);
            }
            
            if (kfListIndex == -1)
                claimedLmt.ModifiedKeyframeLists.Add(newKfList);
            else
                claimedLmt.ModifiedKeyframeLists[kfListIndex] = newKfList;
        }

        private void AddKeyframe()
        {
            if (_selectedLmt is null)
                return;

            var kfList = _addKeyframeParamMember->Keyframes;
            var newKfList = NativeArray<MetadataKeyframe>.Create(_selectedParamMember->KeyframeNum + 1);
            var newLen = 0;
            for (var k = 0; k < _selectedParamMember->KeyframeNum; k++)
                newKfList[newLen++] = kfList[k];

            var newKf = new MetadataKeyframe
            {
                Frame = _addKeyframeFrame,
                ApplyType = _addKeyframeApplyType,
                InterpolationType = _addKeyframeInterpType
            };

            newKfList[newLen++] = newKf;
            _selectedParamMember->KeyframeNum = newLen;

            _selectedParamMember->KeyframePtr = newKfList.Pointer;
            var claimedLmt = _claimedLmts.Find(lmt => lmt.Lmt == _selectedLmt);
            if (claimedLmt is null)
            {
                claimedLmt = new ClaimedLmt(_selectedLmt);
                _claimedLmts.Add(claimedLmt);
            }

            claimedLmt.ModifiedKeyframeLists.Add(newKfList);
        }

        private void AddParamMember()
        {
            Ensure.NotNull(_selectedLmt);
            Ensure.NotNull(_addParamDti);
            Ensure.NotNull(_addParamDef);
            Ensure.NotNull(_addParamMotion);

            var metaParams = _addParamMotion->Metadata.Params;
            MetadataParam* existingParam = null;
            foreach (ref var param in metaParams)
            {
                if (param.Hash == _addParamDti.Id)
                {
                    existingParam = MemoryUtil.AsPointer(ref param);
                    foreach (ref var member in param.Members)
                    {
                        if (member.Hash == _addParamDef->Hash)
                            return;
                    }
                }
            }

            if (existingParam is not null)
            {
                var newMember = new MetadataParamMember
                {
                    Hash = _addParamDef->Hash,
                    KeyframeNum = 0,
                    KeyframePtr = null,
                    KeyframeType = 0
                };

                var newMemberList = NativeArray<MetadataParamMember>.Create(existingParam->MemberNum + 1);
                for (var i = 0; i < existingParam->MemberNum; i++)
                    newMemberList[i] = existingParam->Members[i];

                newMemberList[existingParam->MemberNum] = newMember;
                existingParam->MemberNum += 1;
                existingParam->MemberPtr = newMemberList.Pointer;

                var claimedLmt = _claimedLmts.Find(lmt => lmt.Lmt == _selectedLmt);
                if (claimedLmt is null)
                {
                    claimedLmt = new ClaimedLmt(_selectedLmt);
                    _claimedLmts.Add(claimedLmt);
                }

                claimedLmt.ModifiedParamMemberLists.Add(newMemberList);

                AddKeyframeToMember(ref newMemberList[^1], claimedLmt);
            }
            else
            {
                var newParam = new MetadataParam
                {
                    MemberPtr = null,
                    MemberNum = 1,
                    Hash = _addParamDti.Id,
                    UniqueId = LmtParamTypes.First(type => type.Dti == _addParamDti).UniqueId
                };

                var newMember = new MetadataParamMember
                {
                    KeyframePtr = null,
                    KeyframeNum = 0,
                    Hash = _addParamDef->Hash,
                    KeyframeType = 0
                };

                var newMemberList = NativeArray<MetadataParamMember>.Create(1);
                newMemberList[0] = newMember;
                newParam.MemberPtr = newMemberList.Pointer;

                var newParamList = NativeArray<MetadataParam>.Create(_addParamMotion->Metadata.ParamNum + 1);
                for (var i = 0; i < _addParamMotion->Metadata.ParamNum; i++)
                    newParamList[i] = _addParamMotion->Metadata.Params[i];

                newParamList[_addParamMotion->Metadata.ParamNum] = newParam;
                _addParamMotion->Metadata.ParamNum += 1;
                _addParamMotion->Metadata.ParamPtr = newParamList.Pointer;

                var claimedLmt = _claimedLmts.Find(lmt => lmt.Lmt == _selectedLmt);
                if (claimedLmt is null)
                {
                    claimedLmt = new ClaimedLmt(_selectedLmt);
                    _claimedLmts.Add(claimedLmt);
                }

                claimedLmt.ModifiedParamLists.Add(newParamList);
                claimedLmt.ModifiedParamMemberLists.Add(newMemberList);

                AddKeyframeToMember(ref newMemberList[0], claimedLmt);
            }

            return;

            static void AddKeyframeToMember(ref MetadataParamMember member, ClaimedLmt claimedLmt)
            {
                Ensure.IsTrue(member.KeyframeNum == 0);

                var newKf = new MetadataKeyframe
                {
                    Frame = 0,
                    ApplyType = ApplyType.Absolute,
                    InterpolationType = InterpType.U32,
                    BounceBackLimit = 0,
                    BounceForwardLimit = 0,
                    UIntValue = 0
                };

                var newKfList = NativeArray<MetadataKeyframe>.Create(1);
                newKfList[0] = newKf;
                member.KeyframeNum = 1;
                member.KeyframePtr = newKfList.Pointer;

                claimedLmt.ModifiedKeyframeLists.Add(newKfList);
            }
        }

        //private readonly NativeArray<ushort> _supplementalGlyphRanges = NativeArray<ushort>.Create(8);

        private void LoadSupplementalFont()
        {
            //_supplementalGlyphRanges[0] = 0x2190; // Arrows
            //_supplementalGlyphRanges[1] = 0x21FF;
            //_supplementalGlyphRanges[2] = 0x25A0; // Geometric Shapes
            //_supplementalGlyphRanges[3] = 0x25FF;
            //_supplementalGlyphRanges[4] = 0x2A00; // Mathematical Operators
            //_supplementalGlyphRanges[5] = 0x2AFF;
            //_supplementalGlyphRanges[6] = 0x0000; // Null terminator
            //_supplementalGlyphRanges[7] = 0x0000;
            
            var io = ImGui.GetIO();
            _supplementalFont = io.Fonts.AddFontFromFileTTF(
                SupplementalFontFile,
                16
            );

            io.Fonts.Build();
            io.Fonts.GetTexDataAsRGBA32(out nint _, out _, out _);
        }

        private static int FindMemberDef(Span<LmtParamMemberDef> memberDefs, uint hash)
        {
            for (var i = 0; i < memberDefs.Length; i++)
            {
                if (memberDefs[i].Hash == hash)
                    return i;
            }

            return -1;
        }

        private static void GetMemberDefs(TimelineObject obj, LmtParamMemberDefPool* pool)
        {
            var populateParams = new NativeAction<nint, nint>(0x14231df70);
            populateParams.Invoke(obj.Instance, (nint)pool);
        }
    }
}
