using Verse;

namespace RimWorld;

public abstract class PsychicPowerDriver
{
    public abstract void UsePower(PsychicPowerDef power, Pawn user, Pawn target);
}