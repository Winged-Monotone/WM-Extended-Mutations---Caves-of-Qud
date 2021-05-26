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

        public int SwingsRemaining;
        public GameObject OriginalOwner;
        public PsionicWeapon()
        {

        }
        public PsionicWeapon(int Tier)
            : base(Tier)
        {

        }
        public bool isOriginalOwner()
        {
            if (OriginalOwner == ParentObject)
            {
                return true;
            }
            else

                return false;
        }
        public override void ApplyModification(GameObject Object)
        {
            OriginalOwner = PsiHolder();

            var ParentsPsiMar = PsiHolder().GetPart<Psychomateriartis>();

            Object.RequirePart<MeleeWeapon>().Stat = "Ego";
            // Create some kind of algorythm that takes the creatures ego and increases it based on the level, think of it as the more you level the mutation, the more you get access to your ego score.

            var ParentsEgo = PsiHolder().Statistics["Ego"].Modifier;
            var ParentsLevel = PsiHolder().Statistics["Level"].BaseValue;

            var WeaponProps = Object.GetPart<MeleeWeapon>();

            // WeaponProps.Ego = (int)Math.Floor(ParentsEgo * (ParentsPsiMar.Level * 0.1));
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
            if (!base.WantEvent(ID, cascade) && ID != CanBeModdedEvent.ID && ID != GetDisplayNameEvent.ID && ID != GetMaximumLiquidExposureEvent.ID && ID != GetShortDescriptionEvent.ID && ID != GetDisplayNameEvent.ID && ID != ModificationAppliedEvent.ID)
            {
                return ID == ObjectCreatedEvent.ID
                        || ID == StatChangeEvent.ID
                        || ID == AwardedXPEvent.ID
                        || ID == OnDestroyObjectEvent.ID;
            }
            return true;
        }
        public GameObject PsiHolder()
        {
            GameObject holder = null;
            if (ParentObject.Equipped != null)
            {
                holder = ParentObject.Equipped;
            }
            else if (ParentObject.InInventory != null)
            {
                holder = ParentObject.InInventory;
            }
            if (holder == null)
            {
                throw new Exception("Couldn't find whoever's holding this object!");
            }
            return holder;
        }
        public void UpdatePsionicProperties()
        {
            var ParentsPsiMar = PsiHolder().GetPart<Psychomateriartis>();

            var ParentsEgo = PsiHolder().Statistics["Ego"].Modifier;
            var ParentsLevel = PsiHolder().Statistics["Level"].BaseValue;

            var WeaponMeleeProps = ParentObject.GetPart<MeleeWeapon>();
            var WeaponCurrency = ParentObject.GetIntProperty("Value");



            SwingsRemaining = 100 * (ParentsEgo);

            ParentObject.SetIntProperty("Value", (int)Math.Min(1, WeaponCurrency * 0.1));
            // WeaponMeleeProps.Ego = (int)Math.Floor(ParentsEgo * (ParentsPsiMar.Level * 0.1));

        }
        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "SyncMutationLevels");
            Object.RegisterPartEvent(this, "AfterLevelGainedEarly");
            Object.RegisterPartEvent(this, "PsionicWeaponManifestedEvent");
            Object.RegisterPartEvent(this, "WeaponAfterAttack");
            Object.RegisterPartEvent(this, "WeaponAfterAttackMissed");

            Object.RegisterPartEvent(this, "EndTurn");

            base.Register(Object);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "WeaponAfterAttack" || E.ID == "WeaponAfterAttackMissed")
            {
                if (ParentObject.EquippedProperlyBy() != OriginalOwner)
                {
                    SwingsRemaining--;
                    if (SwingsRemaining <= 0)
                    {
                        var ParentsPsiMar = PsiHolder().GetPart<Psychomateriartis>();

                        GameObject equipped = ParentObject.Equipped;
                        DidX("disappear", null, null, null, null, equipped);
                        ParentObject.ForceUnequipRemoveAndRemoveContents(Silent: true);
                        ParentObject.Destroy();
                        ParentsPsiMar.WeaponCounter -= 1;
                        if (equipped != null && equipped.IsValid() && !equipped.IsPlayer() && equipped.pBrain != null)
                        {
                            equipped.pBrain.PerformReequip();
                        }
                    }
                }
            }
            else if ((E.ID == "SyncMutationLevels" || E.ID == "AfterLevelGainedEarly"))
            {
                // AddPlayerMessage("Sync Mutation or Level Gained early, Psionics Props update fire");
                UpdatePsionicProperties();
            }
            else if (E.ID == "PsionicWeaponManifestedEvent")
            {
                var ParentsPsiMar = PsiHolder().GetPart<Psychomateriartis>();

                // var ColorSelected = E.GetStringParameter("ColorChoice");
                var WeaponManifested = E.GetGameObjectParameter("ManifestedWeapon");

                string newName = Popup.AskString("Give your bonded-weapon a name.", "", 99);
                if (!String.IsNullOrEmpty(newName))
                {
                    WeaponManifested.DisplayName = "{{psionic|psionic}} " + newName;
                    WeaponManifested.SetIntProperty("ProperNoun", 1);
                    ParentObject.id = ParentsPsiMar.PsiWeaponsID;
                }

                // WeaponManifested.pRender.TileColor = ParentsPsiMar.GetWeaponTileColor($"&{ColorSelected}");
                // WeaponManifested.pRender.ColorString = ParentsPsiMar.GetWeaponTileColor($"&{ColorSelected}");

            }
            else if (E.ID == "EndTurn")
            {
                // AddPlayerMessage("EndTurnCheck, current weapons ID: " + ParentObject.id);
            }

            return base.FireEvent(E);
        }
        public override bool HandleEvent(OnDestroyObjectEvent E)
        {
            var CheckObject = E.Object;

            if (CheckObject == ParentObject)
            {
                var ParentsPsiMar = PsiHolder().GetPart<Psychomateriartis>();
                if (ParentsPsiMar.WeaponCounter > 0)
                    ParentsPsiMar.WeaponCounter -= 1;
            }
            return false;
        }
        public override bool HandleEvent(AwardedXPEvent E)
        {
            // AddPlayerMessage("Updating Weapon Ego Score: AwardXP");
            UpdatePsionicProperties();
            return false;
        }
        public override bool HandleEvent(StatChangeEvent E)
        {
            // AddPlayerMessage("Updating Weapon Ego Score: StatChange");
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
        public override bool HandleEvent(GetDisplayNameEvent E)
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