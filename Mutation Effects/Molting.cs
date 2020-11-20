using System;
using XRL.UI;
using XRL.Core;
using System.Collections.Generic;
using System.Text;
using XRL.World.Parts;

namespace XRL.World.Effects
{
    [Serializable]

    public class Molting : Effect
    {
        public int saveTarget;
        public int saveTargetTurnDivisor;
        public int turns;


        public Molting() : base()
        {
            base.DisplayName = "Molting";
        }

        public Molting(int Duration)
        {
            base.Duration = 1;
            saveTarget = 20;
            saveTargetTurnDivisor = 5;
            turns = 0;
        }

        public override string GetDetails()
        {
            return "You are currently molting your exoskeleton.\n";
        }

        public override void Register(GameObject go)
        {
            go.RegisterEffectEvent((Effect)this, "MovementModeChanged");
            go.RegisterEffectEvent((Effect)this, "CanChangeMovementMode");
            go.RegisterEffectEvent((Effect)this, "EndTurn");
            go.RegisterEffectEvent((Effect)this, "IsMobile");
            go.RegisterEffectEvent((Effect)this, "LeaveCell");
            go.RegisterEffectEvent((Effect)this, "BeginTakeAction");
            base.Register(Object);
        }

        public override void Unregister(GameObject go)
        {
            go.UnregisterEffectEvent((Effect)this, "MovementModeChanged");
            go.UnregisterEffectEvent((Effect)this, "CanChangeMovementMode");
            go.UnregisterEffectEvent((Effect)this, "EndTurn");
            go.UnregisterEffectEvent((Effect)this, "IsMobile");
            go.UnregisterEffectEvent((Effect)this, "LeaveCell");
            go.UnregisterEffectEvent((Effect)this, "BeginTakeAction");
            base.Unregister(Object);
        }

        public override bool Apply(GameObject Object)
        {
            return true;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "IsMobile")
            {
                // AddPlayerMessage("IsMobile Works.");
                if (base.Duration > 0)
                    return true;
            }

            else if (E.ID == "BeginTakeAction")
            {
                // AddPlayerMessage("BeginTakeAction Works.");
                if (this.turns > 0)
                {
                    int effectiveSaveTarget = this.GetEffectiveSaveTarget();
                    if (Object.MakeSave("Strength", effectiveSaveTarget, null, null, "Molting"))
                    {
                        Object.RemoveEffect(this);
                    }
                }
                this.turns++;
            }

            else if (E.ID == "LeaveCell")
            {
                // AddPlayerMessage("LeaveCell Works.");
                if (base.Duration > 0 && !E.HasParameter("Teleporting") && !E.HasParameter("Dragging") && !E.HasParameter("Forced"))
                {
                    if (base.Object.IsPlayer())
                    {
                        Popup.Show("You cannot move, you are molting.");
                    }
                    return false;
                }
            }

            else if (E.ID == "EndTurn")
            {
                // AddPlayerMessage("EndTurn Works.");
                if (base.Duration > 0)
                {
                    Effect.AddPlayerMessage("You are molting.");
                }
                else
                {
                    Object.RemoveEffect(this);
                }
            }

            else if (E.ID == "CanChangeMovementMode")
            {
                // AddPlayerMessage("CanChangeMovementMode Works.");
                if (this.Duration > 0 && E.GetIntParameter("Involuntary", 0) <= 0)
                {
                    if (E.GetIntParameter("ShowMessage", 0) > 0 && Object.IsPlayer())
                    {
                        Popup.Show("You are molting.", true);
                    }
                    return false;
                }
            }

            else if (E.ID == "MovementModeChanged")
            {
                // AddPlayerMessage("MovementModeChanged Works.");
                Object.RemoveEffect(this);
            }

            return base.FireEvent(E);
        }

        public override bool Render(RenderEvent E)
        {
            if (this.Duration > 0)
            {
                int num = XRLCore.CurrentFrame % 60;
                if (num > 25 && num < 35)
                {
                    E.Tile = null;
                    E.RenderString = "M";
                    E.ColorString = "&m";
                }
            }
            return base.Render(E);
        }

        public int GetEffectiveSaveTarget()
        {
            int num = this.saveTarget;
            if (this.saveTargetTurnDivisor != 0)
            {
                num -= this.turns / this.saveTargetTurnDivisor;
            }
            return num;
        }



        public override void Remove(GameObject Object)
        {
            Object.FireEvent(Event.New("CommandMolt", 0, 0, 0));
        }

    }
}