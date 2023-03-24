using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CombatExtended;
using HarmonyLib;
using Verse;

namespace MortarAssistance
{
    [HarmonyPatch(typeof(Building_TurretGunCE))]
    public static class HarmonyPatch_Building_TurretGunCE
    {
        [HarmonyPostfix()]
        [HarmonyPatch("get_CanSetForcedTarget")]
        public static bool CanSetForcedTargetPostfix(bool __result, Building_TurretGunCE __instance)
        {
            if (!__result && __instance is Building_TurretMortarAssistance && __instance.PlayerControlled)
                return true;
            return __result;
        }
        [HarmonyPrefix()]
        [HarmonyPatch(nameof(Building_TurretGunCE.TryFindNewTarget))]
        public static bool TryFindNewTargetPrefix(object __instance,ref object __result)
        {
            if (__instance is Building_TurretMortarAssistance)
            {
                __result = LocalTargetInfo.Invalid;
                return false;
            }

            return true;

        }
    }
}
