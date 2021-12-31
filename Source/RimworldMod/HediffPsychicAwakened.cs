using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld;

public class HediffPsychicAwakened : HediffWithComps
{
    public PsychicPowerDef currentPower;
    public List<PsychicPowerDef> powersKnown;

    public override void PostMake()
    {
        base.PostMake();
        powersKnown = new List<PsychicPowerDef>();
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Collections.Look(ref powersKnown, "powers");
        Scribe_Defs.Look(ref currentPower, "currentPower");
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        var newList = base.GetGizmos().ToList();
        foreach (var power in powersKnown)
        {
            newList.Add(PsychicMod.generateGizmo(this, power));
        }

        return newList;
    }
}