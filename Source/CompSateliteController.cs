using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace MortarAssistance
{
    public class CompSateliteController : CompRefuelable
    {
        private bool isSateliteSend;
        public new CompProperties_SateliteController Props => props as CompProperties_SateliteController;
        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.isSateliteSend, "isSateliteSend", false, false);
        }
        public bool IsSateliteSend
        {
            get => isSateliteSend;
            set
            {
                if (value == true)
                {
                    this.allowAutoRefuel = false;
                    this.TargetFuelLevel = 0f;
                }
                else
                {
                    this.allowAutoRefuel = true;
                    this.TargetFuelLevel = 150f;
                }
                isSateliteSend = value;
            }
        }
        public override void CompTick()
        {
            base.CompTick();
            if (IsSateliteSend&& this.parent.IsHashIntervalTick(900))
            {
                Log.Message("Decrease! "+(parent.GetComp<CompRefuelable>()==null).ToString());
                ConsumeFuel(this.Props.decreasePerLongTick);
                if (Fuel <= 0f) IsSateliteSend = false;
            }
        }
        public override void CompTickRare()
        {
            base.CompTickRare();
            
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var item in base.CompGetGizmosExtra())
            {
                if (isSateliteSend)//'till satelite not send, we dont care
                {
                    if(item is Command_SetTargetFuelLevel||item is Command_Toggle)
                    {
                        item.Disable("SateliteAlreadyAtOrbit");
                    }
                }
                yield return item;
            }
            yield break;
        }
    }
    public class CompProperties_SateliteController:CompProperties_Refuelable
    {
        public float decreasePerLongTick=0.5f;
        public float decrasePerAIMing = 0.01f;
        public CompProperties_SateliteController()
        {
            this.compClass = typeof(CompSateliteController);
        }
    }
}
