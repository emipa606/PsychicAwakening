using Verse;

namespace RimWorld
{
    internal class PsychicPowerDriver_Drone : PsychicPowerDriver
    {
        public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
        {
            foreach (var p in user.Map.mapPawns.AllPawnsSpawned)
            {
                if (p.Faction != null && (p.Faction == user.Faction || !p.Faction.HostileTo(user.Faction)))
                {
                    continue;
                }

                if (target.needs.mood == null)
                {
                    continue;
                }

                var theThought = (Thought_Memory) ThoughtMaker.MakeThought(power.thought);
                theThought.age = (int) (theThought.def.DurationTicks *
                                        (1 - (user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                              p.GetStatValue(StatDefOf.PsychicSensitivity))));
                theThought.moodPowerFactor = user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                             p.GetStatValue(StatDefOf.PsychicSensitivity);
                p.needs.mood.thoughts.memories.TryGainMemory(theThought, user);
            }
        }
    }
}