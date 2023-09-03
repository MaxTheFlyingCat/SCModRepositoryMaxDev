using System;
using System.Collections.Generic;
using ProtoBuf;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Lights;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.Input;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;
using CoreSystems.Api;
using ParallelTasks;
using VRageRender;
using BlendTypeEnum = VRageRender.MyBillboard.BlendTypeEnum;
using Color = VRageMath.Color;
using Draygo.API;
using System.Text;
using Sandbox.Definitions;
using static Draygo.API.HudAPIv2;
using System.Linq;

namespace klime.Visual
{
    //Render grid
    public class GridR
    {
        public MyCubeGrid grid;
        public EntRender entRender = new EntRender();
        public MatrixD controlMatrix;
        public double scale, tempscale = 0.8;
        internal Task GridTask = new Task();
        Vector3D relTrans;
        public BoundingBoxD gridBox;
        public static MatrixD gridMatrix;
        public static MatrixD gridMatrixBackground;
        public static float billboardScaling;
        public Color BillboardRED;
        public Vector4 Billboardcolor;
        private MyStringId PaperDollBGSprite = MyStringId.TryGet("paperdollBG");
        private String ArmorMat = "Hazard_Armor";
        string ArmorRecolorHex = "#02f726";
        public bool runonce = false;
        public float oldFOV;
        public bool needsRescale;
        public static Vector3D GridBoxCenter;
        public static Vector3D GridBoxCenterGlobal;
        public static Vector3D hateVector;
        //public MyStringHash MaterialHash = MyStringHash.GetOrCompute("SciFi");

        public GridR(MyCubeGrid grid, EntRender entRender = null)
        {
            this.grid = grid;
            this.entRender = entRender ?? this.entRender;
        }

        public void UpdateMatrix(MatrixD renderMatrix)
        {
            var camera = MyAPIGateway.Session.Camera;
            float newFov = camera.FovWithZoom;
            if (oldFOV != newFov) { oldFOV = newFov; needsRescale = true; }

            float aspect = camera.ViewportSize.X / camera.ViewportSize.Y, fovTan = (float)Math.Tan(newFov * 0.5f), scaleFov = 0.1f * fovTan;
            Vector2D offset = new Vector2D(0.1, 0.1) * scaleFov;
            offset.X *= aspect; offset.Y *= aspect;

            float scale = scaleFov * 0.23f;


            MatrixD clonedWorldMatrix = grid.WorldMatrix;
            clonedWorldMatrix.Translation += Vector3D.Transform(grid.PositionComp.LocalAABB.Center, grid.PositionComp.WorldMatrixRef);

            renderMatrix.Translation += Vector3D.TransformNormal(relTrans, clonedWorldMatrix);

            MatrixD rotRenderMatrix = MatrixD.CreateTranslation(grid.PositionComp.WorldAABB.Center) * renderMatrix;
            hateVector = renderMatrix.Translation;
            hateVector -= Vector3D.TransformNormal(relTrans, clonedWorldMatrix);

            float lowerLimit = newFov < 1.2 ? 0.03f : 0.04f;
            float backOffsetQtr = (scale - 0.001f) * 0.25f, backOffsetHalf = (scale - 0.001f) * 0.2f, backOffsetEighth = (scale - 0.001f) * 0.125f;

            MatrixD transpMatrix = MatrixD.Transpose(renderMatrix);
            Vector3D moveVecBack = Vector3D.TransformNormal(-camera.WorldMatrix.Forward, renderMatrix);
            Vector3D moveVecLeft = Vector3D.TransformNormal(-camera.WorldMatrix.Right, renderMatrix);
            MatrixD bruhMatrix = renderMatrix;
            bruhMatrix.Translation += moveVecBack * backOffsetQtr + moveVecLeft * backOffsetHalf;

            MatrixD painMatrix = bruhMatrix, greaterPainMatrix = bruhMatrix;
            greaterPainMatrix.Translation += moveVecBack * backOffsetEighth + moveVecLeft * backOffsetEighth;
            painMatrix.Translation += Vector3D.TransformNormal(camera.WorldMatrix.Forward, renderMatrix) * backOffsetEighth + Vector3D.TransformNormal(camera.WorldMatrix.Right, renderMatrix) * backOffsetEighth;

            Vector3D left = camera.WorldMatrix.Left, up = camera.WorldMatrix.Up;
            float tempBillboardScaling = MathHelper.Clamp((float)grid.PositionComp.Scale * 0.9f, lowerLimit, 0.09f);
            billboardScaling = tempBillboardScaling;



            if (needsRescale)
            {
                float backOffset = (scale - 0.001f) * 0.1f;
                 if (backOffset > 0)
                 {
                     Vector3D moveVec = Vector3D.TransformNormal(-camera.WorldMatrix.Forward, transpMatrix);
                     renderMatrix.Translation += moveVec * backOffset;
                 }
                DoRescale();
            }


            grid.WorldMatrix = renderMatrix;
            gridMatrixBackground = renderMatrix;



            AddBillboard(Color.Lime * 0.75f, hateVector, left, up, tempBillboardScaling, BlendTypeEnum.SDR);
            AddBillboard(Color.Red * 0.5f, greaterPainMatrix.Translation -= Vector3D.TransformNormal(relTrans, clonedWorldMatrix), left, up, tempBillboardScaling, BlendTypeEnum.AdditiveTop);
            AddBillboard(Color.DodgerBlue * 0.5f, painMatrix.Translation -= Vector3D.TransformNormal(relTrans, clonedWorldMatrix), left, up, tempBillboardScaling, BlendTypeEnum.AdditiveTop);

        }

        private void AddBillboard(Color color, Vector3D pos, Vector3D left, Vector3D up, float scale, BlendTypeEnum blendType)
        {
            MyTransparentGeometry.AddBillboardOriented(PaperDollBGSprite, color.ToVector4(), pos, left, up, scale, blendType);
        }

