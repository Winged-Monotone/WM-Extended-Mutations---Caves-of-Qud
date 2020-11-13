using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConsoleLib.Console;
using Genkit;
using XRL.Rules;
using XRL.UI;

namespace XRL.World.Parts.Mutation
{

    [Serializable]
    public class MindEater : BaseMutation
    {
        //Properties/Member Variables / Not Static, Exist on the Instance of this Class
        public Guid ActivatedAbilitiesID;
        public int HealthIncrease;
        public int PermCurrentBonus;
        public MindEater()
        {
            this.DisplayName = "Mind Eater";
            this.Type = "Mental";

            //Intialization Assignment
            HealthIncrease = 0;
            PermCurrentBonus = 0;
        }
        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CommandDevourMind");
            Object.RegisterPartEvent(this, "AIGetOffensiveMutationList");
            base.Register(Object);
        }
        public override bool Mutate(GameObject GO, int Level)
        {
            this.ActivatedAbilitiesID = base.AddMyActivatedAbility("Devour Mind", "CommandDevourMind", "Mental Mutation", "Devour the mind of an enemy.", "U", null, false, false, false, false, false, false, false, 40, null);
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
        public override int GetMaxLevel()
        {
            return 9999;
        }
        public override string GetDescription()
        {
            return "Your consciousness predates on other minds, devour the ego of sentient beings as sustenance for a growing psychic in this grander aether and contend with other mass minds.\n"
                    + "\n{{white|-400 Reputation with}} {{blue|Seekers of the Sightless Way.}}";
        }
        public override string GetLevelText(int Level)
        {
            return "{{gray|Make a psychic attack that deals {{astral|psionic damage}} to sentient creatures, if the creature dies from the attack, you devour their mind:\n"
            + "\n"
            + "{{white|Gain Health =}} Creatures with larger egos regenerate more health.\n"
            + "{{white|Permanent Ego Boost =}} +1 per Ego Modifier of highest overall creature's ego you've devoured.\n"
            + "\n"
            + "{{white|Devour Attack DC:}} Increases via your Level, Mutation Level and {{light blue|Ego Modifier.}}\n"
            + "{{white|Devour Attack Damage:}} " + Level + " + Ego Modifier + " + 5
            + "\n"
            + "Injured creatures are easier to devour. Lower level Creatures are also easier to devour. \n";

        }
        public int DevourerDamage(int MutationLevel, int CasterEgo, GameObject Target)
        {
            int LevelMultiplier = Stat.Random(1, Level);
            int DamageCalc = (MutationLevel + CasterEgo) + ParentObject.Statistics["Level"].Value + 1;
            int LevelMultVs = Math.Min(0, Stats.GetCombatMA(Target));
            return DamageCalc * (LevelMultiplier - LevelMultVs);
        }
        public void DevourPsychicPulse(Cell TargetCell)
        {
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    TargetCell.ParticleText("&R" + (char)(219 + Stat.Random(0, 10)), 4.9f, 5);
                }
                for (int k = 0; k < 2; k++)
                {
                    TargetCell.ParticleText("&M" + (char)(219 + Stat.Random(0, 10)), 4.9f, 5);
                }
                for (int l = 0; l < 4; l++)
                {
                    TargetCell.ParticleText("&W" + (char)(219 + Stat.Random(0, 10)), 4.9f, 5);
                }
            }
        }
        public int MindEaterSaveDC(int MutationLevel, int TargetHealthPercent, GameObject Target)
        {
            int EgoMod = ParentObject.Statistics["Ego"].Modifier;
            int UserLevel = ParentObject.Statistics["Level"].Value / 2;
            int HealthPenalty = (100 - TargetHealthPercent) / 10;
            int DefenderMath = Target.Statistics["Level"].Value + Stats.GetCombatMA(Target);
            return 5 + EgoMod + UserLevel + MutationLevel + HealthPenalty - DefenderMath;
        }
        public int HealthBoostIncrease(int CreaturesEgo)
        {
            return CreaturesEgo;
        }
        public int PermanentMindEaterBonus(int CreaturesEgo)
        {
            return CreaturesEgo;
        }
        private List<Action<MindEater, GameObject>> VividChoices = new List<Action<MindEater, GameObject>>()
        {
            (ME, Target) =>
            {
                ME.DidXToY("reach", "into the aether and tear into the imprints of", Target, PossessiveObject: true, extra: "psyche, devouring " + Target.them, terminalPunctuation: "!", ColorAsGoodFor: ME.ParentObject, ColorAsBadFor: Target);
            },
            (ME, Target) =>
            {
                ME.DidXToY("devour", "the consciousness of", Target, ColorAsGoodFor: ME.ParentObject, ColorAsBadFor: Target, terminalPunctuation: "!");
            },
            (ME, Target) =>
            {
                ME.DidXToY("absorb", "the consciousness of", Target, ColorAsGoodFor: ME.ParentObject, ColorAsBadFor: Target, terminalPunctuation: "!");
            },
        };
        public void DevourStatGains(GameObject Target)
        {
            int PermNextBonus = PermanentMindEaterBonus(Target.Statistics["Ego"].Modifier);

            if (PermCurrentBonus < PermNextBonus)
            {
                PermCurrentBonus = PermNextBonus;
            }

            StatShifter.SetStatShift("Ego", PermCurrentBonus);
            ParentObject.Heal(HealthBoostIncrease(Target.Statistics["Ego"].Modifier * 3 + 1));
            VividChoices.GetRandomElement()(this, Target);
            ParentObject.UseEnergy(1000);
            //this.DevourPsychicPulse(Target.CurrentCell);
            //Target.CurrentCell.GetRandomLocalAdjacentCell().PsychicPulse();
        }
        public void DevourMinds()
        {
            TextConsole _TextConsole = UI.Look._TextConsole;
            ScreenBuffer Buffer = TextConsole.ScrapBuffer;
            Core.XRLCore.Core.RenderMapToBuffer(Buffer);
            Cell cell = PickDirection();
            if (cell == null)
            {
                return;
            }

            // Be sure to check if these statements return null, I.E "No Game oBject in that cell," or "cancel picking direction." 

            GameObject Target = cell.FindObject(o => o.HasPart("Brain"));
            if (IsPlayer() && Target == null)
            {
                Popup.Show("Lacks a proper mind to devour.");
                return;
            }
            if (IsPlayer() && Target == ParentObject)
            {
                Popup.Show("You can't devour your own mind.");
                return;
            }
            int TargetHealthPercent = 100;
            if (Target.baseHitpoints != 0)
            {
                TargetHealthPercent = Target.hitpoints * 100 / Target.baseHitpoints;
            }
            if (!Target.MakeSave("Willpower", MindEaterSaveDC(this.Level, TargetHealthPercent, Target), ParentObject, "Ego"))
            {
                int CreaturesLevelVS = (int)Math.Min(1, (Stats.GetCombatMA(Target)));
                if ((Target.Stat("Level") + CreaturesLevelVS) < ParentObject.Stat("Level") && Target.Die(ParentObject, "", "Your mind was devoured and assimilated.", false))
                {
                    //AddPlayerMessage("They took InstaDeath [" + (Target.Statistics["Level"].Value + CreaturesLevelVS) + "]");
                    DevourStatGains(Target);
                }

                else
                {
                    Target.TakeDamage(DevourerDamage(Level, ParentObject.Statistics["Ego"].Modifier, Target), "Mental", DeathReason: "Mind was ripped from vessel.", Owner: ParentObject, Attacker: ParentObject, Message: "from %t mind eater attack!");
                    if (Target.Statistics["Hitpoints"].Value <= 0)
                    {
                        //AddPlayerMessage("They took Damage");

                        DevourStatGains(Target);
                    }
                }
                this.DevourPsychicPulse(cell);
            }
            else
            {
                XDidYToZ(Target, "resist", "an attempt from", ParentObject, "mind devouring powers", "!");
            }

            CooldownMyActivatedAbility(ActivatedAbilitiesID, 25);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandDevourMind")
            {
                if (IsMyActivatedAbilityUsable(this.ActivatedAbilitiesID))
                {
                    DevourMinds();
                    return false;
                }
            }
            if (E.ID == "AIGetOffensiveMutationList")
            {
                int intParameter = E.GetIntParameter("Distance");
                GameObject Target = E.GetGameObjectParameter("Target");
                if (Target != null && intParameter <= 1 && !this.ParentObject.IsFrozen() && base.IsMyActivatedAbilityUsable(this.ActivatedAbilitiesID))
                {
                    E.AddAICommand("CommandDevourMind", 1, null, false);
                }
            }
            return base.FireEvent(E);
        }
    }
}