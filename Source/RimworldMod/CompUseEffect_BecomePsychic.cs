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

    public override bool CanBeUsedBy(Pawn p, out string failReason)
    {
        if (p.health.hediffSet.HasHediff(HediffDef.Named("PsychicConversion")) ||
            p.health.hediffSet.HasHediff(HediffDef.Named("PsychicAwakened")))
        {
            failReason = "AlreadyPsychic".Translate(p);
            return false;
        }

        if (p.GetStatValue(StatDefOf.PsychicSensitivity) == 0)
        {
            failReason = "PsychicDeafFail".Translate(p);
        }

        return base.CanBeUsedBy(p, out failReason);
    }
}