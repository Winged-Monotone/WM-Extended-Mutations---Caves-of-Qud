using System;
using XRL.World;
using XRL.World.Parts.Mutation;
using System.Collections.Generic;
using XRL.Rules;
using System.Linq;
using XRL.World.Effects;
using XRL.Language;
using XRL.World.Capabilities;
using UnityEngine;

namespace XRL.World.Parts
{

    [Serializable]
    public class WMTailMutator : IPart
    {
        public static bool AddsRepDone = false;
        public bool NeedsSynergyPrefix = true;
        private List<string> SynergyMutations = new List<string>()
        {
            "Quills",
            "RoughScales",
            "ThickFur",
            "LightManipulation",
            "Amphibious",
            "Chimera",
        };
        public struct SynergyConstruct
        {
            public bool IsActive;
            public Action<GameObject> Effect;
            public string Prefix;
        }
        private Dictionary<string, SynergyConstruct> SynergyEffects = new Dictionary<string, SynergyConstruct>
        {
            {"RoughScales", new SynergyConstruct{
                Prefix = "{{lightblue|plated}}",
                Effect = (ParentObject) =>
            {
                GameObject Tail = ParentObject;
                GameObject Owner = Tail.Equipped;
                Tail.Armor.AV = Math.Min(2, (2 + Owner.StatMod("Toughness") / 2)) ;
                var shield = Tail.RequirePart<Shield>();
                shield.AV = Math.Min(1, (1 + Owner.StatMod("Strength") / 2));
            }}},
            {"Quills", new SynergyConstruct{
                Prefix = "{{brown|quilled}}",
                Effect = (ParentObject) =>
            {
                GameObject Tail = ParentObject;
                GameObject Owner = Tail.Equipped;
                Tail.Armor.AV = 1;
            }}},
            {"ThickFur", new SynergyConstruct{
                Prefix = "{{red|furry}}",
                Effect = (ParentObject) =>
            {
                GameObject Tail = ParentObject;
                GameObject Owner = Tail.Equipped;
                Tail.Armor.DV = Math.Min(1, (1 + Owner.StatMod("Agility") / 2)) ;
                Tail.Armor.Cold = Math.Min(10, (10 + (5 * Owner.StatMod("Toughness"))));
                Tail.Armor.Heat = Math.Min(10, (10 + (5 * Owner.StatMod("Toughness"))));
            }}},
            {"LightManipulation", new SynergyConstruct{
                Prefix = "{{yellow|luminous}}",
                Effect = (ParentObject) =>
            {
                GameObject Tail = ParentObject;
                GameObject Owner = Tail.Equipped;
                Tail.Armor.Elec = Math.Min(10, (10 + (5 * Owner.StatMod("Toughness"))));
                if (!Owner.HasEffect("Luminous"))
                {
                    Owner.ApplyEffect(new Luminous(Effect.DURATION_INDEFINITE));
                }
            }}},
            {"Amphibious", new SynergyConstruct{
                Prefix = "{{blue|finned}}",
                Effect = (ParentObject) =>
            {
                GameObject Tail = ParentObject;
                GameObject Owner = Tail.Equipped;
                Tail.Armor.Acid = Math.Max(10, (5 * Owner.StatMod("Toughness")));

            }}},
            {"Chimera", new SynergyConstruct{
                Prefix = "{{black|proto}}-{{shyrhak|chimeraen}}",
                Effect = (ParentObject) =>
            {
                GameObject Tail = ParentObject;
                GameObject Owner = Tail.Equipped;
                if (AddsRepDone == false)
                {
                    AddsRep.AddModifier(Tail, "Fish", 100);
                    AddsRep.AddModifier(Tail, "Beasts", 100);
                    AddsRep.AddModifier(Tail, "Bears", 100);
                    AddsRep.AddModifier(Tail, "Tortoises", 100);
                    AddsRep.AddModifier(Tail, "Unshelled Reptiles", 100);
                    AddsRepDone = true;
                }
            }
            }}};

