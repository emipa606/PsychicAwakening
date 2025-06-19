using HarmonyLib;
using RimWorld;
using Verse;

namespace RimworldMod.HarmonyPatches;

[HarmonyPatch(typeof(SkillRecord), nameof(SkillRecord.Level), MethodType.Getter)]
public static class SkillRecord_Level
{
    public static void Postfix(SkillRecord __instance, ref int __result, ref Pawn ___pawn)
    {
        if (___pawn != null && !___pawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerUnity")))
        {
            return;
        }

        var comp = ___pawn?.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("PsychicPowerUnity"))
            .TryGetComp<HediffComp_OtherPawn>();

        var other = comp?.otherPawn;
        if (other?.skills != null && other.skills.GetSkill(__instance.def).levelInt > __result)
        {
            __result = other.skills.GetSkill(__instance.def).levelInt;
        }
    }
}