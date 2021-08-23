
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

using XRL.World.Parts;
using XRL.World;
using XRL.World.Effects;
using XRL.World.AI.GoalHandlers;
using XRL.World.Parts.Mutation;
using XRL.World.Capabilities;

using XRL.Core;
using XRL.Rules;
using XRL.Messages;
using XRL.UI;

using UnityEngine;
using AiUnity.NLog.Core.Targets;
using HarmonyLib;
using ConsoleLib.Console;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Psybrachiomancy : BaseMutation
    {
        public int ArmCounter = 0;
        public int ArmCost;
        public int NewArmCost = 0;
        // private bool PsionicArmActive = false;
        public string ManagerID => ParentObject.id + "::Psybrachiomancy";
        public Guid ActivatedAbilityID = Guid.Empty;
        public Psybrachiomancy()
        {
            this.DisplayName = "Psybrachiomancy";
            this.Type = "Mental";
        }
        public override bool ChangeLevel(int NewLevel)
        {
            // XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("highly entropic beings", 100, false);
            return base.ChangeLevel(NewLevel);
        }
        public override bool CanLevel()
        {
            return false;
        }

        public string ArmCounterStrings()
        {
            string ArmCounterStringConversion = ArmCounter.ToString();
            return ArmCounterStringConversion;
        }
        public override string GetDescription()
        {
            return "You can manifest psionic arm's with your thoughtstuff.\n"
                    + "\n{{cyan|+100 Reputation with}} {{cyan|highly entropic beings.}}";
        }
        public override string GetLevelText(int Level)
        {
            return "{{gray|You can weave psionic arms into reality, they function like any other arm, but dissipate entirely upon being dismembered.}}";
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == AttackerDealingDamageEvent.ID;
        }

        // public override bool HandleEvent(AttackerDealingDamageEvent E)
        // {
        //     var EgoSum = ParentObject.Statistics["Ego"].Modifier;
        //     var Attacker = E.Actor;
        //     var Defender = E.Object;
        //     var GetEquippedPsionicLimb = E.Weapon?.EquippedOn();



        //     AddPlayerMessage("Attacker : " + Attacker.DisplayName);

        //     AddPlayerMessage("Defender : " + Defender.DisplayName);

        //     // AddPlayerMessage("LimbAttacking? : " + GetEquippedPsionicLimb.Description);


        //     if (E.Actor == ParentObject && (GetEquippedPsionicLimb?.VariantType == "Psionic Hands") && E.Projectile == null)
        //     {
        //         AddPlayerMessage("Firing Damage reduction for multiple arms.");
        //         var aDamage = E.Damage.Amount;
        //         E.Damage.Amount = (aDamage / ArmCounter) + EgoSum + Stat.Random(1, EgoSum);
        //     }
        //     return base.HandleEvent(E);
        // } 


        public override bool Mutate(GameObject GO, int Level)
        {
            // string PsybrachiomancyinfoSource = "{ \"Psybrachiomancy\": [\"*cult*, the Asuran\", \"Many-Armed *cult*\"] }";
            // SimpleJSON.JSONNode PsybrachiomancyInfo = SimpleJSON.JSON.Parse(PsybrachiomancyinfoSource);

            // WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", PsybrachiomancyInfo["Psybrachiomancy"]);

            Mutations GainPSiFocus = GO.GetPart<Mutations>();
            if (!GainPSiFocus.HasMutation("FocusPsi"))
            {
                GainPSiFocus.AddMutation("FocusPsi", 1);
            }
            this.ActivatedAbilityID = base.AddMyActivatedAbility("Manifest Limb", "CommandManifestLimb", "Mental Mutation", "Manifest a psychic arm.\n\n Instead of using strength for penetration, a weapon’s penetration when wielded with a psionic arm is limited to your Ego modifier and the weapons’ penetration value.\n\n"
            + "(Dismembered psionic arms may not dissapate and may require re-weaving to repair, no damage is taken from blows that sever psionic limbs.)\n"
            + "If the set of arms you summon are more than your Willpower Modifier, you will be given the 'psi-exhaustion' effect temporarily.", ">", null, false, false, false, false, false, false, false, 10, null);
            this.ActivatedAbilityID = base.AddMyActivatedAbility("Dismiss Limb", "CommandDismissLimb", "Mental Mutation", "Dismiss a psychic arm.\n\n", "<", null, false, false, false, false, false, false, false, 10, null);
            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }

        public void AddPsionicArms()
        {
            Body SourceBody = ParentObject.GetPart("Body") as Body;
            if (SourceBody != null)
            {
                BodyPart ReadyBody = SourceBody.GetBody();
                BodyPart AttatchArmTemplate = ReadyBody.AddPartAt("Psionic Arm", 2, null, null, null, null, ManagerID + ArmCounterStrings(), 17, null, null, null, null, null, null, null, null, null, null, null, "Arm", new string[4]
                {
                "Hands",
                "Feet",
                "Roots",
                "Thrown Weapon"
                });
                AttatchArmTemplate.AddPart("Psionic Hand", 2, null, "Psionic Hands", null, null, ManagerID + ArmCounterStrings(), 17);
                ReadyBody.AddPartAt(AttatchArmTemplate, "Psionic Arm", 1, null, null, null, null, ManagerID + ArmCounterStrings(), 17).AddPart("Psionic Hand", 1, null, "Psionic Hands", null, null, ManagerID + ArmCounterStrings(), 17);
                ReadyBody.AddPartAt("Psionic Hands", 0, null, null, "Psionic Hands", null, ManagerID + ArmCounterStrings(), 17, null, null, null, null, null, null, null, null, null, null, null, "Hands", new string[3]
                {
                "Feet",
                "Roots",
                "Thrown Weapon"
                });
                ReadyBody.AddPartAt("Missile Weapon", Laterality.RIGHT, null, null, "Psionic Hands", null, ManagerID + ArmCounterStrings(), Category: 17, null, null, null, null, null, null, null, null, null, null, null, "Hands", new string[1]
                {
                "Missile Weapon"
                });
                ReadyBody.AddPartAt("Missile Weapon", Laterality.LEFT, null, null, "Psionic Hands", null, ManagerID + ArmCounterStrings(), Category: 17, null, null, null, null, null, null, null, null, null, null, null, "Hands", new string[1]
                {
                "Missile Weapon"
                });
            }
        }

        public void RemovePsionicArms()
        {
            ParentObject.RemoveBodyPartsByManager(ManagerID + ArmCounterStrings(), EvenIfDismembered: true);
        }
        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart)this, "EndTurn");
            Object.RegisterPartEvent(this, "Dismember");
            Object.RegisterPartEvent(this, "CommandManifestLimb");
            Object.RegisterPartEvent(this, "CommandDismissLimb");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandManifestLimb")
            {
                FocusPsi focusPsi = ParentObject.GetPart<FocusPsi>();
                if (NewArmCost <= ParentObject.Statistics["PsiCharges"].BaseValue)
                {
                    ArmCost = (2 + ArmCounter) + (ArmCounter * NewArmCost) - 1;
                    NewArmCost = ArmCost;
                    ArmCounter += 1;
                    focusPsi.focusPsiCurrentCharges = focusPsi.maximumPsiCharge();
                    AddPsionicArms();
                    AddPlayerMessage(ParentObject.It + " manifest psionic limbs.");
                    UseEnergy(500);
                    focusPsi.UpdateCharges();
                    ParentObject.FireEvent(Event.New("FireEventDebuffSystem", 0, 0, 0));
                }
                else if (NewArmCost <= ParentObject.Statistics["PsiCharges"].BaseValue || ParentObject.Statistics["PsiCharges"].BaseValue <= 0)
                {
                    ArmCost = (2 + ArmCounter) + (ArmCounter * NewArmCost) - 1;
                    AddPlayerMessage(ParentObject.It + " do not have enough {{red|maximum charges}} to materialize a new limb.");
                    return true;
                }
            }
            if (E.ID == "CommandDismissLimb")
            {
                FocusPsi focusPsi = ParentObject.GetPart<FocusPsi>();
                if (ArmCounter >= 1)
                {
                    focusPsi.UpdateCharges();
                    RemovePsionicArms();
                    AddPlayerMessage(ParentObject.It + " dismiss " + ParentObject.its + " psionic arms.");
                    ArmCounter -= 1;
                    ParentObject.FireEvent(Event.New("FireEventDebuffSystem", 0, 0, 0));
                }
            }
            if (E.ID == "Dismember")
            {
                if (E.HasStringParameter("Psionic") || E.HasIntParameter("17"))
                {
                    AddPlayerMessage("{{Red|A blow disrupts the tangibility of one of " + ParentObject.its + " arms, it dissipates!}}");
                    string ArmInQuestion = E.GetStringParameter("LimbSourceGameObjectID");
                    ParentObject.RemoveBodyPartsByManager(ArmInQuestion, EvenIfDismembered: true);
                }
            }
            if (E.ID == "EndTurn")
            {
                FocusPsi focusPsi = ParentObject.GetPart<FocusPsi>();
                focusPsi.UpdateCharges();
                // AddPlayerMessage("ArmCounter: " + ArmCounter);
                // AddPlayerMessage("CurrentID: " + ManagerID + ArmCounter);
                // AddPlayerMessage("ArmCost: " + ArmCost);
                // AddPlayerMessage("PsiMaximum: " + focusPsi.maximumPsiCharge());
                // AddPlayerMessage("PsiArmCounter: " + focusPsi.ArmCounter);
                // AddPlayerMessage("PsiArmcost: " + focusPsi.ArmCost);
            }
            return base.FireEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
    }
}

