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

        // public override bool HandleEvent(GetWaterRitualLiquidEvent E)
        // {
        //     // AddPlayerMessage("1");
        //     var GeneSpicer = E.Target;

        //     if (E.Actor.IsPlayer() && GeneSpicer != null)
        //     {
        //         if (Popup.ShowYesNo("Take on this individuals gene-spice, this will improve your next brood by taking on the mutation aspects of your water-sib?", false, DialogResult.Yes) == DialogResult.Yes)
        //         {
        //             var Genes = GeneSpicer.GetPart<Mutations>();
        //             var GeneList = Genes.MutationList;

        //             foreach (var M in GeneList)
        //             {
        //                 var mStringed = M.ToString();

        //                 CollectedGeneSpice.Add(mStringed);
        //             }

        //             Popup.Show("This will spice up your next brood.");
        //         }
        //         else
        //         {

        //         }
        //     }
        //     return base.HandleEvent(E);
        // }

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

                // AddPlayerMessage("1");

                var eCurrentNode = E.GetParameter<ConversationNode>("CurrentNode");
                var eChoice = E.GetParameter<List<ConversationChoice>>("Choices");


                // AddPlayerMessage("2");

                if (eCurrentNode.ID == "*waterritual")
                {
                    var field = typeof(WaterRitualNode).GetField("currentInitializedSpeaker", BindingFlags.Static | BindingFlags.NonPublic);
                    var speaker = field?.GetValue(null) as GameObject;

                    var choice = new ConversationChoice();

                    choice.ParentNode = eCurrentNode;
                    choice.GotoID = choice.ParentNode.ID;
                    choice.Text = "{{M|Take on this individuals' gene-spice?}}";

                    choice.onAction = () =>
                    {
                        var SpeakerProperty = speaker.HasIntProperty("GeneSpiced");

                        speaker.SetIntProperty("GeneSpiced", 1);

                        if (!SpeakerProperty)
                        {
                            var Genes = speaker.GetPart<Mutations>();
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
                    if (!speaker.HasIntProperty("GeneSpiced"))
                    {
                        eChoice.Add(choice);
                        choice.ParentNode.SortEndChoicesToEnd();
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