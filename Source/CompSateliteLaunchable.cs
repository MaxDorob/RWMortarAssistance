using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static MortarAssistance.PlaceWorker_NeedsSateliteLauncher;

namespace MortarAssistance
{
    public class CompSateliteLaunchable : ThingComp
    {
        public CompProperties_SateliteLaunchable Props => props as CompProperties_SateliteLaunchable;
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }



            Command_Action command_Action = new Command_Action();
            command_Action.defaultLabel = "CommandLaunchGroup".Translate();
            command_Action.defaultDesc = "CommandLaunchGroupDesc".Translate();
            command_Action.icon = CompLaunchable.LaunchCommandTex;
            command_Action.alsoClickIfOtherInGroupClicked = false;
            command_Action.action = delegate ()
            {
                this.TryLaunch();
            };
            if (!this.ConnectedToFuelingPort)
            {
                command_Action.Disable("CommandLaunchGroupFailNotConnectedToFuelingPort".Translate());
            }
            else if (!this.FuelingPortSourceHasAnyFuel || this.FuelingPortSource.GetComp<CompSateliteController>().IsSateliteSend)
            {
                command_Action.Disable("CommandLaunchGroupFailNoFuel".Translate());
            }
            else if (this.IsUnderRoof)
            {
                command_Action.Disable("CommandLaunchGroupFailUnderRoof".Translate());
            }
            yield return command_Action;

            yield break;
        }
        public Building FuelingPortSource
        {
            get
            {
                return SateliteLauncherUtility.FuelingPortGiverAtFuelingPortCell(this.parent.Position, this.parent.Map);
            }
        }
        public bool ConnectedToFuelingPort
        {
            get
            {
                return /*!this.Props.requireFuel || */
                    this.FuelingPortSource != null;
            }
        }
        public bool FuelingPortSourceHasAnyFuel
        {
            get
            {
                return /*!this.Props.requireFuel || */
                    (this.ConnectedToFuelingPort && this.FuelingPortSource.GetComp<CompRefuelable>().HasFuel);
            }
        }
        public bool EnoughFuel
        => (FuelingPortSource?.GetComp<CompSateliteController>()?.Fuel ?? 0f) > Props.ConsumePerStart;
        public bool IsUnderRoof
        {
            get => parent.Position.Roofed(this.parent.Map);


        }
        public void TryLaunch()
        {
            if (!this.parent.Spawned)
            {
                Log.Error("Tried to launch " + this.parent + ", but it's unspawned.");
                return;
            }

            if (!this.ConnectedToFuelingPort || !this.FuelingPortSourceHasAnyFuel || !EnoughFuel)
            {
                return;
            }
            //Map map = this.parent.Map;
            //int num = Find.WorldGrid.TraversalDistanceBetween(map.Tile, destinationTile, true, int.MaxValue);
            //if (num > this.MaxLaunchDistance)
            //{
            //    return;
            //}
            //this.Transporter.TryRemoveLord(map);
            //int groupID = this.Transporter.groupID;
            //float amount = Mathf.Max(CompLaunchable.FuelNeededToLaunchAtDist((float)num), 1f);
            //for (int i = 0; i < transportersInGroup.Count; i++)
            //{
            //CompTransporter compTransporter = transportersInGroup[i];
            Building fuelingPortSource = FuelingPortSource;
            if (fuelingPortSource != null)
            {
                var comp = fuelingPortSource.TryGetComp<CompSateliteController>();
                comp.IsSateliteSend = true;
                comp.ConsumeFuel(Props.ConsumePerStart);

            }
            //ThingOwner directlyHeldThings = compTransporter.GetDirectlyHeldThings();
            //ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDefOf.ActiveDropPod, null);
            //activeDropPod.Contents = new ActiveDropPodInfo();
            //activeDropPod.Contents.innerContainer.TryAddRangeOrTransfer(directlyHeldThings, true, true);
            var flyShipLeaving = SkyfallerMaker.MakeSkyfaller(/*this.Props.skyfallerLeaving ??*/ DefDatabase<ThingDef>.GetNamed("SateliteLeaving")/*, activeDropPod*/);
            //flyShipLeaving.groupID = 9999999;
            //flyShipLeaving.createWorldObject = false;
            //flyShipLeaving.Contents = null;
            //flyShipLeaving.destinationTile = destinationTile;
            //flyShipLeaving.arrivalAction = arrivalAction;
            //flyShipLeaving.worldObjectDef = WorldObjectDefOf.TravelingTransportPods;
            //compTransporter.CleanUpLoadingVars(map);
            parent.Destroy(DestroyMode.Vanish);
            GenSpawn.Spawn(flyShipLeaving, parent.Position, fuelingPortSource.Map, WipeMode.Vanish);
            //}
            //CameraJumper.TryHideWorld();
        }
    }
    public class CompProperties_SateliteLaunchable : CompProperties
    {
        public float ConsumePerStart = 10f;
        public CompProperties_SateliteLaunchable()
        {
            this.compClass = typeof(CompSateliteLaunchable);
        }
    }
}
