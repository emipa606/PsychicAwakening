using Verse;

namespace RimWorld
{
    internal class PsychicPowerDriver_AddHediff : PsychicPowerDriver
    {
        public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
        {
            var theHediff = HediffMaker.MakeHediff(power.hediff, target);
            theHediff.Severity = user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                 target.GetStatValue(StatDefOf.PsychicSensitivity);
            if (theHediff.TryGetComp<HediffComp_OtherPawn>() != null)
            {
                theHediff.TryGetComp<HediffComp_OtherPawn>().otherPawn = user;
                var theOtherHediff = HediffMaker.MakeHediff(power.hediff, user);
                theOtherHediff.Severity = user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                          target.GetStatValue(StatDefOf.PsychicSensitivity);
                theOtherHediff.TryGetComp<HediffComp_OtherPawn>().otherPawn = target;
                user.health.AddHediff(theOtherHediff);
            }

            target.health.AddHediff(theHediff);
        }
    }
}