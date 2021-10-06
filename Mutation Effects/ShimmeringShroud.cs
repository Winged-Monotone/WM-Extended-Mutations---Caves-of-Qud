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
    public class ShimmeringShroud : Effect
    {
        public int Potency = 1;
        public GameObject Owner;


        public ShimmeringShroud() : base()
        {
            base.DisplayName = "{{yellow|Shimmering Shroud}}";
        }
        public ShimmeringShroud(int Duration, GameObject Owner)
        {
            this.Owner = Owner;

            if (Potency > 0)
            { base.Duration = 1; }

            base.DisplayName = "{{yellow|Shimmering Shroud}}";
        }
        public override string GetDetails()
        {
            return "Something is altering " + Object.its + " resistances to the elements.\n\n"
            + "Duration: " + Duration;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == EndTurnEvent.ID;
        }

        public override void Register(GameObject go)
        {
            go.RegisterEffectEvent((Effect)this, "BeginTakeAction");
        }

        public override void Unregister(GameObject go)
        {
            go.UnregisterEffectEvent((Effect)this, "BeginTakeAction");
        }

        public override bool FireEvent(Event E)
        {
            return base.FireEvent(E);
        }

        public override bool Apply(GameObject Object)
        {
            return true;
        }


        public override void Remove(GameObject Object)
        {
            // XDidY(Object, "emerge", "from the depths", "!");
            StatShifter.RemoveStatShifts();
        }
    }
}