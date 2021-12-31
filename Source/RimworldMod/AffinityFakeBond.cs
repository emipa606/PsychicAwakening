using HarmonyLib;
using Verse;

namespace RimWorld;

[HarmonyPatch(typeof(Pawn_RelationsTracker), "DirectRelationExists")]
public static class AffinityFakeBond
{
    [HarmonyPostfix]
    public static void FakeBond(PawnRelationDef def, Pawn otherPawn, ref bool __result)
    {
        if (def == PawnRelationDefOf.Bond &&
            otherPawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerAffinity")))
        {
            __result = true;
        }
    }
}