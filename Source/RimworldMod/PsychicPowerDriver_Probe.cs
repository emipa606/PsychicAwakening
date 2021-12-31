using System.Collections.Generic;
using Verse;

namespace RimWorld;

internal class PsychicPowerDriver_Probe : PsychicPowerDriver
{
    public override void UsePower(PsychicPowerDef power, Pawn user, Pawn target)
    {
        var unused = new IncidentParms { faction = target.Faction, forced = true, target = Find.World };

        var possibleQuests = new List<QuestScriptDef>();
        foreach (var def in DefDatabase<QuestScriptDef>.AllDefs)
        {
            if (def.IsRootRandomSelected)
            {
                possibleQuests.Add(def);
            }
        }

        QuestUtility.GenerateQuestAndMakeAvailable(possibleQuests.RandomElement(),
            StorytellerUtility.DefaultThreatPointsNow(user.Map));
    }
}