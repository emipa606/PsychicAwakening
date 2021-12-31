using System.Reflection;
using HarmonyLib;
using Verse;

namespace RimWorld;

[HarmonyPatch(typeof(SkillRecord))]
[HarmonyPatch("Level", MethodType.Getter)]
public static class UnitySkills
{
    [HarmonyPostfix]
    public static void UseHigherLevel(SkillRecord __instance, ref int __result)
    {
        var pawn = (Pawn)typeof(SkillRecord).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.GetValue(__instance);
        if (pawn != null && !pawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerUnity")))
        {
            return;
        }

        var comp = pawn?.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("PsychicPowerUnity"))
            .TryGetComp<HediffComp_OtherPawn>();
        if (comp == null)
        {
            return;
        }

        var other = comp.otherPawn;
        if (other.skills != null && other.skills.GetSkill(__instance.def).levelInt > __result)
        {
            __result = other.skills.GetSkill(__instance.def).levelInt;
        }
    }
}