        public static int TheChimera(GameObject ParentObject)
        {
            GameObject Tail = ParentObject;
            GameObject Owner = Tail.Equipped;
            int ChimeraSummit = Owner.StatMod("Strength") + Owner.StatMod("Toughness") + Owner.StatMod("Strength");
            return ChimeraSummit;
        }

        public bool HasPermutation(GameObject Parent)
        {
            Mutations HasSynergyMutation = Parent.GetPart<Mutations>();
            return (SynergyMutations.Any(HasSynergyMutation.HasMutation));
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == EquippedEvent.ID
            || ID == GetShortDescriptionEvent.ID
            || ID == GetSwimmingPerformanceEvent.ID
            || (ID == GetDisplayNameEvent.ID && NeedsSynergyPrefix)
            ;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            try
            {
                if (!NeedsSynergyPrefix)
                {
                    return true;
                }

                GameObject Tail = ParentObject;
                GameObject Owner = Tail.Equipped;
                Mutations HasSynergyMutation = Owner.GetPart<Mutations>();
                int Synergies = SynergyMutations.Count(HasSynergyMutation.HasMutation);
                if (Synergies >= 3)
                {
                    return true;
                }
                foreach (var kv in SynergyEffects)
                {
                    if (HasSynergyMutation.HasMutation(kv.Key))
                    {
                        E.AddBase(kv.Value.Prefix, DescriptionBuilder.ORDER_ADJUST_EARLY);
                    }
                }
            }
            catch
            {

            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            GameObject Tail = ParentObject;
            GameObject Owner = Tail.Equipped;
            Mutations HasSynergyMutation = Owner.GetPart<Mutations>();
            string DescriptTest = "TESTING THIS ABOMINATION!";
            string DescriptQuills = "{{light blue|This thick muscular tail is lined with large barbs, angled for striking potential predators. [Bonus Damage on Tail Whip]}}\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n";
            string DescriptFurred = "{{light blue|A large, thick fuzzy tail. You feel the urge to curl up in it.}}\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n";
            string DescriptChimera = "{{red|The beginnings of a primordial shah.}}\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n";
            string DescriptPlated = "{{light blue|Its armed with plated scales, their strength allows this appendage to deflect blows.\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n"
                                    + "Your tail can deflect blows on successful blocks: Strength Modifier -> AC.";
            string PlatedAccesser = "[Your tail can deflect blows on successful blocks: Strength Modifier -> AC.]";
            string DescriptLum = "{{grey|The fleshy end of this tail glows luminously as chemical reactions perform a mesmerizing dance in a membrane at its tip.}}\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n";
            string DescriptFin = "{{grey|This tail has a large dorsal fin, its strength aids in swimming strides, a rare skill.}}\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n";
            string DescriptTwo = "This one is a somewhat mutated tail with several grotesque appendages granting it odd qualities.\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n";
            string DescriptThree = "This strange tail echoes nature's mockeries of order and anarchy; it offers its bearer a variety of perks at their disposal.\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n";
            string DescriptFour = "This tail is worthy of the name zenith, a deadly composition sculpted through nature's will to evolve.\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n";
            string DescriptFive = "To look upon this twisted limb is to be reminded of the meaning chimera. It awakens primitive fears, maybe even a sudden thrill.\n"
                                    + "{{green|" + Tail.Armor.AV + "+ Bonus AV\n"
                                    + Tail.Armor.DV + "+ Bonus DV}}\n";
            string[] outputs = SynergyMutations.GetRange(0, 4).ToArray();
            int Synergies = SynergyMutations.Count(HasSynergyMutation.HasMutation);
            try
            {
                if (Synergies == 1 && HasSynergyMutation.HasMutation("Quills"))
                {
                    Debug.Log("MY Log: Initiate quills");
                    E.Infix.Append('\n').Append(DescriptQuills);
                }
                else if (Synergies == 1 && HasSynergyMutation.HasMutation("ThickFur"))
                {
                    Debug.Log("MY Log: Initiate fur");
                    E.Infix.Append('\n').Append(DescriptFurred);
                }
                else if (Synergies == 1 && HasSynergyMutation.HasMutation("RoughScales"))
                {
                    Debug.Log("MY Log: Initiate scale");
                    E.Infix.Append('\n').Append(DescriptPlated);
                }
                else if (Synergies == 1 && HasSynergyMutation.HasMutation("LightManipulation"))
                {
                    Debug.Log("MY Log: Initiate lum");
                    E.Infix.Append('\n').Append(DescriptLum);
                }
                else if (Synergies == 1 && HasSynergyMutation.HasMutation("Amphibious"))
                {
                    E.Infix.Append('\n').Append(DescriptFin);
                }
                else if (Synergies == 1 && HasSynergyMutation.HasMutation("Chimera"))
                {
                    E.Infix.Append('\n').Append(DescriptChimera);
                }
                else if (Synergies == 2)
                {
                    E.Infix.Append('\n').Append(DescriptTwo);
                }
                else if (Synergies == 3)
                {
                    E.Infix.Append('\n').Append(DescriptThree);
                }
                else if (Synergies == 4)
                {
                    E.Infix.Append('\n').Append(DescriptFour);
                }
                else if (Synergies >= 5)
                {
                    E.Infix.Append('\n').Append(DescriptFive);
                }
                if (HasSynergyMutation.HasMutation("RoughScales") && Synergies > 1)
                {
                    E.Postfix.Append('\n').Append(PlatedAccesser);
                }
            }
            catch
            { E.Infix.Append('\n').Append(DescriptTest); }
            return base.HandleEvent(E);
        }


        public override bool HandleEvent(GetSwimmingPerformanceEvent E)
        {
            GameObject Tail = ParentObject;
            GameObject Owner = Tail.Equipped;
            Mutations PMu = Owner.GetPart<Mutations>();
            ThickTail TailMutation = base.ParentObject.Equipped.GetPart<Mutations>().GetMutation("ThickTail") as ThickTail;
            var data = TailMutation.GetData(TailMutation.Level);
            if (PMu.HasMutation("Amphibious"))
                E.MoveSpeedPenalty -= data.SwimSpeed;
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(EquippedEvent E)
        {
            UpdateTailParts();
            E.Actor.RegisterPartEvent(this, "MutationAdded");
            return base.HandleEvent(E);
        }
        public void UpdateTailParts()
        {
            GameObject Tail = ParentObject;
            GameObject Owner = Tail.Equipped;
            Mutations PMu = Owner.GetPart<Mutations>();
            ThickTail TailMutation = base.ParentObject.Equipped.GetPart<Mutations>().GetMutation("ThickTail") as ThickTail;
            int ChimeraBoost = ((Owner.StatMod("Strength") + Owner.StatMod("Toughness") + Owner.StatMod("Agility")) / 3);
            if (Owner == null)
            {
                return;
            }
            if (PMu.HasMutation("Chimera"))
            {
                SynergyConstruct Construct;
                bool SynergizedChi = SynergyEffects.TryGetValue("Chimera", out Construct);
                if (SynergizedChi && Construct.IsActive == false)
                {
                    Construct.Effect.Invoke(ParentObject);
                    Construct.IsActive = true;
                }
            }
            foreach (BaseMutation Mut in PMu.MutationList)
            {
                SynergyConstruct Construct;
                bool Synergized = SynergyEffects.TryGetValue(Mut.Name, out Construct);
                if (Synergized && Construct.IsActive == false)
                {
                    Construct.Effect.Invoke(ParentObject);
                    Construct.IsActive = true;
                }
            }
        }


        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "PerformingMeleeAttack");
            Object.RegisterPartEvent(this, "Unequipped");
            Object.RegisterPartEvent(this, "Equipped");
            Object.RegisterPartEvent(this, "CommandTailWhip");
            base.Register(Object);
        }
        public override bool FireEvent(Event E)
        {
            var Owner = ParentObject?.Equipped;
            if (Owner == null)
            {
                return base.FireEvent(E);
            }
            Mutations HasSynergyMutation = Owner.GetPart<Mutations>();
            ThickTail TailMutation = base.ParentObject.Equipped.GetPart<Mutations>().GetMutation("ThickTail") as ThickTail; if (E.ID == "MutationAdded")
            {
                UpdateTailParts();
                GameObject Tail = ParentObject;
                int Synergies = SynergyMutations.Count(HasSynergyMutation.HasMutation);
                if (Synergies == 3)
                {
                    NeedsSynergyPrefix = false;
                    Tail.DisplayName = "{{blue|bizarre}} tail";
                }
                else if (Synergies == 4)
                {
                    NeedsSynergyPrefix = false;
                    Tail.DisplayName = "{{zetachrome|zenith}} tail";
                }
                else if (Synergies >= 5)
                {
                    NeedsSynergyPrefix = false;
                    Tail.DisplayName = "{{shyrhak|chimeraen}} tail";
                }
            }
            if (E.ID == "Equipped")
            {
                GameObject EventOwner = E.GetGameObjectParameter("EquippingObject");
                EventOwner.RegisterPartEvent(this, "PerformingMeleeAttack");
                EventOwner.RegisterPartEvent(this, "GetDefenderHitDice");
                UpdateTailParts();
            }
            else if (E.ID == "Unequipped")
            {
                GameObject EventOwner = E.GetGameObjectParameter("UnequippingObject");
                EventOwner.UnregisterPartEvent(this, "PerformingMeleeAttack");
                EventOwner.UnregisterPartEvent(this, "GetDefenderHitDice");
                UpdateTailParts();
            }

            if (E.ID == "PerformingMeleeAttack")
            {
                GameObject Defender = E.GetGameObjectParameter("Defender");
                if (Stat.Random(1, 100) <= 20 + (TailMutation.Level * 5))
                {
                    TailStrike(Defender, 2 + TailMutation.Level);
                    if (HasSynergyMutation.HasMutation("Quills"))
                        AddPlayerMessage("Your quilled tail delivers extra damage!");
                }
            }
            if (E.ID == "GetDefenderHitDice" && HasSynergyMutation.HasMutation("RoughScales"))
            {
                GameObject Attacker = E.GetGameObjectParameter("Attacker");
                GameObject Tail = ParentObject;
                var TailShield = Tail.RequirePart<Shield>();
                if (TailShield == null)
                {
                    return true;
                }
                if (E.HasParameter("ShieldBlocked"))
                {
                    return true;
                }
                if (!Owner.CanMoveExtremities(null, false, false, false))
                {
                    return true;
                }
                if (Stat.Random(1, 100) <= 15 + (5 * TailMutation.Level))
                {
                    E.SetParameter("ShieldBlocked", true);
                    if (Owner.IsPlayer())
                    {
                        IComponent<GameObject>.AddPlayerMessage("You deflect an attack with your " + base.ParentObject.DisplayName + "!" + "(" + TailShield.AV + " AV)", 'g');
                    }
                    else
                    {
                        Owner.ParticleText(string.Concat(new object[]
                        {
                            "{{",
                            IComponent<GameObject>.ConsequentialColor(Owner, null),
                            "|Block! (+",
                            TailShield.AV,
                            " AV)}}"
                        }), ' ', false, 1.5f, -8f);
                    }
                    E.SetParameter("AV", E.GetIntParameter("AV", 0) + TailShield.AV);
                }
            }
            return base.FireEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public void TailStrike(XRL.World.GameObject Defender, int Hitbonus)
        {
            ThickTail Tail = base.ParentObject.Equipped.GetPart<Mutations>().GetMutation("ThickTail") as ThickTail;

            var TailSource = ParentObject;
            var Owner = TailSource.Equipped;
            Mutations HasSynergyMutation = Owner.GetPart<Mutations>();
            if (Defender != null && Defender.PhaseAndFlightMatches(Owner) && Defender.CurrentCell != null && Owner.CurrentCell != null && Owner.DistanceTo(Defender) <= 1)
            {
                if (Defender.pBrain != null)
                {
                    Defender.pBrain.GetAngryAt(Owner, -20);
                }
                if (Stat.Random(1, 20) + Hitbonus + Owner.StatMod("Agility", 0) > Stats.GetCombatDV(Defender))
                {
                    var data = Tail.GetData(Tail.Level);
                    int PenetrationCont = Stat.RollDamagePenetrations(Stats.GetCombatAV(Defender), (Stat.Roll(data.Penetration, null) + Stat.Roll(data.BonusPen)), Stat.Roll(data.Penetration, null) + Stat.Roll(data.BonusPen));
                    string resultColor = Stat.GetResultColor(PenetrationCont);
                    int DamageBaseInit = 0;

                    if (PenetrationCont > 0)
                    {
                        for (int i = 0; i < PenetrationCont; i++)
                        {
                            DamageBaseInit += Stat.Roll(data.BaseDamage, null);
                            CombatJuice.punch(Owner, Defender);
                        }
                        Damage damage = new Damage(DamageBaseInit);
                        damage.AddAttribute("Physical");
                        damage.AddAttribute("Bludgeoning");
                        Event @event = Event.New("TakeDamage", 0, 0, 0);
                        @event.AddParameter("Damage", damage);
                        @event.AddParameter("Owner", Owner);
                        @event.AddParameter("Attacker", Owner);
                        if (HasSynergyMutation.HasMutation("Quills"))
                        {
                            @event.AddParameter("Damage", data.BonusDamage);
                        }
                        if (DamageBaseInit > 0 && Defender.FireEvent(@event))
                        {
                            if (Owner.IsPlayer())
                            {
                                IComponent<GameObject>.AddPlayerMessage(string.Concat(new string[]
                                {
                                    "&gYou strike ",
                                    resultColor,
                                    "(x",
                                    PenetrationCont.ToString(),
                                    ")&y for ",
                                    damage.Amount.ToString(),
                                    " &ydamage with your " + base.ParentObject.DisplayName + "!"
                                }));
                            }
                            else if (Defender.IsPlayer())
                            {
                                IComponent<GameObject>.AddPlayerMessage(string.Concat(new string[]
                                {
                                    ParentObject.The,
                                    ParentObject.ShortDisplayName,
                                    " &r",
                                    ParentObject.GetVerb("strike", false, false),
                                    " ",
                                    resultColor,
                                    "(x",
                                    PenetrationCont.ToString(),
                                    ")&r for ",
                                    damage.Amount.ToString(),
                                    " &ydamage with ",
                                    ParentObject.its,
                                    " tail!"
                                }));
                            }
                        }
                        else if (Owner.IsPlayer())
                        {
                            IComponent<GameObject>.AddPlayerMessage("&rYou fail to deal damage to " + Defender.the + Defender.DisplayNameOnly + " &rwith your " + base.ParentObject.DisplayName + "!");
                        }
                        else if (Defender.IsPlayer())
                        {
                            IComponent<GameObject>.AddPlayerMessage(string.Concat(new string[]
                            {
                                ParentObject.The,
                                ParentObject.DisplayName,
                                " &g",
                                ParentObject.GetVerb("fail", false, false),
                                " to damage you with ",
                               ParentObject.its,
                                " tail!"
                            }));
                        }
                    }
                }
            }
        }
    }
}