using System;
using XRL.UI;
using XRL.Rules;
using XRL.World.Effects;
using System.Collections.Generic;
using XRL.World.Capabilities;
using ConsoleLib.Console;
using XRL.Core;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class GelatinousFormSlime : BaseDefaultEquipmentMutation
    {
        public int nRegrowCount;
        public int nNextLimb = 1000;
        public string SlimePool = "SlimePool";
        public string ManagerID => ParentObject.id + "::GelatinousFormSlime";
        public int Density = 1;
        public Guid ActivatedAbilityID = Guid.Empty;
        public GelatinousFormSlime()
        {
            DisplayName = "Gelatinous Form {{slimy|(Slime)}}";
        }

        public override bool CanLevel()
        {
            return true;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            Density = GetDensity();
            return base.ChangeLevel(NewLevel);
        }

        public int GetDensity()
        {
            int NewDensity = Density * (Level / 2);
            return (NewDensity);
        }

        public override bool AffectsBodyParts()
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
            go.RegisterPartEvent((IPart)this, "Regenerating");
            go.RegisterPartEvent((IPart)this, "BeforeApplyDamage");
            go.RegisterPartEvent((IPart)this, "CommandSpitSlime");
            go.RegisterPartEvent((IPart)this, "OnEquipped");
            base.Register(go);
        }

        public override string GetDescription()
        {
            return "You lack a muscuskeletal system, your genome chose an amorphous eukaryote's physique. Yours is especially {{slimy|slimy.}}";
        }

        public override string GetLevelText(int Level)
        {
            string text = string.Empty;

            if (Level == base.Level)
            {
                text += "You gain a {{cyan|25%}} damage resistance bonus to melee weapons.\n";
                text += "Take more damage from projectiles and explosives.\n";
                text += "When dealt damage, there's a random chance you bleed slime in a random tile around you.\n";
                text += "\nYou can spit slime at your foes.\n\n";
                text += "{{cyan|+200 reputation with oozes}}\n";
            }
            else
            {
                text += "Increased density of slime release upon being struck.\n";
                text += "You regenerate lost limbs more quickly.\n";
            }
            return text;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforeApplyDamage")
            {
                Damage parameter = E.GetParameter("Damage") as Damage;
                if (parameter.HasAttribute("Slashing"))
                    parameter.Amount -= (int)((double)parameter.Amount * (0.25 * (int)new Decimal(2)));
                else if (parameter.HasAttribute("Melee"))
                    parameter.Amount -= (int)((double)parameter.Amount * (0.25 * (int)new Decimal(2)));
                else if (parameter.HasAttribute("Ranged"))
                    parameter.Amount += (int)((double)parameter.Amount * (0.25 * (int)new Decimal(2)));

                if (ParentObject.CurrentCell != null && parameter.Amount != 0)
                {
                    List<Cell> adjacentCells1 = ParentObject.CurrentCell.GetAdjacentCells(true);
                    adjacentCells1.Add(ParentObject.CurrentCell);
                    foreach (Cell cell in adjacentCells1)
                    {
                        if (!cell.IsOccluding() && Stat.Random(1, 100) <= 10 + (5 * Level / 2))
                        {
                            GameObject SlimeContainer = GameObject.create(this.SlimePool);
                            var SlimeProperties = SlimeContainer.GetPart<LiquidVolume>();
                            SlimeProperties.Volume *= Level;
                            cell.AddObject(GO: SlimeContainer,
                                            Forced: true,
                                            System: false,
                                            IgnoreGravity: false,
                                            NoStack: false);
                        }
                    }
                }
            }
            else if (E.ID == "Regenerating")
            {
                nRegrowCount++;
                if (nRegrowCount >= nNextLimb)
                {
                    nRegrowCount = 0;
                    nNextLimb = 1000 - 400 * base.Level + Stat.Roll("1d" + ((11 - Math.Min(base.Level, 10)) * 1000).ToString());
                    ParentObject.FireEvent(Event.New("RegenerateLimb", 0, 0, 0));
                }
                E.AddParameter("Amount", E.GetIntParameter("Amount") + (20 + base.Level * 4));
                return true;
            }
            else if (E.ID == "CommandSpitSlime")
            {
                if (!this.ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap())
                {
                    List<Cell> list = PickBurst(1, 8, false, AllowVis.OnlyVisible);
                    if (list == null)
                    {
                        return true;
                    }
                    foreach (Cell item in list)
                    {
                        if (item.DistanceTo(ParentObject) > 9)
                        {
                            if (ParentObject.IsPlayer())
                            {
                                Popup.Show("That is out of range! (8 squares)");
                            }
                            return true;
                        }
                    }
                    if (list != null)
                    {
                        SlimeGlands.SlimeAnimation("&G", ParentObject.CurrentCell, list[0]);
                        CooldownMyActivatedAbility(ActivatedAbilityID, 40);
                        int num = 0;
                        foreach (Cell item2 in list)
                        {
                            if (num == 0 || Stat.Random(1, 100) <= 80)
                            {
                                item2.AddObject(GameObject.create("SlimePool"));
                            }
                            num++;
                        }
                        UseEnergy(1000);
                    }
                }
            }
            return base.FireEvent(E);
        }
        public void AddSlimeGlobul()
        {
            Body SourceBody = ParentObject.GetPart("Body") as Body;
            if (SourceBody != null)
            {
                BodyPart ReadyBody = SourceBody.GetBody();
                var AttatchPseudotemplate = ReadyBody.AddPartAt(
                    Base: "Oral Arm",
                    Laterality: Laterality.UPPER,
                    Manager: ManagerID,
                    OrInsertBefore: new string[1]
                {
                "Head",
                });
                if (Stat.Random(1, 100) <= 50)
                {
                    var mBodyPart = AttatchPseudotemplate.AddPart(
                        Base: "Pseudopod",
                        Laterality: Laterality.UPPER,
                        DefaultBehavior: "Slime_Humor_Pseudopod",
                        Manager: ManagerID);
                    mBodyPart.DefaultBehaviorBlueprint = "Slime_Humor_Pseudopod";
                }
                else
                {
                    var mBodyPart = AttatchPseudotemplate.AddPart(
                       Base: "Tentacle",
                       Laterality: Laterality.UPPER,
                       DefaultBehavior: "Slime_Humor_Tentacle",
                       Manager: ManagerID);
                    mBodyPart.DefaultBehaviorBlueprint = "Slime_Humor_Tentacle";
                }
            }
            SourceBody.UpdateBodyParts();
        }
        public override bool Mutate(GameObject GO, int Level)
        {
            Unmutate(GO);
            ParentObject.SetStringProperty("BleedLiquid", "Slime-1000");
            ActivatedAbilityID = AddMyActivatedAbility(Name: "Spit Slime", Command: "CommandSpitSlime", Class: "Physical Mutation", Icon: "*");
            if (!ParentObject.HasIntProperty("Slimewalking"))
            {
                ParentObject.SetIntProperty("Slimewalking", 1);
            }
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            RemoveMyActivatedAbility(ref ActivatedAbilityID);
            return base.Unmutate(GO);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == EquippedEvent.ID || ID == ObjectEnteredCellEvent.ID || ID == ObjectEnteringCellEvent.ID;
        }
    }
}