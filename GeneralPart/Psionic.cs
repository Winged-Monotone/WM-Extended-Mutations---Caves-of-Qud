// XRL.World.Parts.Metal
using System;
using XRL.World;

namespace XRL.World.Parts
{
    [Serializable]
    public class Psionic : IPart
    {
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == AfterGameLoadedEvent.ID || ID == BeforeApplyDamageEvent.ID || ID == GetMaximumLiquidExposureEvent.ID || ID == ModificationAppliedEvent.ID || ID == ObjectCreatedEvent.ID;
        }

        public override bool HandleEvent(AfterGameLoadedEvent E)
        {
            var AccessBodyPartDescriptors = ParentObject.GetPart("BodyPartCategory");
            return true;
        }

        public override bool HandleEvent(GetMaximumLiquidExposureEvent E)
        {
            E.PercentageReduction = 0;
            return true;
        }

        public override bool HandleEvent(BeforeApplyDamageEvent E)
        {
            if (E.Object == ParentObject && !ParentObject.HasTag("Creature") && E.Damage.Amount < 0)
            {
                E.Damage.Amount /= 2;
            }
            return true;
        }

        public override bool HandleEvent(ModificationAppliedEvent E)
        {
            MakeNonflammable();
            return true;
        }

        public override bool HandleEvent(TransparentToEMPEvent E)
        {
            return false;
        }

        public override bool HandleEvent(ObjectCreatedEvent E)
        {
            MakeNonflammable();
            return true;
        }

        public void MakeNonflammable()
        {
            if (!ParentObject.HasTag("Creature") && ParentObject.pPhysics != null)
            {
                ParentObject.pPhysics.FlameTemperature = ParentObject.pPhysics.VaporTemperature + 1;
            }
        }
    }
}