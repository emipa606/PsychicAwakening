using System.Linq;
using Verse;

namespace RimWorld;

internal class CompUseEffect_BecomePsychic : CompUseEffect
{
    public override void DoEffect(Pawn user)
    {
        user.health.AddHediff(HediffDef.Named("PsychicConversion"),
            user.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).First());
        base.DoEffect(user);
    }

    public override AcceptanceReport CanBeUsedBy(Pawn p)
    {
        if (p.health.hediffSet.HasHediff(HediffDef.Named("PsychicConversion")) ||
            p.health.hediffSet.HasHediff(HediffDef.Named("PsychicAwakened")))
        {
            return "AlreadyPsychic".Translate(p);
        }

        return p.GetStatValue(StatDefOf.PsychicSensitivity) == 0 ? "PsychicDeafFail".Translate(p) : base.CanBeUsedBy(p);
    }
}