using Verse;

namespace RimWorld
{
    internal class PsychicPowerDriver_Affinity : PsychicPowerDriver
    {
        public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
        {
            if (target.RaceProps.Humanlike)
            {
                var theThought = (Thought_Memory)ThoughtMaker.MakeThought(power.thought);
                theThought.age = (int)(theThought.def.DurationTicks *
                                        (1 - (user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                              target.GetStatValue(StatDefOf.PsychicSensitivity))));
                theThought.moodPowerFactor = user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                             target.GetStatValue(StatDefOf.PsychicSensitivity);
                target.needs.mood.thoughts.memories.TryGainMemory(theThought, user);
            }
            else
            {
                var theHediff = HediffMaker.MakeHediff(power.hediff, target);
                theHediff.Severity = user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                     target.GetStatValue(StatDefOf.PsychicSensitivity);
                target.health.AddHediff(theHediff);
            }
        }
    }
}