using Verse;

namespace RimWorld
{
    internal class PsychicPowerDriver_Shock : PsychicPowerDriver
    {
        public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
        {
            if (Rand.Chance(0.4f))
            {
                if (target.Dead)
                {
                    return;
                }

                var hediff = HediffMaker.MakeHediff(HediffDefOf.PsychicShock, target);
                target.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource)
                    .TryRandomElement(out var part);
                target.health.AddHediff(hediff, part);
                if (!Rand.Chance(0.25f))
                {
                    return;
                }

                var brain = target.health.hediffSet.GetBrain();
                if (brain == null)
                {
                    return;
                }

                var num = Rand.RangeInclusive(1, 2);
                var flame = DamageDefOf.Flame;
                float amount = num;
                target.TakeDamage(new DamageInfo(flame, amount, 1f, -1f, user, brain));
            }
            else
            {
                if (target.needs.mood != null)
                {
                    var theThought = (Thought_Memory) ThoughtMaker.MakeThought(power.thought);
                    theThought.age = (int) (theThought.def.DurationTicks *
                                            (1 - (user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                                  target.GetStatValue(StatDefOf.PsychicSensitivity))));
                    theThought.moodPowerFactor = user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                                 target.GetStatValue(StatDefOf.PsychicSensitivity);
                    target.needs.mood.thoughts.memories.TryGainMemory(theThought, user);
                }

                var theHediff = HediffMaker.MakeHediff(power.hediff, target);
                theHediff.Severity = user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                     target.GetStatValue(StatDefOf.PsychicSensitivity);
                target.health.AddHediff(theHediff);
            }
        }
    }
}