using System.Linq;
using Verse;

namespace RimWorld
{
    internal class HediffPsychicConversion : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();
            if (!(Severity >= 1))
            {
                return;
            }

            Messages.Message("PsychicAwakeningComplete".Translate(pawn.LabelShort), pawn,
                MessageTypeDefOf.PositiveEvent);
            pawn.health.AddHediff(HediffDef.Named("PsychicAwakened"),
                pawn.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).First());
            var psychic =
                (HediffPsychicAwakened)pawn.health.hediffSet.GetFirstHediffOfDef(
                    HediffDef.Named("PsychicAwakened"));
            if (psychic != null)
            {
                psychic.powersKnown.Add(DefDatabase<PsychicPowerDef>.GetRandom());
            }

            pawn.health.RemoveHediff(this);
            LessonAutoActivator.TeachOpportunity(ConceptDef.Named("PsychicAwakened"), OpportunityType.Critical);
        }
    }
}