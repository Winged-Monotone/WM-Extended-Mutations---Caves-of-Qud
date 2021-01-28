using System;
using XRL;
using XRL.World;
namespace XRL.World.Parts
{
    [Serializable]
    public class PsionicWeapon : IModification
    {
        public PsionicWeapon()
        {

        }
        public PsionicWeapon(int Tier)
            : base(Tier)
        {

        }
        public override void ApplyModification(GameObject Object)
        {
            Object.RequirePart<MeleeWeapon>().Stat = "Ego";
            Object.RemovePart("TinkerItem");
            IncreaseComplexityIfComplex(1);
        }
        public override bool SameAs(IPart p)
        {
            return false;
        }
        public override bool ModificationApplicable(GameObject Object)
        {
            return Object.HasPart("MeleeWeapon");
        }
        public override bool WantEvent(int ID, int cascade)
        {
            if (!base.WantEvent(ID, cascade) && ID != CanBeModdedEvent.ID && ID != GetDisplayNameEvent.ID && ID != GetMaximumLiquidExposureEvent.ID && ID != GetShortDescriptionEvent.ID && ID != GetShortDisplayNameEvent.ID && ID != ModificationAppliedEvent.ID)
            {
                return ID == ObjectCreatedEvent.ID;
            }
            return true;
        }
        public override bool HandleEvent(GetMaximumLiquidExposureEvent E)
        {
            E.PercentageReduction = 100;
            return false;
        }
        public override bool HandleEvent(CanBeModdedEvent E)
        {
            return false;
        }

        public override bool HandleEvent(IDisplayNameEvent E)
        {
            if (!ParentObject.Understood() || !ParentObject.HasProperName)
            {
                E.AddAdjective("{{psionic|psionic}}");
            }
            return true;
        }

		
    }
}