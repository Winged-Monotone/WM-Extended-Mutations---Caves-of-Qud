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
using System.Reflection;

namespace XRL.World.Parts.Mutation
{

    [Serializable]
    public class Ovipositor : BaseDefaultEquipmentMutation
    {
        public GameObject Mother;
        public string SpawnBlueprint = "OvipositorEgg";
        public Guid ActivatedAbilityID;
        const int PlaceHolder = 20000;
        // Make sure to close out your stuff via new list<string>(), these crash if you 
        public List<string> CollectedGeneSpice = new List<string>();

        public Ovipositor()
        {
            DisplayName = "Ovipositor";
        }
        public override string GetDescription()
        {
            return "You can periodically lay eggs, and spawn drones of yourself who will do your bidding.";
        }
        public override string GetLevelText(int Level)
        {
            return "Following a three month gestation period, you may lay eggs that develop into drones that take on aspects of yourself at the time they were lain.\n";
        }
        public override bool CanLevel()
        {
            return false;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == GetWaterRitualLiquidEvent.ID;
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
            go.RegisterPartEvent((IPart)this, "ShowConversationChoices");
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
            else if (E.ID == "ShowConversationChoices")
            {

                var eCurrentNode = E.GetParameter<ConversationNode>("CurrentNode");
                var eChoice = E.GetParameter<List<ConversationChoice>>("Choices");


                if (eCurrentNode.ID == "*waterritual")
                {
                    var eField = typeof(WaterRitualNode).GetField("currentInitializedSpeaker", BindingFlags.Static | BindingFlags.NonPublic);
                    var eSpeaker = eField?.GetValue(null) as GameObject;

                    var choice = new ConversationChoice();

                    choice.ParentNode = eCurrentNode;
                    choice.GotoID = choice.ParentNode.ID;
                    choice.Text = "{{M|Take on this individuals' gene-spice?}}";

                    choice.onAction = () =>
                    {
                        var SpeakerProperty = eSpeaker.HasIntProperty("GeneSpiced");

                        eSpeaker.SetIntProperty("GeneSpiced", 1);

                        if (!SpeakerProperty)
                        {
                            var Genes = eSpeaker.GetPart<Mutations>();
                            var GeneList = Genes.MutationList;

                            foreach (var M in GeneList)
                            {
                                var mStringed = M.Name;

                                CollectedGeneSpice.Add(mStringed);
                            }

                            Popup.Show("{{M|This will spice up your next brood.}}");
                        }

                        return !SpeakerProperty;
                    };
                    if (!eSpeaker.HasIntProperty("GeneSpiced"))
                    {
                        eChoice.Add(choice);
                        choice.ParentNode.Choices.Sort();
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

                var mBody = GO.GetPart<Body>();
                var mBodypart = mBody.GetBody();
                var mTail = mBodypart.AddPartAt(Base: "Tail", DefaultBehavior: "Ovipositor", InsertAfter: "Feet", OrInsertBefore: "Hands");
                mTail.DefaultBehaviorBlueprint = "Ovipositor";
                mTail.DefaultBehavior = TailObj;
                mBody.UpdateBodyParts();
            }
            else
            {
                return false;
            }
            CooldownMyActivatedAbility(ActivatedAbilityID, PlaceHolder, ParentObject);
            ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
            ActivatedAbilityID = activatedAbilities.AddAbility(Name: "Lay Egg", Command: "CommandLayEgg", Class: "Physical Mutation", Description: "Lay an egg.", Icon: "O", Cooldown: PlaceHolder);
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
                    var eBody = ParentObject.GetPart<Body>();
                    var eBodyPart = eBody.GetBody();
                    var eTail = eBodyPart.GetFirstPart("Tail");
                    eTail.Equipped.ForceUnequipAndRemove();
                    eBodyPart.RemovePart(eTail);
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