using System;
using XRL.UI;
using XRL.Rules;
using ConsoleLib.Console;
using XRL.World.ZoneBuilders;
using XRL.World.Effects;
using XRL.World.Parts.Skill;
using System.Collections.Generic;
using XRL.World.AI.GoalHandlers;


namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class PsychoplethoricDeterioration : BaseMutation
    {
        public int DecayingFormDurationCycle = 600;
        public int HuskWeakeningDuration;
        public int SetCounterUberAptitude = 0;
        public int UbernostrumScaling = 0;
        public Guid ActivatedAbilityID;

        public PsychoplethoricDeterioration()
        {
            this.DisplayName = "Psychoplethoric Deterioration ({{purple|H}})";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override string GetDescription()
        {
            return "Your incredible psionic power comes at the cost of overwhelming the stability of your physical form, you are doomed to hunting down physical husk to maintain your tether to this reality, albeit the magnitude of your psionic abilities are of a realm of its own.\n"
                + "\n"
                + "{{cyan|-400 reputation with highly entropic beings.\n}}"
                + "{{cyan|+400 reputation the Seekers of the Sightless Way.\n}}";
        }
        public override string GetLevelText(int Level)
        {
            return "{{purple|Soulshunt (Cooldown 2 days - 2400 turns)\n\n}}"
            + "Shunt the imprints of your victims' mind from their body, and assume the throne of their vessel.\n\n";
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            if (!ParentObject.HasPart("NotOriginalEntity"))
            {
                // Statshifter - Setting all stats for starting husk
                StatShifter.SetStatShift(ParentObject, "Ego", 6, true);
                StatShifter.SetStatShift(ParentObject, "Willpower", 6, true);
                StatShifter.SetStatShift(ParentObject, "Intelligence", 6, true);
                StatShifter.SetStatShift(ParentObject, "Strength", -6, true);
                StatShifter.SetStatShift(ParentObject, "Toughness", -6, true);
                StatShifter.SetStatShift(ParentObject, "Agility", -6, true);
            }
            if (!ParentObject.HasEffect<Disintegrating>())
            {
                ParentObject.ApplyEffect(new Disintegrating(9999));
            }

            ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
            this.ActivatedAbilityID = activatedAbilities.AddAbility(Name: "Soulshunt", Command: "CommandSoulShunt", Class: "Mental Mutation", Description: "Shunt the imprints of your victims' mind from their body, and assume the throne of their vessel.\n\n" + "Target makes a Willpower saving-throw vs your Ego Modifier {{cyan|(+10)}} or be shunted from its body; you assume control of the target's body permanently and its INT, WIL, and EGO become your own. Your new husk will wither over time. On a successful soulshunt you have a {{cyan|10%}} chance to increase your ego score by {{cyan|1}}." + "\n\n{{dark gray|Base cooldown: 2400}}", Icon: "(O)", Cooldown: -1);
            return base.Mutate(GO, Level);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            if (!ParentObject.HasSkill("Survival"))
            {
                ParentObject.AddSkill("Survival");
                if (!ParentObject.HasSkill("Survival_Camp"))
                {
                    ParentObject.AddSkill("Survival_Camp");
                }
            }

            return base.ChangeLevel(NewLevel);
        }

        public override bool Unmutate(GameObject GO)
        {
            StatShifter.RemoveStatShifts();
            return base.Unmutate(GO);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == GetShortDescriptionEvent.ID;
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            string Glyph = "{{dark gray|Atomized dust wafts from disir lesions riddling " + ParentObject.its + " withering form, this husk won't last ...}}";
            if (E.Postfix.Length > 0 && E.Postfix[E.Postfix.Length - 1] != '\n')
            {
                E.Postfix.Append('\n');
            }
            E.Postfix.Append('\n').Append(Glyph);
            return true;
        }
        public override bool WantTurnTick()
        {
            return true;
        }

        public override bool WantTenTurnTick()
        {
            return true;
        }

        public override bool WantHundredTurnTick()
        {
            return true;
        }

        public override void TurnTick(long TurnNumber)
        {
            DecayingFormDurationCycle -= 1;
            if (DecayingFormDurationCycle <= 0)
            {
                ParentObject.FireEvent(Event.New("DamageFromDecay"));
                DecayingFormDurationCycle = 600;
            }
        }

        public override void TenTurnTick(long TurnNumber)
        {
            DecayingFormDurationCycle -= 10;
            if (DecayingFormDurationCycle <= 0)
            {
                ParentObject.FireEvent(Event.New("DamageFromDecay"));
                DecayingFormDurationCycle = 600;

            }
        }

        public override void HundredTurnTick(long TurnNumber)
        {
            DecayingFormDurationCycle -= 100;
            if (DecayingFormDurationCycle <= 0)
            {
                ParentObject.FireEvent(Event.New("DamageFromDecay"));
                DecayingFormDurationCycle = 600;
            }
        }



        public void SoulShunt()
        {
            Physics pGOPhysics = ParentObject.GetPart("Physics") as Physics;
            TextConsole _TextConsole = UI.Look._TextConsole;
            ScreenBuffer Buffer = TextConsole.ScrapBuffer;
            Core.XRLCore.Core.RenderMapToBuffer(Buffer);
            Cell TargetCell = PickDirection();
            if (TargetCell == null)
            {
                AddPlayerMessage("Soulshunt requires a target.");
                return;
            }

            var TargetHusk = TargetCell.GetFirstObjectWithPart("Brain");

            var SkillAccess = ParentObject.GetPart<Skills>();
            var SkillList = SkillAccess.SkillList;

            if (TargetHusk != null)
            {
                var OwnersEgo = ParentObject.Statistics["Ego"];
                var OwnersWillpower = ParentObject.Statistics["Willpower"];
                var OwnersIntelligence = ParentObject.Statistics["Intelligence"];
                var OwnersMentalMutations = ParentObject.GetMentalMutations();
                var OwnersMutationAlterations = ParentObject.GetPart<Mutations>();

                var TargetsEgo = TargetHusk.Statistics["Ego"];
                var TargetsWillpower = TargetHusk.Statistics["Willpower"];
                var TargetsIntelligence = TargetHusk.Statistics["Intelligence"];
                var TargetsMentalMutations = TargetHusk.GetMentalMutations();
                var TargetsMutationAlterations = TargetHusk.GetPart<Mutations>();

                int OwnersLevel = ParentObject.Stat("Level");
                int TargetsLevel = TargetHusk.Stat("Level");

                var LevelDifference = OwnersLevel - TargetsLevel;


                if (!TargetHusk.MakeSave("Willpower", 8 + LevelDifference, ParentObject, "Ego", ParentObject.It + " attempted to shunt " + TargetHusk.Its + " mind from " + TargetHusk.Its + " body.", false, false, false, false))
                {
                    if (!TargetHusk.HasPart("NotOriginalEntity"))
                    {
                        TargetHusk.AddPart<NotOriginalEntity>();
                    }
                    if (ParentObject.IsEsper())
                    {
                        if (!TargetHusk.IsEsper())
                        {
                            TargetHusk.SetStringProperty("Genotype", "Esper");
                        }
                    }

                    TargetsEgo.BaseValue = OwnersEgo.BaseValue;
                    TargetsIntelligence.BaseValue = OwnersIntelligence.BaseValue;
                    TargetsWillpower.BaseValue = OwnersWillpower.BaseValue;

                    // AddPlayerMessage("removing old mutes");
                    //removes husk original mental mutations
                    foreach (var key1 in TargetsMentalMutations)
                    {
                        TargetsMutationAlterations.RemoveMutation(key1);
                    }
                    // AddPlayerMessage("adding old mutes");
                    //adds parent objects mental mutations to husk
                    foreach (var key2 in OwnersMentalMutations)
                    {
                        TargetsMutationAlterations.AddMutation(key2.GetMutationEntry(), key2.BaseLevel);
                    }

                    if (!TargetsMutationAlterations.HasMutation("PsychoplethoricDeterioration"))
                    {
                        TargetsMutationAlterations.AddMutation("PsychoplethoricDeterioration", 1);
                    }

                    if (!TargetHusk.HasPart("Survival"))
                    {
                        TargetHusk.RequirePart<Survival>();
                        TargetHusk.RequirePart<Survival_Camp>();
                        TargetHusk.RequirePart<Survival_Trailblazer>();
                    }

                    if (ParentObject.IsPlayer())
                        XRL.The.Game.Player.Body = TargetHusk;
                    else
                        XRL.The.Game.Player.Body = ParentObject;

                    TargetHusk.UseEnergy(1000);
                    ParentObject.UseEnergy(5000);


                    TargetHusk.FireEvent(Event.New("SuccessfulDethroning", "OriginalBody", ParentObject));
                    UbernostrumScaling = 0;
                }
                else
                {
                    TargetHusk.GetAngryAt(ParentObject, -100);
                    AddPlayerMessage(TargetHusk.it + " resisted " + ParentObject.its + " attempt at shunting " + TargetHusk.its + " mind.");
                }
            }
        }

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "AIGetPassiveMutationList");
            go.RegisterPartEvent((IPart)this, "SuccessfulDethroning");
            go.RegisterPartEvent((IPart)this, "DebuffsFromDecay");
            go.RegisterPartEvent((IPart)this, "EndTurn");
            go.RegisterPartEvent((IPart)this, "ApplyingTonic");
            go.RegisterPartEvent((IPart)this, "CommandSoulShunt");
            go.RegisterPartEvent((IPart)this, "DamageFromDecay");
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "AIGetOffensiveMutationList")
            {
                int intParameter = E.GetIntParameter("Distance");
                if (intParameter <= 1 & IsMyActivatedAbilityCoolingDown(ActivatedAbilityID, ParentObject))
                {
                    GameObject gameObjectParameter2 = E.GetGameObjectParameter("Target");
                    if (gameObjectParameter2.PhaseAndFlightMatches(ParentObject))
                    {
                        List<AICommandList> list = E.GetParameter("List") as List<AICommandList>;
                        list.Add(new AICommandList("CommandSoulShunt", 1));
                    }
                }
            }
            if (E.ID == "DamageFromDecay")
            {
                int DegradateLevel = ParentObject.Stat("Level");

                if (!ParentObject.MakeSave("Toughness", (28 + DegradateLevel), null, null, "Husk Deterioration"))
                {
                    ParentObject.Statistics["Hitpoints"].BaseValue -= Stat.Random(0, 3);
                }
            }
            else if (E.ID == "ApplyingTonic")
            {
                if (E.GetParameter<GameObject>("Tonic").Blueprint == "UbernostrumTonic" && SetCounterUberAptitude <= 0)
                {
                    UbernostrumScaling = (int)Math.Round(Stat.Random(0.10f, 0.30f) * ParentObject.GetStatValue("Hitpoints", 1));

                    StatShifter.SetStatShift(ParentObject, "Hitpoints", UbernostrumScaling, true);
                    SetCounterUberAptitude += 1;
                }
                else if (E.GetParameter<GameObject>("Tonic").Blueprint == "UbernostrumTonic" && SetCounterUberAptitude > 0)
                {
                    UbernostrumScaling = (int)Math.Round(Stat.Random(0.10f, 0.30f) * ParentObject.GetStatValue("Hitpoints", 1));
                    UbernostrumScaling -= (int)Math.Round(UbernostrumScaling * (SetCounterUberAptitude + 0.10));

                    StatShifter.SetStatShift(ParentObject, "Hitpoints", UbernostrumScaling, true);
                    SetCounterUberAptitude += 1;
                }
            }
            else if (E.ID == "EndTurn")
            {
                if (HuskWeakeningDuration > 0)
                {
                    --HuskWeakeningDuration;
                    if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.7)
                    {
                        ParentObject.FireEvent(Event.New("DebuffsFromDecay"));
                    }
                    else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.3)
                    {
                        ParentObject.FireEvent(Event.New("DebuffsFromDecay"));
                    }
                    else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.1)
                    {
                        ParentObject.FireEvent(Event.New("DebuffsFromDecay"));
                    }
                    else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.05)
                    {
                        ParentObject.FireEvent(Event.New("DebuffsFromDecay"));
                    }
                }
                else if (ParentObject.Statistics["Hitpoints"].BaseValue <= 1)
                {
                    if (ParentObject.IsPlayer())
                    { ParentObject.Die(null, null, "As your husk crumbles to dust, so do your last tethers to world as your form radiates away.", Force: false); }
                }
                if (!ParentObject.HasEffect<Disintegrating>())
                {
                    ParentObject.ApplyEffect(new Disintegrating(9999));
                }
            }
            else if (E.ID == "CommandSoulShunt")
            {
                ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
                activatedAbilities.GetAbility(ActivatedAbilityID).Cooldown = 24000;
                SoulShunt();
                var HuskCurrentToughness = ParentObject.Stat("Toughness");
                HuskWeakeningDuration = 1200 * Math.Min(1, HuskCurrentToughness);
            }
            else if (E.ID == "DebuffsFromDecay")
            {
                var OwnersStrength = ParentObject.Stat("Strength");
                var OwnersToughness = ParentObject.Stat("Toughness");
                var OwnersAgility = ParentObject.Stat("Agility");

                if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.6)
                {

                }
                else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.5)
                {
                    StatShifter.SetStatShift(ParentObject, "Strength", -((int)Math.Round(OwnersStrength * 0.2)), false);
                    StatShifter.SetStatShift(ParentObject, "Toughness", -((int)Math.Round(OwnersToughness * 0.2)), false);
                    StatShifter.SetStatShift(ParentObject, "Agility", -((int)Math.Round(OwnersAgility * 0.2)), false);
                }
                else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.3)
                {
                    StatShifter.SetStatShift(ParentObject, "Strength", -((int)Math.Round(OwnersStrength * 0.4)), false);
                    StatShifter.SetStatShift(ParentObject, "Toughness", -((int)Math.Round(OwnersToughness * 0.4)), false);
                    StatShifter.SetStatShift(ParentObject, "Agility", -((int)Math.Round(OwnersAgility * 0.4)), false);
                }
                else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.1)
                {
                    StatShifter.SetStatShift(ParentObject, "Strength", -((int)Math.Round(OwnersStrength * 0.6)), false);
                    StatShifter.SetStatShift(ParentObject, "Toughness", -((int)Math.Round(OwnersToughness * 0.6)), false);
                    StatShifter.SetStatShift(ParentObject, "Agility", -((int)Math.Round(OwnersAgility * 0.6)), false);
                }
                else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.05)
                {
                    StatShifter.SetStatShift(ParentObject, "Strength", -((int)Math.Round(OwnersStrength * 0.8)), false);
                    StatShifter.SetStatShift(ParentObject, "Toughness", -((int)Math.Round(OwnersToughness * 0.8)), false);
                    StatShifter.SetStatShift(ParentObject, "Agility", -((int)Math.Round(OwnersAgility * 0.8)), false);
                }
                else if (HuskWeakeningDuration <= 0)
                {
                    HuskWeakeningDuration = (int)Math.Round(HuskWeakeningDuration * 0.7);
                }
            }
            else if (E.ID == "SuccessfulDethroning")
            {

                GameObject OriginalBody = E.GetGameObjectParameter("OriginalBody");

                var SkillAccess = OriginalBody.GetPart<Skills>();
                var SkillListing = SkillAccess.SkillList;

                var CreatureTier = OriginalBody.GetTier();
                var PrimaryFaction = OriginalBody.GetPrimaryFaction();
                var FactionVar = Factions.get(PrimaryFaction);
                var NewBodyPrimaryFaction = OriginalBody.GetPrimaryFaction();

                var ParentIntelligenceSkillAward = (ParentObject.BaseStat("Intelligence") - 10) * 4;

                ParentObject.FireEvent(Event.New("EntityHasSwappedBodies"));

                if (!ParentObject.HasSkill("Survival"))
                {
                    ParentObject.AddSkill("Survival");
                    if (!ParentObject.HasSkill("Survival_Camp"))
                    {
                        ParentObject.AddSkill("Survival_Camp");
                    }
                }

                foreach (var k in SkillListing)
                {
                    ParentObject.GetStat("SP").BaseValue += ParentIntelligenceSkillAward;
                }
                if (FactionVar.Visible)
                {
                    try
                    {
                        XRL.Core.XRLCore.Core.Game.PlayerReputation.modify(NewBodyPrimaryFaction, -CreatureTier * 50, true);
                    }
                    catch
                    {
                        return true;
                    }
                }

                if (OriginalBody != null)
                {

                    if (!ParentObject.HasProperName)
                    {
                        ParentObject.DisplayName = OriginalBody.DisplayNameOnly;
                        OriginalBody.DisplayName = Names.NameMaker.MakeName(ParentObject);
                    }
                    else
                    {
                        var NewName = ParentObject.DisplayNameOnly;
                        ParentObject.DisplayName = OriginalBody.DisplayNameOnly;
                        OriginalBody.DisplayName = NewName;

                    }
                    OriginalBody.GetAngryAt(ParentObject, -100);

                    PlayWorldSound("soulshunt");

                    var DifferenceVar = ParentObject.StatMod("Ego") - OriginalBody.StatMod("Ego");
                    DifferenceVar *= 5;

                    if (Stat.Random(1, 100) <= DifferenceVar && IsPlayer())
                    {
                        if (Popup.ShowYesNo("You feel the remnants of tender light pulsating within your new husk, would you like to imprint these codings upon your own animus?", false, DialogResult.Yes) == DialogResult.Yes)
                        {
                            StatShifter.SetStatShift(ParentObject, "Ego", 1, true);
                        }
                        else
                        {
                            Popup.Show("You cast the remnants away.");
                        }
                    }
                }
            }

            return base.FireEvent(E);
        }
    }
}