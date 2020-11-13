
using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Effects;
using XRL.World.Parts;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Psybrachiomancy : BaseMutation
    {
        public int ArmCounter = 0;
        public int ArmCost;
        public int NewArmCost;
        private bool PsionicArmActive = false;
        public string ManagerID => ParentObject.id + "::Psybrachiomancy";
        public Guid ActivatedAbilityID = Guid.Empty;
        public Psybrachiomancy()
        {
            this.DisplayName = "Psybrachiomancy";
            this.Type = "Mental";
        }
        public override bool ChangeLevel(int NewLevel)
        {
            XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("highly entropic beings", 100, false);
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
                    + "\n{{white|+100 Reputation with}} {{orange|highly entropic beings.}}\n"
                    + "\n";
        }
        public override string GetLevelText(int Level)
        {
            return "{{gray|You can weave psionic arms into reality, they function like any other arm, but dissipate entirely upon being dismembered. Instead of using strength for penetration, a weapon’s penetration when wielded with a psionic arm is limited to your Ego modifier and the weapons’ penetration value. \n"
            + "\n"
            + "{{white|Materialize Arms:}} Create two arms. {{lightblue|Cost: 4}}, reduces maximum psi, increases dramatically per weaved psionic arm. \n"
            + "\n"
            + "{{orange|Maximum Psi-Points}} needed for Next Arm: {{orange|" + ArmCost + "}}\n\n"
            + "{{orange|(Dismembered psionic arms may not dissapate and may require re-weaving to repair, no damage is taken from blows that sever psionic limbs.)}}"
            + "{{red|if the set of arms you summon are more than your Willpower Modifier, you will be given the 'psi-burdening' effect temporarily.}}";
        }
        public override bool Mutate(GameObject GO, int Level)
        {
            Mutations GainPSiFocus = GO.GetPart<Mutations>();
            if (!GainPSiFocus.HasMutation("FocusPsi"))
            {
                GainPSiFocus.AddMutation("FocusPsi", 1);
            }
            this.ActivatedAbilityID = base.AddMyActivatedAbility("Manifest Limb", "CommandManifestLimb", "Mental Mutation", "Manifest a psychic arm.", ">", null, false, false, false, false, false, false, false, 10, null);
            this.ActivatedAbilityID = base.AddMyActivatedAbility("Dismiss Limb", "CommandDismissLimb", "Mental Mutation", "Dismiss a psychic arm.", "<", null, false, false, false, false, false, false, false, 10, null);
            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }

        public void AddPsionicArms()
        {
            Body SourceBody = ParentObject.GetPart("Body") as Body;
            if (SourceBody != null)
            {
                BodyPart ReadyBody = SourceBody.GetBody();
                BodyPart AttatchArmTemplate = ReadyBody.AddPartAt("Psionic-Arm", 2, null, null, null, null, ManagerID + ArmCounterStrings(), 17, null, null, null, null, null, null, null, null, null, null, null, "Arm", new string[4]
                {
                "Hands",
                "Feet",
                "Roots",
                "Thrown Weapon"
                });
                AttatchArmTemplate.AddPart("Psionic-Hand", 2, null, "Psionic-Hands", null, null, ManagerID + ArmCounterStrings(), 17);
                ReadyBody.AddPartAt(AttatchArmTemplate, "Psionic-Arm", 1, null, null, null, null, ManagerID + ArmCounterStrings(), 17).AddPart("Psionic-Hand", 1, null, "Psionic-Hands", null, null, ManagerID + ArmCounterStrings(), 17);
                ReadyBody.AddPartAt("Psionic-Hands", 0, null, null, "Psionic-Hands", null, ManagerID + ArmCounterStrings(), 17, null, null, null, null, null, null, null, null, null, null, null, "Hands", new string[3]
                {
                "Feet",
                "Roots",
                "Thrown Weapon"
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
                ArmCost = (2 + ArmCounter) + (2 * ArmCounter) - 1;
                NewArmCost = ArmCost;
                FocusPsi PsiMutation = ParentObject.GetPart<FocusPsi>();
                if (NewArmCost <= PsiMutation.maximumPsiCharge())
                {
                    ArmCounter += 1;
                    PsiMutation.focusPsiCurrentCharges = PsiMutation.maximumPsiCharge();
                    AddPsionicArms();
                    AddPlayerMessage(ParentObject.It + " manifest psionic limbs.");
                    UseEnergy(500);
                    PsiMutation.DisplayCurrentCharges();
                    ParentObject.FireEvent(Event.New("FireEventDebuffSystem", 0, 0, 0));
                }
                else if (NewArmCost > PsiMutation.maximumPsiCharge() || PsiMutation.maximumPsiCharge() <= 0)
                {
                    AddPlayerMessage(ParentObject.It + " do not have enough {{red|maximum charges}} to materialize a new limb.");
                    return true;
                }
            }
            if (E.ID == "CommandDismissLimb")
            {
                FocusPsi PsiMutation = ParentObject.GetPart<FocusPsi>();
                if (ArmCounter >= 1)
                {
                    PsiMutation.DisplayCurrentCharges();
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
                FocusPsi PsiMutation = ParentObject.GetPart<FocusPsi>();
                PsiMutation.DisplayCurrentCharges();
                // AddPlayerMessage("ArmCounter: " + ArmCounter);
                // AddPlayerMessage("CurrentID: " + ManagerID + ArmCounter);
                // AddPlayerMessage("ArmCost: " + ArmCost);
                // AddPlayerMessage("PsiMaximum: " + PsiMutation.maximumPsiCharge());
                // AddPlayerMessage("PsiArmCounter: " + PsiMutation.ArmCounter);
                // AddPlayerMessage("PsiArmcost: " + PsiMutation.ArmCost);
            }
            return base.FireEvent(E);
        }
    }
}