        public void DoRescale()
        {
            var volume = grid.PositionComp.WorldVolume;
            var camera = MyAPIGateway.Session.Camera;
            var newFov = camera.FovWithZoom;
            var aspectRatio = camera.ViewportSize.X / camera.ViewportSize.Y;

            var fov = Math.Tan(newFov * 0.5);
            var scaleFov = 0.1 * fov;
            var scaleFov2 = 0.2 * fov;

            var hudscale = 0.04f;
            var scale = MathHelper.Clamp((float)(scaleFov * (hudscale * 0.23f)), 0.0004f, 0.0008f);

           // GridBoxCenter = grid.PositionComp.LocalVolume.Center;

            var modifiedCenter = Vector3D.Transform(GridBoxCenter, grid.PositionComp.WorldMatrixRef);
            controlMatrix *= MatrixD.CreateTranslation(-modifiedCenter) * grid.PositionComp.WorldMatrixRef;
            var localCenter = new Vector3D(grid.PositionComp.WorldVolume.Center);
            var trueCenter = Vector3D.Transform(localCenter, grid.WorldMatrix);
            grid.PositionComp.Scale = scale;
            relTrans = Vector3D.TransformNormal(GridBoxCenter, MatrixD.Transpose(grid.WorldMatrix)) * scale;

            GridBoxCenter = grid.PositionComp.LocalVolume.Center;
            relTrans = -GridBoxCenter;

            needsRescale = false;
        }

        public void DoCleanup()
        {
            grid.ChangeGridOwnership(MyAPIGateway.Session.Player.IdentityId, MyOwnershipShareModeEnum.Faction);
            IMyCubeGrid iGrid = grid as IMyCubeGrid;
            iGrid.Render.DrawInAllCascades = iGrid.Render.FastCastShadowResolve = iGrid.Render.MetalnessColorable = true;

            Vector3 armorHSV = MyColorPickerConstants.HSVToHSVOffset(ColorExtensions.ColorToHSV(ColorExtensions.HexToColor(ArmorRecolorHex)));
            armorHSV = new Vector3((float)Math.Round(armorHSV.X, 2), (float)Math.Round(armorHSV.Y, 2), (float)Math.Round(armorHSV.Z, 2));
            iGrid.SkinBlocks(grid.Min, grid.Max, armorHSV, ArmorMat);

            List<IMySlimBlock> allBlocks = new List<IMySlimBlock>();
            iGrid.GetBlocks(allBlocks);
            foreach (var block in allBlocks) { block.Dithering = 2.45f; }

            foreach (var fatblock in grid.GetFatBlocks())
            {
                DisableBlock(fatblock as IMyFunctionalBlock);
                StopEffects(fatblock as IMyExhaustBlock);
                DisableBlock(fatblock as IMyLightingBlock);
            }
            iGrid.Render.InvalidateRenderObjects();
        }


        private void DisableBlock(IMyFunctionalBlock block)
        {
            if (block != null)
            {

                block.Enabled = false;
                block.Render.ShadowBoxLod = false;
                block.SlimBlock.Dithering = 2.45f; // this works!
                block.Visible = true;
                block.CastShadows = false;
                block.SlimBlock.UpdateVisual();
                // block.Render.UpdateTransparency();


            }

        }


        private void StopEffects(IMyExhaustBlock exhaust)
        {
            exhaust?.StopEffects();
        }
    }


    public class EntRender
    {
        public MyLight light;

        public EntRender()
        {
            light = new MyLight();
        }
    }

    public class GridG
    {
        public List<GridR> gridGroup;
        public bool doneInitialCleanup, doneRescale;
        public double rotationForward, rotationUp, rotationForwardBase;
        public int timer;
        public List<IMyCubeBlock> DelList = new List<IMyCubeBlock>();
        public List<Vector3I> SlimList = new List<Vector3I>();
        public List<Vector3I> SlimDelList = new List<Vector3I>();
        public List<Vector3I> FatDelList = new List<Vector3I>();
        public Dictionary<IMyCubeBlock, int> DelDict = new Dictionary<IMyCubeBlock, int>();
        public Dictionary<Vector3I, int> SlimDelDict = new Dictionary<Vector3I, int>();
        public MyStringHash stringHash;
        public Dictionary<Vector3I, float> BlockIntegrityDict = new Dictionary<Vector3I, float>();
        public Dictionary<Vector3I, float> FatBlockIntegrityDict = new Dictionary<Vector3I, float>();
        public List<DamageEntry> DamageEntries = new List<DamageEntry>();
        public static string DamageEntriesString = "";
        public static float TotalDamageSum = 0;
        public int SlimBlocksDestroyed = 0;
        public int FatBlocksDestroyed = 0;
        public Vector3D Position;
        public GridG(List<GridR> gridGroup, double rotationForwardBase) { Init(gridGroup, rotationForwardBase); }
        public GridG(GridR gridR, double rotationForwardBase) { Init(new List<GridR> { gridR }, rotationForwardBase); }
        private void Init(List<GridR> group, double rotationForwardBase) { gridGroup = group; this.rotationForwardBase = rotationForwardBase; }

        public void DoCleanup() { ExecuteActionOnGrid(g => g.DoCleanup(), ref doneInitialCleanup); }
        public void DoRescale() { ExecuteActionOnGrid(g => g.DoRescale(), ref doneRescale); }
        private void ExecuteActionOnGrid(Action<GridR> action, ref bool flag) { foreach (var sg in gridGroup) { if (sg.grid != null) { action(sg); flag = true; } } }



        public void DoBlockRemove(Vector3I position)
        {
            HandleException(() => SlimListClearAndAdd(position), "Clearing and Adding to SlimList");

            foreach (var subgrid in gridGroup)
            {
                HandleException(() => ProcessSubgrid(subgrid, position), "Iterating through gridGroup");
            }
        }

        private void SlimListClearAndAdd(Vector3I position)
        {
            SlimList.Clear();
            SlimList.Add(position);
        }

        private void ProcessSubgrid(GridR subgrid, Vector3I position) // Replace SubGridType with the actual type
        {
            if (subgrid.grid == null) return;

            var slim = subgrid.grid.GetCubeBlock(position) as IMySlimBlock;
            if (slim == null) return;

            float integrity = slim.MaxIntegrity;

            if (slim.FatBlock == null && (!SlimDelDict.ContainsKey(slim.Position)))
            {
                ProcessSlimBlock(slim, subgrid, integrity);
            }
            else
            {
                ProcessFatBlock(slim, integrity);
            }
        }

