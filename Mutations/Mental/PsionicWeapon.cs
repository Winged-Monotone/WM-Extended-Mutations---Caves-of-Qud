using System;
using XRL;
using XRL.World;
using XRL.World.Parts.Mutation;
using Qud.API;
using System.Collections.Generic;
using System.Globalization;
using XRL.Core;
using XRL.Language;
using XRL.UI;




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
            var ParentObject = Object.Equipped;
            var ParentsPsiMar = ParentObject.GetPart<Psychomateriartis>();

            Object.RequirePart<MeleeWeapon>().Stat = "Ego";
            // Create some kind of algorythm that takes the creatures ego and increases it based on the level, think of it as the more you level the mutation, the more you get access to your ego score.

            var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;
            var ParentsLevel = ParentObject.Statistics["Level"].BaseValue;

            var WeaponProps = Object.GetPart<MeleeWeapon>();

            WeaponProps.Ego = (int)Math.Floor(ParentsEgo * (ParentsPsiMar.Level * 0.1));
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
                return ID == ObjectCreatedEvent.ID
                        || ID == StatChangeEvent.ID
                        || ID == AwardedXPEvent.ID;
            }
            return true;
        }

        public void UpdatePsionicProperties()
        {
            try
            {
                var ObjectOwner = ParentObject.Equipped;
                var ParentsPsiMar = ObjectOwner.GetPart<Psychomateriartis>();

                var ParentsEgo = ObjectOwner.Statistics["Ego"].Modifier;
                var ParentsLevel = ObjectOwner.Statistics["Level"].BaseValue;

                var WeaponProps = ParentObject.GetPart<MeleeWeapon>();

                WeaponProps.Ego = (int)Math.Floor(ParentsEgo * (ParentsPsiMar.Level * 0.1));
            }
            catch
            {

            }
        }
        public override void Register(GameObject Object)
        {
            ParentObject.RegisterPartEvent((IPart)this, "SyncMutationLevels");
            ParentObject.RegisterPartEvent((IPart)this, "AfterLevelGainedEarly");
            ParentObject.RegisterPartEvent((IPart)this, "PsionicWeaponManifestedEvent");
            Object.RegisterPartEvent(this, "SyncMutationLevels");
            Object.RegisterPartEvent(this, "AfterLevelGainedEarly");
            Object.RegisterPartEvent(this, "PsionicWeaponManifestedEvent");

            Object.RegisterPartEvent(this, "EndTurn");


            base.Register(Object);
        }
        public override bool FireEvent(Event E)
        {
            if ((E.ID == "SyncMutationLevels" || E.ID == "AfterLevelGainedEarly"))
            {
                AddPlayerMessage("Sync Mutation or Level Gained early, Psionics Props update fire");
                UpdatePsionicProperties();
            }
            else if (E.ID == "PsionicWeaponManifestedEvent")
            {
                var ParentsPsiMar = ParentObject.GetPart<Psychomateriartis>();

                var ColorSelected = E.GetStringParameter("ColorChoice");
                var WeaponManifested = E.GetGameObjectParameter("ManifestedWeapon");

                string newName = Popup.AskString("Give your bonded-weapon a name.", "", 99);
                if (!String.IsNullOrEmpty(newName))
                {
                    WeaponManifested.DisplayName = newName;
                    WeaponManifested.SetIntProperty("ProperNoun", 1);
                }

                WeaponManifested.pRender.TileColor = ParentsPsiMar.GetWeaponTileColor(ColorSelected);
            }
            else if (E.ID == "EndTurn")
            {
                AddPlayerMessage("EndTurnCheck, Is this Working?");

            }

            return base.FireEvent(E);
        }
        public override bool HandleEvent(AwardedXPEvent E)
        {
            AddPlayerMessage("Updating Weapon Ego Score: AwardXP");
            UpdatePsionicProperties();
            return false;
        }
        public override bool HandleEvent(StatChangeEvent E)
        {
            AddPlayerMessage("Updating Weapon Ego Score: StatChange");
            UpdatePsionicProperties();
            return false;
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
        public static string GetDescription(int Tier)
        {
            return "Psionic: This weapon uses the wielder's Ego modifier for penetration bonus instead of Strength mod and attacks MA instead of AV. It will dissipate from the corporeal realm after some use when used by anyone but its former master.";
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.AppendRules(GetDescription(Tier));
            return true;
        }
    }
}