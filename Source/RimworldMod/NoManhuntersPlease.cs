using HarmonyLib;
using Verse;
using Verse.AI;

namespace RimWorld
{
    [HarmonyPatch(typeof(Pawn_MindState), "CheckStartMentalStateBecauseRecruitAttempted")]
    public static class NoManhuntersPlease
    {
        [HarmonyPrefix]
        public static bool AffinityInterrupts(Pawn_MindState __instance)
        {
            return !__instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerAffinity"));
        }

        [HarmonyPostfix]
        public static void AffinityPrevents(Pawn_MindState __instance, ref bool __result)
        {
            if (__instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerAffinity")))
            {
                __result = false;
            }
        }
    }
}