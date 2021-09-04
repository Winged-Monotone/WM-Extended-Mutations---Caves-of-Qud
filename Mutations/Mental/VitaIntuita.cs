
using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Effects;
using XRL.World.Parts;
using ConsoleLib.Console;
using XRL.Core;
using XRL.Rules;
using XRL.World.Capabilities;
using System.Linq;
using System.Text;

using XRL.Messages;
using XRL.UI;

using XRL.World.AI.GoalHandlers;
using XRL.World.Parts.Mutation;

using UnityEngine;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class VitaIntuita : BaseMutation
    {
        public Guid ActivatedAbilityID;
        public VitaIntuita()
        {
            this.DisplayName = "Vita Intuita";
            this.Type = "Mental";
        }
        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }
        public override bool CanLevel()
        {
            return false;
        }
        public override string GetDescription()
        {
            return "Your intricate mind pierces the laylines of the meta-plane, intuit unforeseen knowledge with your piercing gaze.";
        }
        public override string GetLevelText(int Level)
        {
            return "{{gray|Gain meta-data through this special look command.}}";
        }
        public override bool Mutate(GameObject GO, int Level)
        {

            this.ActivatedAbilityID = base.AddMyActivatedAbility(Name: "In-Gaze", Command: "CommandSpecialLook", Class: "Mental Mutation", Description: "Gain meta-data through this special look command. [Must be activated at least once in each parasang, creature's that spawn as new entities on the map will also require you activate it again.]", Icon: "o", Cooldown: 10);

            return base.Mutate(GO, Level);
        }

        public void VitaIntuitaGaze()
        {
            var CurrentZone = ParentObject.CurrentZone;
            var ZonesCells = CurrentZone.GetCells();

            var CellQuery = ZonesCells.Where(Cell => Cell.HasObjectWithPart("Brain") || Cell.HasObjectWithPart("Combat") && Cell.HasCombatObject() && !Cell.HasObject(ParentObject));

            foreach (var c in CellQuery)
            {
                var ObjectsInCell = c.GetObjectsInCell();

                var ObjectQuery = ObjectsInCell.Where(Obj => !Obj.HasPart("Brain") || Obj.HasPart("Combat"));

                foreach (var o in ObjectQuery)
                {
                    if (!o.HasPart("IntuitLook"))
                    {
                        o.AddPart<IntuitLook>();
                    }
                }
            }

            PlayWorldSound("intuit");
            UI.Look.ShowLooker(81, ParentObject.CurrentCell.X, ParentObject.CurrentCell.Y);
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CommandSpecialLook");

            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {

            if (E.ID == "CommandSpecialLook")
            {
                VitaIntuitaGaze();
            }

            return base.FireEvent(E);
        }
    }
}