using Verse;

namespace RimWorld
{
    internal class HediffComp_OtherPawn : HediffComp
    {
        public Pawn otherPawn;

        public override void CompExposeData()
        {
            Scribe_References.Look(ref otherPawn, "otherPawn");
        }
    }
}