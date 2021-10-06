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
    public class Disintegrating : Effect
    {
        public Disintegrating() : base()
        {
            base.DisplayName = "{{red|Disintegrating}}";
        }

        public Disintegrating(int Duration) : this()
        {
            base.Duration = Duration;
        }

        public override string GetDetails()
        {
            PsychoplethoricDeterioration Psychopleth = Object.GetPart<PsychoplethoricDeterioration>();
            return "Your form is atomizing before your eyes ... it won't last long.\n\n"
            + "Your physical form {{red|rapidly disintegrates}}, every {{light blue|600}} turns, you must pass a {{light blue|Toughness Saving Throw}} at difficulty {{light blue|28 + " + Object.Statistics["Level"].BaseValue + "}} or lose {{red|0-3}} points of your maximum HP. Using an ubernostrum injector will partially regenerate your lost maximum HP, however continued use on the same husk becomes less effective.\n\n"
            + "Turns Til Decay Tick: {{purple|" + Psychopleth.DecayingFormDurationCycle + "}}";
        }

        public override bool Apply(GameObject Object)
        {
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
            }

            return base.FireEvent(E);
        }
    }
}