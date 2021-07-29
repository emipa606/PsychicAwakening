using System.Linq;
using Verse;

namespace RimWorld
{
    internal class PsychicPowerDriver_Assimilation : PsychicPowerDriver
    {
        public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
        {
            if (target.skills != null && user.skills != null)
            {
                foreach (var skill in target.skills.skills)
                {
                    if (skill.Level > user.skills.GetSkill(skill.def).Level)
                    {
                        user.skills.GetSkill(skill.def).Level += 1;
                    }
                }
            }

            if (Rand.Chance(0.4f))
            {
                target.health.AddHediff(HediffDefOf.MissingBodyPart,
                    target.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).First());
            }
            else
            {
                var brain = target.health.hediffSet.GetBrain();
                if (brain == null)
                {
                    return;
                }

                var num = Rand.RangeInclusive(1, 5);
                var flame = DamageDefOf.Flame;
                float amount = num;
                target.TakeDamage(new DamageInfo(flame, amount, 0f, -1f, user, brain));
            }
        }
    }
}