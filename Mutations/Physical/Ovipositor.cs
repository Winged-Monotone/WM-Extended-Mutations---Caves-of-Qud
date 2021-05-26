using System;
using Qud.API;
using XRL.Core;
using XRL.Messages;
using XRL.World.Capabilities;
using System.Collections.Generic;
using System.Threading;
using ConsoleLib.Console;
using UnityEngine;
using XRL.Language;
using XRL.Rules;
using XRL.UI;

namespace XRL.World.Parts.Mutation
{

    [Serializable]
    public class Ovipositor : BaseDefaultEquipmentMutation
    {

        public string BodyPartType = "Tail";
        public string TailObjectId;
        public string AdditionsManagerID => ParentObject.id + "::ThickTail::Add";
        public string ChangesManagerID => ParentObject.id + "::ThickTail::Change";
        public GameObject TailObject = null;
        public GameObject Mother;
        public string SpawnBlueprint = "OvipositorEgg";
        public Guid ActivatedAbilityID;

        public int TailID;
        const int PlaceHolder = 20000;

        public Ovipositor()
        {
            DisplayName = "Ovipositor";
        }
        public override string GetDescription()
        {
            return "You can periodically lay eggs, and spawn drones of yourself who will do your bidding.\n";
        }
        public override string GetLevelText(int Level)
        {
            return "Following a three month gestation period, you may lay eggs that develop into drones that take on aspects of yourself at the time they were lain.\n";
        }
        public override bool CanLevel()
        {
            return false;
        }

        public void BirthEgg()
        {
            Cell currentCell = ParentObject.GetCurrentCell();
            GameObject gameObject = currentCell.AddObject(SpawnBlueprint);
            OvipositorEgg Spawn = gameObject.GetPart<OvipositorEgg>();
            Spawn.Mother = ParentObject;
            XDidY(ParentObject, "lay", "an egg", ".");
        }

        public void EggParent()
        {
            Mother = ParentObject;
        }
        public String MotherID()
        {
            var MotherID = ParentObject.id;
            return MotherID;
        }
        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "EndTurn");
            go.RegisterPartEvent((IPart)this, "CommandLayEgg");
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandLayEgg")
            {
                {
                    if (!this.ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap())
                    {
                        BirthEgg();
                        CooldownMyActivatedAbility(ActivatedAbilityID, PlaceHolder, ParentObject);
                    }
                }

            }

            return base.FireEvent(E);
        }

        public string ManagerID
        {
            get
            {
                return ParentObject.id + "::Ovipositor::Add";

            }
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Body SourceBody = GO.GetPart<Body>();
            if (SourceBody != null)
            {
                GameObject TailObj = GameObject.create("Ovipositor");

                var body = GO.GetPart<Body>();
                var core = body.GetBody();
                var tail = core.AddPartAt(Base: "Tail", DefaultBehavior: "Ovipositor", InsertAfter: "Feet", OrInsertBefore: "Hands");
                tail.DefaultBehaviorBlueprint = "Ovipositor";
                tail.DefaultBehavior = TailObj;
                // Armor part = TailObj.GetPart<Armor>();
                // part.AV = 1;
                // part.DV = 0;
                body.UpdateBodyParts();
            }
            CooldownMyActivatedAbility(ActivatedAbilityID, PlaceHolder, ParentObject);
            ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
            ActivatedAbilityID = activatedAbilities.AddAbility("Lay Egg", "CommandLayEgg", "Physical Mutation", "Lay an egg.", "O", null, false, false, false, false, false, false, false, true, PlaceHolder);
            ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            try
            {
                Body SourceBody = GO.GetPart<Body>();
                if (SourceBody != null)
                {
                    var body = ParentObject.GetPart<Body>();
                    var core = body.GetBody();
                    var tail = core.GetFirstPart("Tail");
                    tail.Equipped.ForceUnequipAndRemove();
                    core.RemovePart(tail);

                }
            }
            catch
            {

            }

            return base.Unmutate(GO);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
    }
}