        private void ProcessSlimBlock(IMySlimBlock slim, GridR subgrid, float integrity)
        {
            slim.Dithering = 1.25f;
            var blockKind = slim.Mass >= 500 ? 1 : 2;
            var colorHex = blockKind == 1 ? "#FF0000" : "#FFA500";
            UpdateSlimColorAndVisual(slim, subgrid.grid, colorHex, integrity);
        }

        private void UpdateSlimColorAndVisual(IMySlimBlock slim, MyCubeGrid subgrid, string colorHex, float integrity)
        {
            var stringHash = MyStringHash.GetOrCompute("Neon_Colorable_Lights");
            var colorHSVOffset = GetRoundedHSVOffset(colorHex);

            // Reverting to the original way of changing the color and skin.
            subgrid.Render.MetalnessColorable = true;
            subgrid.ChangeColorAndSkin(subgrid.GetCubeBlock(slim.Position), colorHSVOffset, stringHash);
            subgrid.Render.MetalnessColorable = true;
            slim.UpdateVisual();

            SlimDelDict.Add(slim.Position, timer + 150);
            BlockIntegrityDict[slim.Position] = integrity;
        }

        private Vector3 GetRoundedHSVOffset(string colorHex)
        {
            var colorHSVOffset = MyColorPickerConstants.HSVToHSVOffset(ColorExtensions.ColorToHSVDX11(ColorExtensions.HexToColor(colorHex)));
            return new Vector3((float)Math.Round(colorHSVOffset.X, 2), (float)Math.Round(colorHSVOffset.Y, 2), (float)Math.Round(colorHSVOffset.Z, 2));
        }

        private void ProcessFatBlock(IMySlimBlock slim, float integrity)
        {
            slim.Dithering = 1.5f;
            var customtimer = timer + 200;
            var color = GetFatBlockColor(slim.FatBlock.BlockDefinition.TypeId.ToString(), ref customtimer);
            int time = slim.FatBlock.Mass > 500 ? customtimer : timer + 10;
            if (!DelDict.ContainsKey(slim.FatBlock)) DelDict.Add(slim.FatBlock, time);
            FatBlockIntegrityDict[slim.Position] = integrity;
            MyVisualScriptLogicProvider.SetHighlightLocal(slim.FatBlock.Name, 3, 1, color);
        }


        private Color GetFatBlockColor(string typeId, ref int customtimer)
        {
            Color color = ColorExtensions.HexToColor("#8B0000");  // Default color

            Dictionary<string, Color> typeToColor = new Dictionary<string, Color>()
    {
        {"MyObjectBuilder_Gyro", Color.SteelBlue},
        {"MyObjectBuilder_ConveyorSorter", Color.Red},
        {"MyObjectBuilder_Thrust", Color.CadetBlue},
        {"MyObjectBuilder_GasTankDefinition", Color.CadetBlue},
        {"MyObjectBuilder_BatteryBlock", Color.Green},
        {"MyObjectBuilder_Reactor", Color.Green},
        {"MyObjectBuilder_SolarPanel", Color.Green},
        {"MyObjectBuilder_WindTurbine", Color.Green},
        {"MyObjectBuilder_Cockpit", Color.Purple},
    };

            if (typeToColor.ContainsKey(typeId))
            {
                color = typeToColor[typeId];
                if (typeId == "MyObjectBuilder_ConveyorSorter" || typeId == "MyObjectBuilder_Cockpit")
                {
                    customtimer = timer + 400;
                }
            }

            return color;
        }
        private void HandleException(Action action, string errorContext)
        {
            try { action(); }
            catch (Exception e)
            {
                MyLog.Default.WriteLine($"Error {errorContext}: {e.Message}");
                MyAPIGateway.Utilities.ShowNotification($"An error occurred while {errorContext}. Please check the log for more details.", 5000, MyFontEnum.Red);
            }
        }
        public class DamageEntry
        {
            public float SlimDamage { get; set; }
            public float FatBlockDamage { get; set; }
            public int Timestamp { get; set; }
            public DamageEntry(float slimDamage, float fatBlockDamage, int timestamp)
            {
                SlimDamage = slimDamage;
                FatBlockDamage = fatBlockDamage;
                Timestamp = timestamp;
            }
        }
        public string FormatDamage(double damage)
        {
            string[] sizes = { "", "K", "M", "B", "T" }; // Add more if needed
            int order = 0;
            while (damage >= 1000 && order < sizes.Length - 1)
            {
                order++;
                damage /= 1000;
            }

            return damage.ToString("F1") + sizes[order];
        }

        public void DisplayTotalDamage(float slimDamageLast10Seconds, float fatBlockDamageLast10Seconds)
        {

            DamageEntriesString = "";
            string damageMessage = "Total Damage: " + FormatDamage(TotalDamageSum) +
                                  "\nSlim Damage Last 10 Seconds: " + FormatDamage(slimDamageLast10Seconds) +
                                  "\nFatBlock Damage Last 10 Seconds: " + FormatDamage(fatBlockDamageLast10Seconds) +
                                  "\nSlim Blocks Destroyed: " + SlimBlocksDestroyed +
                                  "\nFat Blocks Destroyed: " + FatBlocksDestroyed;

            DamageEntriesString = damageMessage;
        }
        public void UpdateMatrix(MatrixD renderMatrix, MatrixD rotMatrix)
        {
            if (!doneRescale || !doneInitialCleanup) return;
            timer++;

            InitializeFrameData();

            ProcessFatBlocks();
            ProcessSlimBlocks();

            UpdateBlockDestructionStats();

            AggregateDamageOverTime();

            UpdateRenderMatrix(renderMatrix, rotMatrix);
        }

        private void InitializeFrameData()
        {
            DelList.Clear();
            SlimDelList.Clear();
            FatDelList.Clear();
        }

        private void ProcessFatBlocks()
        {
            foreach (var fatblock in DelDict.Keys)
            {
                if (DelDict[fatblock] == timer)
                {
                    fatblock.Close();
                    DelList.Add(fatblock);
                    FatDelList.Add(fatblock.Position);
                }
            }
            foreach (var item in DelList) DelDict.Remove(item);
        }

