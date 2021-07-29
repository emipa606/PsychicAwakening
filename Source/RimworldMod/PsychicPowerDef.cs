using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
    public class PsychicPowerDef : Def
    {
        public float brainBurnCost;
        public HediffDef hediff;
        public bool hostile = false;
        public IncidentDef incident;
        public Type powerClass;
        public string powerIcon;
        public PsychicTargetType target;
        public ThoughtDef thought;

        public virtual Texture2D Icon => ContentFinder<Texture2D>.Get(powerIcon);
    }
}