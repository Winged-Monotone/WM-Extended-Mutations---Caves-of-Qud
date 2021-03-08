
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
    public class Electrokinesis : BaseMutation
    {
        public int Potency = 0;
        public int NewPotency = 0;
        public int EletricCharge = 0;
        public int NewElectricCharge = 0;
        public Guid ActivateShimmeringShroudAbilityID;
        public Guid ChargePotentialAbilityID;

        public Electrokinesis()
        {
            this.DisplayName = "Electrokinesis";
            this.Type = "Mental";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CommandShimmeringShroud");
            Object.RegisterPartEvent(this, "CommandChargePotencyShroud");
            Object.RegisterPartEvent(this, "BeginTakeAction");

            base.Register(Object);
        }

        public override int GetMaxLevel()
        {
            return 9999;
        }

        public override string GetDescription()
        {
            return "You've become a deadly walking battery, take charge of the forces of electricity.";
        }

        public override string GetLevelText(int Level)
        {
            try
            {
                var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;

                return "Create an electric shroud around yourself and manipulate powerful eletric forces dependent on a careful balancing of both eletric charge and potency.\n\n"
              + "Potency towards Resistance Alteration: " + "{{M|" + ParentsEgo + "}}" + " * " + "{{B|Charges}}\n"
              + "Potency towards Shimmering Shroud: " + "{{W|" + 0.1f + "}}" + " * " + "{{B|Potency}}\n\n"
              + "Save Target Vs' Negative Resistance Changes: " + "{{B|" + (10 + ParentsEgo + Level) + "}}";
            }
            catch
            {
                return "Create an electric shroud around yourself and manipulate powerful eletric forces dependent on a careful balancing of both eletric charge and potency.\n\n"
                + "Potency towards Resistance Alteration: \n{{M|Ego}} * {{B|Charges}}\n\n"
                + "Potency towards Shimmering Shroud: \n{{W|0.1}} * {{B|Potency}}\n\n\n"
                + "Save Target Vs' Negative Resistance Changes: \n{{B|10}} + {{M|Ego}} + {{G|Level}}";
            }
        }

        public void ApplyElectricShroud()
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

            // AddPlayerMessage("MethodStep: Applying Effect");

            if (!Target.HasEffect("ShimmeringShroud"))
            {
                Target.ApplyEffect(new ShimmeringShroud(0, Owner: ParentObject));
                ElectricChargePulse(TargetCell, Charges);
                Target.ParticleText("&W\u000f", 0.02f, 10);
                PlayWorldSound("Electroblast" + (Stat.Random(1, 4)));
                Event cE = Event.New("AlteringTemperatureEffectEvent", "ChargesSpent", Charges, "Potency", Potency, "Caster", ParentObject, "MutationLevel", Level);

                // AddPlayerMessage("MethodStep: FireEvent CE");

                Target.FireEvent(cE);
            }
            else
            {
                ElectricChargePulse(TargetCell, Charges);
                Target.ParticleText("&W\u000f", 0.02f, 10);
                PlayWorldSound("Electroblast" + (Stat.Random(1, 4)));
                Event cE = Event.New("AlteringTemperatureEffectEvent", "ChargesSpent", Charges, "Potency", Potency, "Caster", ParentObject, "MutationLevel", Level);

                // AddPlayerMessage("MethodStep: FireEvent CE/ No effect");

                Target.FireEvent(cE);
            }

            // AddPlayerMessage("MethodStep: Setting Flavour Text");


            XDidYToZ(Target, "dawn", null, Target, "shimmering shroud", "!", null, Target, PossessiveObject: true);
            CooldownMyActivatedAbility(ActivateShimmeringShroudAbilityID, Charges * 3);
        }

        public void ElectricChargePulse(Cell TargetCell, int Charges)
        {
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 3 + Charges; j++)
                {
                    TargetCell.ParticleText("&W" + (char)Stat.RandomCosmetic(191, 198), 2.9f, 5 + Charges);
                }
                for (int k = 0; k < 2 + Charges; k++)
                {
                    TargetCell.ParticleText("&Y" + (char)Stat.RandomCosmetic(191, 198), 3.9f, 5 + Charges);
                }
                for (int l = 0; l < 4 + Charges; l++)
                {
                    TargetCell.ParticleText("&B" + (char)Stat.RandomCosmetic(191, 198), 4.9f, 5 + Charges);
                }
            }
        }

        public void PotencyChargeEffectPulse(int Potency)
        {
            AddPlayerMessage("Its Working");
            int ParticlesCount = Potency;

            if (Potency >= 21)
            {
                ParticlesCount = 20;
            }


            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 1 + (ParticlesCount); j++)
                {
                    ReverseParticleText("&W" + (char)Stat.RandomCosmetic(191, 198), 2.9f, (1 + ParticlesCount));
                }
                for (int k = 0; k < 1 + (ParticlesCount); k++)
                {
                    ReverseParticleText("&Y" + (char)Stat.RandomCosmetic(191, 198), 3.9f, (1 + ParticlesCount));
                }
                for (int l = 0; l < 1 + (ParticlesCount); l++)
                {
                    ReverseParticleText("&B" + (char)Stat.RandomCosmetic(191, 198), 4.9f, (1 + ParticlesCount));
                }
            }
        }

        public void ReverseParticleText(string Text, float Velocity, int Life)
        {
            Zone ParentZone = ParentObject.CurrentZone;
            if (ParentZone == XRLCore.Core.Game.ZoneManager.ActiveZone)
            {
                int X = ParentObject.CurrentCell.X;
                int Y = ParentObject.CurrentCell.Y;
                float num = (float)Stat.Random(0, 359) / 58f;
                float YVelocity = (float)Math.Sin(num) / 4f;
                float XVelocity = (float)Math.Cos(num) / 4f;

                YVelocity *= Velocity;
                XVelocity *= Velocity;

                // YVelocity = 0.5421961f * Velocity;
                // XVelocity = 0.4812487f * Velocity;

                int DeltaX = (int)(XVelocity * Life);
                int DeltaY = (int)(YVelocity * Life);

                // int DeltaX = 6;
                // int DeltaY = 6;

                X = X + DeltaX;
                Y = Y + DeltaY;

                if (X >= 80 || X < 0 || Y >= 25 || Y < 0)
                {
                    if (X >= 80 && ParentObject.CurrentCell.X != 79)
                    { X = 79; }

                    else if (X < 0 && ParentObject.CurrentCell.X != 0)
                    { X = 0; }

                    if (Y >= 25 && ParentObject.CurrentCell.Y != 24)
                    { Y = 24; }

                    else if (Y < 0 && ParentObject.CurrentCell.Y != 0)
                    { Y = 0; }

                    YVelocity = ParentObject.CurrentCell.Y - Y;
                    XVelocity = ParentObject.CurrentCell.X - X;

                    float Magnitude = (float)Math.Sqrt((XVelocity * XVelocity) + (YVelocity * YVelocity));

                    // XVelocity = -((Life / Magnitude) * (float)XVelocity);
                    // YVelocity = -((Life / Magnitude) * (float)YVelocity);

                    XVelocity = -(XVelocity / Life);
                    YVelocity = -(YVelocity / Life);

                    // AddPlayerMessage("X: " + X);
                    // AddPlayerMessage("Y: " + Y);
                    // AddPlayerMessage("Xvel: " + XVelocity);
                    // AddPlayerMessage("Yvel: " + YVelocity);
                    // AddPlayerMessage("Mag: " + Magnitude);
                    // AddPlayerMessage("Potency: " + Potency);

                }




                // XRLCore.ParticleManager.Add(Text, X, Y, -YVelocity, -XVelocity, Life, 0f, 0f);
                XRLCore.ParticleManager.Add(Text, X, Y, -XVelocity, -YVelocity, Life, 0f, 0f);

                // AddPlayerMessage("DeltaX : " + DeltaX);
                // AddPlayerMessage("DeltaY : " + DeltaY);
                // AddPlayerMessage("Velocity : " + Velocity);
                // AddPlayerMessage("YVelocity : " + YVelocity);
                // AddPlayerMessage("XVelocity : " + XVelocity);
                // AddPlayerMessage("Life : " + Life);
            }
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandShimmeringShroud")
            {
                ApplyElectricShroud();
            }
            else if (E.ID == "CommandChargePotencyShroud")
            {
                ToggleMyActivatedAbility(ChargePotentialAbilityID);
            }
            else if (E.ID == "BeginTakeAction")
            {
                if (IsMyActivatedAbilityToggledOn(ChargePotentialAbilityID, ParentObject))
                {
                    ++Potency;
                    PotencyChargeEffectPulse(Potency);
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
            }
            this.ActivateShimmeringShroudAbilityID = base.AddMyActivatedAbility("Activate Shroud", "CommandShimmeringShroud", "Mental Mutation", null, "*", null, false, false, false, false, false);
            this.ChargePotentialAbilityID = base.AddMyActivatedAbility("Charge Potential", "CommandChargePotencyShroud", "Mental Mutation", null, "*", null, true, false, true, false, false);

            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            base.RemoveMyActivatedAbility(ref this.ActivateShimmeringShroudAbilityID);
            return base.Unmutate(GO);
        }

    }
}
