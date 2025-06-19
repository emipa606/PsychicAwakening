using HarmonyLib;
using Verse;
using Verse.AI;

namespace RimworldMod.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_MindState), "CheckStartMentalStateBecauseRecruitAttempted")]
public static class Pawn_MindState_CheckStartMentalStateBecauseRecruitAttempted
{
    public static bool Prefix(Pawn_MindState __instance)
    {
        return !__instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerAffinity"));
    }

    public static void Postfix(Pawn_MindState __instance, ref bool __result)
    {
        if (__instance.pawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerAffinity")))
        {
            __result = false;
        }
    }
}