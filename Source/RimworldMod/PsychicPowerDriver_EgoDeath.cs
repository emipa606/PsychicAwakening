using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
    internal class PsychicPowerDriver_EgoDeath : PsychicPowerDriver
    {
        private readonly List<TraitDef> physicalTraits = new List<TraitDef>
            {TraitDefOf.Beauty, TraitDefOf.Tough, TraitDef.Named("Immunity")};

        public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
        {
            if (target.needs.mood != null)
            {
                var theThought = (Thought_Memory)ThoughtMaker.MakeThought(power.thought);
                theThought.age = (int)(theThought.def.DurationTicks *
                                        (1 - (user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                              target.GetStatValue(StatDefOf.PsychicSensitivity))));
                theThought.moodPowerFactor = user.GetStatValue(StatDefOf.PsychicSensitivity) *
                                             target.GetStatValue(StatDefOf.PsychicSensitivity);
                target.needs.mood.thoughts.memories.TryGainMemory(theThought, user);
            }

            if (target.RaceProps.Humanlike)
            {
                var toReplace = new List<Trait>();
                foreach (var t in target.story.traits.allTraits)
                {
                    if (!physicalTraits.Contains(t.def))
                    {
                        toReplace.Add(t);
                    }
                }

                foreach (var t in toReplace)
                {
                    target.story.traits.allTraits.Remove(t);
                }

                var newTraitNum = Math.Min(Rand.RangeInclusive(1, 3), 3 - target.story.traits.allTraits.Count);
                for (var i = 0; i < newTraitNum; i++)
                {
                    target.story.traits.GainTrait(RandomMentalTrait(target));
                }
            }
            else
            {
                if (Rand.Chance(0.6f))
                {
                    target.health.AddHediff(HediffDefOf.MissingBodyPart,
                        target.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).First());
                }
                else
                {
                    user.relations.AddDirectRelation(PawnRelationDefOf.Bond, target);
                    InteractionWorker_RecruitAttempt.DoRecruit(user, target, false);
                    if (user.Faction == Faction.OfPlayer || target.Faction == Faction.OfPlayer)
                    {
                        TaleRecorder.RecordTale(TaleDefOf.BondedWithAnimal, user, target);
                    }

                    var notHuman = false;
                    string value = null;
                    if (target.Name == null || target.Name.Numerical)
                    {
                        notHuman = true;
                        value = target.Name != null ? target.Name.ToStringFull : target.LabelIndefinite();
                        target.Name = PawnBioAndNameGenerator.GeneratePawnName(target);
                    }

                    if (!PawnUtility.ShouldSendNotificationAbout(user) &&
                        !PawnUtility.ShouldSendNotificationAbout(target))
                    {
                        return;
                    }

                    string text;
                    if (notHuman)
                    {
                        text = "MessageNewBondRelationNewName"
                            .Translate(user.LabelShort, value, target.Name.ToStringFull, user.Named("HUMAN"),
                                target.Named("ANIMAL")).AdjustedFor(target).CapitalizeFirst();
                    }
                    else
                    {
                        text = "MessageNewBondRelation".Translate(user.LabelShort, target.LabelShort,
                            user.Named("HUMAN"), target.Named("ANIMAL")).CapitalizeFirst();
                    }

                    Messages.Message(text, user, MessageTypeDefOf.PositiveEvent);
                }
            }
        }

        private Trait RandomMentalTrait(Pawn target)
        {
            var def = DefDatabase<TraitDef>.GetRandom();
            if (target.story.traits.HasTrait(def) || physicalTraits.Contains(def))
            {
                return RandomMentalTrait(target);
            }

            if (def.degreeDatas != null)
            {
                return new Trait(def, def.degreeDatas.RandomElement().degree, true);
            }

            return new Trait(def, forced: true);
        }
    }
}