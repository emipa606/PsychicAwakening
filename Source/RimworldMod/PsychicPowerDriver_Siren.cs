using Verse;

namespace RimWorld;

internal class PsychicPowerDriver_Siren : PsychicPowerDriver
{
    public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
    {
        var raidParms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, user.Map);
        raidParms.forced = true;
        var qi = new QueuedIncident(new FiringIncident(IncidentDef.Named("RaidFriendlySiren"), null, raidParms),
            Find.TickManager.TicksGame + 30);
        Find.Storyteller.incidentQueue.Add(qi);
    }
}