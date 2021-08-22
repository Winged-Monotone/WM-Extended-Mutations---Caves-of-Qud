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
    public class wmSubmerged : Effect
    {
        public wmSubmerged() : base()
        {
            base.DisplayName = "{{B|submerged}}";
            Duration = 1;

        }

        public wmSubmerged(int Duration) : this()
        {
            base.Duration = Duration;
            // base.Duration = 1;
        }

        public override string GetDetails()
        {
            return "Can't be attacked in melee by non-submerged creatures.";
        }

        public override bool Apply(GameObject Object)
        {
            XDidY(Object, "dive", "into the depths", "!");
            return true;
        }

        public override void Remove(GameObject Object)
        {
            XDidY(Object, "emerge", "from the depths", "!");
        }

        public override bool Render(RenderEvent E)
        {
            if (base.Object.GetEffect(base.ClassName) != this)
            {
                return true;
            }
            int num = XRLCore.CurrentFrame % 60;
            if (num > 5 && num < 15)
            {
                E.Tile = null;
                E.RenderString = "\u0019";
                E.ColorString = "&C";
                return false;
            }
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            if (!base.WantEvent(ID, cascade))
            {
                return ID == GetDisplayNameEvent.ID;
            }
            return true;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (base.Object.GetEffect(base.ClassName) == this)
            {
                E.AddTag("[{{C|submerged}}]");
            }
            return true;
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
            if (E.ID == "BeginTakeAction")
            {
                if (Object.HasEffect("Submerged"))
                {
                    Duration--;
                }
            }
            return true;
        }

    }
}
