using HarmonyLib;
using Verse;

namespace RimWorld
{
    [HarmonyPatch(typeof(Storyteller), "TryFire")]
    public static class FixPremonitionBug
    {
        [HarmonyPrefix]
        public static void fakeTarget(FiringIncident fi)
        {
            if (fi.parms.target == null)
            {
                fi.parms.target = Find.World;
            }
        }
    }
}