
using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Effects;
using XRL.World.Parts;
using ConsoleLib.Console;
using XRL.Core;
using XRL.Rules;
using XRL.World.Capabilities;
using System.Linq;
using System.Text;

using XRL.Messages;
using XRL.UI;

using XRL.World.AI.GoalHandlers;
using XRL.World.Parts.Mutation;

using UnityEngine;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Thermokinesis : BaseMutation
    {
        public Guid ActivateThermokinesisAbilityID;
        public int MaximumRadius = 1;
        public static readonly List<string> MainOptions = new List<string>()
        {
            "Alter Resistances",
            "Alter Ambient Temperature",
            "Cancel"
        };
        public static readonly List<string> ResistancesOptions = new List<string>()
        {
            "Increase Resistances",
            "Decrease Resistances",
            "Remove Resistances"
        };
        public static readonly List<string> ElementOptions = new List<string>()
        {
            "Cold",
            "Heat",
        };
        public static readonly List<string> AmbientTempOptions = new List<string>()
        {
            "Increase Temperature",
            "Decrease Temperature"
        };
        public void BeginTempAlteration()
        {

            string mainChoice = GetChoice(MainOptions);

            if (!string.IsNullOrEmpty(mainChoice))
            {
                if (mainChoice == "Alter Resistances")
                {
                    string ResistanceChoice = GetChoice(ResistancesOptions);
                    if (!string.IsNullOrEmpty(ResistanceChoice))
                    {
                        if (ResistanceChoice == "Increase Resistances" || ResistanceChoice == "Decrease Resistances")
                        {
                            string ElementChoice = GetChoice(ElementOptions);
                            if (!string.IsNullOrEmpty(ElementChoice))
                            {
                                ChangeTargetResistances(ResistanceChoice, ElementChoice);
                            }
                        }
                        else
                            RemoveResistances();
                    }
                }
                if (mainChoice == "Alter Ambient Temperature")
                {
                    string AmbientTempChoices = GetChoice(AmbientTempOptions);
                    if (!string.IsNullOrEmpty(AmbientTempChoices))
                    {
                        //
                        ChangeAmbientTemperature(AmbientTempChoices);
                    }
                }
            }
        }
        public static string GetChoice(List<string> valuesToShow)
        {
            int result = Popup.ShowOptionList(
                Title: "Select an Option",
                Options: valuesToShow.ToArray(),
                AllowEscape: true);
            if (result < 0 || valuesToShow[result].ToUpper() == "CANCEL")
            {
                // The user escaped out of the menu, or chose "Cancel"
                return string.Empty;
            }
            else
            {
                // The user selected a value - return the select value as a string
                return valuesToShow[result];
            }
        }
        public Thermokinesis()
        {
            this.DisplayName = "Thermokinesis";
            this.Type = "Mental";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CommandAlterTemperatures");
            base.Register(Object);
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
            return "You can manipulate latent thermal energy of the world around you.";
        }

        public override string GetLevelText(int Level)
        {
            try
            {
                var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;
                MaximumRadius = 1 + Level;

                if (Level == base.Level)
                    return "Alter the resistances of creatures at your will, or change the ambient temperature of an area around you.\n"
                  + "Charge Potency towards Resistance Alteration: " + "{{cyan|" + ParentsEgo + "}}" + " * " + "{{cyan|Charges}}\n"
                  + "Charge Potency towards Ambient Temperature Manipulation: " + "{{cyan|" + 250 + "}}" + " * " + "{{cyan|Charges}}\n\n"
                  + "Save Target Vs' Negative Resistance Changes: " + "{{cyan|" + (10 + ParentsEgo + Level) + "}}\n"
                  + "Maximum Radius: {{cyan|" + MaximumRadius + "}}";
                else
                    return "Save Target Vs' Negative Resistance Changes: " + "{{cyan|" + (10 + ParentsEgo + Level) + "}}\n"
                      + "Maximum Radius: {{cyan|" + MaximumRadius + "}}";


            }
            catch
            {
                return "Alter the resistances of creatures at your will, or change the ambient temperature of an area around you. The range and area-effect of the ability scales as Thermokinesis increases in level.\n";
            }
        }

        public void RemoveResistances()
        {
            Cell TargetCell = PickDestinationCell(12 + Level, AllowVis.OnlyVisible, false, true, false, true);
            GameObject Target = TargetCell.GetFirstObjectWithPart("Combat");

            if (!Target.HasEffect("Temperatura"))
            {
                AddPlayerMessage("{{R|Creature is not effected by any temperature altering effects.}}");
                return;
            }
            else if (Target.HasEffect("Temperatura"))
            {
                Target.RemoveEffect("Temperatura");
                PlayWorldSound("tempaltered", PitchVariance: 1);
                XDidYToZ(ParentObject, "returns", null, Target, "resistances to normal", ".", null, ParentObject, Target, true, PossessiveObject: true);
                return;
            }
        }

        public void ChangeTargetResistances(string ResistanceChoice, string ElementChoice)
        {
            // AddPlayerMessage("MethodFiring: ChangeTargetResistances");

            FocusPsi PsiMutation = ParentObject.GetPart<FocusPsi>();

            var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;
            var ParentsLevel = ParentObject.Statistics["Level"].BaseValue;
            var ParentsCharges = ParentObject.Statistics["PsiCharges"].BaseValue;

            string ChargesSpent = PsiMutation.focusPsiCurrentCharges.ToString();



            if (PsiMutation == null)
            {
                // AddPlayerMessage("You lack the ability to do this.");
                string verb1 = "lack";
                string extra1 = "ability to do this";
                string termiPun1 = "!";
                XDidY(ParentObject, verb1, extra1, termiPun1);
                return;
            }
            if (IsPlayer())
            {
                ChargesSpent = Popup.AskString("Expend how many charges", "1", 3, 1, "0123456789");
            }

            int Charges = Convert.ToInt32(ChargesSpent);

            if (!PsiMutation.usePsiCharges(Charges))
            {
                AddPlayerMessage("You do not have enough psi-charges!");
                return;
            }
            if (IsPlayer() && Charges <= 0)
            {
                AddPlayerMessage("That's not a valid amount of charges.");
                return;
            }
            if (Charges > 1 + ParentsEgo + ParentsLevel && !ParentObject.HasEffect("Psiburdening"))
            {
                int fatigueVar = 25;
                ParentObject.ApplyEffect(new Psiburdening(fatigueVar * Charges));
            }

            // AddPlayerMessage("MethodStep: Getting Target");

            Cell TargetCell = PickDestinationCell(12 + Level, AllowVis.OnlyVisible, false, true, false, true);
            GameObject Target = TargetCell.GetFirstObjectWithPart("Combat");

            // AddPlayerMessage("MethodStep: Getting Element Type");


            string Element = null;
            if (ElementChoice == "Heat")
            {
                Element = "HeatResistance";
            }
            else if (ElementChoice == "Cold")
            {
                Element = "ColdResistance";
            }


            // AddPlayerMessage("MethodStep: Applying Effect");

            if (!Target.HasEffect("Temperatura"))
            {
                Target.ApplyEffect(new Temperatura(0, Owner: ParentObject));
                Target.ParticleBlip("{{W|#}}", 3);
                PlayWorldSound("tempaltered");
                Event cE = Event.New("AlteringTemperatureEffectEvent", "ChargesSpent", Charges, "Element", Element, "Caster", ParentObject, "IncreaseOrDecrease", ResistanceChoice, "MutationLevel", Level);

                // AddPlayerMessage("MethodStep: FireEvent CE");

                Target.FireEvent(cE);
            }
            else
            {
                PlayWorldSound("tempaltered");
                Event cE = Event.New("AlteringTemperatureEffectEvent", "ChargesSpent", Charges, "Element", Element, "Caster", ParentObject, "IncreaseOrDecrease", ResistanceChoice, "MutationLevel", Level);

                // AddPlayerMessage("MethodStep: FireEvent CE/ No effect");

                Target.FireEvent(cE);
            }

            // AddPlayerMessage("MethodStep: Setting Flavour Text");

            if (ResistanceChoice == "Increase Resistances")
            {
                XDidYToZ(ParentObject, "alter", null, Target, "resistances to the elements", "!", null, Target, PossessiveObject: true);
            }
            if (ResistanceChoice == "Decrease Resistances")
            {
                Target.GetAngryAt(ParentObject, -100);
                XDidYToZ(ParentObject, "alter", null, Target, "resistances to the elements", "!", null, ParentObject, Target, true, PossessiveObject: true);
            }

            CooldownMyActivatedAbility(ActivateThermokinesisAbilityID, Charges * 3);
        }


        // 
        // 
        // 
        // 
        // 


        public void ChangeAmbientTemperature(string AmbientTempChoices)
        {

            FocusPsi PsiMutation = ParentObject.GetPart<FocusPsi>();

            var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;
            var ParentsLevel = ParentObject.Statistics["Level"].BaseValue;
            var ParentsCharges = ParentObject.Statistics["PsiCharges"].BaseValue;

            string ChargesSpent = PsiMutation.focusPsiCurrentCharges.ToString();
            string RadiusChoice = null;

            MaximumRadius = Math.Min(1 + Level, 8);

            if (!this.ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap())
            {
                if (PsiMutation == null)
                {
                    // AddPlayerMessage("You lack the ability to do this.");
                    string verb1 = "lack";
                    string extra1 = "ability to do this";
                    string termiPun1 = "!";
                    XDidY(ParentObject, verb1, extra1, termiPun1);
                    return;
                }
                if (IsPlayer())
                {
                    ChargesSpent = Popup.AskString("Expend how many charges", "1", 3, 1, "0123456789");
                }
                if (IsPlayer())
                {
                    RadiusChoice = Popup.AskString("Radius of the effect " + "(Maximum Radius: " + MaximumRadius + ")", "1", 3, 1, "0123456789");
                }

                int Charges = Convert.ToInt32(ChargesSpent);
                int Radius = Convert.ToInt32(RadiusChoice);

                if (Radius > MaximumRadius)
                {
                    AddPlayerMessage("The Radius you've chosen is greater than you can yield.");
                    ChargesSpent += Charges;
                    return;
                }
                if (Radius <= 0)
                {
                    AddPlayerMessage("Not a valid input for radius.");
                    ChargesSpent += Charges;
                    return;
                }
                if (!PsiMutation.usePsiCharges(Charges))
                {
                    AddPlayerMessage("You do not have enough psi-charges!");
                    return;
                }
                if (IsPlayer() && Charges <= 0)
                {
                    AddPlayerMessage("That's not a valid amount of charges.");
                    return;
                }
                if (Charges > 1 + ParentsEgo + ParentsLevel / 2 && !ParentObject.HasEffect("Psiburdening"))
                {
                    int fatigueVar = 25;
                    ParentObject.ApplyEffect(new Psiburdening(fatigueVar * Charges));
                }


                int Range = 6 + Level / 2 + ParentsEgo;


                List<Cell> Cells = PickBurst(Radius, Range, false, AllowVis.OnlyExplored);

                if (AmbientTempChoices == "Increase Temperature")
                {
                    foreach (var c in Cells)
                    {
                        c.TemperatureChange(Charges * 100, ParentObject);
                        if (Charges >= 15)
                        {
                            c.LargeFireblast();
                        }
                        GameObject cTarget = c.GetFirstObjectWithPart("Brain");
                        if (cTarget != null)
                            cTarget.GetAngryAt(ParentObject, -100);
                    }
                }
                else if (AmbientTempChoices == "Decrease Temperature")
                {
                    foreach (var c in Cells)
                    {
                        c.TemperatureChange((Charges * 100) * -1, ParentObject);
                        //
                        if (Charges >= 15)
                        {
                            c.AddObject("CryoGas").GetPart<Gas>().Density = 1 * Charges;
                        }
                        GameObject cTarget = c.GetFirstObjectWithPart("Brain");
                        if (cTarget != null)
                            cTarget.GetAngryAt(ParentObject, -100);
                    }
                }


                PlayWorldSound("ambientaltered");
                CooldownMyActivatedAbility(ActivateThermokinesisAbilityID, Charges * 15);
            }
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandAlterTemperatures")
            {
                if (base.IsMyActivatedAbilityUsable(this.ActivateThermokinesisAbilityID))
                {
                    BeginTempAlteration();

                    return false;
                }
            }
            return base.FireEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            this.Unmutate(GO);
            Mutations GainPSiFocus = GO.GetPart<Mutations>();
            if (!GainPSiFocus.HasMutation("FocusPsi"))
            {
                GainPSiFocus.AddMutation("FocusPsi", 1);
                //AddPlayerMessage("Has Focus Psi.");
            }
            this.ActivateThermokinesisAbilityID = base.AddMyActivatedAbility(Name: "Alter Temperatures", Command: "CommandAlterTemperatures", Class: "Mental Mutation", Icon: "*");
            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            base.RemoveMyActivatedAbility(ref this.ActivateThermokinesisAbilityID);
            return base.Unmutate(GO);
        }
    }
}