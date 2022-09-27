using System.Reflection;
using HarmonyLib;
using Verse;

namespace RimWorld;

[HarmonyPatch(typeof(SkillRecord), "Learn")]
public static class UnityLearning
{
    [HarmonyPostfix]
    public static void BothGainXP(SkillRecord __instance, float xp, bool direct)
    {
        if (direct || !(xp > 0))
        {
            return;
        }

        var pawn = (Pawn)typeof(SkillRecord).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(__instance);
        if (pawn != null && !pawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerUnity")))
        {
            return;
        }

        var comp = pawn?.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("PsychicPowerUnity"))
            .TryGetComp<HediffComp_OtherPawn>();

        var other = comp?.otherPawn;
        other?.skills?.Learn(__instance.def, xp, true);
    }
}