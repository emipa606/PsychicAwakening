using Verse;

namespace RimWorld;

public class CompPsychicEngram : CompUsable
{
    public PsychicPowerDef power;

    protected override string FloatMenuOptionLabel(Pawn pawn)
    {
        return string.Format(Props.useLabel, power.label);
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Defs.Look(ref power, "power");
    }

    public override void Initialize(CompProperties props)
    {
        base.Initialize(props);
        power = DefDatabase<PsychicPowerDef>.GetRandom();
    }

    public override string TransformLabel(string label)
    {
        return power.LabelCap + " " + label;
    }

    public override bool AllowStackWith(Thing other)
    {
        if (!base.AllowStackWith(other))
        {
            return false;
        }

        var compPsychicEngram = other.TryGetComp<CompPsychicEngram>();
        return compPsychicEngram != null && compPsychicEngram.power == power;
    }

    public override void PostSplitOff(Thing piece)
    {
        base.PostSplitOff(piece);
        var compPsychicEngram = piece.TryGetComp<CompPsychicEngram>();
        if (compPsychicEngram != null)
        {
            compPsychicEngram.power = power;
        }
    }

    public override string GetDescriptionPart()
    {
        return power.LabelCap + ": " + power.description;
    }
}