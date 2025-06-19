using HarmonyLib;
using RimWorld;
using Verse;

namespace RimworldMod.HarmonyPatches;

[HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Learn))]
public static class SkillRecord_Learn
{
    public static void Postfix(SkillRecord __instance, float xp, bool direct, ref Pawn ___pawn)
    {
        if (direct || !(xp > 0))
        {
            return;
        }

        if (___pawn != null && !___pawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerUnity")))
        {
            return;
        }

        var comp = ___pawn?.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("PsychicPowerUnity"))
            .TryGetComp<HediffComp_OtherPawn>();

        var other = comp?.otherPawn;
        other?.skills?.Learn(__instance.def, xp, true);
    }
}