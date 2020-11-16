using System;
using XRL.UI;
using XRL.Rules;
using ConsoleLib.Console;
using XRL.World.ZoneBuilders;


namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class PsychoplethoricDeterioration : BaseMutation
    {
        public int DecayingFormDurationCycle = 1200;
        public int HuskWeakeningDuration;
        public int SetCounterUberAptitude = 0;
        public int UbernostrumScaling = 0;
        public Guid ActivatedAbilityID;

        public PsychoplethoricDeterioration()
        {
            this.DisplayName = "Psychoplethoric Deterioration ({{purple|H}})";
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override string GetDescription()
        {
            return "Your incredible psionic power comes at the cost of overwhelming the stability of your physical form, you are doomed to hunting down physical husk to maintain your tether to this reality, albeit the magnitude of your psionic abilities are of a realm of its own.\n"
                + "\n"
                + "{{orange|-400 reputation with highly entropic beings.\n}}"
                + "{{light blue|+400 reputation the Seekers of the Sightless Way.\n\n}}";
        }
        public override string GetLevelText(int Level)
        {
            return "Your physical form {{red|rapidly disintegrates}}, every {{light blue|600}} turns, you must pass a {{light blue|Toughness Saving Throw}} at difficulty {{light blue|28 + " + Level + "}} or lose {{light blue|0-3}} points of your maximum HP. Using an ubernostrum injector will partially regenerate your lost maximum HP, however continued use on the same husk becomes less effective. Your Strength, Toughness and Agility are reduced by {{light blue|-6}}, and your mental stats are increased by {{light blue|+6}}.\n\n"
            + "{{purple|Soulshunt (Cooldown 2 days - 2400 turns)\n}}"
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

            ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
            this.ActivatedAbilityID = activatedAbilities.AddAbility("Soulshunt", "CommandSoulShunt", "Mental Mutation", "Shunt the imprints of your victims' mind from their body, and assume the throne of their vessel.\n\n" + "Target makes a Willpower saving-throw vs your Ego Modifier {{light blue|(+10)}} or be shunted from its body; you assume control of the target's body permanently. Your new husk will wither over time. On a successful soulshunt you gain a 10% chance to increase your ego score by {{light blue|1}}." + "\n\n{{dark gray|Base cooldown: 2400}}", "(O)", null, false, false, false, false, false, false, false, false, -1);
            return base.Mutate(GO, Level);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            if (ParentObject != null)
            {
                XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("highly entropic beings", -400, false);
                XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("Seekers", 400, false);
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
                return;
            }

            var TargetHusk = TargetCell.GetFirstObjectWithPart("Brain");

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

                // AddPlayerMessage("owners level: " + OwnersLevel);
                // AddPlayerMessage("targets level: " + TargetsLevel);

                var LevelDifference = OwnersLevel - TargetsLevel;

                if (!TargetHusk.MakeSave("Willpower", 8 + LevelDifference, ParentObject, "Ego", ParentObject.It + " attempted to shunt " + TargetHusk.Its + " mind from " + TargetHusk.Its + " body.", false, false, false, false))
                {
                    if (!TargetHusk.HasPart("NotOriginalEntity"))
                    {
                        TargetHusk.AddPart<NotOriginalEntity>();
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

                    game.Player.Body = TargetHusk;


                    TargetHusk.FireEvent(Event.New("SuccessfulDethroning", "OriginalBody", ParentObject));
                    UbernostrumScaling = 0;
                }
                else
                {
                    TargetHusk.GetAngryAt(ParentObject, -100);
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
            if (E.ID == "DamageFromDecay")
            {
                int DegradateLevel = ParentObject.Stat("Level");

                if (!ParentObject.MakeSave("Toughness", (28 + DegradateLevel), null, null, "Husk Deterioration"))
                {
                    // StatShifter.SetStatShift(ParentObject, "Hitpoints", -Stat.Random(0, 3), true);
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
                    { ParentObject.Die(null, null, "As your husk crumbles to dust, so do your last tethers as your mind is cast into the darkness of the void.", Force: false); }
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

                if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.7)
                {
                    StatShifter.SetStatShift(ParentObject, "Strength", -((int)Math.Round(OwnersStrength * 0.7)), false);
                    StatShifter.SetStatShift(ParentObject, "Toughness", -((int)Math.Round(OwnersToughness * 0.7)), false);
                    StatShifter.SetStatShift(ParentObject, "Agility", -((int)Math.Round(OwnersAgility * 0.7)), false);
                }
                else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.3)
                {
                    StatShifter.SetStatShift(ParentObject, "Strength", -((int)Math.Round(OwnersStrength * 0.3)), false);
                    StatShifter.SetStatShift(ParentObject, "Toughness", -((int)Math.Round(OwnersToughness * 0.3)), false);
                    StatShifter.SetStatShift(ParentObject, "Agility", -((int)Math.Round(OwnersAgility * 0.3)), false);
                }
                else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.1)
                {
                    StatShifter.SetStatShift(ParentObject, "Strength", -((int)Math.Round(OwnersStrength * 0.1)), false);
                    StatShifter.SetStatShift(ParentObject, "Toughness", -((int)Math.Round(OwnersToughness * 0.1)), false);
                    StatShifter.SetStatShift(ParentObject, "Agility", -((int)Math.Round(OwnersAgility * 0.1)), false);
                }
                else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.05)
                {
                    StatShifter.SetStatShift(ParentObject, "Strength", -((int)Math.Round(OwnersStrength * 0.05)), false);
                    StatShifter.SetStatShift(ParentObject, "Toughness", -((int)Math.Round(OwnersToughness * 0.05)), false);
                    StatShifter.SetStatShift(ParentObject, "Agility", -((int)Math.Round(OwnersAgility * 0.05)), false);
                }
                else if (HuskWeakeningDuration >= HuskWeakeningDuration * 0.03)
                {

                }
                else if (HuskWeakeningDuration <= 0)
                {
                    HuskWeakeningDuration = (int)Math.Round(HuskWeakeningDuration * 0.7);
                }
            }
            else if (E.ID == "SuccessfulDethroning")
            {

                GameObject OriginalBody = E.GetGameObjectParameter("OriginalBody");

                var CreatureTier = OriginalBody.GetTier();
                var PrimaryFaction = OriginalBody.GetPrimaryFaction();
                var FactionVar = Factions.get(PrimaryFaction);

                var NewBodyPrimaryFaction = OriginalBody.GetPrimaryFaction();

                ParentObject.FireEvent(Event.New("EntityHasSwappedBodies"));


                if (FactionVar.Visible)
                {
                    try
                    {
                        XRL.Core.XRLCore.Core.Game.PlayerReputation.modify(PrimaryFaction, -CreatureTier * 50, true);
                    }
                    catch
                    {
                        return true;
                    }
                }

                if (OriginalBody != null)
                {

                    // AddPlayerMessage("Original Body: " + OriginalBody + ".");
                    // AddPlayerMessage("Parent Body: " + ParentObject + ".");

                    if (!ParentObject.HasProperName)
                    {
                        ParentObject.DisplayName = OriginalBody.DisplayNameOnly;
                        OriginalBody.DisplayName = HeroMaker.MakeHeroName(ParentObject, new string[0], new string[0], false);
                    }
                    else
                    {
                        var NewName = ParentObject.DisplayNameOnly;
                        ParentObject.DisplayName = OriginalBody.DisplayNameOnly;
                        OriginalBody.DisplayName = NewName;

                    }
                    OriginalBody.GetAngryAt(ParentObject, -100);

                    PlayWorldSound("disintegration");

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