using System.Linq;
using HugsLib;
using Verse;
using Verse.AI;

namespace RimWorld;

public class PsychicMod : ModBase
{
    public override string ModIdentifier => "PsychicAwakening";

    public static bool knowsPower(Pawn p, PsychicPowerDef power)
    {
        var psychic =
            (HediffPsychicAwakened)p.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("PsychicAwakened"));
        return psychic != null && psychic.powersKnown.Contains(power);
    }

    public static Gizmo generateGizmo(HediffPsychicAwakened psychic, PsychicPowerDef power)
    {
        Command giz;
        switch (power.target)
        {
            case PsychicTargetType.Pawn:
            {
                giz = new Command_Target();
                var parms = new TargetingParameters
                {
                    canTargetBuildings = false,
                    canTargetFires = false,
                    canTargetItems = false,
                    canTargetLocations = false,
                    canTargetPawns = true,
                    canTargetSelf = true,
                    validator = delegate(TargetInfo targ)
                    {
                        if (!targ.HasThing)
                        {
                            return false;
                        }

                        return targ.Thing.GetStatValue(StatDefOf.PsychicSensitivity) > 0;
                    }
                };
                ((Command_Target)giz).targetingParams = parms;
                ((Command_Target)giz).action = delegate(LocalTargetInfo target)
                {
                    var theJob = new Job(DefDatabase<JobDef>.GetNamed("UsePsychicPower"), target);
                    psychic.currentPower = power;
                    psychic.pawn.jobs.StartJob(theJob, JobCondition.InterruptForced);
                };
                break;
            }
            case PsychicTargetType.DownedPawn:
                giz = new Command_Target();
                ((Command_Target)giz).targetingParams = TargetingParameters.ForRescue(psychic.pawn);
                ((Command_Target)giz).targetingParams.validator = delegate(TargetInfo targ)
                {
                    if (!targ.HasThing)
                    {
                        return false;
                    }

                    return targ.Thing.GetStatValue(StatDefOf.PsychicSensitivity) > 0;
                };
                ((Command_Target)giz).action = delegate(LocalTargetInfo target)
                {
                    var theJob = new Job(DefDatabase<JobDef>.GetNamed("UsePsychicPower"), target);
                    psychic.currentPower = power;
                    psychic.pawn.jobs.StartJob(theJob, JobCondition.InterruptForced);
                };
                break;
            case PsychicTargetType.PawnHumanlike:
            {
                giz = new Command_Target();
                var parms = new TargetingParameters
                {
                    canTargetBuildings = false,
                    canTargetFires = false,
                    canTargetItems = false,
                    canTargetLocations = false,
                    canTargetPawns = true,
                    canTargetSelf = true,
                    validator = delegate(TargetInfo targ)
                    {
                        if (!targ.HasThing)
                        {
                            return false;
                        }

                        return targ.Thing is Pawn pawn && pawn.RaceProps.Humanlike &&
                               pawn.GetStatValue(StatDefOf.PsychicSensitivity) > 0;
                    }
                };
                ((Command_Target)giz).targetingParams = parms;
                ((Command_Target)giz).action = delegate(LocalTargetInfo target)
                {
                    var theJob = new Job(DefDatabase<JobDef>.GetNamed("UsePsychicPower"), target);
                    psychic.currentPower = power;
                    psychic.pawn.jobs.StartJob(theJob, JobCondition.InterruptForced);
                };
                break;
            }
            case PsychicTargetType.PawnOtherFaction:
            {
                giz = new Command_Target();
                var parms = new TargetingParameters
                {
                    canTargetBuildings = false,
                    canTargetFires = false,
                    canTargetItems = false,
                    canTargetLocations = false,
                    canTargetPawns = true,
                    canTargetSelf = true,
                    validator = delegate(TargetInfo targ)
                    {
                        if (!targ.HasThing)
                        {
                            return false;
                        }

                        return targ.Thing.Faction != psychic.pawn.Faction &&
                               targ.Thing.GetStatValue(StatDefOf.PsychicSensitivity) > 0;
                    }
                };
                ((Command_Target)giz).targetingParams = parms;
                ((Command_Target)giz).action = delegate(LocalTargetInfo target)
                {
                    var theJob = new Job(DefDatabase<JobDef>.GetNamed("UsePsychicPower"), target);
                    psychic.currentPower = power;
                    psychic.pawn.jobs.StartJob(theJob, JobCondition.InterruptForced);
                };
                break;
            }
            //if(power.target == PsychicTargetType.Self)
            default:
                giz = new Command_Action();
                ((Command_Action)giz).action = delegate
                {
                    var theJob = new Job(DefDatabase<JobDef>.GetNamed("UsePsychicPower"), psychic.pawn);
                    psychic.currentPower = power;
                    psychic.pawn.jobs.StartJob(theJob, JobCondition.InterruptForced);
                };
                break;
        }

        giz.icon = power.Icon;
        giz.defaultDesc = power.description + "\n\nBurnout cost: " + (power.brainBurnCost * 100) + "%";
        giz.defaultLabel = power.LabelCap;
        return giz;
    }

    public static void addBrainBurn(Pawn user, PsychicPowerDef power)
    {
        LessonAutoActivator.TeachOpportunity(ConceptDef.Named("PsychicBrainBurn"), OpportunityType.Critical);
        var brain = user.RaceProps.body.GetPartsWithTag(BodyPartTagDefOf.ConsciousnessSource).First();
        var burn = user.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named("PsychicBrainBurn"));
        var recoveryRate = 1f;
        if (ModLister.RoyaltyInstalled)
        {
            recoveryRate = user.GetStatValue(StatDefOf.PsychicEntropyRecoveryRate);
        }

        if (burn != null)
        {
            burn.Severity += power.brainBurnCost * (1.2f - (0.025f * recoveryRate));
        }
        else
        {
            burn = HediffMaker.MakeHediff(HediffDef.Named("PsychicBrainBurn"), user, brain);
            burn.Severity = power.brainBurnCost *
                            (1.2f - (0.025f * recoveryRate));
            user.health.AddHediff(burn);
        }

        if (!(burn.Severity >= 1))
        {
            return;
        }

        GenExplosion.DoExplosion(user.Position, user.Map, 2, DamageDefOf.Flame, user, 10,
            chanceToStartFire: 0.5f);
        if (brain.parent != null)
        {
            brain = brain.parent.parent;
        }

        user.health.AddHediff(HediffDefOf.MissingBodyPart, brain);
        Messages.Message("PsychicHeadASplode".Translate(user.LabelShort), user,
            MessageTypeDefOf.NegativeHealthEvent);
    }
}

//TODO add "cast powers on schedule" like drug policy