using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace RimWorld;

internal class JobDriver_PsychicPower : JobDriver
{
    private float workDone;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        if (TargetA != LocalTargetInfo.Invalid)
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
        }

        var usePower = Toils_General.Wait(100, TargetIndex.A);
        usePower.WithProgressBar(TargetIndex.A, () => workDone / 100);
        usePower.initAction = delegate
        {
            pawn.pather.StopDead();
            pawn.rotationTracker.FaceTarget(TargetA);
            workDone = 0;
        };
        usePower.tickAction = delegate { workDone++; };
        usePower.AddFinishAction(delegate
        {
            if (!(workDone >= 95) || pawn.health.State != PawnHealthState.Mobile || !TargetA.HasThing ||
                TargetA.Thing.DestroyedOrNull() || ((Pawn)TargetA.Thing).Dead)
            {
                return;
            }

            SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(pawn.Map);
            FleckMaker.ThrowAirPuffUp(TargetA.CenterVector3, Map);
            var psychic =
                (HediffPsychicAwakened)pawn?.health?.hediffSet?.GetFirstHediffOfDef(
                    HediffDef.Named("PsychicAwakened"));
            psychic?.currentPower?.powerClass?.GetMethod("UsePower", BindingFlags.Instance | BindingFlags.Public)
                ?.Invoke(Activator.CreateInstance(psychic.currentPower.powerClass),
                [
                    psychic.currentPower, pawn,
                    TargetA != LocalTargetInfo.Invalid ? (Pawn)TargetA.Thing : null
                ]);
            if (psychic?.currentPower?.hostile == true && psychic.pawn.Faction == Faction.OfPlayer &&
                TargetA != LocalTargetInfo.Invalid && TargetA.Thing?.Faction != Faction.OfPlayer)
            {
                if (TargetA.Thing is { Faction: not null })
                {
                    TargetA.Thing.Faction.TryAffectGoodwillWith(Faction.OfPlayer, -100);
                    TargetA.Thing.Faction.Notify_GoodwillSituationsChanged(Faction.OfPlayer, true,
                        "Used a hostile psychic power", TargetThingA);
                }
            }

            PsychicMod.addBrainBurn(pawn, psychic?.currentPower);
        });
        yield return usePower;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref workDone, "WorkDone");
    }
}