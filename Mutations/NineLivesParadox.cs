using System;
using XRL.UI;
using XRL.Rules;
using XRL.World.Effects;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class NineLivesParadox : BaseMutation
    {
        public Cell LastCat;
        public string LastCatID;
        // this is the constructor
        public NineLivesParadox()
        {
            this.DisplayName = "Nine-Lives Paradox";
            this.MaxLevel = 9;
        }

        public override bool CanLevel()
        {
            return true;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("Cats", (NewLevel - LastLevel) * 50, true);
            return base.ChangeLevel(NewLevel);
        }

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent(this, "BeforeDie");
        }

        public override string GetDescription()
        {
            return "Petting a feline creature leaves an imprint on the time-flux ley-lines--creating a paradox.\n"
            + "\n"
            + "{{cyan|+400}} Reputation with {{cyan|cats}}.\n"
            + "\n"
            + "Whenever you would die, you are teleported to the last cats you've pet.\n"
            + "Dying removes both a level from the mutation and {{red|-50}} reputation with cats.\n";
        }

        public override string GetLevelText(int Level)
        {
            // the 'return' gives a description "+" being that is adds the (object) which is the mutation itself, plus the "1" integer Math.Ceiling((Decimal) (Level / 2))), in plain english its + whatever it is divided by 2 and then rounded up and then added to the av/n value descripter. Math.Ceiling((Decimal) is representitive of rounding up. \n denounces a new line in the description.

            return "Increase reputation with cats.\n";
        }

        public void CatPetted(GameObject pettedCat)
        {
            Cell LastPettedFeline = pettedCat.pPhysics.CurrentCell;
            this.LastCat = LastPettedFeline;
            this.LastCatID = pettedCat.id;
            Popup.Show("{{green|You feel a comforting sensation as you pet this feline.}}");
            if (!pettedCat.HasProperName)
            {
                string newName = Popup.AskString("What would you like to name this cat?", "", 99);
                if (!String.IsNullOrEmpty(newName))
                {
                    pettedCat.DisplayName = newName;
                    pettedCat.SetIntProperty("ProperNoun", 1);
                }
            }
            this.DisplayName = "Nine Lives (" + pettedCat.DisplayNameOnly + ")";

            //AddPlayerMessage("This cat, is yours.");
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforeDie")
            {
                if (LastCat == null)
                {
                    return true;
                }
                ParentObject.DirectMoveTo(LastCat, 0, true);
                GameObject Cat = GameObject.findById(LastCatID);
                if (Cat != null)
                {
                    ParentObject.DirectMoveTo(Cat.pPhysics.CurrentCell.GetFirstEmptyAdjacentCell(), 0, true);
                    Popup.Show("Right before your life begins to fade, you suddenly feel spirited away and find yourself before a feline friend ... at a cost.");
                }
                else
                {
                    LastCat = null;
                    Popup.Show("As you feel your life dwindle, your soul drifts towards where its astral tether once existed. But the feline is gone, and now, so are you.");
                    return true;
                }
                ParentObject.SpatialDistortionBlip();
                if (ParentObject.HasStat("Hitpoints"))
                {
                    ParentObject.Statistics["Hitpoints"].Penalty = 0;
                    //AddPlayerMessage("You died.");
                }
                if (Level <= 1)
                {
                    if (Cat != null)
                    {
                        Cat.pBrain.GetAngryAt(ParentObject, -400);
                    }
                    ParentObject.GetPart<Mutations>().RemoveMutation(this);
                    Popup.Show("{{red|Your strange connection with cats has been severed.}}");
                }
                else
                {
                    Level = Level - 1;
                }

                return false;
            }
            return base.FireEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
    }
}