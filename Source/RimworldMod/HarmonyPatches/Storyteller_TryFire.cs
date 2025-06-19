using HarmonyLib;
using RimWorld;
using Verse;

namespace RimworldMod.HarmonyPatches;

[HarmonyPatch(typeof(Storyteller), nameof(Storyteller.TryFire))]
public static class Storyteller_TryFire
{
    public static void Prefix(FiringIncident fi)
    {
        fi.parms.target ??= Find.World;
    }
}