using CombatExtended;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace MortarAssistance
{
    public class CompMortarAssistance : ThingComp//, IVerbOwner
    {
        //public CompMortarAssistance()
        //{
        //    verbTracker = new VerbTracker(this);
        //}
        public override void CompTick()
        {
            base.CompTick();
        }
        

        //    public string UniqueVerbOwnerID()
        //    {
        //        return "MortarAssistance_" + parent.ThingID;
        //    }

        //    public bool VerbsStillUsableBy(Pawn p)
        //    {
        //        return false;
        //        throw new NotImplementedException();
        //    }

        //    private VerbTracker verbTracker;

        //    public VerbTracker VerbTracker => verbTracker;

        //    public List<VerbProperties> VerbProperties => parent.def.Verbs;

        //    public List<Tool> Tools => parent.def.tools;

        //    public ImplementOwnerTypeDef ImplementOwnerTypeDef => ImplementOwnerTypeDefOf.NativeVerb;

        //    public Thing ConstantCaster => parent;
    }
}