        private void ProcessSlimBlocks()
        {
            foreach (var slim in SlimDelDict.Keys)
            {
                if (SlimDelDict[slim] == timer)
                {
                    SlimDelList.Add(slim); // add vis here
                }
            }
        }

        private void UpdateBlockDestructionStats()
        {
            float slimDamageThisFrame = 0;
            float fatBlockDamageThisFrame = 0;

            foreach (var subgrid in gridGroup)
            {
                if (subgrid.grid == null) continue;
                UpdateSlimBlockDestruction(subgrid, ref slimDamageThisFrame);
                UpdateFatBlockDestruction(ref fatBlockDamageThisFrame);

                FatBlocksDestroyed += DelList.Count;
                SlimBlocksDestroyed += SlimDelList.Count;
            }

            DamageEntries.Add(new DamageEntry(slimDamageThisFrame, fatBlockDamageThisFrame, timer));
        }

        private void UpdateSlimBlockDestruction(GridR subgrid, ref float slimDamageThisFrame)
        {
            foreach (var item in SlimDelList)
            {
                slimDamageThisFrame += BlockIntegrityDict[item];
                BlockIntegrityDict.Remove(item);
                subgrid.grid.RazeGeneratedBlocks(SlimDelList);
            }
        }

        private void UpdateFatBlockDestruction(ref float fatBlockDamageThisFrame)
        {
            foreach (var item in FatDelList)
            {
                fatBlockDamageThisFrame += FatBlockIntegrityDict[item];
                FatBlockIntegrityDict.Remove(item);
            }
        }

        private void AggregateDamageOverTime()
        {
            float slimDamageLast10Seconds = 0;
            float fatBlockDamageLast10Seconds = 0;

            List<DamageEntry> oldEntries = new List<DamageEntry>();
            foreach (var entry in DamageEntries)
            {
                if (timer - entry.Timestamp <= 600)
                {
                    slimDamageLast10Seconds += entry.SlimDamage;
                    fatBlockDamageLast10Seconds += entry.FatBlockDamage;
                }
                else
                {
                    oldEntries.Add(entry);
                }
            }

            TotalDamageSum += slimDamageLast10Seconds;
            TotalDamageSum += fatBlockDamageLast10Seconds;

            foreach (var oldEntry in oldEntries) DamageEntries.Remove(oldEntry);
            DisplayTotalDamage(slimDamageLast10Seconds, fatBlockDamageLast10Seconds);
        }

        private void UpdateRenderMatrix(MatrixD renderMatrix, MatrixD rotMatrix)
        {
            var origTranslation = renderMatrix.Translation;
            renderMatrix = rotMatrix * renderMatrix;
            renderMatrix.Translation = origTranslation;

            foreach (var subgrid in gridGroup)
            {
                if (subgrid.grid != null)
                {
                    subgrid.UpdateMatrix(renderMatrix);
                }
            }
        }
    }

    public class EntVis
    {
        public MyCubeGrid realGrid;
        public MatrixD realGridBaseMatrix;
        public GridG visGrid;
        public GridG visGridString;
        public int lifetime;
        public ushort netID = 39302;
        public bool isClosed;
        public double xOffset, yOffset, rotOffset;
        public List<IMySlimBlock> BlocksForBillboards = new List<IMySlimBlock>();
        public List<MyBillboard> persistantbillboards = new List<MyBillboard>();
        public Color BillboardRED;
        public Vector4 Billboardcolor;
        private MyStringId PaperDollBGSprite = MyStringId.TryGet("paperdollBG");
        public EntVis(MyCubeGrid realGrid, double xOffset, double yOffset, double rotOffset)
        {

            this.realGrid = realGrid;
            this.realGridBaseMatrix = realGrid.WorldMatrix;
            this.xOffset = xOffset;
            this.yOffset = yOffset;
            this.rotOffset = rotOffset;
            RegisterEvents();
            GenerateClientGrids();
        }

        private void RegisterEvents() => SendMessage(new UpdateGridPacket(realGrid.EntityId, RegUpdateType.Add));

        private void SendMessage(object packet) => MyAPIGateway.Multiplayer.SendMessageTo(netID, MyAPIGateway.Utilities.SerializeToBinary(packet), MyAPIGateway.Multiplayer.ServerId);

        public void BlockRemoved(Vector3I pos) => visGrid?.DoBlockRemove(pos);

        public void GenerateClientGrids()
        {
            HandleException(() =>
            {
                var realOB = (MyObjectBuilder_CubeGrid)realGrid.GetObjectBuilder();
                realOB.CreatePhysics = false;
                MyEntities.RemapObjectBuilder(realOB);
                MyAPIGateway.Entities.CreateFromObjectBuilderParallel(realOB, false, CompleteCall);
            }, "generating client grids");
        }

        private void CompleteCall(IMyEntity obj)
        {
            HandleException(() =>
            {
                if (isClosed) return;
                var grid = (MyCubeGrid)obj;
                grid.SyncFlag = grid.Save = grid.Render.NearFlag = grid.Render.FadeIn = grid.Render.FadeOut = grid.Render.CastShadows = grid.Render.NeedsResolveCastShadow = false;
                grid.GridPresenceTier = MyUpdateTiersGridPresence.Tier1;
                MyAPIGateway.Entities.AddEntity(grid); //right hewre
                visGrid = new GridG(new GridR(grid), rotOffset);
            }, "completing the call");
        }

        public void Update()
        {
            UpdateVisLogic();
            UpdateVisPosition();
            UpdateRealLogic();
            lifetime++;
        }

