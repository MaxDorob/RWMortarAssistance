
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static MortarAssistance.PlaceWorker_NeedsSateliteLauncher;

namespace MortarAssistance
{
    [StaticConstructorOnStartup]
    public class PlaceWorker_SateliteLauncher : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Map currentMap = Find.CurrentMap;
            if (def.building == null || def != PlaceWorker_NeedsSateliteLauncher.SateliteLauncher)
            {
                return;
            }
            if (!SateliteLauncherUtility.GetFuelingPortCell(center, rot).Standable(currentMap))
            {
                return;
            }
            PlaceWorker_SateliteLauncher.DrawFuelingPortCell(center, rot);
        }

        // Token: 0x06009BA2 RID: 39842 RVA: 0x003853AC File Offset: 0x003835AC
        public static void DrawFuelingPortCell(IntVec3 center, Rot4 rot)
        {
            Vector3 position = SateliteLauncherUtility.GetFuelingPortCell(center, rot).ToVector3ShiftedWithAltitude(AltitudeLayer.MetaOverlays);
            Graphics.DrawMesh(MeshPool.plane10, position, Quaternion.identity, PlaceWorker_SateliteLauncher.FuelingPortCellMaterial, 0);
        }

        // Token: 0x04006209 RID: 25097
        private static readonly Material FuelingPortCellMaterial = MaterialPool.MatFrom("UI/Overlays/FuelingPort", ShaderDatabase.Transparent);
    }

    public class PlaceWorker_NeedsSateliteLauncher : PlaceWorker
    {
        internal static Def SateliteLauncher = DefDatabase<ThingDef>.GetNamed("SatelliteLauncher");
        public override AcceptanceReport AllowsPlacing(BuildableDef def, IntVec3 center, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            if (SateliteLauncherUtility.FuelingPortGiverAtFuelingPortCell(center, map) == null)
            {
                return "MustPlaceNearFuelingPort".Translate();
            }
            return true;
        }

        // Token: 0x06009BA6 RID: 39846 RVA: 0x0038541C File Offset: 0x0038361C
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            Map currentMap = Find.CurrentMap;
            List<Building> allBuildingsColonist = currentMap.listerBuildings.allBuildingsColonist;
            for (int i = 0; i < allBuildingsColonist.Count; i++)
            {
                Building building = allBuildingsColonist[i];
                if (building.def == SateliteLauncher && !Find.Selector.IsSelected(building) && SateliteLauncherUtility.GetFuelingPortCell(building).Standable(currentMap))
                {
                    PlaceWorker_SateliteLauncher.DrawFuelingPortCell(building.Position, building.Rotation);
                }
            }
        }


        public static class SateliteLauncherUtility
        {
            public static IntVec3 GetFuelingPortCell(Building podLauncher)
            {
                return SateliteLauncherUtility.GetFuelingPortCell(podLauncher.Position, podLauncher.Rotation);
            }


            public static IntVec3 GetFuelingPortCell(IntVec3 center, Rot4 rot)
            {
                rot.Rotate(RotationDirection.Clockwise);
                return center + rot.FacingCell;
            }


            public static bool AnyFuelingPortGiverAt(IntVec3 c, Map map)
            {
                return SateliteLauncherUtility.FuelingPortGiverAt(c, map) != null;
            }

            public static Building FuelingPortGiverAt(IntVec3 c, Map map)
            {
                List<Thing> thingList = c.GetThingList(map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    Building building = thingList[i] as Building;
                    if (building != null && building.def==SateliteLauncher)
                    {
                        return building;
                    }
                }
                return null;
            }


            public static Building FuelingPortGiverAtFuelingPortCell(IntVec3 c, Map map)
            {
                for (int i = 0; i < 4; i++)
                {
                    IntVec3 c2 = c + GenAdj.CardinalDirections[i];
                    if (c2.InBounds(map))
                    {
                        List<Thing> thingList = c2.GetThingList(map);
                        for (int j = 0; j < thingList.Count; j++)
                        {
                            Building building = thingList[j] as Building;
                            if (building != null && building.def == SateliteLauncher && SateliteLauncherUtility.GetFuelingPortCell(building) == c)
                            {
                                return building;
                            }
                        }
                    }
                }
                return null;
            }

            // Token: 0x060096C9 RID: 38601 RVA: 0x0036A478 File Offset: 0x00368678
            public static CompLaunchable LaunchableAt(IntVec3 c, Map map)
            {
                List<Thing> thingList = c.GetThingList(map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    CompLaunchable compLaunchable = thingList[i].TryGetComp<CompLaunchable>();
                    if (compLaunchable != null)
                    {
                        return compLaunchable;
                    }
                }
                return null;
            }
        }
    }

}
