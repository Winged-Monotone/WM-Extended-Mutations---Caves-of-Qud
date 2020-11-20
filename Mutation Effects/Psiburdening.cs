using System;
using System.Collections.Generic;
using System.Text;
using XRL.Core;
using XRL.UI;
using XRL.World.Parts.Mutation;
using XRL.World.Parts;

namespace XRL.World.Effects
{
    [Serializable]
    public class Psiburdening : Effect
    {

        public Psiburdening() : base()
        {
            base.DisplayName = "{{purple|psi-exhaustion}}";
        }
        public Psiburdening(int Duration)
        {
            base.Duration = Duration;
        }

        public override string GetDetails()
        {
            return "You are straining your mental powers!\n"
            + "{{red|" + -8 + " Willpower.}}\n\n"
            + "" + "Turns Remaining: " + Duration;
        }


        public override bool Apply(GameObject Object)
        {
            StatShifter.SetStatShift("Willpower", -8);
            return true;
        }

        public override void Remove(GameObject Object)
        {
            StatShifter.RemoveStatShifts();
        }

        public override void Register(GameObject go)
        {
            go.RegisterEffectEvent((Effect)this, "EndTurn");
            base.Register(Object);
        }

        public override void Unregister(GameObject go)
        {
            go.UnregisterEffectEvent((Effect)this, "EndTurn");
            base.Unregister(Object);
        }

        public override bool FireEvent(Event E)
        {

            if (E.ID == "EndTurn")
            {
                --Duration;
                // AddPlayerMessage("EndTurn - Effect");
                // AddPlayerMessage("" + Duration + "");
            }

            return base.FireEvent(E);
        }
    }
}