using System;
using System.Collections.Generic;
using System.Threading;
using ConsoleLib.Console;
using XRL.Core;
using XRL.Rules;
using XRL.UI;
using XRL.World.AI.GoalHandlers;
using XRL.World.Effects;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class FocusPsi : BaseMutation
    {
        public const string VERB_1 = "begin to gather";
        public const string VERB_2 = "stop gathering";
        public const string VERB_3 = "are gathering";
        public const string VERB_4 = "cannot accumulate anymore";
        public const string EXTRA_1 = "psionic energy";
        public const string TERMPUNC_1 = "!";
        public int focusPsiCurrentCharges;
        public bool isCharging;
        public int turnsTilPsiDecay;
        public int effectiveSaveTarget;
        public int ArmCounter = 0;
        public int ArmCost;
        public int NewArmCost;
        public Guid PsiFocusActivatedAbilityID = Guid.Empty;

        public FocusPsi()
        {
            this.DisplayName = "Focus Psi";
            turnsTilPsiDecay = 0;
            effectiveSaveTarget = 15;
            isCharging = false;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == GetShortDisplayNameEvent.ID;
        }

        public override bool HandleEvent(GetShortDisplayNameEvent E)
        {
            try
            {
                MyActivatedAbility(this.PsiFocusActivatedAbilityID).DisplayName
                = "Psi (" + (this.focusPsiCurrentCharges) + "/" + Math.Max(0, (maximumPsiCharge())) + " charges)";
            }
            catch
            {
                MyActivatedAbility(this.PsiFocusActivatedAbilityID).DisplayName
                = "Psi (" + (this.focusPsiCurrentCharges) + "/" + Math.Max(0, (maximumPsiCharge())) + " charges)";
            }
            return true;
        }

        //method gets charge descriptor to render for player and shows current charges
        public override bool CanLevel()
        {
            return false;
        }
        public void DisplayCurrentCharges()
        {
            if (ArmCounter >= 1)
            {
                try
                {
                    MyActivatedAbility(this.PsiFocusActivatedAbilityID).DisplayName
                    = "Psi (" + (this.focusPsiCurrentCharges) + "/" + Math.Max(0, (maximumPsiCharge())) + " charges)";
                }
                catch
                {
                    MyActivatedAbility(this.PsiFocusActivatedAbilityID).DisplayName
                = "Psi (" + (this.focusPsiCurrentCharges) + "/" + Math.Max(0, (maximumPsiCharge())) + " charges)";
                }
            }
            else
            {
                MyActivatedAbility(this.PsiFocusActivatedAbilityID).DisplayName
            = "Psi (" + (this.focusPsiCurrentCharges) + "/" + Math.Max(0, (maximumPsiCharge())) + " charges)";
            }
        }

        public override string GetDescription()
        {
            return "Focus your psionic power to channel into psionic abilities.";
        }
        public override string GetLevelText(int Level)
        {
            return "You have the ability to focus on your psionic energy and manifest it into powerful effects:\n"
            + "\n"
            + "Activating Focus Psi will allow you to gather psionic energy into {{lightblue|Charges}} which can be utilized by other psionic abilities.\n"
            + "\n"
            + "While Focus Psi is active, you will have a {{purple|'charging psi'}} effect in your status. You generate psi Charges equal to ((1) + your [Ego Modifier]), per turn. Your maximum psi charges is calculated by your ego-modifer, level and toughness modifer.\n"
            + "\n"
            + "After charging your psionic energy, charges will decay at an amount determined by your Willpower modifier.";
        }

        public bool HandlingCharging()
        {
            if (isCharging)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        //allows combustion blast and other mutations to check if the player is charged psionically and has a psi point available
        public bool usePsiCharges(int psiabilitycost)
        {
            if (psiabilitycost <= focusPsiCurrentCharges)
            {
                focusPsiCurrentCharges -= psiabilitycost;
                DisplayCurrentCharges();
                return true;
            }
            return false;
        }

        public int focusPsiCharges()
        {
            return (Stat.Random(0, 2));
        }

        // public void FocusPsiDeficiency()
        // {
        //     if ((maximumPsiCharge() < 0) && !ParentObject.HasEffect("Psiburdening"))
        //     {
        //         ParentObject.ApplyEffect(new Psiburdening(9999));
        //     }
        //     else if ((maximumPsiCharge() > 0) && ParentObject.HasEffect("Psiburdening"))
        //     {
        //         ParentObject.RemoveEffect("Psiburdening");
        //     }
        // }

        // maximum psi an entity can hold
        public int maximumPsiCharge(int Modifier1 = 0)
        {
            try
            {
                Mutations GetMutations = ParentObject.GetPart<Mutations>();
                Psybrachiomancy BrachMutation = ParentObject.GetPart<Psybrachiomancy>();
                ArmCost = (2 + BrachMutation.ArmCounter) + (2 * BrachMutation.ArmCounter) - 1;
                NewArmCost = ArmCost;
                int differenceSum = NewArmCost;
                if (GetMutations.HasMutation("Psybrachiomancy") && ArmCounter >= 1)
                {
                    Dictionary<string, Statistic> Stats = ParentObject.Statistics;
                    float Result = 1 + Stats["Ego"].Modifier;
                    Result += Stats["Toughness"].Modifier / 2f;
                    Result += (Stats["Level"].Value * 0.1f) * Stats["Ego"].Modifier;
                    if (Result < 0f) return 0;
                    return (int)Math.Round(Result) - (differenceSum);
                }
                else
                {
                    Dictionary<string, Statistic> Stats = ParentObject.Statistics;
                    float Result = 1 + Stats["Ego"].Modifier;
                    Result += Stats["Toughness"].Modifier / 2f;
                    Result += (Stats["Level"].Value * 0.1f) * Stats["Ego"].Modifier;
                    if (Result < 0f) return 0;
                    return (int)Math.Round(Result);
                }
            }
            catch
            {
                Dictionary<string, Statistic> Stats = ParentObject.Statistics;
                float Result = 1 + Stats["Ego"].Modifier;
                Result += Stats["Toughness"].Modifier / 2f;
                Result += (Stats["Level"].Value * 0.1f) * Stats["Ego"].Modifier;
                if (Result < 0f) return 0;
                return (int)Math.Round(Result);
            }
        }

        public int FinalizeMaximumCharges()
        {
            Psybrachiomancy BrachMutation = ParentObject.GetPart<Psybrachiomancy>();
            int BrachMath = (2 + (BrachMutation.ArmCounter * 2));
            int differenceSum = -BrachMath;
            return maximumPsiCharge() - differenceSum;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CommandFocusPsi");
            Object.RegisterPartEvent(this, "PsionicDecay");
            Object.RegisterPartEvent(this, "EndTurn");
            Object.RegisterPartEvent(this, "LeaveCell");
            Object.RegisterPartEvent(this, "CanChangeMovementMode");
            Object.RegisterPartEvent(this, "MovementModeChanged");
            Object.RegisterPartEvent(this, "AIGetPassiveMutationList");
            Object.RegisterPartEvent(this, "FireEventDebuffSystem");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandManifestLimb")
            {
                ArmCost = (2 + ArmCounter) + (2 * ArmCounter) - 1;
                NewArmCost = ArmCost;
                if (NewArmCost <= maximumPsiCharge())
                {
                    DisplayCurrentCharges();
                    ArmCounter += 1;
                }
                else if (NewArmCost > maximumPsiCharge())
                {
                    DisplayCurrentCharges();
                    return true;
                }
            }

            if (E.ID == "CommandDismissLimb")
            {
                if (ArmCounter >= 1)
                {
                    ArmCounter -= 1;
                }
                DisplayCurrentCharges();
            }

            if (E.ID == "AIGetPassiveMutationList")
            {
                // AddPlayerMessage("Hey prepare, to eat my combustion blast.");
                // AddPlayerMessage($"Currentcharges: {focusPsiCurrentCharges}");
                // AddPlayerMessage($"MaximumPsi: {maximumPsiCharge()}");
                if (focusPsiCurrentCharges < Math.Max(0, (maximumPsiCharge()) / 2) && !HandlingCharging())
                {
                    E.AddAICommand("CommandFocusPsi");
                    // AddPlayerMessage($"Currentcharges: {focusPsiCurrentCharges}");
                    AddPlayerMessage("Something is gathering psionic energy ...");
                }
                else if (focusPsiCurrentCharges >= Math.Max(0, (maximumPsiCharge()) - 1) && HandlingCharging())
                {
                    E.AddAICommand("CommandFocusPsi");
                    // AddPlayerMessage($"Currentcharges: {focusPsiCurrentCharges}");
                    AddPlayerMessage("Something is gathering psionic energy ...");
                }
            }

            if (E.ID == "CommandFocusPsi")
            {
                if (!base.IsMyActivatedAbilityToggledOn(this.PsiFocusActivatedAbilityID))
                {
                    base.ToggleMyActivatedAbility(this.PsiFocusActivatedAbilityID);
                    string verb1 = "begin to gather";
                    string extra1 = "psionic energy";
                    string TermiP1 = ".";
                    XDidY(ParentObject, verb1, extra1, TermiP1);
                    isCharging = true;
                }
                else
                {
                    base.ToggleMyActivatedAbility(this.PsiFocusActivatedAbilityID);
                    string verb2 = "stop";
                    string extra2 = "gathering psi energy";
                    string termiPun2 = ".";
                    XDidY(ParentObject, verb2, extra2, termiPun2);
                    isCharging = false;
                    UseEnergy(1000);
                }
                DisplayCurrentCharges();
                return false;
            }

            else if (E.ID == "EndTurn")
            {
                try
                {
                    Psybrachiomancy BrachMutation = ParentObject.GetPart<Psybrachiomancy>();
                    var PsiburdeningCatch = ParentObject.GetEffect<Psiburdening>();
                    if (base.IsMyActivatedAbilityToggledOn(this.PsiFocusActivatedAbilityID) && focusPsiCurrentCharges < Math.Max(0, (maximumPsiCharge())))
                    {
                        focusPsiCurrentCharges = Math.Max(focusPsiCurrentCharges + focusPsiCharges(), (Math.Max(0, (maximumPsiCharge()))));
                        turnsTilPsiDecay++;
                        // string verb3 = "are";
                        // string extra3 = "charging psi energy";
                        // string termiPun3 = ".";
                        AddPlayerMessage(ParentObject.Is + " charging psi energy.");
                    }
                    else if (BrachMutation.ArmCounter <= Math.Min(1, ParentObject.StatMod("Willpower")) && ParentObject.HasEffect("Psiburdening"))
                    {
                        ParentObject.RemoveEffect(PsiburdeningCatch);
                    }

                    //  AddPlayerMessage(maximumPsiCharge().ToString());
                    //  AddPlayerMessage(focusPsiCurrentCharges.ToString());

                    DisplayCurrentCharges();
                }
                catch
                {

                }

                return true;
            }

            else if (E.ID == "PsionicDecay")
            {
                if (!(ParentObject.MakeSave("Willpower", effectiveSaveTarget, null, null, "Psionic Decay")))
                {
                    focusPsiCurrentCharges -= E.GetIntParameter("Amount", (int)0); // Current default value is zero, change default value to something more appropriate later
                }
                DisplayCurrentCharges();
            }

            else if (E.ID == "FireEventDebuffSystem")
            {
                Psybrachiomancy BrachMutation = ParentObject.GetPart<Psybrachiomancy>();
                if (BrachMutation.ArmCounter > ParentObject.StatMod("Willpower") && !ParentObject.HasEffect("Psiburdening"))
                {
                    ParentObject.ApplyEffect(new Psiburdening((Stat.Random(10, 20) - ParentObject.StatMod("Willpower")) * Stat.Random(50, 125)));
                }
            }

            return true;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            DisplayCurrentCharges();
            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            focusPsiCurrentCharges += focusPsiCharges();
            this.PsiFocusActivatedAbilityID = base.AddMyActivatedAbility("Focus Psi", "CommandFocusPsi", "Mental Mutation", "Focus your psionic energy, channel it to manifest other psionic abilities that require Psi Charges.", "*", null, false, false, true);
            this.ChangeLevel(Level);
            return true;
        }

        public override bool Unmutate(GameObject GO)
        {
            if (this.PsiFocusActivatedAbilityID != Guid.Empty)
            {
                base.RemoveMyActivatedAbility(ref this.PsiFocusActivatedAbilityID);
            }
            return true;
        }



    }
}