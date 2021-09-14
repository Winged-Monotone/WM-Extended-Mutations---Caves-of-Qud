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
        public int ImmobilityDuration = 0;
        public bool HasSaproStyme = false;
        public GameObject FungalLimbNode;


        public ChitinousSkin()
        {
            this.DisplayName = "Chitinous Skin";
            turnsTilMolt = (int)Stat.GaussianRandom(36001.0f, 7766.40641657f);
            totalTurnsTilMolt = turnsTilMolt;
        }

        public override bool CanLevel()
        {
            return true;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "AIGetPassiveMutationList");
            go.RegisterPartEvent((IPart)this, "BeforeApplyDamage");
            go.RegisterPartEvent((IPart)this, "EndTurn");
            go.RegisterPartEvent((IPart)this, "ModifyDefendingSave");
            go.RegisterPartEvent((IPart)this, "CommandMolting");
            go.RegisterPartEvent((IPart)this, "CommandMolt");
            go.RegisterPartEvent((IPart)this, "CommandChitinHarden");
            go.RegisterPartEvent((IPart)this, "Unequipped");
        }

        public override string GetDescription()
        {
            return "You bear chitinous flesh, as if incased in lacquered leather--you're resistant to physical attacks, but dread its compromise.\n\n"
                + "Your chitinous form gives you a strong, durable body increasing your AV and strength. You molt periodically, losing your armor bonus for some time but gain a small bonus to movement speed. You are more susceptible to poison, venom and disease.\n\n"
                + "{{cyan|+100}} reputation with &Cinsects&y, &Ccrabs&y and &Carachnids&y";
        }

        public override string GetLevelText(int Level)
        {

            int Math1 = (int)Math.Ceiling((Decimal)(Level / 2));

            return "+{{cyan|" + (object)(1 + Math1) + "}} AV\n"
           + "+{{cyan|" + (object)(1 + Math1) + "}} Strength Score\n"
           + "-{{cyan|" + (object)(3 + Math1) + "}} to saves vs. Disease\n"
           + "Take {{cyan|" + (object)(Math1 + 1) + "x}} more Poison Damage\n";
        }

        public void GetMutationTimerAssistant()
        {
            if (turnsTilMolt == totalTurnsTilMolt * 0.9 && IsPlayer())
            {
                AddPlayerMessage("{{green|You note your new skin feels unencumbering and wellworn.}}");
            }
            else if (turnsTilMolt == totalTurnsTilMolt * 0.7 && IsPlayer())
            {
                AddPlayerMessage("{{yellow|You note your skin's segmented clusters clamber a little as you move about.}}");
            }
            else if (turnsTilMolt == totalTurnsTilMolt * 0.3 && IsPlayer())
            {
                AddPlayerMessage("{{orange|Your chitin armor feels heavy and aged.}}");
            }
            else if (turnsTilMolt == totalTurnsTilMolt * 0.1 && IsPlayer())
            {
                AddPlayerMessage("{{red|Your chitin skin is beginning to impede your movements, it'll be ready to molt soon.}}");
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
            if (E.ID == "ModifyDefendingSave" && E.GetStringParameter("Vs", (string)null).Contains("Disease"))
            {
                int ParameterSum = (3 + (int)Math.Ceiling((Decimal)this.Level / new Decimal(3)));
                int roll = E.GetIntParameter("Roll", 0) - ParameterSum;

                E.SetParameter("Roll", roll);
            }
            else if (E.ID == "BeforeApplyDamage")
            {
                Damage eDamage = E.GetParameter("Damage") as Damage;
                int ParameterAmount = (int)((double)eDamage.Amount * (((Level / 2) + 1)));

                if (eDamage.HasAttribute("Poison"))
                { eDamage.Amount = ParameterAmount; }
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
                        this.ActivatedAbilityID = activatedAbilities.AddAbility(Name: "Molt", Command: "CommandMolt", Class: "Physical Mutation", Description: "Molts your Husk.", Icon: "(Z)");
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

                var ParsBody = ParentObject.Body.GetParts();

                foreach (var B in ParsBody)
                {

                    if (B != null && B.Equipped != null)
                    {
                        // AddPlayerMessage("Passed Null Check");
                        if (B.Equipped.HasPropertyOrTag("FungalInfection"))
                        {
                            // AddPlayerMessage("Adding Sapro");
                            FungalLimbNode = B.Equipped;
                            HasSaproStyme = true;
                            // AddPlayerMessage("Fungal Limb Found :" + FungalLimbNode);
                        }
                        if (FungalLimbNode != null && HasSaproStyme == true)
                        {
                            // AddPlayerMessage("Fungal Node Adding Effect");
                            if (!ParentObject.HasEffect("Saprostymie"))
                            {
                                ParentObject.ApplyEffect(new Saprostymie(9999));
                            }
                        }
                        else if (FungalLimbNode == null)
                        {
                            // AddPlayerMessage("Fungal Node Removing Effect");
                            if (ParentObject.HasEffect("Saprostymie"))
                            {
                                ParentObject.RemoveEffect("Saprostymie");
                            }
                        }
                    }
                    // AddPlayerMessage("Leaving Sapro Loop");
                }
                FungalLimbNode = null;
                GetMutationTimerAssistant();
            }
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
            STRBonus = 1 + (int)Math.Ceiling((Decimal)(NewLevel / 2));

            StatShifter.SetStatShift(target: ParentObject, statName: "AV", amount: avBonus, baseValue: true);
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
