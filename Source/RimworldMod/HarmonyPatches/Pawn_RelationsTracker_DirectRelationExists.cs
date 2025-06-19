using HarmonyLib;
using RimWorld;
using Verse;

namespace RimworldMod.HarmonyPatches;

[HarmonyPatch(typeof(Pawn_RelationsTracker), nameof(Pawn_RelationsTracker.DirectRelationExists))]
public static class Pawn_RelationsTracker_DirectRelationExists
{
    public static void Postfix(PawnRelationDef def, Pawn otherPawn, ref bool __result)
    {
        if (def == PawnRelationDefOf.Bond &&
            otherPawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerAffinity")))
        {
            __result = true;
        }
    }
}