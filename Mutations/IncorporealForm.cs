using System;
using XRL.World.Effects;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class IncorporealForm : BaseMutation
    {
        public IncorporealForm()
        {
            this.DisplayName = "Incorporeal Form";
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override void Register(GameObject Object)
        {
            {
                Object.RegisterPartEvent(this, "BeginTakeAction");
                Object.RegisterPartEvent(this, "EnteredCell");
                base.Register(Object);
            }
            base.Register(Object);
        }

        public override string GetDescription()
        {
            return "Your physical form was banished to the aether, forever.";
        }

        public override string GetLevelText(int Level)
        {
            return "You are permanently in the phase.";
        }

        public override bool FireEvent(Event E)
        {
            if ((E.ID == "BeginTakeAction" || E.ID == "EnteredCell") && !this.ParentObject.HasEffect("Phased") && !this.ParentObject.HasEffect("RealityStabilized"))
            {
                this.ParentObject.ApplyEffect(new Phased(9999), null);
            }
            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            Phased Phased = ParentObject.GetEffect("Phased") as Phased;
            if (ParentObject.HasEffect("Phased"))
            {
                ParentObject.RemoveEffect(Phased);
            }
            return base.Unmutate(GO);
        }
    }
}