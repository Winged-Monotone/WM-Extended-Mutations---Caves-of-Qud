using System;
using XRL.World.Effects;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Volvate : BaseMutation
    {
        public Guid ActivatedAbilityID;

        public Volvate()
        {
            this.DisplayName = "Volvate";
        }

        public override bool CanLevel()
        {
            return true;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            {
                Object.RegisterPartEvent(this, "BeginTakeAction");
                Object.RegisterPartEvent(this, "EnteredCell");
                Object.RegisterPartEvent(this, "wmCommandVolvation");
                base.Register(Object);
            }
            base.Register(Object);
        }

        public override string GetDescription()
        {
            return "You possesess the ability to roll your body into a tightly packed ball, exposing only the hardest portions of integuments, making you a smaller target and allowing you roll away from your enemies; all at the cost of some sight.";
        }

        public override string GetLevelText(int Level)
        {
            if (Level == base.Level)
            {
                return "Roll into a ball and gain a bonus to movement speed, your AV and your DV, however you sightlines are reduced to half, you can not fire projectiles, and your attacks are converted to a bashing tackle.";
            }
            else
            {
                return "Movement Speed Bonus: {{cyan|" + Level + "0" + "}}\n"
                    + "AV Bonus: {{cyan|" + Level / 2 + "}}\n"
                    + "DV Bonus: {{cyan|" + Level + "}}\n";
            }
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
            ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
            ActivatedAbilityID = activatedAbilities.AddAbility(Name: "Volvate", Command: "wmCommandVolvation", Class: "Physical Mutation", Description: "Volvate.", Icon: "O");

            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {

            return base.Unmutate(GO);
        }
    }
}