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
    public class GelatinousFormAcid : BaseDefaultEquipmentMutation
    {
        public int nRegrowCount;
        public int nNextLimb = 1000;
        public string acidPool = "AcidPool";
        public int Density = 1;
        public string ManagerID => ParentObject.id + "::GelatinousFormAcid";
        public bool IsDissolving = false;
        public Guid ActivatedAbilityID = Guid.Empty;
        public int Damage = 2;
        public int Duration;
        public GelatinousFormAcid()
        {
            DisplayName = "Gelatinous Form {{green|(Acid)}}";
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
            go.RegisterPartEvent((IPart)this, "CommandSpitAcid");
            go.RegisterPartEvent((IPart)this, "OnEquipped");
            base.Register(go);
        }

        public override string GetDescription()
        {
            return "You lack a muscuskeletal system, your genome chose an amorphous eukaryote's physique. Yours is especially {{green|Acidic.}}\n";
        }

        public override string GetLevelText(int Level)
        {
            string text = string.Empty;
            if (Level == base.Level)
            {
                text += "{{cyan|25%}} damage resistance bonus to melee weapons.\n";
                text += "Immunity from the element acid.\n";
                text += "Take {{cyan|25%}} more damage from projectiles and explosives.\n";
                text += "When dealt damage, there's a random chance you bleed acid in a random square around you, dealing damage to your enemies.\n";
                text += "You can spit acid at your foes.\n";
                text += "You cannot wear armor below tier {{cyan|5}}.\n";
                text += "You quickly regenerate lost limbs.\n\n";
                text += "{{cyan|+200 reputation with oozes}}\n";
                text += "\n{{red|Stay away from salt.}}\n";
            }
            else
            {
                text += "Increased chance of acid release by {{cyan|5%}}, and the density of acid release upon being struck by an enemy.";
                text += "\nYou regenerate lost limbs more quickly.";
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
                            GameObject AcidContainer = GameObject.create(this.acidPool);
                            var AcidProperties = AcidContainer.GetPart<LiquidVolume>();
                            AcidProperties.Volume *= Level;
                            cell.AddObject(GO: AcidContainer,
                                            Forced: true,
                                            System: false,
                                            IgnoreGravity: false,
                                            NoStack: false);
                        }
                    }
                }
            }
            else if (E.ID == "EndTurn")
            {
                Cell Cell = ParentObject.CurrentCell;

                if (IsDissolving == true && (Cell.HasObject(X => WaterThatHurts.Contains(X.Blueprint))))
                {
                    this.Duration += 2;
                    Damage += 1;
                    ParentObject.TakeDamage(ref Damage, DeathReason: "{{green|Dissolved into visceral soup.}}", Message: "from salt diffusion!");
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
            else if (E.ID == "CommandSpitAcid")
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
                                item2.AddObject(GameObject.create("AcidPool"));
                            }
                            num++;
                        }
                        UseEnergy(1000);
                    }
                }
            }
            return base.FireEvent(E);
        }
        public void AddAcidGlobul()
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
                        DefaultBehavior: "Acid_Humor_Pseudopod",
                        Manager: ManagerID);
                    mBodyPart.DefaultBehaviorBlueprint = "Acid_Humor_Pseudopod";
                }
                else
                {
                    var mBodyPart = AttatchPseudotemplate.AddPart(
                       Base: "Tentacle",
                       Laterality: Laterality.UPPER,
                       DefaultBehavior: "Acid_Humor_Tentacle",
                       Manager: ManagerID);
                    mBodyPart.DefaultBehaviorBlueprint = "Acid_Humor_Tentacle";
                }
            }
            SourceBody.UpdateBodyParts();
        }
        public override bool Mutate(GameObject GO, int Level)
        {
            Unmutate(GO);
            ParentObject.SetStringProperty("BleedLiquid", "acid-1000");
            ParentObject.AddPart<AcidImmunity>(true);
            ParentObject.AddPart<WaterHazardous>(true);
            ParentObject.AddPart<DamagesArmorOnEquipped>(true);
            AddAcidGlobul();
            ActivatedAbilityID = AddMyActivatedAbility(Name: "Spit Acid", Command: "CommandSpitAcid", Class: "Physical Mutation", Icon: "*");
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

        private List<string> WaterThatHurts = new List<string>()
        {
            "SaltyWaterPuddle",
            "SaltyWaterDeepPool",
            "SaltyWaterDeepPool",
            "BrackishWaterPuddle",
            "SaltPool",
            "BrackishPool",
        };

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == EquippedEvent.ID || ID == ObjectEnteredCellEvent.ID || ID == ObjectEnteringCellEvent.ID || ID == EndTurnEvent.ID;
        }

        public override bool HandleEvent(ObjectEnteringCellEvent E)
        {
            if (E.Object == ParentObject && E.Object.IsPlayer() && E.Cell.HasObject(X => WaterThatHurts.Contains(X.Blueprint)) && !ParentObject.HasEffect("Dissolving"))
            {
                if (Popup.ShowYesNo("This liquid is harmful to you, continue") != 0)
                {
                    return base.HandleEvent(E);
                }

                AutoAct.Interrupt();

                AddPlayerMessage("This liquid is harmful to you.");
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(ObjectEnteredCellEvent E)
        {
            if (E.Object == ParentObject && E.Cell.HasObject(X => WaterThatHurts.Contains(X.Blueprint)) && !ParentObject.HasEffect("Dissolving") && !ParentObject.HasEffect("Flying"))
            {
                ParentObject.ApplyEffect(new Dissolving(1, ParentObject), ParentObject);
                ParentObject.TakeDamage(Amount: ref Damage, DeathReason: "{{green|Dissolved into visceral soup.}}", Message: "from salt diffusion!");
                IsDissolving = true;
            }
            else if (IsDissolving == true && (E.Object == ParentObject && E.Cell.HasObject(X => WaterThatHurts.Contains(X.Blueprint))))
            {
                this.Duration += 2;
                Damage += 1;
                ParentObject.TakeDamage(Amount: ref Damage, DeathReason: "{{green|Dissolved into visceral soup.}}", Message: "from salt diffusion!");
            }
            else if ((IsDissolving == true && E.Object == ParentObject && (!E.Cell.HasObject(X => WaterThatHurts.Contains(X.Blueprint)))))
            {
                this.Duration -= 1;
                ParentObject.TakeDamage(Amount: ref Damage, DeathReason: "{{green|Dissolved into visceral soup.}}", Message: "from salt diffusion!");
            }
            if (Duration <= 0)
            {
                IsDissolving = false;
                if (ParentObject.HasEffect("Dissolving"))
                {
                    ParentObject.RemoveEffect("Dissolving");
                    Damage = 2;
                }
            }
            return base.HandleEvent(E);
        }
    }
}