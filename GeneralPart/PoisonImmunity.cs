using System;

namespace XRL.World.Parts
{
    [Serializable]
    public class PoisonImmunity : IPart
    {
        public override bool SameAs(IPart p)
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "BeforeApplyDamage");
            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (!(E.ID == "BeforeApplyDamage"))
            {
                return true;
            }
            Damage damage = E.GetParameter("Damage") as Damage;
            if (damage.HasAttribute("Poison"))
            {
                damage.Amount = 0;
                return false;
            }
            return true;
        }
        public int CloneCooldown;
    }
}
