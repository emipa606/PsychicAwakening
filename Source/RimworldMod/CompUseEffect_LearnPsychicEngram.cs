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

    public override bool CanBeUsedBy(Pawn p, out string failReason)
    {
        var psychic =
            (HediffPsychicAwakened)p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("PsychicAwakened"));
        if (psychic == null)
        {
            failReason = "MustBePsychic".Translate(p.LabelShort);
            return false;
        }

        if (!psychic.powersKnown.Contains(Power))
        {
            return base.CanBeUsedBy(p, out failReason);
        }

        failReason = "AlreadyKnowsPower".Translate(p.LabelShort, Power.LabelCap);
        return false;
    }
}