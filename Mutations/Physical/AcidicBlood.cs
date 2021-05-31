using System;

namespace XRL.World.Parts.Mutation
{
    public class AcidicBlood : BaseMutation
    {
        public AcidicBlood()
        {
            this.DisplayName = "Acidic Blood";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override string GetDescription()
        {
            return "Your blood is acidic.";
        }

        public override string GetLevelText(int Level)
        {
            return "You release {{green|acidic}} blood upon being wounded by an attack that would make you bleed, you're also immune to {{green|acid}}.";
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            ParentObject.SetStringProperty("BleedLiquid", "acid-1000");
            ParentObject.AddPart<AcidImmunity>(true);
            Unmutate(GO);
            return base.Mutate(GO, Level);
        }
    }
}