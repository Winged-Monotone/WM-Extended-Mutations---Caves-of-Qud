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
    public class Saprostymie : Effect
    {

        public Saprostymie() : base()
        {
            base.DisplayName = "{{brown|Saprostymie}}";
        }
        public Saprostymie(int Duration)
        {
            base.Duration = Duration;
        }

        public override string GetDetails()
        {
            return "A fungas infection is causing blockages within your chitin, your agility will be severely hampered until it is cured.\n"
            + "{{red|-" + 10 + " Agility.}}\n\n";
        }


        public override bool Apply(GameObject Object)
        {
            StatShifter.SetStatShift("Agility", -10);
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

                // AddPlayerMessage("EndTurn - Effect");
                // AddPlayerMessage("" + Duration + "");
            }

            return base.FireEvent(E);
        }
    }
}