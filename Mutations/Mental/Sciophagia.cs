using System;
using XRL.Rules;
using XRL.UI;
using Qud.API;
using XRL.Language;

namespace XRL.World.Parts.Mutation
{

    [Serializable]
    public class Sciophagia : BaseMutation
    {
        //Properties/Member Variables / Not Static, Exist on the Instance of this Class
        public Guid ActivatedAbilitiesID;
        public int HealthIncrease;
        public int PermCurrentBonus;
        public Sciophagia()
        {
            this.DisplayName = "Sciophagia";
            this.Type = "Mental";

            //Intialization Assignment
            HealthIncrease = 0;
            PermCurrentBonus = 0;
        }



        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "KilledPlayer");
            base.Register(Object);
        }
        public override bool Mutate(GameObject GO, int Level)
        {
            // if (ParentObject != null)
            // { XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("Seekers", 1200, false); }

            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            StatShifter.RemoveStatShifts();
            base.RemoveMyActivatedAbility(ref this.ActivatedAbilitiesID);
            return base.Unmutate(GO);
        }
        public override bool CanLevel()
        {
            return false;
        }
        public override string GetDescription()
        {
            return "Your consciousness predates on other minds, devour the animuses of sentient beings as sustenance for a growing psychic in this grander aether and contend with other mass minds.\n"
                    + "\n{{white|+200 Reputation with}} {{blue|Seekers of the Sightless Way.}}\n\n";
        }
        public override string GetLevelText(int Level)
        {
            return "{{white|Upon defeating an enemy with higher ego than your own, there is a {{light blue|10%}} chance you will encode that creature's psyche onto the holograms of your own, gaining +1 to your ego score permanently.  \n"
            + "\n";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == KilledEvent.ID;
        }

        public override bool HandleEvent(KilledEvent E)
        {
            var DyingEgo = E.Dying.StatMod("Ego");
            var KillerEgo = ParentObject.StatMod("Ego");

            var OwnersLevel = ParentObject.Stat("Level");
            var TargetsLevel = E.Dying.Stat("Level");

            var LevelDifference = OwnersLevel - TargetsLevel;
            var DevourChance = Stat.Random(1, 100) <= 10 + (LevelDifference * 0.10);

            var Killed = E.Dying;


            if (E.Killer == ParentObject && DyingEgo > KillerEgo)
            {
                if (DevourChance)
                {
                    if (Popup.ShowYesNo("&yAt the moment of victory, your swelling ego curves the psychic aether and causes the psyche of " + Killed.ShortDisplayName + "&y, to collide with your own. As the weaker of the two, its binding energy is exceeded and it explodes. Would you like to encode its psionic bits on the holographic boundary of your own psyche? \n\n(+1 Ego permanently)") == DialogResult.Yes)
                    {
                        IComponent<GameObject>.ThePlayer.Statistics["Ego"].BaseValue += 1;
                        Popup.Show("&yYou encode the psyche of " + Killed.ShortDisplayName + " and gain +&C1 &YEgo&y!");
                        JournalAPI.AddAccomplishment("You slew " + Killed.DisplayNameOnly + " and encoded their psyche's psionic bits on the holographic boundary of your own psyche.", "After a climactic battle of wills, =name= slew " + Killed.the + Killed.DisplayNameOnlyDirect + " and absorbed " + Killed.its + " psyche, thickening toward Godhood.", "general", JournalAccomplishment.MuralCategory.Slays, JournalAccomplishment.MuralWeight.High, null, -1L);
                    }
                    else
                    {
                        Popup.Show("&yYou pause as the psyche of " + Killed.ShortDisplayName + " radiates into nothingness.");
                        JournalAPI.AddAccomplishment("You slew " + Killed.DisplayNameOnly + " and watched their psyche radiate into nothingness.", "After a climactic battle of wills, =name= slew " + Killed.the + Killed.DisplayNameOnlyDirect + " and watched " + Killed.its + " their psyche radiate into nothingness.", "general", JournalAccomplishment.MuralCategory.Slays, JournalAccomplishment.MuralWeight.Medium, null, -1L);
                    }
                }
                else
                {
                    JournalAPI.AddAccomplishment("You slew " + Killed.DisplayNameOnly + ".", "After a climactic battle of wills, =name= slew " + Killed.the + Killed.DisplayNameOnlyDirect + ".", "general", JournalAccomplishment.MuralCategory.Slays, JournalAccomplishment.MuralWeight.Medium, null, -1L);
                }
            }

            return base.HandleEvent(E);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "KilledPlayer")
            {
                int ABSORB_CHANCE = 10;
                string value = (ParentObject.pBrain.GetPrimaryFaction() == "Seekers") ? "You were resorbed into the Mass Mind." : ((!ABSORB_CHANCE.in100()) ? ("You were killed by " + ParentObject.DisplayNameOnly + "&Y.") : ("Your psyche exploded, and its psionic bits were encoded on the holographic boundary surrounding the psyche of " + Grammar.MakePossessive(ParentObject.DisplayNameOnly) + "&Y."));
                E.SetParameter("Reason", value);
            }

            return base.FireEvent(E);
        }
    }
}