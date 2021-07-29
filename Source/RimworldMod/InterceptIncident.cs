using System;
using System.Linq;
using HarmonyLib;
using Verse;

namespace RimWorld
{
    [HarmonyPatch(typeof(IncidentWorker), "TryExecute")]
    public static class InterceptIncident
    {
        private static bool premonitionActive;

        [HarmonyPrefix]
        public static bool Intercept(IncidentParms parms, IncidentWorker __instance)
        {
            premonitionActive = false;
            if (parms.forced)
            {
                return true;
            }

            foreach (var pawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_Colonists)
            {
                if (!pawn.health.hediffSet.HasHediff(HediffDef.Named("PsychicPowerPremonition")))
                {
                    continue;
                }

                premonitionActive = true;
                break;
            }

            if (!__instance.CanFireNow(parms))
            {
                premonitionActive = false;
            }

            if (!premonitionActive)
            {
                return !premonitionActive;
            }

            parms.forced = true;
            var delay = Rand.Range(2500, 20000);
            var firingIncident = new FiringIncident(__instance.def, null, parms);
            if (firingIncident.parms.target == null)
            {
                firingIncident.parms.target = Find.Maps.First();
            }

            var queuedIncident = new QueuedIncident(firingIncident, Find.TickManager.TicksGame + delay);
            Find.Storyteller.incidentQueue.Add(queuedIncident);
            Find.LetterStack.ReceiveLetter("Premonition",
                "A colonist's premonitions have become clear! The next " + __instance.def.label +
                " event will occur in approximately " + (int) Math.Round((float) delay / 2500) + " hours.",
                LetterDefOf.NeutralEvent);

            return !premonitionActive;
        }

        [HarmonyPostfix]
        public static void ReturnTrue(ref bool __result)
        {
            if (premonitionActive)
            {
                __result = true;
            }
        }
    }
}