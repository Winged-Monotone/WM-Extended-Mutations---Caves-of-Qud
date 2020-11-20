using System;
using XRL.UI;
using XRL.Rules;
using XRL.World.Effects;
using System.Collections.Generic;
using ConsoleLib.Console;
using XRL.Core;


namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class GelatinousFormPoison : BaseMutation
    {
        public int nRegrowCount;
        public int nNextLimb = 1000;
        public int Pairs = 1;
        public List<int> myLimbs = new List<int>(5);
        public List<int> myOldLimbs;
        public string PoisonIchorObj = "poisonichorpool";
        public int Density = 1;
        public int PsuedopodID = 0;
        public int OldArmID = 0;
        public int OldHandID = 0;
        public GelatinousFormPoison()
        {
            DisplayName = "Gelatinous Form {{poisonous|(Poison)}}";
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
        }

        public override string GetDescription()
        {
            return "You lack a muscuskeletal system, your genome chose an amorphous eukaryote's physique. Yours is especially {{poisonous|poisonous.}}\n";
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Unmutate(GO);
            ParentObject.SetStringProperty("BleedLiquid", "poisonichor-1000");
            ParentObject.AddPart<PoisonImmunity>(true);
            return base.Mutate(GO, Level);
        }



        public override string GetLevelText(int Level)
        {
            string text = string.Empty;
            if (Level == base.Level)
            {
                text += "You gain a 25% damage resistance bonus to melee weapons and immunity from poison, but take more damage from projectiles and explosives.\n";
                text += "\n";
                text += "When dealt damage or struck from a melee weapon, there's a random chance you release poison ichor in random squares around you.\n";
                text += "You quickly regenerate lost limbs.\n\n";
                text += "+200 rep with {{blue|oozes}}";
            }
            else
            {
                text += "Increased density of poison release upon hit.";
                text += "You quickly regenerate lost limbs.\n";
            }
            return text;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeforeApplyDamage")
            {
                Damage parameter = E.GetParameter("Damage") as Damage;
                if (parameter.HasAttribute("Slashing"))
                    parameter.Amount = (int)((double)parameter.Amount * (0.25 * (int)Math.Ceiling((Decimal)Level / new Decimal(2))));
                else if (parameter.HasAttribute("Melee"))
                    parameter.Amount = (int)((double)parameter.Amount * (0.25 * (int)Math.Ceiling((Decimal)Level / new Decimal(2))));
                else if (parameter.HasAttribute("Ranged"))
                    parameter.Amount = (int)((double)parameter.Amount * (1 + (0.25 * (int)Math.Ceiling((Decimal)Level / new Decimal(2)))));

                if (ParentObject.CurrentCell != null && parameter.Amount != 0)
                {
                    List<Cell> adjacentCells1 = ParentObject.CurrentCell.GetAdjacentCells(true);
                    adjacentCells1.Add(ParentObject.CurrentCell);
                    foreach (Cell cell in adjacentCells1)
                    {
                        if (!cell.IsOccluding() && Stat.Random(1, 100) >= 90)
                        {
                            GameObject IchorContainer = GameObject.create(this.PoisonIchorObj);
                            cell.AddObject(IchorContainer);
                        }
                    }
                }
            }

            if (E.ID == "Regenerating")
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
            return base.FireEvent(E);
        }

    }
}