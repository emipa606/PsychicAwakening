using System.Reflection;
using Verse;

namespace RimWorld;

internal class PsychicPowerDriver_Inspiration : PsychicPowerDriver
{
    public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
    {
        if (!target.mindState.inspirationHandler.Inspired)
        {
            target.mindState.inspirationHandler.TryStartInspiration((InspirationDef)typeof(InspirationHandler)
                .GetMethod("GetRandomAvailableInspirationDef", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(target.mindState.inspirationHandler, new object[0]));
        }
    }
}