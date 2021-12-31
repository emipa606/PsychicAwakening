using Verse;

namespace RimWorld;

internal class PsychicPowerDriver_Soothe : PsychicPowerDriver
{
    public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
    {
        if (target.InMentalState)
        {
            var baseChance = 0.25f;
            if (target.MentalStateDef.IsExtreme)
            {
                baseChance = 0.125f;
            }

            baseChance = baseChance * (user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                       target.GetStatValue(StatDefOf.PsychicSensitivity));
            if (Rand.Chance(baseChance))
            {
                target.ClearMind();
            }
        }

        if (target.needs.mood == null)
        {
            return;
        }

        var theThought = (Thought_Memory)ThoughtMaker.MakeThought(power.thought);
        theThought.age = (int)(theThought.def.DurationTicks *
                               (1 - (user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                     target.GetStatValue(StatDefOf.PsychicSensitivity))));
        theThought.moodPowerFactor = user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                     target.GetStatValue(StatDefOf.PsychicSensitivity);
        target.needs.mood.thoughts.memories.TryGainMemory(theThought, user);
    }
}