        private void UpdateVisPosition()
        {
            if (visGrid != null && realGrid != null && !realGrid.MarkedForClose)
            {
                // GridR.GridBoxCenter = visGrid.gridGroup[0].grid.PositionComp.WorldVolume.Center;
                // GridR.GridBoxCenterGlobal = visGrid.gridGroup[0].grid.PositionComp.WorldVolume.Center;
                // Get the origin from the UpdateBackground method
              
                var playerCamera = MyAPIGateway.Session.Camera;
                var renderMatrix = playerCamera.WorldMatrix;

                var camera = MyAPIGateway.Session.Camera;
                var newFov = camera.FovWithZoom;
                var aspectRatio = camera.ViewportSize.X / camera.ViewportSize.Y;

                var fov = Math.Tan(newFov * 0.5);
                var scaleFov = 0.1 * fov;
                var offset = new Vector2D(xOffset + 2.52, yOffset + 1.5);
                offset.X *= scaleFov * aspectRatio;
                offset.Y *= scaleFov;
                var tempMatrix = MyAPIGateway.Session.Camera.WorldMatrix;
                var position = Vector3D.Transform(new Vector3D(offset.X, offset.Y, 10 * scaleFov), tempMatrix);

                var origin = GetBillboardOrigin(playerCamera);
                var left = tempMatrix.Left;
                var up = tempMatrix.Up;
                var hudscale = 2.55f;
                var scale = (float)(scaleFov * (hudscale * 0.23f));


                var localCenterRealGrid = new Vector3D(realGrid.PositionComp.LocalAABB.Center);
                var trueCenterRealGrid = Vector3D.Transform(localCenterRealGrid, realGrid.WorldMatrix);
                trueCenterRealGrid = localCenterRealGrid;

                var offsetMatrix = MatrixD.CreateTranslation(trueCenterRealGrid - realGrid.PositionComp.WorldAABB.Center);
                var newWorldMatrix = offsetMatrix * realGrid.WorldMatrix;

                var sanitizedRenderMatrix = renderMatrix.Translation;
                renderMatrix.Translation += renderMatrix.Forward * (0.1 / (0.6 * playerCamera.FovWithZoom)) + renderMatrix.Right * xOffset + renderMatrix.Down * yOffset;


                visGrid.UpdateMatrix(renderMatrix, newWorldMatrix * MatrixD.Invert(renderMatrix));


                UpdateBackground();

            }
        }

        // Function to get the origin from UpdateBackground
        private Vector3D GetBillboardOrigin(IMyCamera camera)
        {
            var cameraMatrix = camera.WorldMatrix;
            var fov = Math.Tan(camera.FovWithZoom * 0.5);
            var scaleFov = 0.1 * fov;
            var offset = new Vector2D(xOffset + 13, yOffset - 10);
            offset.X *= scaleFov * (camera.ViewportSize.X / camera.ViewportSize.Y);
            offset.Y *= scaleFov;
            var position = Vector3D.Transform(new Vector3D(offset.X, offset.Y, -0.9), cameraMatrix);
            return position;
        }


        //DS reference code:
        public void UpdateBackground()
        {

            var camera = MyAPIGateway.Session.Camera;
            var newFov = camera.FovWithZoom;
            var aspectRatio = camera.ViewportSize.X / camera.ViewportSize.Y;

            var fov = Math.Tan(newFov * 0.5);
            var scaleFov = 0.1 * fov;
            var offset = new Vector2D(xOffset + 6.5, yOffset - 4.85);
            offset.X *= scaleFov * aspectRatio;
            offset.Y *= scaleFov;
            var tempMatrix = MyAPIGateway.Session.Camera.WorldMatrix;
            var position = Vector3D.Transform(new Vector3D(offset.X, offset.Y, -.9), tempMatrix);
            //fix billboard rendering on top of paper doll when at maximum zoom by changing that position value from -.9 to something like -1.1 I guess
            var origin = position;
            var left = tempMatrix.Left;
            var up = tempMatrix.Up;
            var hudscale = 18f;
            var scale = (float)(scaleFov * (hudscale * 0.23f));

            Billboardcolor = (Color.Lime * 0.75f).ToVector4();
           // MyTransparentGeometry.AddBillboardOriented(PaperDollBGSprite, Billboardcolor, origin, left, up, scale, BlendTypeEnum.SDR);

        }


        private void UpdateVisLogic()
        {
            if (visGrid == null) return;
            if (!visGrid.doneInitialCleanup) visGrid.DoCleanup();
            if (!visGrid.doneRescale) visGrid.DoRescale();
        }

        private void UpdateRealLogic()
        {
            if (realGrid?.MarkedForClose == true || realGrid?.Physics == null || !realGrid.IsPowered) Close();
        }

        public void Close()
        {
            visGrid?.gridGroup.ForEach(sub => sub.grid.Close());
            SendMessage(new UpdateGridPacket(realGrid.EntityId, RegUpdateType.Remove));
            isClosed = true;
        }

        private void HandleException(Action action, string errorContext)
        {
            try { action(); }
            catch (Exception e)
            {
                MyLog.Default.WriteLine($"Error {errorContext}: {e.Message}");
                MyAPIGateway.Utilities.ShowNotification($"An error occurred while {errorContext}. Please check the log for more details.", 5000, MyFontEnum.Red);
            }
        }
    }

    // Networking
    [ProtoInclude(1000, typeof(UpdateGridPacket))]
    [ProtoInclude(2000, typeof(FeedbackDamagePacket))]
    [ProtoContract]
    public class Packet
    {
        public Packet()
        {
        }
    }

    [ProtoContract]
    public class UpdateGridPacket : Packet
    {
        [ProtoMember(1)]
        public RegUpdateType regUpdateType;
        [ProtoMember(2)]
        public List<long> entityIds;

        public UpdateGridPacket()
        {
        }

        public UpdateGridPacket(List<long> registerEntityIds, RegUpdateType regUpdateType)
        {
            this.entityIds = new List<long>(registerEntityIds);
            this.regUpdateType = regUpdateType;
        }

        public UpdateGridPacket(long registerEntityId, RegUpdateType regUpdateType)
        {
            this.entityIds = new List<long>
            {
                registerEntityId
            };
            this.regUpdateType = regUpdateType;
        }
    }

    [ProtoContract]
    public class FeedbackDamagePacket : Packet
    {
        [ProtoMember(11)]
        public long entityId;
        [ProtoMember(12)]
        public Vector3I position;

        public FeedbackDamagePacket()
        {
        }

