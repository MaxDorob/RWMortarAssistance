using CombatExtended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MortarAssistance
{
    
    public class Building_TurretMortarAssistance : Building_TurretGunCE
    {
        CompSateliteController SateliteController;
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            SateliteController = GetComp<CompSateliteController>();
        }
        public override void Tick()
        {
            if (SateliteController != null && SateliteController.Fuel <= 0f)
                ResetForcedTarget();
            if (SateliteController!=null&& forcedTarget != LocalTargetInfo.Invalid&& SateliteController.IsSateliteSend)
            {
                SateliteController.ConsumeFuel(SateliteController.Props.decrasePerAIMing);
            }
            base.Tick();
        }
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (var item in base.GetGizmos())
            {
                var vt = item as Command_VerbTarget;
                if (vt != null && (!this.GetComp<CompSateliteController>()?.IsSateliteSend ?? false))
                {
                    vt.Disable("CannotFire".Translate() + ": " + "Roofed".Translate().CapitalizeFirst());
                }
                yield return item;

            }
            
            //var vt = new Command_VerbTarget
            //{
            //    defaultLabel = "CommandSetForceAttackTarget".Translate(),
            //    defaultDesc = "CommandSetForceAttackTargetDesc".Translate(),
            //    icon = ContentFinder<Texture2D>.Get("UI/Commands/Attack", true),
            //    verb = (parent as Building_TurretGunCE).AttackVerb

            //};
            //Log.Message(verbTracker.AllVerbs[0].Targetable.ToString()+", "+verbTracker.AllVerbs[0].verbProps.nonInterruptingSelfCast.ToString());

            //yield return vt;
        }
    }
}
