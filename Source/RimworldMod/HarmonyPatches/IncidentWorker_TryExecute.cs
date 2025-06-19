using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimworldMod.HarmonyPatches;

[HarmonyPatch(typeof(IncidentWorker), nameof(IncidentWorker.TryExecute))]
public static class IncidentWorker_TryExecute
{
    private static bool premonitionActive;

    public static bool Prefix(IncidentParms parms, IncidentWorker __instance)
    {
        premonitionActive = false;
        if (parms.forced)
        {
            return true;
        }

        foreach (var pawn in PawnsFinder.AllMapsCaravansAndTravellingTransporters_Alive_Colonists)
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
        firingIncident.parms.target ??= Find.Maps.First();

        var queuedIncident = new QueuedIncident(firingIncident, Find.TickManager.TicksGame + delay);
        Find.Storyteller.incidentQueue.Add(queuedIncident);
        Find.LetterStack.ReceiveLetter("Premonition",
            $"A colonist's premonitions have become clear! The next {__instance.def.label} event will occur in approximately {(int)Math.Round((float)delay / 2500)} hours.",
            LetterDefOf.NeutralEvent);

        return !premonitionActive;
    }

    public static void Postfix(ref bool __result)
    {
        if (premonitionActive)
        {
            __result = true;
        }
    }
}