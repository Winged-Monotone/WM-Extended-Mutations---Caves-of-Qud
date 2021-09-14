using System;
using XRL.Rules;
using XRL.UI;
using Qud.API;
using XRL.Language;
using System.Timers;

namespace XRL.World.Parts.Mutation
{

    [Serializable]
    public class Sciophagia : BaseMutation
    {
        public Guid ActivatedAbilitiesID;
        public Sciophagia()
        {
            this.DisplayName = "Sciophagia";
            this.Type = "Mental";
        }
        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "BeforeDie");
            Object.RegisterPartEvent(this, "KilledPlayer");

            base.Register(Object);
        }
        public override bool Mutate(GameObject GO, int Level)
        {
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
            return true;
        }
        public override string GetDescription()
        {
            return "Your consciousness predates on other minds, devour the animuses of sentient beings as sustenance for a growing psychic in this grander aether and contend with other mass minds.\n"
                    + "\n{{cyan|+200 Reputation with}} {{cyan|Seekers of the Sightless Way.}}";
        }
        public override string GetLevelText(int Level)
        {
            if (Level == base.Level)
                return "{{white|Upon defeating an enemy with higher ego than your own, there is a {{cyan|" + (9 + Level) + "%}} chance you will encode that creature's psyche onto the holograms of your own, gaining {{cyan|+1}} to your ego score permanently.\n"
                + "\n";
            else
                return "Chance of Absorption: " + "{{cyan|" + (9 + Level) + "%}}";
        }

        // public override bool AllowStaticRegistration()
        // {
        //     return true;
        // }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == KilledEvent.ID
            || ID == KilledPlayerEvent.ID;
        }

        public bool HandleDeathEvent(IDeathEvent E)
        {
            // AddPlayerMessage("Starting Scio Grab.");

            var DyingEgo = E.Dying.Statistics["Ego"].Modifier;
            var KillerEgo = ParentObject.Statistics["Ego"].Modifier;

            // AddPlayerMessage("Got Ego Parameters.");

            var OwnersLevel = ParentObject.Statistics["Level"].Value;
            var TargetsLevel = E.Dying.Statistics["Level"].Value;

            // AddPlayerMessage("Got Level Parameters.");

            var LevelDifference = TargetsLevel - OwnersLevel;
            var DevourNumerator = Stat.Random(1, 100);
            var DevourChance = DevourNumerator <= 9 + Level + (LevelDifference * 0.50);

            // AddPlayerMessage("Setting Varitability.");

            var Killed = E.Dying;

            // AddPlayerMessage("Got target");
            // AddPlayerMessage("Devour Roll: " + DevourNumerator);

            if (E.Killer == ParentObject && E.Killer.IsPlayer() && DyingEgo > KillerEgo)
            {
                // AddPlayerMessage("Setting Kill Bool.");
                if (DevourChance)
                {

                    // AddPlayerMessage("Devour Chance Procced");

                    if (Popup.ShowYesNo("&yAt the moment of victory, your swelling ego curves the psychic aether and causes the psyche of " + Killed.the + Killed.ShortDisplayName + "&y, to collide with your own. As the weaker of the two, its binding energy is exceeded and it explodes. Would you like to encode its psionic bits on the holographic boundary of your own psyche? \n\n(+1 Ego permanently)") == DialogResult.Yes)
                    {
                        // AddPlayerMessage("You Chose Yes");
                        IComponent<GameObject>.ThePlayer.Statistics["Ego"].BaseValue += 1;
                        Popup.Show("&yYou encode the psyche of " + Killed.the + Killed.DisplayNameOnlyDirect + " and gain +&C1 &YEgo&y!");
                        JournalAPI.AddAccomplishment("You slew " + Killed.the + Killed.DisplayNameOnlyDirect + " and encoded their psyche's psionic bits on the holographic boundary of your own psyche.", "After a climactic battle of wills, =name= slew " + Killed.the + Killed.DisplayNameOnlyDirect + " and absorbed " + Killed.its + " psyche, thickening toward Godhood.", "general", JournalAccomplishment.MuralCategory.Slays, JournalAccomplishment.MuralWeight.High, null, -1L);
                    }
                    else
                    {
                        Popup.Show("&yYou pause as the psyche of " + Killed.the + Killed.DisplayNameOnlyDirect + " radiates into nothingness.");
                        JournalAPI.AddAccomplishment("You slew " + Killed.the + Killed.DisplayNameOnlyDirect + " and watched their psyche radiate into nothingness.", "After a climactic battle of wills, =name= slew " + Killed.the + Killed.DisplayNameOnlyDirect + " and watched " + Killed.its + " their psyche radiate into nothingness.", "general", JournalAccomplishment.MuralCategory.Slays, JournalAccomplishment.MuralWeight.Medium, null, -1L);
                    }
                }
                else
                {
                    JournalAPI.AddAccomplishment("You slew " + Killed.DisplayNameOnly + ".", "After a climactic battle of wills, =name= slew " + Killed.the + Killed.DisplayNameOnlyDirect + ".", "general", JournalAccomplishment.MuralCategory.Slays, JournalAccomplishment.MuralWeight.Medium, null, -1L);
                }
            }
            else if (E.Dying == ParentObject && ParentObject.IsPlayer())
            {

                if (DevourChance)
                {
                    string value = (E.Killer.pBrain.GetPrimaryFaction() == "Seekers") ? "You were resorbed into the Mass Mind." : ((!DevourChance) ? ("You were killed by " + ParentObject.DisplayNameOnly + "&Y.") : ("Your psyche exploded, and its psionic bits were encoded on the holographic boundary surrounding the psyche of " + Grammar.MakePossessive(ParentObject.DisplayNameOnly) + "&Y."));
                    E.Reason = value;
                }
            }
            else if (E.Killer == ParentObject && !E.Killer.IsPlayer() && DyingEgo > KillerEgo)
            {
                if (DevourChance)
                {
                    ParentObject.Statistics["Ego"].BaseValue += 1;
                    ParentObject.ParticleBlip("{{violet|*}}", 1, true);
                }
            }
            return true;
        }
        public override bool HandleEvent(KilledEvent E)
        {
            return HandleDeathEvent(E);
        }
        public override bool HandleEvent(KilledPlayerEvent E)
        {
            return HandleDeathEvent(E);
        }
        public override bool FireEvent(Event E)
        {
            return base.FireEvent(E);
        }
    }
}