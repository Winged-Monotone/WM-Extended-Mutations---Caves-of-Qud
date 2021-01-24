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

        // public int DevourerDamage(int MutationLevel, int CasterEgo, GameObject Target)
        // {
        //     int LevelMultiplier = Stat.Random(1, Level);
        //     int DamageCalc = (MutationLevel + CasterEgo) + ParentObject.Statistics["Level"].Value + 1;
        //     int LevelMultVs = Math.Min(0, Stats.GetCombatMA(Target));
        //     return DamageCalc * (LevelMultiplier - LevelMultVs);
        // }
        // public void DevourPsychicPulse(Cell TargetCell)
        // {
        //     for (int i = 0; i < 1; i++)
        //     {
        //         for (int j = 0; j < 3; j++)
        //         {
        //             TargetCell.ParticleText("&R" + (char)(219 + Stat.Random(0, 10)), 4.9f, 5);
        //         }
        //         for (int k = 0; k < 2; k++)
        //         {
        //             TargetCell.ParticleText("&M" + (char)(219 + Stat.Random(0, 10)), 4.9f, 5);
        //         }
        //         for (int l = 0; l < 4; l++)
        //         {
        //             TargetCell.ParticleText("&W" + (char)(219 + Stat.Random(0, 10)), 4.9f, 5);
        //         }
        //     }
        // }
        // public int SciophagiaSaveDC(int MutationLevel, int TargetHealthPercent, GameObject Target)
        // {
        //     int EgoMod = ParentObject.Statistics["Ego"].Modifier;
        //     int UserLevel = ParentObject.Statistics["Level"].Value / 2;
        //     int HealthPenalty = (100 - TargetHealthPercent) / 10;
        //     int DefenderMath = Target.Statistics["Level"].Value + Stats.GetCombatMA(Target);
        //     return 5 + EgoMod + UserLevel + MutationLevel + HealthPenalty - DefenderMath;
        // }
        // public int HealthBoostIncrease(int CreaturesEgo)
        // {
        //     return CreaturesEgo;
        // }
        

        // public void DevourStatGains(GameObject Target)
        // {
        //     bool AborptionChance = Stat.Random(0, 100) <= 10;

        //     if (AborptionChance)
        //     {
        //         if (Popup.ShowYesNo("You feel the remnants of tender light pulsating within your new husk, would you like to imprint these codings upon your own animus?", false, DialogResult.Yes) == DialogResult.Yes)
        //         {
        //             StatShifter.SetStatShift(ParentObject, "Ego", 1, true);
        //         }
        //         else
        //         {
        //             Popup.Show("You cast the remnants away.");
        //         }
        //     }
        // }

        // public void DevourMinds()
        // {
        //     TextConsole _TextConsole = UI.Look._TextConsole;
        //     ScreenBuffer Buffer = TextConsole.ScrapBuffer;
        //     Core.XRLCore.Core.RenderMapToBuffer(Buffer);
        //     Cell cell = PickDirection();
        //     if (cell == null)
        //     {
        //         return;
        //     }

        //     // Be sure to check if these statements return null, I.E "No Game oBject in that cell," or "cancel picking direction." 

        //     GameObject Target = cell.FindObject(o => o.HasPart("Brain"));
        //     if (IsPlayer() && Target == null)
        //     {
        //         Popup.Show("Lacks a proper mind to devour.");
        //         return;
        //     }
        //     if (IsPlayer() && Target == ParentObject)
        //     {
        //         Popup.Show("You can't devour your own mind.");
        //         return;
        //     }
        //     int TargetHealthPercent = 100;
        //     if (Target.baseHitpoints != 0)
        //     {
        //         TargetHealthPercent = Target.hitpoints * 100 / Target.baseHitpoints;
        //     }
        //     if (!Target.MakeSave("Willpower", SciophagiaSaveDC(this.Level, TargetHealthPercent, Target), ParentObject, "Ego"))
        //     {
        //         int CreaturesLevelVS = (int)Math.Min(1, (Stats.GetCombatMA(Target)));
        //         if ((Target.Stat("Level") + CreaturesLevelVS) < ParentObject.Stat("Level") && Target.Die(ParentObject, "", "Your mind was devoured and assimilated.", false))
        //         {
        //             //AddPlayerMessage("They took InstaDeath [" + (Target.Statistics["Level"].Value + CreaturesLevelVS) + "]");
        //             DevourStatGains(Target);
        //         }

        //         else
        //         {
        //             Target.TakeDamage(DevourerDamage(Level, ParentObject.Statistics["Ego"].Modifier, Target), "Mental", DeathReason: "Mind was ripped from vessel.", Owner: ParentObject, Attacker: ParentObject, Message: "from %t mind eater attack!");
        //             if (Target.Statistics["Hitpoints"].Value <= 0)
        //             {
        //                 //AddPlayerMessage("They took Damage");

        //                 DevourStatGains(Target);
        //             }
        //         }
        //         this.DevourPsychicPulse(cell);
        //     }
        //     else
        //     {
        //         XDidYToZ(Target, "resist", "an attempt from", ParentObject, "mind devouring powers", "!");
        //     }

        //     CooldownMyActivatedAbility(ActivatedAbilitiesID, 25);
        // }



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