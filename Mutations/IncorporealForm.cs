using System;
using XRL.World.Capabilities;
using XRL.World.Effects;
using AiUnity.NLog.Core.LayoutRenderers;
using Qud.API;
using System.Collections.Generic;
using System.Globalization;
using XRL.Core;
using XRL.Language;
using XRL.UI;
using System.Linq;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class IncorporealForm : BaseMutation
    {
        public bool ReversePhaseActive = false;
        public int UnphasedDuration = 0;

        public IncorporealForm()
        {
            this.DisplayName = "Incorporeal Form";
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            {
                Object.RegisterPartEvent(this, "EndTurn");
                Object.RegisterPartEvent(this, "BeginTakeAction");
                Object.RegisterPartEvent(this, "EnteredCell");
                Object.RegisterPartEvent(this, "CommandPhaseIn");
                Object.RegisterPartEvent(this, "CommandPhaseOut");
                base.Register(Object);
            }
            base.Register(Object);
        }

        public override string GetDescription()
        {
            return "Your physical form was banished to the aether, forever.";
        }

        public override string GetLevelText(int Level)
        {
            return "You are permanently in the phase.";
        }

        public static readonly List<string> InteractionCommands = new List<string>()
        {
            "Open",
            "Close",
            "SitOnChair",
            "StandUpFromChair",
            "Harvest",
            "Butcher",
            "LightCampfire",
            "Cook",
            "DrawWithCrayons",
            "RifleThroughGarbage",
            "SmokeHookah",
            "ActivatePortableWall",
            "ActivateSpiralBorer",
            "Fill",
            "ArmMine",
            "DisarmMine",
            "Detonate",
            "FlipSwitch",
            "Eat",
            "Drink",
        };

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
             || ID == InventoryActionEvent.ID;
        }

        public override bool HandleEvent(InventoryActionEvent E)
        {
            AddPlayerMessage("1a");
            if (InteractionCommands.Contains<string>(E.Command) && !ParentObject.Inventory.HasObjectDirect(E.ObjectTarget) && ParentObject.HasEffect("Phased"))
            {
                if (ParentObject.IsPlayer())
                {
                    AddPlayerMessage("You cannot interact with this while incorporeal.");
                    return false;
                }
            }
            return base.HandleEvent(E);
        }

        public override bool FireEvent(Event E)
        {
            if ((E.ID == "BeginTakeAction" || E.ID == "EnteredCell") && !this.ParentObject.HasEffect("Phased") && !this.ParentObject.HasEffect("RealityStabilized") && ReversePhaseActive == false)
            {
                this.ParentObject.ApplyEffect(new Phased(9999), null);
            }
            else if ((E.ID == "BeginTakeAction" || E.ID == "EnteredCell") && this.ParentObject.HasEffect("Phased") && !this.ParentObject.HasEffect("RealityStabilized") && ReversePhaseActive == true)
            {
                ParentObject.RemoveEffect(new Phased());
            }
            else if ((E.ID == "CommandPhaseOut"))
            {
                ReversePhaseActive = false;
                ParentObject.ApplyEffect(new Phased(9999));
            }
            else if ((E.ID == "CommandPhaseIn"))
            {
                var PhasedMutInstance = ParentObject.GetPart<Phasing>();
                UnphasedDuration = 6 + PhasedMutInstance.Level + 1;

                ReversePhaseActive = true;
                ParentObject.RemoveEffect("Phased");
            }
            else if ((E.ID == "EndTurn") && UnphasedDuration > 0)
            {
                --UnphasedDuration;
            }
            else if ((E.ID == "EndTurn") && UnphasedDuration <= 0 && ReversePhaseActive == true)
            {
                ReversePhaseActive = false;
                ParentObject.ApplyEffect(new Phased(9999));
            }
            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            Phased Phased = ParentObject.GetEffect("Phased") as Phased;
            if (ParentObject.HasEffect("Phased"))
            {
                ParentObject.RemoveEffect(Phased);
            }
            return base.Unmutate(GO);
        }
    }
}