using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld;

internal class IncidentWorker_PsychicSiren : IncidentWorker_Raid
{
    protected override bool CanFireNowSub(IncidentParms parms)
    {
        return true;
    }

    protected override LetterDef GetLetterDef()
    {
        return LetterDefOf.PositiveEvent;
    }

    protected override string GetLetterLabel(IncidentParms parms)
    {
        return parms.raidStrategy.letterLabelFriendly;
    }

    protected override string GetLetterText(IncidentParms parms, List<Pawn> pawns)
    {
        var text = string.Format(parms.raidArrivalMode.textFriendly, parms.faction.def.pawnsPlural,
            parms.faction.Name);
        text += "\n\n";
        text += parms.raidStrategy.arrivalTextFriendly;
        var pawn = pawns.Find(x => x.Faction.leader == x);
        if (pawn == null)
        {
            return text;
        }

        text += "\n\n";
        text += "FriendlyRaidLeaderPresent".Translate(pawn.Faction.def.pawnsPlural, pawn.LabelShort,
            pawn.Named("LEADER"));

        return text;
    }

    protected override string GetRelatedPawnsInfoLetterText(IncidentParms parms)
    {
        return "LetterRelatedPawnsRaidFriendly".Translate(Faction.OfPlayer.def.pawnsPlural,
            parms.faction.def.pawnsPlural);
    }

    protected override void ResolveRaidPoints(IncidentParms parms)
    {
        if (parms.points <= 0f)
        {
            parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);
        }
    }

    public override void ResolveRaidStrategy(IncidentParms parms, PawnGroupKindDef groupKind)
    {
        if (parms.raidStrategy != null)
        {
            return;
        }

        parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
    }

    protected override bool TryResolveRaidFaction(IncidentParms parms)
    {
        var map = (Map)parms.target;
        if (!CandidateFactions(map).Any() || Rand.Chance(0.2f))
        {
            if (Rand.Chance(0.5f))
            {
                if (Find.FactionManager.AllFactions.All(x => x.def.defName != "MechanoidPsychicResponse"))
                {
                    makeNewFaction(FactionDef.Named("MechanoidPsychicResponse"));
                }

                parms.faction =
                    Find.FactionManager.AllFactions.First(x => x.def.defName == "MechanoidPsychicResponse");
            }
            else
            {
                if (Find.FactionManager.AllFactions.All(x => x.def.defName != "InsectPsychicResponse"))
                {
                    makeNewFaction(FactionDef.Named("InsectPsychicResponse"));
                }

                parms.faction =
                    Find.FactionManager.AllFactions.First(x => x.def.defName == "InsectPsychicResponse");
            }
        }
        else
        {
            parms.faction = CandidateFactions(map).RandomElementByWeight(fac => fac.PlayerGoodwill + 120.000008f);
        }

        return true;
    }

    protected override bool FactionCanBeGroupSource(Faction f, Map map, bool desperate = false)
    {
        var badGuys = map.attackTargetsCache.TargetsHostileToColony;
        var badGuyFaction = badGuys.First(x => x.Thing.Faction != null).Thing.Faction;
        return f.HostileTo(badGuyFaction) && base.FactionCanBeGroupSource(f, map, desperate) &&
               f.PlayerRelationKind == FactionRelationKind.Ally;
    }

    private void makeNewFaction(FactionDef factionDef)
    {
        var newFac = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(factionDef));
        foreach (var f in Find.FactionManager.AllFactions)
        {
            newFac.TryAffectGoodwillWith(f, -200, false, false);
        }

        newFac.TryAffectGoodwillWith(Faction.OfPlayer, 100, false, false);
        Find.FactionManager.Add(newFac);
    }
}