        public FeedbackDamagePacket(long entityId, Vector3I position)
        {
            this.entityId = entityId;
            this.position = position;
        }
    }

    public enum RegUpdateType
    {
        Add,
        Remove
    }

    public enum ReqPDoll
    {
        On,
        Off
    }

    public enum ViewState
    {
        Idle,
        Searching,
        SearchingAll,
        SearchingWC,
        Locked,
        GoIdle,
        GoIdleWC,
        DoubleSearching
    }

    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class Visual : MySessionComponentBase
    {
        public ushort feedbackNetID = 38492;
        public ushort netID = 39302;
        Dictionary<ulong, List<IMyCubeGrid>> sTrkr = new Dictionary<ulong, List<IMyCubeGrid>>();
        bool validInputThisTick = false; 
        public ViewState viewState = ViewState.Idle;
        public ReqPDoll reqPDoll = ReqPDoll.Off;
        private MyStringId PDollBGSprite = MyStringId.TryGet("paperdollBG");
        public List<EntVis> allVis = new List<EntVis>();
        WcApi wcAPI;
        public HudAPIv2 hudAPI;
        public BillBoardHUDMessage billmessage;
        public HUDMessage gHud;

        public override void Init(MyObjectBuilder_SessionComponent sessionComponent)
        {

        }

        public override void LoadData()
        {
            if (MyAPIGateway.Session.IsServer)
                MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(netID, NetworkHandler);
            MyAPIGateway.Multiplayer.RegisterSecureMessageHandler(feedbackNetID, FeedbackHandler);
            if (!MyAPIGateway.Utilities.IsDedicated)
            {
                wcAPI = new WcApi();
                wcAPI.Load(WCRegistered, true);
                hudAPI = new HudAPIv2(CreateHud);
            }
        }

        private void WCRegistered() { } // needs to be here

        public override void UpdateAfterSimulation()
        {
            if (IsInvalidSession()) return;

            HandleHUDUpdates();
            HandleUserInput();

            switch (viewState)
            {
                case ViewState.SearchingWC:
                    HanVSearchWC();
                    break;
                case ViewState.GoIdle:
                case ViewState.GoIdleWC:
                    HandleViewStateIdle();
                    break;
            }
        }

        private bool IsInvalidSession()
        {
            return MyAPIGateway.Utilities.IsDedicated || MyAPIGateway.Session.Player?.Character == null || MyAPIGateway.Session.Camera == null;
        }

        private void HandleHUDUpdates()
        {
            if (hudAPI.Heartbeat)
            {
                UpdateHud();
            }
        }

        private void HandleUserInput()
        {
            validInputThisTick = ValidInput();

            if (validInputThisTick && IsAdmin(MyAPIGateway.Session.Player) && MyAPIGateway.Input.IsNewKeyPressed(MyKeys.T))
            {
                ToggleViewState();
                ToggleRequestPaperDoll();
            }
        }

        private void ToggleViewState()
        {
            viewState = viewState == ViewState.GoIdleWC ? ViewState.SearchingWC : ViewState.GoIdleWC;
        }

        private void HanVSearchWC()
        {
            MyEntity controlEnt = (MyEntity)(MyAPIGateway.Session.Player.Controller?.ControlledEntity?.Entity as IMyCockpit);
            ExecuteVSearchUpdate(controlEnt);
        }

        private void HandleViewStateIdle()
        {
            ClearAVis();
            if (viewState == ViewState.GoIdleWC && reqPDoll == ReqPDoll.On)
            {
                viewState = ViewState.SearchingWC;
            }
            else
            {
                viewState = ViewState.Idle;
            }
        }

        private void ToggleRequestPaperDoll()
        {
            reqPDoll = reqPDoll == ReqPDoll.On ? ReqPDoll.Off : ReqPDoll.On;
            string status = reqPDoll == ReqPDoll.On ? "ENABLED" : "DISABLED";
            string color = reqPDoll == ReqPDoll.On ? "Green" : "Red";
            MyAPIGateway.Utilities.ShowNotification($"PAPER DOLL {status}", 1000, color);
        }

        private void ExecuteVSearchUpdate(MyEntity controlEnt)
        {
            if (controlEnt == null || wcAPI == null)
            {
                viewState = ViewState.GoIdleWC;
                return;
            }

            var ent = wcAPI.GetAiFocus(controlEnt, 0);

            if (ent == null)
            {
                viewState = ViewState.GoIdleWC;
                return;
            }

            MyCubeGrid cGrid = ent as MyCubeGrid;

            if (cGrid != null && cGrid.Physics != null)
            {
                allVis.Add(new EntVis(cGrid, 0.11, 0.05, 0));
                viewState = ViewState.Locked;
            }
            else
            {
                viewState = ViewState.GoIdleWC;
            }
        }

        private void ClearAVis()
        {
            foreach (var entVis in allVis)
            {
                entVis.Close();
            }
            allVis.Clear();
        }



        public void CreateHud()
        {
            InitializeMainReadout();
            InitializeBillMessage();
        }

        private void InitializeMainReadout()
        {
            gHud = new HUDMessage(
                Scale: 2f,
                Font: "BI_SEOutlined",
                Message: new StringBuilder("deez"),
                Origin: new Vector2D(-.99, .99),
                HideHud: false,
                Blend: BlendTypeEnum.PostPP)
            {
                Visible = false,
                InitialColor = Color.GreenYellow * 0.75f,
            };
        }

        private void InitializeBillMessage()
        {
            billmessage = new BillBoardHUDMessage(
                PDollBGSprite,
                new Vector2D(0, 0),
                Color.Lime * 0.75f,
                new Vector2(0, 0),
                -1, 1, 1, 1, 0,
                false, true,
                BlendTypeEnum.PostPP)
            {
                Visible = false,
            };
        }

        public void UpdateHud()
        {
            HandEx(() =>
            {
                if (gHud == null || billmessage == null)
                {
                    CreateHud();
                }
                gHud.Message.Clear();
            }, "initializing HUD");

            foreach (var entVis in allVis)
            {
                UpdateHudElement(entVis);
            }
        }

        private void UpdateHudElement(EntVis entVis)
        {
            HandEx(() =>
            {
                float tempScaling = GridR.billboardScaling * 25;
                Vector3D position = GridR.hateVector;
                Vector3D targetHudPos = MyAPIGateway.Session.Camera.WorldToScreen(ref position);
                Vector2D newOrigin = new Vector2D(targetHudPos.X, targetHudPos.Y);
                Vector3D cameraForward = MyAPIGateway.Session.Camera.WorldMatrix.Forward;
                Vector3D toTarget = position - MyAPIGateway.Session.Camera.WorldMatrix.Translation;
                float fov = MyAPIGateway.Session.Camera.FieldOfViewAngle;
                var angle = GetAngBetwDeg(toTarget, cameraForward);
                string bruh = GridG.DamageEntriesString;
                var distance = Vector3D.Distance(MyAPIGateway.Session.Camera.WorldMatrix.Translation, position);

                gHud.Visible = true;
                gHud.Scale = tempScaling - MathHelper.Clamp(distance / 20000, 0, 0.9) + (30 / Math.Max(60, angle * angle * angle));
                gHud.Message.Append(bruh);
                gHud.Origin = new Vector2D(targetHudPos.X, targetHudPos.Y);
                gHud.Offset = -gHud.GetTextLength() / 2 + new Vector2(0, 0.3f);
            }, "updating HUD element for " + entVis);
        }

        private static double GetAngBetwDeg(Vector3D vectorA, Vector3D vectorB)
        {
            vectorA.Normalize();
            vectorB.Normalize();
            return Math.Acos(MathHelper.Clamp(vectorA.Dot(vectorB), -1, 1)) * (180.0 / Math.PI);
        }
        public override void Draw()
        {
            HandEx(() =>
            {
                if (InvalForDraw()) return;

                if (allVis == null)
                {
                    MyLog.Default.WriteLine("allVis is null");
                    return;
                }

                if (viewState == ViewState.Locked)
                {
                    UpdateAllVis();
                    HandleControlEntity();
                }

            }, "Drawing On-Screen Elements");
        }

        private bool InvalForDraw()
        {
            return MyAPIGateway.Utilities.IsDedicated ||
                   MyAPIGateway.Session.Player?.Character == null ||
                   MyAPIGateway.Session.Camera == null;
        }

        private void UpdateAllVis()
        {
            for (int i = allVis.Count - 1; i >= 0; i--)
            {
                allVis[i].Update();
                if (allVis[i].isClosed) allVis.RemoveAtFast(i);
            }
        }

        private void HandleControlEntity()
        {
            MyEntity cEnt = null;
            if (MyAPIGateway.Session.Player.Controller?.ControlledEntity?.Entity is IMyCockpit)
            {
                IMyCockpit cock = MyAPIGateway.Session.Player.Controller?.ControlledEntity?.Entity as IMyCockpit;
                cEnt = cock.CubeGrid as MyEntity;
            }

            if (cEnt != null && wcAPI != null)
            {
                ManEntFoc(cEnt);
            }
            else
            {
                ClearAVis();
            }

            if (allVis.Count == 0 || reqPDoll == ReqPDoll.Off)
            {
                viewState = ViewState.GoIdleWC;
            }
        }

        private void ManEntFoc(MyEntity cEnt)
        {
            var ent = wcAPI.GetAiFocus(cEnt, 0);
            if (ent == null)
            {
                ClearAVis();
                return;
            }

            MyCubeGrid cGrid = ent as MyCubeGrid;
            if (cGrid != null && cGrid.Physics != null)
            {
                bool isTrack = IsEntityTracked(cGrid);
                if (!isTrack)
                {
                    ClearAVis();
                    EntVis vis = new EntVis(cGrid, 0, 0, 0);
                    allVis.Add(vis);
                }
            }
            else
            {
                ClearAVis();
            }
        }

        private bool IsEntityTracked(MyCubeGrid cGrid)
        {
            foreach (var vis in allVis)
            {
                if (vis.realGrid.EntityId == cGrid.EntityId)
                {
                    return true;
                }
            }
            return false;
        }


        private void NetworkHandler(ushort arg1, byte[] arg2, ulong iSID, bool arg4)
        {
            HandEx(() =>
            {
                if (IsInvalidPacket(arg2)) return;

                var packet = DesPacket(arg2);
                if (packet == null || !MyAPIGateway.Session.IsServer) return;

                var uGP = packet as UpdateGridPacket;
                if (uGP == null) return;

                UpSerTrkr(iSID, uGP);

            }, "Handling Network Packet");
        }

        private bool IsInvalidPacket(byte[] arg2)
        {
            if (arg2 == null)
            {
                MyLog.Default.WriteLine("Null argument 'arg2' NetworkHandler!");
                return true;
            }
            return false;
        }

        private Packet DesPacket(byte[] arg2)
        {
            return MyAPIGateway.Utilities.SerializeFromBinary<Packet>(arg2);
        }
        private void FeedbackHandler(ushort arg1, byte[] arg2, ulong arg3, bool arg4)
        {
            HandEx(() =>
            {
                if (ArgInvalid(arg2)) return;

                var packet = DesPacket(arg2);
                if (packet == null) return;

                var fDP = packet as FeedbackDamagePacket;
                if (fDP == null) return;

                UpEnFd(fDP);

            }, "Handling Feedback Packet");
        }

        private bool ArgInvalid(byte[] arg2)
        {
            if (arg2 == null || allVis == null)
            {
                MyLog.Default.WriteLine("Null arguments to FeedbackHandler.");
                return true;
            }
            return false;
        }

        private void UpEnFd(FeedbackDamagePacket fDP)
        {
            foreach (var eVis in allVis)
            {
                if (eVis?.realGrid?.EntityId == fDP.entityId)
                {
                    eVis.BlockRemoved(fDP.position);
                }
            }
        }

        private void UpSerTrkr(ulong sID, UpdateGridPacket uGP)
        {
            HandEx(() =>
            {
                if (ArgInvalid(uGP)) return;

                switch (uGP.regUpdateType)
                {
                    case RegUpdateType.Add:
                        HandleAddOperation(sID, uGP);
                        break;
                    case RegUpdateType.Remove:
                        HandleRemoveOperation(sID, uGP);
                        break;
                }

            }, "Updating Server Tracker");
        }

        private bool ArgInvalid(UpdateGridPacket uGP)
        {
            if (uGP == null || sTrkr == null)
            {
                MyLog.Default.WriteLine("Null in UpdateServerTracker.");
                return true;
            }
            return false;
        }

        private void HandleAddOperation(ulong sID, UpdateGridPacket uGP)
        {
            if (sTrkr.ContainsKey(sID))
            {
                AddGrdTrkr(sID, uGP.entityIds);
            }
            else
            {
                List<IMyCubeGrid> gTrack = CreateGrdTrkr(uGP.entityIds);
                sTrkr.Add(sID, gTrack);
            }
        }

        private void HandleRemoveOperation(ulong sID, UpdateGridPacket uGP)
        {
            if (sTrkr.ContainsKey(sID))
            {
                RemGrdTrkr(sID, uGP.entityIds);
            }
        }

        // Adds grids to the server tracker
        private void AddGrdTrkr(ulong sID, List<long> eID)
        {
            HandEx(() =>
            {
                if (AreArgumentsInvalid(eID)) return;

                foreach (var entId in eID)
                {
                    AddEntityToTracker(sID, entId);
                }

            }, "Adding Grids to Tracker");
        }

        // Creates a new grid tracker
        private List<IMyCubeGrid> CreateGrdTrkr(List<long> eIDs)
        {
            List<IMyCubeGrid> gTracker = new List<IMyCubeGrid>();

            HandEx(() =>
            {
                if (eIDs == null)
                {
                    LogInvalidArguments("CreateGTrack");
                    return;
                }

                foreach (var entId in eIDs)
                {
                    AddEntToTrk(gTracker, entId);
                }

            }, "Creating GTrack");

            return gTracker;
        }

        // Removes grids from the server tracker
        private void RemGrdTrkr(ulong sID, List<long> eID)
        {
            HandEx(() =>
            {
                if (AreArgumentsInvalid(eID, sID)) return;

                foreach (var entId in eID)
                {
                    RemEntTrk(sID, entId);
                }

            }, "Removing Grids from Tracker");
        }

        // Helper methods
        private bool AreArgumentsInvalid(List<long> eID, ulong? sID = null)
        {
            if (eID == null || sTrkr == null || (sID.HasValue && !sTrkr.ContainsKey(sID.Value)))
            {
                LogInvalidArguments("Arguments are null or missing keys");
                return true;
            }
            return false;
        }

        private void LogInvalidArguments(string mName)
        {
            MyLog.Default.WriteLine($"Null arguments provided to {mName}. Exiting to prevent issues.");
        }

        private void AddEntityToTracker(ulong sID, long entId)
        {
            IMyCubeGrid cGrid = MyAPIGateway.Entities.GetEntityById(entId) as IMyCubeGrid;
            if (cGrid != null)
            {
                cGrid.OnBlockRemoved += SerBRem;
                if (sTrkr.ContainsKey(sID))
                {
                    sTrkr[sID].Add(cGrid);
                }
                else
                {
                    MyLog.Default.WriteLine($"SteamID {sID} not found in serverTracker. Abandon ship!");
                }
            }
        }

        private void AddEntToTrk(List<IMyCubeGrid> gTrack, long entId)
        {
            IMyCubeGrid cGrid = MyAPIGateway.Entities.GetEntityById(entId) as IMyCubeGrid;
            if (cGrid != null)
            {
                cGrid.OnBlockRemoved += SerBRem;
                gTrack.Add(cGrid);
            }
        }

        private void RemEntTrk(ulong sID, long entId)
        {
            IMyCubeGrid cGrid = MyAPIGateway.Entities.GetEntityById(entId) as IMyCubeGrid;
            if (cGrid != null)
            {
                cGrid.OnBlockRemoved -= SerBRem;
                sTrkr[sID]?.Remove(cGrid);
            }
        }


        //fun stops here
        private void SerBRem(IMySlimBlock obj)
        {
            HandEx(() =>
            {
                if (obj == null || sTrkr == null)
                {
                    MyLog.Default.WriteLine("Null arguments in ServerBlockRemoved.");
                    return;
                }

                var dgrd = obj.CubeGrid;
                if (dgrd == null) return;

                foreach (var sID in sTrkr.Keys)
                {
                    if (sTrkr[sID]?.Count > 0)
                    {
                        foreach (var cGrid in sTrkr[sID])
                        {
                            if (cGrid?.EntityId == dgrd.EntityId)
                            {
                                var fDP = new FeedbackDamagePacket(dgrd.EntityId, obj.Position);
                                var byteArray = MyAPIGateway.Utilities.SerializeToBinary(fDP);
                                MyAPIGateway.Multiplayer.SendMessageTo(feedbackNetID, byteArray, sID);
                                break;
                            }
                        }
                    }
                }
            }, "Removing Server Block");
        }
        private static void HandEx(Action act, string ctx)
        {
            try { act(); }
            catch (Exception e)
            {
                MyLog.Default.WriteLine($"Err {ctx}: {e.Message}");
                MyAPIGateway.Utilities.ShowNotification($"Error in {ctx}. Check Log.", 5000, MyFontEnum.Red);
            }
        }

        private bool ValidInput()
        {
            var gui = MyAPIGateway.Gui;
            return MyAPIGateway.Session.CameraController != null && !gui.ChatEntryVisible && !gui.IsCursorVisible && gui.GetCurrentScreen == MyTerminalPageEnum.None;
        }

        private bool IsAdmin(IMyPlayer s) => s != null && (s.PromoteLevel == MyPromoteLevel.Admin || s.PromoteLevel == MyPromoteLevel.Owner);

        protected override void UnloadData()
        {
            foreach (var e in allVis) e.Close();
            var mp = MyAPIGateway.Multiplayer;
            if (MyAPIGateway.Session.IsServer) mp.UnregisterSecureMessageHandler(netID, NetworkHandler);
            mp.UnregisterSecureMessageHandler(feedbackNetID, FeedbackHandler);
            wcAPI?.Unload();
        }

    }

}