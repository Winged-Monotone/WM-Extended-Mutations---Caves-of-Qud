using System;
using XRL.UI;
using XRL.Rules;
using XRL.World.Effects;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class ChitinousSkin : BaseMutation
    {
        public int turnsTilMolt;
        public int avBonus;
        public int STRBonus;
        public Guid ActivatedAbilityID = Guid.Empty;
        public int totalTurnsTilMolt;
        int DoubleCheckDuration = 10;
        public int ImmobilityDuration = 0;


        // this is the constructor


        public ChitinousSkin()
        {
            this.DisplayName = "Chitinous Skin";
            turnsTilMolt = (int)Stat.GaussianRandom(36001.0f, 7766.40641657f);
            // turnsTilMolt = 10;
            totalTurnsTilMolt = turnsTilMolt;
        }


        // This a method, this allows me set whether a mutation levels.


        public override bool CanLevel()
        {
            return true;
        }


        // This is a method that is called by the game, it allows me to register events, and these events already exist, they are here to update the mutation, it calls the fire event method, using else if (E.ID == "PutEventIDHere") variable, it allows me to register event id's to listen to, they do not have to be already existing events.

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "AIGetPassiveMutationList");
            go.RegisterPartEvent((IPart)this, "BeforeApplyDamage");
            go.RegisterPartEvent((IPart)this, "EndTurn");
            go.RegisterPartEvent((IPart)this, "ModifyDefendingSave");
            go.RegisterPartEvent((IPart)this, "CommandMolting");
            go.RegisterPartEvent((IPart)this, "CommandMolt");
            go.RegisterPartEvent((IPart)this, "CommandChitinHarden");
        }

        // this method calls the descriptor of the mutation seen in the mutation description.

        public override string GetDescription()
        {
            return "You bear chitinous flesh, as if incased in lacquered leather--you're resistant to physical attacks, but dread its compromise.\n";
        }

        // this gets the description of the mutation and shows the mutations changes overtime with each level both in the effects list, and the mutation select screen.

        public override string GetLevelText(int Level)
        {
            // the 'return' gives a description "+" being that is adds the (object) which is the mutation itself, plus the "1" integer Math.Ceiling((Decimal) (Level / 2))), in plain english its + whatever it is divided by 2 and then rounded up and then added to the av/n value descripter. Math.Ceiling((Decimal) is representitive of rounding up. \n denounces a new line in the description.

            return "+" + (object)(1 + (int)Math.Ceiling((Decimal)(Level / 2))) + " AV\n"
            + "+" + (object)(1 + (int)Math.Ceiling((Decimal)(Level / 2))) + " Strength Score\n"
            + "-" + (object)(3 + (int)Math.Ceiling((Decimal)Level / new Decimal(3))) + " to saves vs. Disease\n"
            + "Take " + (object)(10 * (int)Math.Ceiling((Decimal)Level / new Decimal(2))) + "% more Poison Damage\n"
            + "You molt periodically, losing your armor bonus for a small period at the cost of a small bonus to movement speed."
            + "+100 reputation with &Cinsects&y and &Carachnids&y\n";
        }

        // the method to call for the E.ID registers that were registered above. Everytime an event list an ID in the registry list, it will call it.
        public void GetMutationTimerAssistant()
        {
            if (turnsTilMolt == totalTurnsTilMolt * 0.9 && IsPlayer())
            {
                AddPlayerMessage("{{green|You note your new skin feels unencumbering and wellworn.}}");
                // XDidY(ParentObject, "{{green|note}}", "{{green|new skin feels unencumbering and wellworn.}}", ".", Color: "green", SubjectPossessedBy: ParentObject);
            }
            else if (turnsTilMolt == totalTurnsTilMolt * 0.7 && IsPlayer())
            {
                AddPlayerMessage("{{yellow|You note your skin's segmented clusters clamber a little as you move about.}}");
                // XDidY(ParentObject, "{{yellow|note}}", "{{yellow|skin's segmented clusters clamber a little as you move about.}}", ".", Color: "yellow", SubjectPossessedBy: ParentObject);
            }
            else if (turnsTilMolt == totalTurnsTilMolt * 0.3 && IsPlayer())
            {
                AddPlayerMessage("{{orange|Your chitin armor feels heavy and aged.}}");
                // XDidY(ParentObject, "{{orange|note}}", "{{orange|skin's segmented clusters clamber a little as you move about.}}", ".", Color: "orange", SubjectPossessedBy: ParentObject);

            }
            else if (turnsTilMolt == totalTurnsTilMolt * 0.1 && IsPlayer())
            {
                AddPlayerMessage("{{red|Your chitin skin is beginning to impede your movements, it'll be ready to molt soon.}}");
                // XDidY(ParentObject, "{{red|chitin skin is beginning to impede}}", "{{orange|impede your movements, it'll be ready to molt soon.}}", ".", Color: "red", SubjectPossessedBy: ParentObject);
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == GetShortDescriptionEvent.ID;
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            string chitinPreviewLookDescriptNew = "{{green|You note your new skin feels unencumbering and wellworn.}}";
            string chitinPreviewLookDescriptWorn = "{{yellow|Your chitin armor feels heavy and aged.}}";
            string chitinPreviewLookDescriptStandard = "{{orange|You note your skin's segmented clusters clamber a little as you move about.}}";
            string chitinPreviewLookDescriptOld = "{{red|Your chitin skin is beginning to impede your movements, it'll be ready to molt soon.}}";
            if (E.Postfix.Length > 0 && E.Postfix[E.Postfix.Length - 1] != '\n')
            {
                E.Postfix.Append('\n');
            }
            if (turnsTilMolt >= totalTurnsTilMolt * 0.7 && IsPlayer())
            {
                E.Postfix.Append('\n').Append(chitinPreviewLookDescriptNew);
            }
            else if (turnsTilMolt >= totalTurnsTilMolt * 0.3 && IsPlayer())
            {
                E.Postfix.Append('\n').Append(chitinPreviewLookDescriptStandard);
            }
            else if (turnsTilMolt >= totalTurnsTilMolt * 0.1 && IsPlayer())
            {
                E.Postfix.Append('\n').Append(chitinPreviewLookDescriptWorn);
            }
            else if (turnsTilMolt >= totalTurnsTilMolt * 0.05 && IsPlayer())
            {
                E.Postfix.Append('\n').Append(chitinPreviewLookDescriptOld);
            }
            return true;
        }

        public override bool FireEvent(Event E)
        {
            // This event calls the AIGetPassiveMutationList event, in this specific event, it controls how AI's passively use certain abilities, traits excedra which are randomly assigned to various mutants-- the second line after the call assigns activatedabilities to pilesOfAbilities and then sets them to equal parentobject.GetPart<ActivatedAbilities>(), the line after is a bool that is "if pileofabilities does not exist, which calls to look at the acitvated abilities, and then it checks if the specific mutation which is assigned its own specail ID in the GUID section of the add abilities list, exist. Iy will then add the command to the ai with the E.AddAICommand, with the string and an integer denoting its priority.
            // if (E.ID == "AIGetPassiveMutationList")
            // {
            //     ActivatedAbilities pilesOfAbilities = ParentObject.GetPart<ActivatedAbilities>();
            //     if (pilesOfAbilities != null)
            //     {
            //         if (pilesOfAbilities.GetAbility(ActivatedAbilityID) != null)
            //         {
            //             E.AddAICommand("CommandMolting", 1);
            //         }
            //     }
            // }
            //Modifydefending save calls for a longer set of commands. essentially it reads as Call event (E.ID) ModifyDefendingSave and then E(Event). Get string, "VS," which denounces the string, string-null,(only if it has one). Contains "Disease.
            //The next parameterter sets up the roll, the int turns roll into an integer that is equivelent to the event.getintparameter which calls for a string, in this case, roll, (I forget the int Paramter)
            if (E.ID == "ModifyDefendingSave" && E.GetStringParameter("Vs", (string)null).Contains("Disease"))
            {
                int roll = E.GetIntParameter("Roll", 0) - (3 + (int)Math.Ceiling((Decimal)this.Level / new Decimal(3)));
                E.SetParameter("Roll", roll);
            }

            else if (E.ID == "BeforeApplyDamage")
            {
                Damage parameter = E.GetParameter("Damage") as Damage;
                if (parameter.HasAttribute("Poison"))
                    parameter.Amount = (int)((double)parameter.Amount * (1 + (0.10 * (int)Math.Ceiling((Decimal)Level / new Decimal(2)))));
            }

            else if (E.ID == "EndTurn")
            {
                if (turnsTilMolt > 0)
                {
                    --turnsTilMolt;
                    if (turnsTilMolt <= 0)
                    {
                        if (ParentObject.IsPlayer())
                            Popup.Show("You've outgrown your exoskeleton, its time to molt.");


                        ParentObject.ApplyEffect(new EncumberingHusk());

                        ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
                        this.ActivatedAbilityID = activatedAbilities.AddAbility("Molt", "CommandMolt", "Physical Mutation", "Molts your Husk.", "(Z)", null, false, false, false, false, false, false, false, false);
                    }
                }
                if (ImmobilityDuration > 0 && ParentObject.HasEffect("Immobilized"))
                {
                    --ImmobilityDuration;
                    if (ImmobilityDuration <= 0)
                    {
                        ParentObject.RemoveEffect("EncumberingHusk", false);
                    }
                }
                GetMutationTimerAssistant();
            }

            // else if (E.ID == "CommandMolting")
            // {
            //     if (!this.ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap() && !ParentObject.HasEffect("Molting"))
            //     {
            //         if (this.ActivatedAbilityID != Guid.Empty)
            //         {
            //             ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
            //             activatedAbilities.RemoveAbility(this.ActivatedAbilityID);
            //         }
            //         this.ParentObject.ApplyEffect(new Molting());
            //     }
            //     else
            //     {
            //         Popup.Show("You must stop travelling before you can molt.");
            //     }
            // }

            else if (E.ID == "CommandMolt")
            {
                if (!this.ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap())
                {
                    if (this.ActivatedAbilityID != Guid.Empty)
                    {
                        ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
                        activatedAbilities.RemoveAbility(this.ActivatedAbilityID);
                    }
                    ParentObject.RemoveEffect("EncumberingHusk", false);
                    Cell currentCell = this.ParentObject.GetCurrentCell();
                    ParentObject.ApplyEffect(new Immobilized(35, "Strength", "Molting Process", "molting"));
                    ParentObject.ApplyEffect(new SoftChitin(avBonus));
                    if (Level <= 3)
                    {
                        currentCell.AddObject("GamerGirlMoltShoddy");
                    }
                    else if (Level <= 6)
                    {
                        currentCell.AddObject("GamerGirlMoltQuality");
                    }
                    else if (Level <= 9)
                    {
                        currentCell.AddObject("GamerGirlMoltHighQuality");
                    }
                    else if (Level < 10)
                    {
                        currentCell.AddObject("GamerGirlMoltDeluxe");
                        GameObject Molt = currentCell.GetObjectWithTagOrProperty("Commerce");
                        Molt.SetIntProperty("Value", +(Level - 9) * 250);
                    }
                }
                else
                {
                    Popup.Show("You must stop travelling before you can molt.");
                }
            }

            else if (E.ID == "CommandChitinHarden")
            {
                turnsTilMolt = (int)Stat.GaussianRandom(360001.0f, 77664.06416574f);
                // turnsTilMolt = 10;
                totalTurnsTilMolt = turnsTilMolt;
            }

            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            StatShifter.DefaultDisplayName = "Chitin Skin";
            avBonus = 1 + (int)Math.Ceiling((Decimal)(NewLevel / 2));
            StatShifter.SetStatShift(target: ParentObject, statName: "AV", amount: avBonus, baseValue: true);
            STRBonus = 1 + (int)Math.Ceiling((Decimal)(NewLevel / 2));
            StatShifter.SetStatShift(target: ParentObject, statName: "Strength", amount: STRBonus, baseValue: true);
            if (ParentObject.HasEffect("SoftChitin"))
            {
                SoftChitin effect = ParentObject.GetEffect("SoftChitin") as SoftChitin;
                effect.setAVDebuff(avBonus, ParentObject);
            }
            return base.ChangeLevel(NewLevel);

        }

        public override bool Mutate(GameObject go, int Level)
        {
            avBonus = 1 + (int)Math.Ceiling((Decimal)(Level / 2));
            STRBonus = 1 + (int)Math.Ceiling((Decimal)(Level / 3));
            StatShifter.SetStatShift(target: ParentObject, statName: "AV", amount: avBonus, baseValue: true);
            StatShifter.SetStatShift(target: ParentObject, statName: "Strength", amount: STRBonus, baseValue: true);
            return true;
        }

        public override bool Unmutate(GameObject go)
        {
            StatShifter.RemoveStatShifts();
            return base.Unmutate(go);
        }
    }
}
