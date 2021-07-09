using System;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class RoughScales : BaseMutation
    {
        public RoughScales()
        {
            this.DisplayName = "Rough Scales";
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override void Register(GameObject Object)
        {
            base.Register(Object);
        }

        public override string GetDescription()
        {
            return "You are covered in rough articulated scales.";
        }

        public override string GetLevelText(int Level)
        {
            return "{{cyan|+1}} AV\n"
            + "{{cyan|+5}} Acid Resistance\n"
            + "{{cyan|+100}} reputation with &Cunshelled reptiles";
        }

        public override bool FireEvent(Event E)
        {
            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            if (GO.Statistics.ContainsKey("AV"))
            {
                StatShifter.SetStatShift("AV", 1);
            }
            if (GO.Statistics.ContainsKey("AcidResistance"))
            {
                GO.Statistics["AcidResistance"].BaseValue += 5;
            }

            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            StatShifter.RemoveStatShifts();
            return base.Unmutate(GO);
        }
    }
}