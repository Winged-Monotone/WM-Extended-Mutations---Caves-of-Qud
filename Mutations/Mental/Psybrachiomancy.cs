
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
        public override bool HandleEvent(AttackerDealingDamageEvent E)
        {
            var EgoSum = ParentObject.Statistics["Ego"].Modifier;
            var Attacker = E.Actor;
            var Defender = E.Object;
            var GetEquippedPsionicLimb = ParentObject.Body?.FindDefaultOrEquippedItem(E.Weapon);

            // AddPlayerMessage("Attacker : " + Attacker.DisplayName);
            // AddPlayerMessage("Defender : " + Defender.DisplayName);
            // AddPlayerMessage("LimbAttacking? : " + GetEquippedPsionicLimb.Description);
            // AddPlayerMessage("Limb Name : " + GetEquippedPsionicLimb?.Name);
            // AddPlayerMessage("Limb VariantType : " + GetEquippedPsionicLimb?.VariantType);
            // AddPlayerMessage("Limb Type : " + GetEquippedPsionicLimb?.Type);
            // AddPlayerMessage("Limb null: " + (GetEquippedPsionicLimb == null));
            // AddPlayerMessage("Weapon Event Var: " + E.Weapon);
            // AddPlayerMessage("Weapon Equipped: " + E.Weapon?.Equipped);
            // AddPlayerMessage("Weapon in BodyList: " + ParentObject.Body?.FindDefaultOrEquippedItem(E.Weapon));

            if (E.Actor == ParentObject && (GetEquippedPsionicLimb?.VariantType == "Psionic Hand") && E.Projectile == null)
            {
                // AddPlayerMessage("Firing Damage reduction for multiple arms.");
                var aDamage = E.Damage.Amount;
                E.Damage.Amount = (aDamage / ArmCounter) + EgoSum + Stat.Random(1, EgoSum);
            }
            return base.HandleEvent(E);
        }

        public override bool Mutate(GameObject GO, int Level)
        {


            Mutations GainPSiFocus = GO.GetPart<Mutations>();
            if (!GainPSiFocus.HasMutation("FocusPsi"))
            {
                GainPSiFocus.AddMutation("FocusPsi", 1);
            }
            this.ActivatedAbilityID = base.AddMyActivatedAbility(Name: "Manifest Limb", Command: "CommandManifestLimb", Class: "Mental Mutation", Description: "Manifest a psychic arm.\n\n Instead of using strength for penetration, a weapon’s penetration when wielded with a psionic arm is limited to your Ego modifier and the weapons’ penetration value.\n\n"
            + "(Dismembered psionic arms may not dissapate and may require re-weaving to repair, no damage is taken from blows that sever psionic limbs.)\n"
            + "If the set of arms you summon are more than your Willpower Modifier, you will be given the 'psi-exhaustion' effect temporarily.", Icon: ">", Cooldown: 10);
            this.ActivatedAbilityID = base.AddMyActivatedAbility(Name: "Dismiss Limb", Command: "CommandDismissLimb", Class: "Mental Mutation", Description: "Dismiss a psychic arm.\n\n", Icon: "<", Cooldown: 10);
            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }

        public void AddPsionicArms()
        {
            Body SourceBody = ParentObject.GetPart("Body") as Body;
            if (SourceBody != null)
            {
                BodyPart ReadyBody = SourceBody.GetBody();
                BodyPart AttatchArmTemplate = ReadyBody.AddPartAt(
                    Base: "Psionic Arm",
                    Laterality: Laterality.NONE,
                    Manager: ManagerID,
                    InsertAfter: "Arm",
                    OrInsertBefore: new string[4]
                {
                "Hands",
                "Feet",
                "Roots",
                "Thrown Weapon"
                });
                AttatchArmTemplate.AddPart(
                    Base: "Psionic Hand",
                    Laterality: Laterality.RIGHT,
                    DefaultBehavior: "Psionic Hands",
                    Manager: ManagerID);
                ReadyBody.AddPartAt(
                    InsertAfter: AttatchArmTemplate,
                    Base: "Psionic Arm",
                    Laterality: 1,
                    Manager: ManagerID).AddPart(
                                        Base: "Psionic Hand",
                                        Laterality: Laterality.LEFT,
                                        SupportsDependent: "Psionic Hands",
                                        Manager: ManagerID);
                ReadyBody.AddPartAt(
                    Base: "Missile Weapon",
                    Laterality: Laterality.RIGHT,
                    DependsOn: "Psionic Hands",
                    Manager: ManagerID,
                    InsertAfter: "Hands",
                    OrInsertBefore: new string[1]
                    {
                    "Missile Weapon"
                    });
                ReadyBody.AddPartAt(
                    Base: "Missile Weapon",
                    Laterality: Laterality.LEFT,
                    DependsOn: "Psionic Hands",
                    Manager: ManagerID,
                    InsertAfter: "Hands",
                    OrInsertBefore: new string[1]
                    {
                    "Missile Weapon"
                    });
            }
        }

        public void RemovePsionicArms()
        {
            ParentObject.RemoveBodyPartsByManager(ManagerID, true);
        }
        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart)this, "EndTurn");
            Object.RegisterPartEvent(this, "Dismember");
            Object.RegisterPartEvent(this, "CommandManifestLimb");
            Object.RegisterPartEvent(this, "CommandDismissLimb");
            Object.RegisterPartEvent(this, "GetPsychicGlimmer ");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandManifestLimb")
            {
                FocusPsi focusPsi = ParentObject.GetPart<FocusPsi>();
                if (NewArmCost <= ParentObject.Statistics["PsiCharges"].BaseValue)
                {

                    ArmCounter += 1;
                    ArmCost = (ArmCounter + NewArmCost + 1);
                    NewArmCost = ArmCost;

                    // AddPlayerMessage("Arm Counter :" + ArmCounter);
                    // AddPlayerMessage("Arm Cost :" + ArmCost);
                    // AddPlayerMessage("New Arm Cost: " + NewArmCost);

                    focusPsi.focusPsiCurrentCharges = focusPsi.maximumPsiCharge();
                    AddPsionicArms();
                    AddPlayerMessage(ParentObject.It + " manifest psionic limbs.");
                    UseEnergy(500);
                    focusPsi.UpdateCharges();
                    ParentObject.FireEvent(Event.New("FireEventDebuffSystem", 0, 0, 0));

                }
                else
                {
                    AddPlayerMessage(ParentObject.It + " do not have enough {{red|maximum charges}} to materialize a new limb.");
                }
            }
            else if (E.ID == "CommandDismissLimb")
            {
                FocusPsi focusPsi = ParentObject.GetPart<FocusPsi>();
                if (ArmCounter >= 1)
                {
                    focusPsi.UpdateCharges();
                    RemovePsionicArms();
                    AddPlayerMessage(ParentObject.It + " dismiss " + ParentObject.its + " psionic arms.");
                    ArmCounter = 0;
                    ArmCost = 1;
                    NewArmCost = 0;
                    ParentObject.FireEvent(Event.New("FireEventDebuffSystem", 0, 0, 0));
                }
                else
                {
                    AddPlayerMessage("You have no psionic arms manifested at the moment.");
                }
            }
            else if (E.ID == "Dismember")
            {
                if (E.HasStringParameter("Psionic") || E.HasIntParameter("17"))
                {
                    AddPlayerMessage("{{Red|A blow disrupts the tangibility of one of " + ParentObject.its + " arms, it dissipates!}}");
                    string ArmInQuestion = E.GetStringParameter("LimbSourceGameObjectID");
                    ParentObject.RemoveBodyPartsByManager(ArmInQuestion, EvenIfDismembered: true);
                }
            }
            else if (E.ID == "EndTurn")
            {
                FocusPsi focusPsi = ParentObject.GetPart<FocusPsi>();
                focusPsi.UpdateCharges();
                // AddPlayerMessage("ArmCounter: " + ArmCounter);
                // AddPlayerMessage("CurrentID: " + ManagerID + ArmCounter);
                // AddPlayerMessage("ArmCost: " + ArmCost);
                // AddPlayerMessage("NewArmCost: " + NewArmCost);
                // AddPlayerMessage("PsiMaximum: " + focusPsi.maximumPsiCharge());
                // AddPlayerMessage("PsiArmCounter: " + focusPsi.ArmCounter);
                // AddPlayerMessage("PsiArmcost: " + focusPsi.ArmCost);
            }
            else if (E.ID == "GetPsychicGlimmer")
            {
                var eAmount = E.GetIntParameter("Amount");

                eAmount /= 2;
            }
            return base.FireEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
    }
}

