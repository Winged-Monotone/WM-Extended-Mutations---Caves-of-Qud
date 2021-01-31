using Qud.API;
using System;
using System.Collections.Generic;
using System.Globalization;
using XRL.Core;
using XRL.Language;
using XRL.UI;



namespace XRL.World.Parts.Mutation
{
    [Serializable]

    public class Psychomateriartis : BaseMutation
    {
        public int WeaponCounter = 0;
        public Guid ManifestPsiWeaponActivatedAbilityID;
        public Guid ReturnPsiWeaponActivatedAbilityID;
        public string PsiWeaponsID => ParentObject.id + "::Psychomaterialus" + WeaponCounter;

        public static readonly List<string> MainOptions = new List<string>()
        {
            "Weave Psi-Weapon",
            "Dismiss Psi-Weapon",
            "Cancel"
        };
        public static readonly List<string> WeaponOptions = new List<string>()
        {
            "Long Sword",
            "Short Sword",
            "Great Sword",
            "Dagger",
            "Cudgel",
            "Warhammer",
            "Axe",
            "Great Axe"
        };
        public static readonly List<string> ColorOptions = new List<string>()
        {
            "Red",
            "Dark Red",
            "Blue",
            "Dark Blue",
            "Green",
            "Dark Green",
            "Yellow",
            "Magenta",
            "Dark Magenta",
            "Orange",
            "Dark Orange",
            "Brown",
            "Cyan",
            "Dark Cyan",
            "White",
            "Gray",
            "Dark Gray"
        };
        public void BeginWeaponManifestOptionList()
        {
            PlayWorldSound("beginmanifest");
            string mainChoice = GetChoice(MainOptions);

            if (!string.IsNullOrEmpty(mainChoice))
            {
                if (mainChoice == "Weave Psi-Weapon")
                {
                    string weaponChoice = GetChoice(WeaponOptions);
                    if (!string.IsNullOrEmpty(weaponChoice))
                    {
                        PlayWorldSound("ManifestWeapon");
                        string colorChoice = GetChoice(ColorOptions);

                        if (!string.IsNullOrEmpty(colorChoice))
                        {
                            // Popup.Show($"You chose to {mainChoice} with a {colorChoice} {weaponChoice}");
                            ManifestPsiWeapon(weaponChoice, colorChoice);
                        }
                    }
                }
                if (mainChoice == "Dismiss Psi-Weapon")
                {
                    PlayWorldSound("dismiss");
                    DismissPsiWeapon();
                }
            }
        }
        public static string GetChoice(List<string> valuesToShow)
        {
            int result = Popup.ShowOptionList(
                Title: "Select an Option",
                Options: valuesToShow.ToArray(),
                AllowEscape: true);
            if (result < 0 || valuesToShow[result].ToUpper() == "CANCEL")
            {
                // The user escaped out of the menu, or chose "Cancel"
                return string.Empty;
            }
            else
            {
                // The user selected a value - return the select value as a string
                return valuesToShow[result];
            }
        }
        public string GetWeaponTileColor(string colorChoice)
        {
            if (colorChoice == "Red")
            {
                return "R";
            }
            else if (colorChoice == "Dark Red")
            {
                return "r";
            }
            else if (colorChoice == "Blue")
            {
                return "B";
            }
            else if (colorChoice == "Dark Blue")
            {
                return "b";
            }
            else if (colorChoice == "Green")
            {
                return "G";
            }
            else if (colorChoice == "Dark Green")
            {
                return "g";
            }
            else if (colorChoice == "Yellow")
            {
                return "W";
            }
            else if (colorChoice == "Magenta")
            {
                return "M";
            }
            else if (colorChoice == "Dark Magenta")
            {
                return "m";
            }
            else if (colorChoice == "Orange")
            {
                return "O";
            }
            else if (colorChoice == "Dark Orange")
            {
                return "o";
            }
            else if (colorChoice == "Brown")
            {
                return "w";
            }
            else if (colorChoice == "Cyan")
            {
                return "C";
            }
            else if (colorChoice == "Dark Cyan")
            {
                return "c";
            }
            else if (colorChoice == "White")
            {
                return "Y";
            }
            else if (colorChoice == "Gray")
            {
                return "y";
            }
            else if (colorChoice == "Black")
            {
                return "K";
            }
            else
                return null;
        }
        public void ManifestPsiWeapon(string weaponOptions, string colorChoice)
        {
            ++WeaponCounter;

            var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;
            var ParentsLevel = ParentObject.Statistics["Level"].BaseValue;

            string WeaponBPToBeManifested = null;
            GameObject WeaponObjToBeManifested = null;

            if (weaponOptions == "Long Sword")
            {
                AddPlayerMessage("Long Sword Branch Selected");

                if (WeaponBPToBeManifested == null)
                {
                    if (ParentsEgo + ParentsLevel <= 3)
                    {
                        AddPlayerMessage("Return Tier 1");

                        WeaponBPToBeManifested = "Long Sword";
                    }
                    else if (ParentsEgo + ParentsLevel <= 7)
                    {
                        AddPlayerMessage("Return Tier 2");

                        WeaponBPToBeManifested = "Long Sword2";
                    }
                    else if (ParentsEgo + ParentsLevel <= 10)
                    {
                        AddPlayerMessage("Return Tier Steel");

                        WeaponBPToBeManifested = "Steel Long Sword";
                    }
                    else if (ParentsEgo + ParentsLevel <= 13)
                    {
                        AddPlayerMessage("Return Tier 3");

                        WeaponBPToBeManifested = "Long Sword3";
                    }
                    else if (ParentsEgo + ParentsLevel <= 17)
                    {
                        AddPlayerMessage("Return Tier 4");

                        WeaponBPToBeManifested = "Long Sword4";
                    }
                    else if (ParentsEgo + ParentsLevel <= 20)
                    {
                        AddPlayerMessage("Return Tier 5");

                        WeaponBPToBeManifested = "Long Sword5";
                    }
                    else if (ParentsEgo + ParentsLevel <= 23)
                    {
                        AddPlayerMessage("Return Tier 6");

                        WeaponBPToBeManifested = "Long Sword6";
                    }
                    else if (ParentsEgo + ParentsLevel <= 26)
                    {
                        AddPlayerMessage("Return Tier 7");

                        WeaponBPToBeManifested = "Long Sword7";
                    }
                    else if ((ParentsEgo + ParentsLevel) < 27)
                    {
                        AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Long Sword8";
                    }
                }
            }


            if (WeaponObjToBeManifested == null)
            {
                AddPlayerMessage("Begin Manifest Weapon");

                WeaponObjToBeManifested = GameObject.create(WeaponBPToBeManifested);

                if (ParentObject.GetFirstBodyPart("Hand").Equipped == null)
                {
                    AddPlayerMessage("Weapon equipped.");
                    ParentObject.GetFirstBodyPart("Hand").Equip(WeaponObjToBeManifested);
                    WeaponObjToBeManifested.AddPart<PsionicWeapon>();
                    WeaponObjToBeManifested.id = PsiWeaponsID;
                    Event e = Event.New("PsionicWeaponManifestedEvent", "ColorChoice", colorChoice, "ManifestedWeapon", WeaponObjToBeManifested);
                    WeaponObjToBeManifested.FireEvent(e);
                    // WeaponObjToBeManifested.pRender.TileColor = GetWeaponTileColor($"&{colorChoice}");
                    // WeaponObjToBeManifested.pRender.ColorString = GetWeaponTileColor($"&{colorChoice}");
                }
                else
                {
                    AddPlayerMessage("Weapon is added to Inventory.");
                    ParentObject.Inventory.AddObject(WeaponObjToBeManifested);
                    WeaponObjToBeManifested.AddPart<PsionicWeapon>();
                    WeaponObjToBeManifested.id = PsiWeaponsID;
                    Event e = Event.New("PsionicWeaponManifestedEvent", "ColorChoice", colorChoice, "ManifestedWeapon", WeaponObjToBeManifested);
                    WeaponObjToBeManifested.FireEvent(e);
                    // WeaponObjToBeManifested.pRender.TileColor = GetWeaponTileColor($"&{colorChoice}");
                    // WeaponObjToBeManifested.pRender.ColorString = GetWeaponTileColor($"&{colorChoice}");
                }
            }
        }
        public void DismissPsiWeapon()
        {
            Zone ParentsCurrentZone = ParentObject.CurrentZone;

            if (ParentsCurrentZone.findObjectById(PsiWeaponsID) != null && !ParentObject.HasObjectInInventory(PsiWeaponsID) || !ParentObject.HasEquippedObject(PsiWeaponsID))
            {
                AddPlayerMessage("Dismissing Item in zone.");
                GameObject PsiWeaponInZone = ParentsCurrentZone.findObjectById(PsiWeaponsID);

                // DidX("disappear", null, null, null, null, PsiWeaponInZone);
                // PsiWeaponInZone.CurrentCell.Splash("{{M|*}}");
                // PsiWeaponInZone.Destroy();
                var ParentsInventory = ParentObject.Inventory;
                var WeaponInInvo = ParentsInventory.GetObjects();

                foreach (var O in WeaponInInvo)
                {
                    AddPlayerMessage("Dismissing: Starting for each.");

                    if (O.id == PsiWeaponsID)
                    {
                        AddPlayerMessage("Dismissing getting ID for: " + PsiWeaponsID);

                        if (O.IsEquippedProperly() || O.IsEquippedOnPrimary() || O.IsEquippedOnType("Hands"))
                        {
                            AddPlayerMessage("Dismissing getting Obj in primary for: " + O);
                            O.ForceUnequipRemoveAndRemoveContents(Silent: true);
                            ParentsInventory.RemoveObject(O);
                            DidX("disappear", null, null, null, null, O);
                        }
                        else
                        {
                            AddPlayerMessage("Dismissing getting Obj in inv for: " + O);
                            ParentsInventory.RemoveObject(O);
                            DidX("disappear", null, null, null, null, O);
                        }
                    }
                }
                if (WeaponCounter > 0)
                { --WeaponCounter; }
            }
            else if (ParentsCurrentZone.findObjectById(PsiWeaponsID) != null && ParentObject.HasObjectInInventory(PsiWeaponsID) || ParentObject.HasEquippedObject(PsiWeaponsID))
            {
                AddPlayerMessage("Dismissing item in Inventory.");
                var ParentsInventory = ParentObject.Inventory;
                var WeaponInInvo = ParentsInventory.GetObjects();

                foreach (var O in WeaponInInvo)
                {
                    if (O.id == PsiWeaponsID)
                    {
                        if (O.IsEquippedProperly())
                        {
                            O.ForceUnequipRemoveAndRemoveContents(Silent: true);
                            ParentsInventory.RemoveObject(O);
                            DidX("disappear", null, null, null, null, O);
                        }
                        else
                        {
                            ParentsInventory.RemoveObject(O);
                            DidX("disappear", null, null, null, null, O);
                        }
                    }
                }
                if (WeaponCounter > 0)
                { --WeaponCounter; }
            }
            else
            {
                AddPlayerMessage("There are no weapons to dismiss.");
            }
        }
        public Psychomateriartis()
        {
            this.DisplayName = "Psychomateriartus";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override string GetDescription()
        {
            return "Your incredible psionic power comes at the cost of overwhelming the stability of your physical form, you are doomed to hunting down physical husk to maintain your tether to this reality, albeit the magnitude of your psionic abilities are of a realm of its own.\n"
                + "\n"
                + "{{orange|-400 reputation with highly entropic beings.\n}}"
                + "{{light blue|+400 reputation the Seekers of the Sightless Way.\n\n}}";
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Mutations GainPSiFocus = GO.GetPart<Mutations>();
            if (!GainPSiFocus.HasMutation("FocusPsi"))
            {
                GainPSiFocus.AddMutation("FocusPsi", 1);
            }
            this.ManifestPsiWeaponActivatedAbilityID = base.AddMyActivatedAbility("Manifest Psi-Weapon", "ManifestWeaponCommand", "Mental Mutation", "Manifest a psionic weapon.", "\u03A9");
            this.ReturnPsiWeaponActivatedAbilityID = base.AddMyActivatedAbility("Return Psi-Weapon", "ReturnWeaponCommand", "Mental Mutation", "Rematerialize a lost psi-weapon to your primary hand.", "\u03A9");

            return base.Mutate(GO, Level);
        }
        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart)this, "EndTurn");
            Object.RegisterPartEvent(this, "Dismember");
            Object.RegisterPartEvent(this, "PsionicWeaponManifestedEvent");
            Object.RegisterPartEvent(this, "ManifestWeaponCommand");
            Object.RegisterPartEvent(this, "ReturnWeaponCommand");
            base.Register(Object);
        }
        public void ReturnPsiWeapon()
        {
            Zone ParentsCurrentZone = ParentObject.CurrentZone;

            if (ParentsCurrentZone.findObjectById(PsiWeaponsID) != null)
            {
                GameObject PsiWeaponInZone = ParentsCurrentZone.findObjectById(PsiWeaponsID);
                var ParentsEquippableSlot = ParentObject.Body.GetPrimaryLimbType();
                PlayWorldSound("return");
                PsiWeaponInZone.CurrentCell.Splash("{{M|*}}");
                var ParentsInventory = ParentObject.Inventory;
                ParentsInventory.AddObject(PsiWeaponInZone, true, true, true);

                // PsiWeaponInZone.EquipObject(ParentObject, ParentsEquippableSlot, true);

                var WeaponInInvo = ParentsInventory.GetObjects();

                foreach (var O in WeaponInInvo)
                {
                    if (O.id == PsiWeaponsID)
                    {
                        if (ParentObject.Body.HasPrimaryHand())
                        {
                            AddPlayerMessage("Return getting Obj in Prime for: " + O);
                            XDidY(O, "suddenly appear", "in " + ParentObject.its + " " + ParentsEquippableSlot, "!", null, ParentObject);
                            O.ForceEquipObject(ParentObject, ParentsEquippableSlot, true);
                        }
                        else
                        {
                            AddPlayerMessage("Dismissing getting Obj in Inv for: " + O);
                            XDidY(O, "suddenly appear", "in " + ParentObject.its + " inventory", "!", null, ParentObject);
                        }

                    }
                }
            }
            else
            {
                AddPlayerMessage("Your weapon is not returnable at this time.");
            }
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "ManifestWeaponCommand")
            {
                if (IsMyActivatedAbilityUsable(ManifestPsiWeaponActivatedAbilityID))
                {
                    BeginWeaponManifestOptionList();
                }
            }
            else if (E.ID == "ReturnWeaponCommand")
            {
                if (IsMyActivatedAbilityUsable(ReturnPsiWeaponActivatedAbilityID))
                {
                    ReturnPsiWeapon();
                }
            }
            return base.FireEvent(E);
        }
    }
}


