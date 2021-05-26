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

        public int PsiCounter = 0;
        public bool isCharging;
        public int turnsTilPsiDecay;
        public int effectiveSaveTarget;
        public int ArmCounter = 0;
        public int ArmCost;
        public int NewArmCost;
        public Guid PsiFocusActivatedAbilityID = Guid.Empty;
        public const string VERB_1 = "begin to gather";
        public const string VERB_2 = "stop gathering";
        public const string VERB_3 = "are gathering";
        public const string VERB_4 = "cannot accumulate anymore";
        public const string EXTRA_1 = "psionic energy";
        public const string TERMPUNC_1 = "!";

        public int focusPsiCurrentCharges
        {
            get
            {
                return GetPsiCharges(ParentObject)?.Value ?? 0;
            }
            set
            {
                PsiCounter = 0;

                Statistic Charges = GetPsiCharges(ParentObject);
                int Max = Charges.BaseValue;
                int difference = value - Max;
                if (difference >= 0)
                {
                    Charges.Penalty = 0;
                }
                else
                {
                    Charges.Penalty = -difference;
                }
            }
        }
        public int focusPsiCurrentMaximumCharges
        {
            get
            {
                return GetPsiCharges(ParentObject)?.BaseValue ?? 0;
            }
            set
            {
                Statistic Charges = GetPsiCharges(ParentObject);
                Charges.BaseValue = value;
            }
        }

        public FocusPsi()
        {
            this.DisplayName = "Focus Psi";
            turnsTilPsiDecay = 0;
            effectiveSaveTarget = 15;
            isCharging = false;
        }
        //method gets charge descriptor to render for player and shows current charges
        public override bool CanLevel()
        {
            return false;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == GetDisplayNameEvent.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            try
            {
                UpdateCharges();
            }
            catch
            {
                UpdateCharges();
            }
            return true;
        }









        public void UpdateCharges()
        {
            focusPsiCurrentMaximumCharges = maximumPsiCharge();
            var AA = MyActivatedAbility(this.PsiFocusActivatedAbilityID);
            if (AA != null)
            {
                AA.DisplayName = "Psi {{purple|(" + (focusPsiCurrentCharges) + "/" + focusPsiCurrentMaximumCharges + " charges)}}";
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
            + "Activating Focus Psi will allow you to gather psionic energy into {{light blue|Charges}} which can be utilized by other psionic abilities.\n"
            + "\n"
            + "While Focus Psi is active, you will have a {{purple|'charging psi'}} effect in your status. You generate psi Charges equal to ({{light blue|(1)}} + your [Ego Modifier]), per turn. Your maximum psi charges is calculated by your ego-modifer, level and toughness modifer.\n"
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
                UpdateCharges();
                return true;
            }
            return false;
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
                if (NewArmCost <= focusPsiCurrentMaximumCharges)
                {
                    UpdateCharges();
                    ArmCounter += 1;
                }
                else if (NewArmCost > focusPsiCurrentMaximumCharges)
                {
                    UpdateCharges();
                    return true;
                }
            }

            if (E.ID == "CommandDismissLimb")
            {
                if (ArmCounter >= 1)
                {
                    ArmCounter -= 1;
                }
                UpdateCharges();
            }

            if (E.ID == "AIGetPassiveMutationList")
            {
                // AddPlayerMessage("Hey prepare, to eat my combustion blast.");
                // AddPlayerMessage($"Currentcharges: {focusPsiCurrentCharges}");
                // AddPlayerMessage($"MaximumPsi: {maximumPsiCharge()}");
                if (focusPsiCurrentCharges < focusPsiCurrentMaximumCharges / 2 && !HandlingCharging())
                {
                    E.AddAICommand("CommandFocusPsi");
                    // AddPlayerMessage($"Currentcharges: {focusPsiCurrentCharges}");
                    AddPlayerMessage("Something is gathering psionic energy ...");
                }
                else if (focusPsiCurrentCharges >= focusPsiCurrentMaximumCharges - 1 && HandlingCharging())
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
                UpdateCharges();
                return false;
            }

            else if (E.ID == "EndTurn")
            {
                Psybrachiomancy BrachMutation = ParentObject.GetPart<Psybrachiomancy>();
                var PsiburdeningCatch = ParentObject.GetEffect<Psiburdening>();
                if (base.IsMyActivatedAbilityToggledOn(this.PsiFocusActivatedAbilityID) && (focusPsiCurrentCharges < focusPsiCurrentMaximumCharges))
                {
                    int chanceforpsi = (ParentObject.StatMod("Willpower") * 3) + PsiCounter;
                    if (Stat.Random(1, 100) < chanceforpsi)
                    {
                        focusPsiCurrentCharges++;
                        DidX("charge", "psi energy", ".", ColorAsGoodFor: ParentObject);
                    }
                    else
                    {
                        PsiCounter++;
                    }
                    turnsTilPsiDecay++;

                }
                else if (ParentObject.HasPart("Psybrachiomancy") && BrachMutation.ArmCounter <= Math.Min(1, ParentObject.StatMod("Willpower")) && ParentObject.HasEffect("Psiburdening"))
                {
                    ParentObject.RemoveEffect(PsiburdeningCatch);
                }

                UpdateCharges();

            }

            else if (E.ID == "PsionicDecay")
            {
                if (!(ParentObject.MakeSave("Willpower", effectiveSaveTarget, null, null, "Psionic Decay")))
                {
                    focusPsiCurrentCharges -= E.GetIntParameter("Amount", (int)0); // Current default value is zero, change default value to something more appropriate later
                }

                UpdateCharges();
            }

            else if (E.ID == "FireEventDebuffSystem")
            {
                Psybrachiomancy BrachMutation = ParentObject.GetPart<Psybrachiomancy>();
                if (BrachMutation.ArmCounter > ParentObject.StatMod("Willpower") + BrachMutation.Level / 2 && !ParentObject.HasEffect("Psiburdening"))
                {
                    ParentObject.ApplyEffect(new Psiburdening((Stat.Random(10, 20) - ParentObject.StatMod("Willpower")) * Stat.Random(50, 125)));
                }
                else if (BrachMutation.ArmCounter < ParentObject.StatMod("Willpower") && ParentObject.HasEffect("Psiburdening"))
                {
                    ParentObject.RemoveEffect("Psiburdening");
                }
            }

            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            UpdateCharges();
            return base.ChangeLevel(NewLevel);
        }


        public Statistic GetPsiCharges(GameObject GO)
        {
            Statistic Result;
            {
                if (GO.Statistics.TryGetValue("PsiCharges", out Result))
                {

                    return Result;

                }

                Result = new Statistic("PsiCharges", 0, 9999, maximumPsiCharge(), GO);
            }
            GO.Statistics.Add("PsiCharges", Result);
            return Result;
        }


        public override bool Mutate(GameObject GO, int Level)
        {
            this.PsiFocusActivatedAbilityID = base.AddMyActivatedAbility("Focus Psi", "CommandFocusPsi", "Mental Mutation", "Focus your psionic energy, channel it to manifest other psionic abilities that require Psi Charges.", "*", null, false, false, true);

            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }







        public override bool Unmutate(GameObject GO)
        {
            if (this.PsiFocusActivatedAbilityID != Guid.Empty)
            {
                base.RemoveMyActivatedAbility(ref this.PsiFocusActivatedAbilityID);
            }
            return base.Unmutate(GO);
        }
    }
}