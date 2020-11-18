using System;
using System.Collections.Generic;
using System.Linq;
using HistoryKit;
using UnityEngine;
using XRL.World;


namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class ThickTail : BaseDefaultEquipmentMutation
    {
        public string BodyPartType = "Tail";
        public string TailType = "Normal";
        public string AdditionsManagerID => ParentObject.id + "::ThickTail::Add";
        public string ChangesManagerID => ParentObject.id + "::ThickTail::Change";
        public int HitBonus = 5;
        public GameObject BaseTail;
        public GameObject SpikedTail;
        public GameObject FurryTail;
        public GameObject ScaledTail;
        public GameObject GelatonousProbe;
        public Guid ActivatedAbilityID;
        public int BasePenModifier;
        public string BaseDamageModifier;
        public int BaseToHitModifier;
        public int TailID;
        public int SwingChanceTextValue;
        public string BaseDamageTextValue;
        public int PenetrationTextValue;
        const int PlaceHolder = 3000;
        public string ThickTailObjectId;

        public ThickTail()
        {
            DisplayName = "Thick Tail";
        }

        public override bool CanLevel()
        {
            return true;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            Body pBody = ParentObject.GetPart("Body") as Body;
            if (pBody != null)
            {
                BodyPart Part = pBody.GetPartByManager(AdditionsManagerID);
                if (Part != null)
                {
                    ParentObject.FireEvent(Event.New("CommandForceUnequipObject", "BodyPart", Part));
                    if (BaseTail == null)
                    {
                        BaseTail = GameObject.create("BaseThickTail");
                    }

                    Armor armor = BaseTail.GetPart("Armor") as Armor;
                    armor.WornOn = Part.Type;
                    Event @event = Event.New("CommandForceEquipObject");
                    @event.SetParameter("Object", BaseTail);
                    @event.SetParameter("BodyPart", Part);
                    @event.SetSilent(Silent: true);
                    ParentObject.FireEvent(@event);
                }
            }
            if (ParentObject.HasObjectInInventory("BaseThickTail"))
            {
                ParentObject.FindObjectInInventory("BaseThickTail").Destroy(null, true, false);
            }
            return base.ChangeLevel(NewLevel);
        }

        public override string GetDescription()
        {
            return "This extension of flesh and bone originates from your spine in the form of a tail, its ability to adapt makes it a useful tool.";
        }

        public override string GetLevelText(int Level)
        {
            return "Deliver concussive tail-whips at your foes periodically:\n"
            + "\n"
            + "{{light blue|" + GetLevelValueSwingChance(Level) + "}}% chance to tail-whip your foe.\n"
            + "{{light blue|" + GetLevelValueBaseDamage(Level) + "}} + {{red|STR-MOD}} Bludgeoning Damage.\n"
            + "{{green|+" + GetLevelValuePenetration(Level) + "}} Penetration Value.\n"
            + "\n"
            + "\n"
            + "Attaining other mutations may cause Thick Tail to evolve into more powerful variants, check the equipment tab to see what bonuses you may receive.\n"
            + "\n"
            + "{{white|Scales primarily from Toughness, minor scaling from Strength and Agility.}}";
        }
        public string GetLevelValueBaseDamage(int Level)
        {
            if (Level == 1)
            {
                BaseDamageTextValue = "1d3";
            }
            if (Level == 2)
            {
                BaseDamageTextValue = "1d4";
            }
            if (Level == 3)
            {
                BaseDamageTextValue = "1d4";
            }
            if (Level == 4)
            {
                BaseDamageTextValue = "1d5";
            }
            if (Level == 5)
            {
                BaseDamageTextValue = "1d5";
            }
            if (Level == 6)
            {
                BaseDamageTextValue = "1d6";
            }
            if (Level == 7)
            {
                BaseDamageTextValue = "1d6";
            }
            if (Level == 8)
            {
                BaseDamageTextValue = "1d7";
            }
            if (Level == 9)
            {
                BaseDamageTextValue = "1d7";
            }
            if (Level >= 10)
            {
                BaseDamageTextValue = "1d8";
            }
            return BaseDamageTextValue;
        }

        public int GetLevelValuePenetration(int Level)
        {
            if (Level == 1)
            {
                PenetrationTextValue = 4;
            }
            if (Level == 2)
            {
                PenetrationTextValue = 4;
            }
            if (Level == 3)
            {
                PenetrationTextValue = 5;
            }
            if (Level == 4)
            {
                PenetrationTextValue = 5;
            }
            if (Level == 5)
            {
                PenetrationTextValue = 6;
            }
            if (Level == 6)
            {
                PenetrationTextValue = 6;
            }
            if (Level == 7)
            {
                PenetrationTextValue = 7;
            }
            if (Level == 8)
            {
                PenetrationTextValue = 7;
            }
            if (Level == 9)
            {
                PenetrationTextValue = 8;
            }
            if (Level >= 10)
            {
                PenetrationTextValue = 8;
            }
            return PenetrationTextValue;
        }

        public int GetLevelValueSwingChance(int Level)
        {
            if (Level == 1)
            {
                return SwingChanceTextValue = 20;
            }
            if (Level == 2)
            {
                return SwingChanceTextValue = 25;
            }
            if (Level == 3)
            {
                return SwingChanceTextValue = 30;
            }
            if (Level == 4)
            {
                return SwingChanceTextValue = 35;
            }
            if (Level == 5)
            {
                return SwingChanceTextValue = 40;
            }
            if (Level == 6)
            {
                return SwingChanceTextValue = 45;
            }
            if (Level == 7)
            {
                return SwingChanceTextValue = 50;
            }
            if (Level == 8)
            {
                return SwingChanceTextValue = 55;
            }
            if (Level == 9)
            {
                return SwingChanceTextValue = 60;
            }
            if (Level >= 10)
            {
                return SwingChanceTextValue = 60 + (ParentObject.StatMod("Agility") * 2);
            }
            return SwingChanceTextValue;
        }

        public ThickTail(string _TailType)
                    : this()
        {
            TailType = _TailType;
        }

        public override bool GeneratesEquipment()
        {
            return true;
        }
        public override bool Mutate(GameObject GO, int Level)
        {
            Body SourceBody = GO.GetPart<Body>();
            if (SourceBody != null)
            {
                var body = GO.GetPart<Body>();
                var core = body.GetBody();
                var tail = core.AddPartAt(Base: "Tail", DefaultBehavior: null, InsertAfter: "Feet", OrInsertBefore: "Hands");
                tail.Native = true;
                TailID = tail.ID;
                body.UpdateBodyParts();
            }
            BodyPart TailArmor = ParentObject.GetPart<Body>().GetFirstPart(BodyPartType);
            GameObject TailArmorObj = TailArmor.Equipped;
            Mutations HasSynergyMutation = ParentObject.GetPart<Mutations>();
            if (TailArmor != null && (TailArmorObj == null || TailArmor.Equipped != TailArmorObj))
            {
                ParentObject.FireEvent(Event.New("CommandForceUnequipObject", "BodyPart", TailArmor));
                if (TailArmorObj != null)
                {
                    TailArmorObj.Destroy(null, false, false);
                }
                TailArmorObj = GameObject.create("BaseThickTail");
                ParentObject.FireEvent(Event.New("CommandForceEquipObject", "Object", TailArmorObj, "BodyPart", TailArmor).SetSilent(true));
                Armor part = TailArmorObj.GetPart<Armor>();
                part.AV = 0;
                part.DV = 1;
            }
            if (TailArmorObj.HasObjectInInventory("BaseThickTail"))
            {
                ParentObject.FindObjectInInventory("BaseThickTail").Destroy(null, true, false);
            }
            return base.Mutate(GO, Level);
        }

        private List<string> SynergyMutations = new List<string>()
        {
            "Quills",
            "RoughScales",
            "ThickFur",
            "LightManipulation",
            "Amphibious",
            "Chimera",
        };
        public bool HasPermutation(GameObject Parent)
        {
            Mutations HasSynergyMutation = Parent.GetPart<Mutations>();
            return (SynergyMutations.Any(HasSynergyMutation.HasMutation));
        }

        public BodyPart AddTail(XRL.World.GameObject GO)
        {
            return GO?.GetPart<Body>()?.GetBody().AddPartAt("Tail", 0, null, null, null, null, AdditionsManagerID, null, null, null, null, null, null, null, null, null, null, null, null, "Feet", new string[3]
            {
            "Roots",
            "Thrown Weapon",
            "Floating Nearby"
            });
        }

        private BodyPart ProcessChangedLimb(BodyPart Part)
        {
            if (Part != null)
            {
                Part.Manager = ChangesManagerID;
            }
            return Part;
        }
        public override void OnRegenerateDefaultEquipment(Body body)
        {
            BodyPart partByID = body.GetPartByManager(AdditionsManagerID);
            if (partByID != null)
            {
                AddThickTailTo(partByID);
            }
        }
        public XRL.World.GameObject findThickTail()
        {
            return ParentObject.GetPart<Body>().FindEquipmentOrDefaultByID(ThickTailObjectId);
        }
        public void AddThickTailTo(BodyPart part)
        {
            if (part.Equipped != null)
            {
                if (part.Equipped.id == ThickTailObjectId)
                {
                    return;
                }
                ParentObject.FireEvent(XRL.World.Event.New("CommandForceUnequipObject", "BodyPart", part));
            }
            if (BaseTail == null)
            {
                BaseTail = XRL.World.GameObject.create("BaseThickTail");
            }
            if (BaseTail != null)
            {
                Armor armor = BaseTail.GetPart("Armor") as Armor;
                if (armor != null)
                {
                    armor.WornOn = part.Type;
                }
                else
                {
                    Debug.LogError("Tail object had no Armor part");
                }
                XRL.World.Event @event = XRL.World.Event.New("CommandForceEquipObject");
                @event.SetParameter("Object", ThickTailObjectId);
                @event.SetParameter("BodyPart", part);
                @event.SetSilent(Silent: true);
                ParentObject.FireEvent(@event);
                if (ParentObject.HasObjectInInventory("BaseThickTail"))
                {
                    ParentObject.FindObjectInInventory("BaseThickTail").Destroy(null, true, false);
                }
            }
            else
            {
                Debug.LogError("Could not create Thick Tail");
            }
        }

        public struct TailData
        {
            public string Penetration;
            public string BaseDamage;
            public string BonusDamage;
            public string BonusPen;
            public int BlockChance;
            public int SwingChance;
            public int SwimSpeed;
        }
        public TailData GetData(int Level)
        {
            Mutations CheckSynergy = ParentObject.GetPart<Mutations>();
            TailData Result = new TailData();
            Result.BaseDamage = string.Empty;
            Result.BonusDamage = string.Empty;
            Result.BonusPen = string.Empty;
            Result.Penetration = string.Empty;
            Result.BlockChance = int.MaxValue;
            Result.SwingChance = int.MaxValue;
            Result.SwimSpeed = int.MaxValue;

            if (Level == 1)
            {
                Result.Penetration = "4";
                Result.BaseDamage = "1d3";
                Result.SwingChance = 20;
            }
            if (Level == 2)
            {
                Result.Penetration = "4";
                Result.BaseDamage = "1d4";
                Result.SwingChance = 25;
            }
            if (Level == 3)
            {
                Result.Penetration = "5";
                Result.BaseDamage = "1d4";
                Result.SwingChance = 30;
            }
            if (Level == 4)
            {
                Result.Penetration = "5";
                Result.BaseDamage = "1d5";
                Result.SwingChance = 35;
            }
            if (Level == 5)
            {
                Result.Penetration = "6";
                Result.BaseDamage = "1d5";
                Result.SwingChance = 40;
            }
            if (Level == 6)
            {
                Result.Penetration = "6";
                Result.BaseDamage = "1d6";
                Result.SwingChance = 45;
            }
            if (Level == 7)
            {
                Result.Penetration = "7";
                Result.BaseDamage = "1d6";
                Result.SwingChance = 50;
            }
            if (Level == 8)
            {
                Result.Penetration = "7";
                Result.BaseDamage = "1d7";
                Result.SwingChance = 55;
            }
            if (Level == 9)
            {
                Result.Penetration = "8";
                Result.BaseDamage = "1d7";
                Result.SwingChance = 60;
            }
            if (Level >= 10)
            {
                Result.Penetration = "8";
                Result.BaseDamage = "1d8";
                Result.SwingChance = 60 + ((ParentObject.StatMod("Agility") * 2));
            }
            if (CheckSynergy.HasMutation("Quills"))
            {
                if (Level == 1 || Level == 2)
                {
                    Result.BonusDamage = "1d4";
                    Result.BonusPen = "1";
                }
                if (Level == 3 || Level == 4)
                {
                    Result.BonusDamage = "2d4";
                    Result.BonusPen = "2";
                }
                if (Level == 5 || Level == 6)
                {
                    Result.BonusDamage = "3d4";
                    Result.BonusPen = "3";
                }
                if (Level > 6)
                {
                    Result.BonusDamage = "4d4+" + Level / 2;
                    Result.BonusPen = "4";
                }
            }
            if (CheckSynergy.HasMutation("Quills"))
            {
                if (Level == 1 || Level == 2)
                {
                    Result.BlockChance = 30;
                }
                if (Level == 3 || Level == 4)
                {
                    Result.BlockChance = 40;
                }
                if (Level == 5 || Level == 6)
                {
                    Result.BlockChance = 50;
                }
                if (Level > 6)
                {
                    Result.BlockChance = 60 + (5 * Level / 3);
                }
            }
            if (CheckSynergy.HasMutation("Amphibious"))
            {
                if (Level == 1)
                {
                    Result.SwimSpeed = 50;
                }
                if (Level == 2)
                {
                    Result.SwimSpeed = 100;
                }
                if (Level == 3)
                {
                    Result.SwimSpeed = 125;
                }
                if (Level == 4)
                {
                    Result.SwimSpeed = 150;
                }
                if (Level == 5)
                {
                    Result.SwimSpeed = 175;
                }
                if (Level == 6)
                {
                    Result.SwimSpeed = 200;
                }
                if (Level == 7)
                {
                    Result.SwimSpeed = 200 + (ParentObject.StatMod("Strength") * 2);
                }
                if (Level == 8)
                {
                    Result.SwimSpeed = 210 + (ParentObject.StatMod("Strength") * 2);
                }
                if (Level == 9)
                {
                    Result.SwimSpeed = 215 + (ParentObject.StatMod("Strength") * 2);
                }
                if (Level >= 10)
                {
                    Result.SwimSpeed = 215 + (ParentObject.StatMod("Strength") * 3);
                }
            }
            return Result;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "AIGetOffensiveMutationList");
            Object.RegisterPartEvent(this, "PerformingMeleeAttack");
            Object.RegisterPartEvent(this, "GetDisplayName");
            Object.RegisterPartEvent(this, "BeginTakeAction");
            Object.RegisterPartEvent(this, "Unequipped");
            Object.RegisterPartEvent(this, "Equipped");
            Object.RegisterPartEvent(this, "MutationAdded");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "MutationAdded")
            {
                Body pBody = ParentObject.GetPart("Body") as Body;
                if (pBody != null)
                {
                    BodyPart Part = pBody.GetFirstPart(BodyPartType);
                    if (Part != null)
                    {
                        ParentObject.FireEvent(Event.New("CommandForceUnequipObject", "BodyPart", Part));
                        if (BaseTail == null)
                        {
                            BaseTail = GameObject.create("BaseThickTail");
                        }

                        Armor armor = BaseTail.GetPart("Armor") as Armor;
                        armor.WornOn = Part.Type;
                        Event @event = Event.New("CommandForceEquipObject");
                        @event.SetParameter("Object", BaseTail);
                        @event.SetParameter("BodyPart", Part);
                        @event.SetSilent(Silent: true);
                        ParentObject.FireEvent(@event);
                    }
                }
                if (ParentObject.HasObjectInInventory("BaseThickTail"))
                {
                    ParentObject.FindObjectInInventory("BaseThickTail").Destroy(null, true, false);
                }
            }
            return base.FireEvent(E);
        }

        public override bool WantTurnTick()
        {
            return true;
        }

        public override bool WantTenTurnTick()
        {
            return true;
        }

        public override bool WantHundredTurnTick()
        {
            return true;
        }

        public override void TurnTick(long TurnNumber)
        {

        }

        public override void TenTurnTick(long TurnNumber)
        {

        }

        public override void HundredTurnTick(long TurnNumber)
        {
            try
            {// AddPlayerMessage("This is Working: Sorta");
                Body pBody = ParentObject.GetPart("Body") as Body;
                if (pBody != null)
                {
                    BodyPart Part = pBody.GetFirstPart(BodyPartType);
                    if (Part != null)
                    {
                        ParentObject.FireEvent(Event.New("CommandForceUnequipObject", "BodyPart", Part));
                        if (BaseTail == null)
                        {
                            BaseTail = GameObject.create("BaseThickTail");
                        }

                        Armor armor = BaseTail.GetPart("Armor") as Armor;
                        armor.WornOn = Part.Type;
                        Event @event = Event.New("CommandForceEquipObject");
                        @event.SetParameter("Object", BaseTail);
                        @event.SetParameter("BodyPart", Part);
                        @event.SetSilent(Silent: true);
                        ParentObject.FireEvent(@event);
                    }
                }
            }
            catch
            {

            }
        }
    }

}
