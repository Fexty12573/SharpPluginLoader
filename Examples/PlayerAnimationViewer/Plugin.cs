using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using ImGuiNET;
using SharpPluginLoader.Core;
using SharpPluginLoader.Core.Entities;
using SharpPluginLoader.Core.IO;
using SharpPluginLoader.Core.Memory;
using SharpPluginLoader.Core.Models;
using SharpPluginLoader.Core.MtTypes;
using SharpPluginLoader.Core.Resources;

namespace PlayerAnimationViewer
{
    public class Plugin : IPlugin
    {
        public string Name => "Player Animation Viewer";

        private delegate nint CreateShellDelegate(nint shellParam, nint source1, nint source2, nint data);
        private MotionList? _switchAxeLmt;
        private Resource? _switchAxeMbd;
        private Resource? _switchAxeCol;
        private bool _didFrameSet;
        private bool _speedLocked;

        public PluginData OnLoad()
        {
            Log.Info("Loaded PlayerAnimationViewer");

            return new PluginData
            {
                OnUpdate = true,
                OnPlayerAction = false,
                OnEntityAnimationUpdate = false,
                OnEntityAnimation = true,
                OnMonsterAction = true,
                OnWeaponChange = true,
                OnRender = true,
            };
        }

        public void OnUpdate(float deltaTime)
        {
            var player = Player.MainPlayer;
            if (player == null)
                return;

            //for (var i = 0; i < 10; i++)
            //{
            //    if (Input.IsPressed(Key.F1 + i))
            //    {
            //        Log.Info($"Creating shell {i}");
            //        player.CreateShell((uint)i, player.Forward * 10);
            //    }
            //}

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
            var player = Player.MainPlayer;
            if (player == null)
                return;

            var animLayer = player.AnimationLayer;
            if (animLayer == null)
                return;

            ImGui.Text($"Current Animation: {player.CurrentAnimation}");
            ImGui.NewLine();
            ImGui.Text($"Current Frame: {animLayer.CurrentFrame}/{animLayer.MaxFrame}");
            ImGui.SliderFloat("Frame", ref animLayer.CurrentFrame, 0, animLayer.MaxFrame);

            var paused = animLayer.Paused;
            if (ImGui.Checkbox("Paused", ref paused))
                animLayer.Paused = paused;

            ImGui.Text($"Speed: {animLayer.Speed}");
            ImGui.SliderFloat("Speed", ref animLayer.Speed, 0, 10);

            if (ImGui.Checkbox("Lock Speed", ref _speedLocked))
            {
                if (_speedLocked)
                    animLayer.LockSpeed(animLayer.Speed);
                else
                    animLayer.UnlockSpeed();
            }
        }

        public void OnPlayerAction(Player player, ref ActionInfo action)
        {
            if (action.ActionSet != 3 || player.AnimationLayer is null)
                return;

            if (action.ActionId is 66 or 67 or 69 or 70 or 68 or 81)
                player.AnimationLayer!.LockSpeed(1.3f);
            else
                player.AnimationLayer!.UnlockSpeed();
        }

        public void OnWeaponChange(Player player, WeaponType weaponType, int weaponId)
        {
            if (weaponType != WeaponType.GreatSword)
                return;

            _switchAxeLmt ??= ResourceManager.GetResource<MotionList>(@"hm\wp\wp08\mot\w08_00\w08_00", MtDti.Find("rMotionList")!, 0x801);
            if (_switchAxeLmt == null)
                return;

            _switchAxeMbd ??= ResourceManager.GetResource<Resource>(@"hm\wp\wp08\mot\w08_00\w08_00", MtDti.Find("rMotBlendData")!, 0x801);
            if (_switchAxeMbd == null)
                return;

            _switchAxeCol = ResourceManager.GetResource<Resource>(@"hm\wp\wp08\collision\wp08", MtDti.Find("rObjCollision")!);

            //var colMetadataHash = Utility.Crc32("nTimelineParam::CollisionTimelineObject");
            //var colBankIdHash = Utility.Crc32("バンク[0]");
            _switchAxeLmt.Header.GetMotion(315).Metadata.GetParam(0).GetMember(0).GetKeyframe(0).IntValue = 1;

            player.AnimationLayer!.RegisterLmt(_switchAxeLmt, 13);
            player.RegisterMbd(_switchAxeMbd, 13);
            Log.Info("Registered Switch Axe LMT");
        }

        public void OnMonsterAction(Monster monster, ref int actionId)
        {
            if (monster.Type == MonsterType.Diablos)
                actionId = 1; // Idle
        }

        public void OnEntityAnimationUpdate(Entity entity, AnimationId currentAnimation, float deltaTime)
        {
            if (!entity.Is("uPlayer") || entity.AnimationLayer is null)
                return;

            if (!_didFrameSet && entity.CurrentAnimation == 53563)
            {
                var animLayer = entity.AnimationLayer;
                animLayer.CurrentFrame = 16;
                _didFrameSet = true;
            }
            
            // Wide Slash: 49258
            // Rising Slash: 49259
            // Jumping Wide Slash: 49300
        }

        public void OnEntityAnimation(Entity entity, ref AnimationId animationId, ref float startFrame)
        {
            if (entity.Is("uPlayer") && animationId == 49258)
            {
                animationId = 53563;
                startFrame = 16.0f;
                _didFrameSet = false;

                if (_switchAxeCol != null)
                {
                    entity.As<Player>().CurrentWeapon?.RegisterObjCollision(_switchAxeCol, 1);
                    _switchAxeCol = null;
                    Log.Info("Registered Switch Axe COL");
                }
            }
        }

        private static bool TogglePause(Model model)
        {
            if (model.AnimationLayer == null)
                return false;

            return model.AnimationLayer.Paused = !model.AnimationLayer.Paused;
        }

        private static unsafe void DumpLmtParams(string dir)
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
    }
}
