using System;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class LustrousFeathers : BaseMutation
    {
        public LustrousFeathers()
        {
            this.DisplayName = "Lustrously Feathered";
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
            return "You are decorated in lustrous feathers.";
        }

        public override string GetLevelText(int Level)
        {
            return "{{cyan|+1}} DV\n"
            + "{{cyan|+5}} Cold Resistance\n"
            + "{{cyan|+100}} reputation with &CBirds&y";
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
            if (GO.Statistics.ContainsKey("DV"))
            {
                StatShifter.SetStatShift("DV", 1);
            }
            if (GO.Statistics.ContainsKey("ColdResistance"))
            {
                GO.Statistics["ColdResistance"].BaseValue += 5;
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