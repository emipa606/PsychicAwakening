using Verse;

namespace RimWorld;

public class CompUseEffect_LearnPsychicEngram : CompUseEffect
{
    private PsychicPowerDef Power => parent.GetComp<CompPsychicEngram>().power;

    public override void DoEffect(Pawn user)
    {
        base.DoEffect(user);
        var power = Power;
        ((HediffPsychicAwakened)user.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("PsychicAwakened")))
            .powersKnown.Add(power);
        if (PawnUtility.ShouldSendNotificationAbout(user))
        {
            Messages.Message("PsychicEngramUsed".Translate(user.LabelShort, power.label), user,
                MessageTypeDefOf.PositiveEvent);
        }
    }

    public override AcceptanceReport CanBeUsedBy(Pawn p)
    {
        var psychic =
            (HediffPsychicAwakened)p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("PsychicAwakened"));
        if (psychic == null)
        {
            return "MustBePsychic".Translate(p.LabelShort);
        }

        return !psychic.powersKnown.Contains(Power)
            ? base.CanBeUsedBy(p)
            : "AlreadyKnowsPower".Translate(p.LabelShort, Power.LabelCap);
    }
}