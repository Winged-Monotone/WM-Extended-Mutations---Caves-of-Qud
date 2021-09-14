using AiUnity.NLog.Core.LayoutRenderers;
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
        public int PsiCost;
        public int NewPsiCost;
        public int WeaponCounter = 0;
        // private int TensPlaceOfLevel = 0;
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
            "Great Hammer",
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
                        // string colorChoice = GetChoice(ColorOptions);

                        // if (!string.IsNullOrEmpty(colorChoice))
                        // {
                        // Popup.Show($"You chose to {mainChoice} with a {colorChoice} {weaponChoice}");
                        ManifestPsiWeapon(weaponChoice);
                        // }
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
        public string GetPsychoMatLevel()
        {
            if (ParentObject != null)
            {
                var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;
                var ParentsLevel = ParentObject.Statistics["Level"].BaseValue;

                if (ParentsEgo + ParentsLevel <= 3)
                {
                    return "{{brown|Bronze}}";
                }
                else if (ParentsEgo + ParentsLevel <= 7)
                {
                    return "{{gray|Iron}}";
                }
                else if (ParentsEgo + ParentsLevel <= 10)
                {
                    return "{{white|Steel}}";
                }
                else if (ParentsEgo + ParentsLevel <= 13)
                {
                    return "{{b|Carbide}}";
                }
                else if (ParentsEgo + ParentsLevel <= 17)
                {
                    return "{{lightblue|Folded Carbide}}";
                }
                else if (ParentsEgo + ParentsLevel <= 20)
                {
                    return "{{K|Fullerite}}";
                }
                else if (ParentsEgo + ParentsLevel <= 23)
                {
                    return "{{crysteel|Crysteel}}";
                }
                else if (ParentsEgo + ParentsLevel <= 26)
                {
                    return "{{crysteel|{{K|Flawless}} Crysteel}}";
                }
                else if ((ParentsEgo + ParentsLevel) >= 27)
                {
                    return "{{zetachrome|Zetachrome}}";
                }
            }
            return "{{brown| Bronze}}";
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
        public void ManifestPsiWeapon(string weaponOptions)
        {
            ++WeaponCounter;

            var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;
            var ParentsLevel = ParentObject.Statistics["Level"].BaseValue;
            var ParentsCharges = ParentObject.Statistics["PsiCharges"].BaseValue;


            string WeaponBPToBeManifested = null;
            GameObject WeaponObjToBeManifested = null;

            PsiCost = 2;
            NewPsiCost = PsiCost * WeaponCounter;
            if (NewPsiCost > ParentsCharges)
            {
                AddPlayerMessage("You lack enough maximum charges to forge another weapon.");
                if (WeaponCounter > 0)
                { --WeaponCounter; }
                return;
            }
            else if (weaponOptions == "Long Sword")
            {
                // AddPlayerMessage("Long Sword Branch Selected");

                if (WeaponBPToBeManifested == null)
                {
                    if (ParentsEgo + ParentsLevel <= 3)
                    {
                        // AddPlayerMessage("Return Tier 1");

                        WeaponBPToBeManifested = "Long Sword";
                    }
                    else if (ParentsEgo + ParentsLevel <= 7)
                    {
                        // AddPlayerMessage("Return Tier 2");

                        WeaponBPToBeManifested = "Long Sword2";
                    }
                    else if (ParentsEgo + ParentsLevel <= 10)
                    {
                        // AddPlayerMessage("Return Tier Steel");

                        WeaponBPToBeManifested = "Steel Long Sword";
                    }
                    else if (ParentsEgo + ParentsLevel <= 13)
                    {
                        // AddPlayerMessage("Return Tier 3");

                        WeaponBPToBeManifested = "Long Sword3";
                    }
                    else if (ParentsEgo + ParentsLevel <= 17)
                    {
                        // AddPlayerMessage("Return Tier 4");

                        WeaponBPToBeManifested = "Long Sword4";
                    }
                    else if (ParentsEgo + ParentsLevel <= 20)
                    {
                        // AddPlayerMessage("Return Tier 5");

                        WeaponBPToBeManifested = "Long Sword5";
                    }
                    else if (ParentsEgo + ParentsLevel <= 23)
                    {
                        // AddPlayerMessage("Return Tier 6");

                        WeaponBPToBeManifested = "Long Sword6";
                    }
                    else if (ParentsEgo + ParentsLevel <= 26)
                    {
                        // AddPlayerMessage("Return Tier 7");

                        WeaponBPToBeManifested = "Long Sword7";
                    }
                    else if ((ParentsEgo + ParentsLevel) >= 27)
                    {
                        // AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Long Sword8";
                    }
                }
            }
            else if (weaponOptions == "Short Sword")
            {
                // AddPlayerMessage("Short Sword Branch Selected");

                if (WeaponBPToBeManifested == null)
                {
                    if (ParentsEgo + ParentsLevel <= 3)
                    {
                        // AddPlayerMessage("Return Tier 1");

                        WeaponBPToBeManifested = "Short Sword";
                    }
                    else if (ParentsEgo + ParentsLevel <= 7)
                    {
                        // AddPlayerMessage("Return Tier 2");

                        WeaponBPToBeManifested = "Short Sword2";
                    }
                    else if (ParentsEgo + ParentsLevel <= 10)
                    {
                        // AddPlayerMessage("Return Tier Steel");

                        WeaponBPToBeManifested = "Steel Short Sword";
                    }
                    else if (ParentsEgo + ParentsLevel <= 13)
                    {
                        // AddPlayerMessage("Return Tier 3");

                        WeaponBPToBeManifested = "Short Sword3";
                    }
                    else if (ParentsEgo + ParentsLevel <= 17)
                    {
                        // AddPlayerMessage("Return Tier 4");

                        WeaponBPToBeManifested = "Short Sword4";
                    }
                    else if (ParentsEgo + ParentsLevel <= 20)
                    {
                        // AddPlayerMessage("Return Tier 5");

                        WeaponBPToBeManifested = "Short Sword5";
                    }
                    else if (ParentsEgo + ParentsLevel <= 23)
                    {
                        // AddPlayerMessage("Return Tier 6");

                        WeaponBPToBeManifested = "Short Sword6";
                    }
                    else if (ParentsEgo + ParentsLevel <= 26)
                    {
                        // AddPlayerMessage("Return Tier 7");

                        WeaponBPToBeManifested = "Short Sword7";
                    }
                    else if ((ParentsEgo + ParentsLevel) >= 27)
                    {
                        // AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Short Sword8";
                    }
                }
            }
            else if (weaponOptions == "Great Sword")
            {
                // AddPlayerMessage("Great Sword Branch Selected");

                if (WeaponBPToBeManifested == null)
                {
                    if (ParentsEgo + ParentsLevel <= 3)
                    {
                        // AddPlayerMessage("Return Tier 1");

                        WeaponBPToBeManifested = "Two-Handed Sword";
                    }
                    else if (ParentsEgo + ParentsLevel <= 7)
                    {
                        // AddPlayerMessage("Return Tier 2");

                        WeaponBPToBeManifested = "Long Sword2th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 10)
                    {
                        // AddPlayerMessage("Return Tier Steel");

                        WeaponBPToBeManifested = "Steel Long Swordth";
                    }
                    else if (ParentsEgo + ParentsLevel <= 13)
                    {
                        // AddPlayerMessage("Return Tier 3");

                        WeaponBPToBeManifested = "Long Sword3th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 17)
                    {
                        // AddPlayerMessage("Return Tier 4");

                        WeaponBPToBeManifested = "Long Sword4th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 20)
                    {
                        // AddPlayerMessage("Return Tier 5");

                        WeaponBPToBeManifested = "Long Sword5th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 23)
                    {
                        // AddPlayerMessage("Return Tier 6");

                        WeaponBPToBeManifested = "Long Sword6th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 26)
                    {
                        // AddPlayerMessage("Return Tier 7");

                        WeaponBPToBeManifested = "Long Sword7th";
                    }
                    else if ((ParentsEgo + ParentsLevel) >= 27)
                    {
                        // AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Long Sword8th";
                    }
                }
            }
            else if (weaponOptions == "Dagger")
            {
                // AddPlayerMessage("Dagger Branch Selected");

                if (WeaponBPToBeManifested == null)
                {
                    if (ParentsEgo + ParentsLevel <= 3)
                    {
                        // AddPlayerMessage("Return Tier 1");

                        WeaponBPToBeManifested = "Dagger";
                    }
                    else if (ParentsEgo + ParentsLevel <= 7)
                    {
                        // AddPlayerMessage("Return Tier 2");

                        WeaponBPToBeManifested = "Dagger2";
                    }
                    else if (ParentsEgo + ParentsLevel <= 10)
                    {
                        // AddPlayerMessage("Return Tier Steel");

                        WeaponBPToBeManifested = "Steel Dagger";
                    }
                    else if (ParentsEgo + ParentsLevel <= 13)
                    {
                        // AddPlayerMessage("Return Tier 3");

                        WeaponBPToBeManifested = "Dagger3";
                    }
                    else if (ParentsEgo + ParentsLevel <= 17)
                    {
                        // AddPlayerMessage("Return Tier 4");

                        WeaponBPToBeManifested = "Dagger4";
                    }
                    else if (ParentsEgo + ParentsLevel <= 20)
                    {
                        // AddPlayerMessage("Return Tier 5");

                        WeaponBPToBeManifested = "Dagger5";
                    }
                    else if (ParentsEgo + ParentsLevel <= 23)
                    {
                        // AddPlayerMessage("Return Tier 6");

                        WeaponBPToBeManifested = "Dagger6";
                    }
                    else if (ParentsEgo + ParentsLevel <= 26)
                    {
                        // AddPlayerMessage("Return Tier 7");

                        WeaponBPToBeManifested = "Dagger7";
                    }
                    else if ((ParentsEgo + ParentsLevel) >= 27)
                    {
                        // AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Dagger8";
                    }
                }
            }
            else if (weaponOptions == "Cudgel")
            {
                // AddPlayerMessage("Cudgel Branch Selected");

                if (WeaponBPToBeManifested == null)
                {
                    if (ParentsEgo + ParentsLevel <= 3)
                    {
                        // AddPlayerMessage("Return Tier 1");

                        WeaponBPToBeManifested = "Club";
                    }
                    else if (ParentsEgo + ParentsLevel <= 7)
                    {
                        // AddPlayerMessage("Return Tier 2");

                        WeaponBPToBeManifested = "Mace2";
                    }
                    else if (ParentsEgo + ParentsLevel <= 10)
                    {
                        // AddPlayerMessage("Return Tier Steel");

                        WeaponBPToBeManifested = "Steel War Hammer";
                    }
                    else if (ParentsEgo + ParentsLevel <= 13)
                    {
                        // AddPlayerMessage("Return Tier 3");

                        WeaponBPToBeManifested = "Cudgel3";
                    }
                    else if (ParentsEgo + ParentsLevel <= 17)
                    {
                        // AddPlayerMessage("Return Tier 4");

                        WeaponBPToBeManifested = "Cudgel4";
                    }
                    else if (ParentsEgo + ParentsLevel <= 20)
                    {
                        // AddPlayerMessage("Return Tier 5");

                        WeaponBPToBeManifested = "Cudgel5";
                    }
                    else if (ParentsEgo + ParentsLevel <= 23)
                    {
                        // AddPlayerMessage("Return Tier 6");

                        WeaponBPToBeManifested = "Cudgel6";
                    }
                    else if (ParentsEgo + ParentsLevel <= 26)
                    {
                        // AddPlayerMessage("Return Tier 7");

                        WeaponBPToBeManifested = "Cudgel7";
                    }
                    else if ((ParentsEgo + ParentsLevel) >= 27)
                    {
                        // AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Cudgel8";
                    }
                }
            }
            else if (weaponOptions == "Great Hammer")
            {
                // AddPlayerMessage("Great Hammer Branch Selected");

                if (WeaponBPToBeManifested == null)
                {
                    if (ParentsEgo + ParentsLevel <= 3)
                    {
                        // AddPlayerMessage("Return Tier 1");

                        WeaponBPToBeManifested = "Walking Stick";
                    }
                    else if (ParentsEgo + ParentsLevel <= 7)
                    {
                        // AddPlayerMessage("Return Tier 2");

                        WeaponBPToBeManifested = "Shillelagh";
                    }
                    else if (ParentsEgo + ParentsLevel <= 10)
                    {
                        // AddPlayerMessage("Return Tier Steel");

                        WeaponBPToBeManifested = "Steel War Hammerth";
                    }
                    else if (ParentsEgo + ParentsLevel <= 13)
                    {
                        // AddPlayerMessage("Return Tier 3");

                        WeaponBPToBeManifested = "Cudgel3th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 17)
                    {
                        // AddPlayerMessage("Return Tier 4");

                        WeaponBPToBeManifested = "Cudgel4th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 20)
                    {
                        // AddPlayerMessage("Return Tier 5");

                        WeaponBPToBeManifested = "Cudgel5th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 23)
                    {
                        // AddPlayerMessage("Return Tier 6");

                        WeaponBPToBeManifested = "Cudgel6th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 26)
                    {
                        // AddPlayerMessage("Return Tier 7");

                        WeaponBPToBeManifested = "Cudgel7th";
                    }
                    else if ((ParentsEgo + ParentsLevel) >= 27)
                    {
                        // AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Cudgel8th";
                    }
                }
            }
            else if (weaponOptions == "Axe")
            {
                // AddPlayerMessage("Axe Branch Selected");

                if (WeaponBPToBeManifested == null)
                {
                    if (ParentsEgo + ParentsLevel <= 3)
                    {
                        // AddPlayerMessage("Return Tier 1");

                        WeaponBPToBeManifested = "Battle Axe";
                    }
                    else if (ParentsEgo + ParentsLevel <= 7)
                    {
                        // AddPlayerMessage("Return Tier 2");

                        WeaponBPToBeManifested = "Battle Axe2";
                    }
                    else if (ParentsEgo + ParentsLevel <= 10)
                    {
                        // AddPlayerMessage("Return Tier Steel");

                        WeaponBPToBeManifested = "Steel Battle Axe";
                    }
                    else if (ParentsEgo + ParentsLevel <= 13)
                    {
                        // AddPlayerMessage("Return Tier 3");

                        WeaponBPToBeManifested = "Battle Axe3";
                    }
                    else if (ParentsEgo + ParentsLevel <= 17)
                    {
                        // AddPlayerMessage("Return Tier 4");

                        WeaponBPToBeManifested = "Battle Axe4";
                    }
                    else if (ParentsEgo + ParentsLevel <= 20)
                    {
                        // AddPlayerMessage("Return Tier 5");

                        WeaponBPToBeManifested = "Battle Axe5";
                    }
                    else if (ParentsEgo + ParentsLevel <= 23)
                    {
                        // AddPlayerMessage("Return Tier 6");

                        WeaponBPToBeManifested = "Battle Axe6";
                    }
                    else if (ParentsEgo + ParentsLevel <= 26)
                    {
                        // AddPlayerMessage("Return Tier 7");

                        WeaponBPToBeManifested = "Battle Axe7";
                    }
                    else if ((ParentsEgo + ParentsLevel) >= 27)
                    {
                        // AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Battle Axe8";
                    }
                }
            }
            else if (weaponOptions == "Great Axe")
            {
                // AddPlayerMessage("Great Axe Branch Selected");

                if (WeaponBPToBeManifested == null)
                {
                    if (ParentsEgo + ParentsLevel <= 3)
                    {
                        // AddPlayerMessage("Return Tier 1");

                        WeaponBPToBeManifested = "Iron Vinereaper";
                    }
                    else if (ParentsEgo + ParentsLevel <= 7)
                    {
                        // AddPlayerMessage("Return Tier 2");

                        WeaponBPToBeManifested = "Steel Vinereaper";
                    }
                    else if (ParentsEgo + ParentsLevel <= 10)
                    {
                        // AddPlayerMessage("Return Tier Steel");

                        WeaponBPToBeManifested = "Steel Battle Axeth";
                    }
                    else if (ParentsEgo + ParentsLevel <= 13)
                    {
                        // AddPlayerMessage("Return Tier 3");

                        WeaponBPToBeManifested = "Battle Axe3th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 17)
                    {
                        // AddPlayerMessage("Return Tier 4");

                        WeaponBPToBeManifested = "Battle Axe4th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 20)
                    {
                        // AddPlayerMessage("Return Tier 5");

                        WeaponBPToBeManifested = "Battle Axe5th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 23)
                    {
                        // AddPlayerMessage("Return Tier 6");

                        WeaponBPToBeManifested = "Battle Axe6th";
                    }
                    else if (ParentsEgo + ParentsLevel <= 26)
                    {
                        // AddPlayerMessage("Return Tier 7");

                        WeaponBPToBeManifested = "Battle Axe7th";
                    }
                    else if ((ParentsEgo + ParentsLevel) >= 27)
                    {
                        // AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Battle Axe8th";
                    }
                }
            }

            if (WeaponObjToBeManifested == null)
            {
                // AddPlayerMessage("Begin Manifest Weapon");

                WeaponObjToBeManifested = GameObject.create(WeaponBPToBeManifested);

                if (ParentObject.GetFirstBodyPart("Hand").Equipped == null)
                {
                    // AddPlayerMessage("Weapon equipped.");
                    ParentObject.GetFirstBodyPart("Hand").Equip(WeaponObjToBeManifested);
                    WeaponObjToBeManifested.AddPart<PsionicWeapon>();
                    WeaponObjToBeManifested.id = PsiWeaponsID;
                    // Event e = Event.New("PsionicWeaponManifestedEvent", "ColorChoice", colorChoice, "ManifestedWeapon", WeaponObjToBeManifested);
                    Event e = Event.New("PsionicWeaponManifestedEvent", "ManifestedWeapon", WeaponObjToBeManifested);

                    WeaponObjToBeManifested.FireEvent(e);

                    // WeaponObjToBeManifested.pRender.TileColor = GetWeaponTileColor($"&{colorChoice}");
                    // WeaponObjToBeManifested.pRender.ColorString = GetWeaponTileColor($"&{colorChoice}");
                }
                else
                {
                    // AddPlayerMessage("Weapon is added to Inventory.");
                    ParentObject.Inventory.AddObject(WeaponObjToBeManifested);
                    WeaponObjToBeManifested.AddPart<PsionicWeapon>();
                    WeaponObjToBeManifested.id = PsiWeaponsID;
                    // Event e = Event.New("PsionicWeaponManifestedEvent", "ColorChoice", colorChoice, "ManifestedWeapon", WeaponObjToBeManifested);
                    Event e = Event.New("PsionicWeaponManifestedEvent", "ManifestedWeapon", WeaponObjToBeManifested);

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

                // AddPlayerMessage("Dismissing Weapon");
                GameObject PsiWeaponInZone = ParentsCurrentZone.findObjectById(PsiWeaponsID);

                // AddPlayerMessage("Finding: " + PsiWeaponsID);

                var ParentsInventory = ParentObject.Inventory;
                var WeaponInInvo = ParentsInventory.GetObjects();
                var ParentsBodySlots = ParentObject.Body.GetBody();
                var ParentsHands = ParentsBodySlots.GetPartByName("Hand");

                ParentObject.FireEvent(Event.New("CommandForceUnequipObject", "BodyPart", ParentsHands));

                if (ParentsBodySlots.IsItemEquippedOnLimbType(PsiWeaponInZone, "Hand"))
                {
                    // AddPlayerMessage("Dismissing getting Obj in Hands for: " + PsiWeaponInZone);
                    PsiWeaponInZone.ForceUnequipAndRemove(true);

                    ParentsInventory.RemoveObject(PsiWeaponInZone);

                    XDidY(PsiWeaponInZone, "disappear", null, null, null, null);
                    if (WeaponCounter > 0)
                    {
                        --WeaponCounter;
                    }
                }
                else if (ParentsInventory.HasObject(PsiWeaponInZone))
                {
                    ParentsInventory.RemoveObject(PsiWeaponInZone);
                    // AddPlayerMessage("Dismissing getting Obj in inv for: " + PsiWeaponInZone);
                    if (WeaponCounter > 0)
                    {
                        --WeaponCounter;
                    }
                }
                else if (ParentsCurrentZone.findObjectById(PsiWeaponsID) != null)
                {
                    if (PsiWeaponInZone.CurrentCell != null)
                    {
                        DidX("disappear", null, null, null, null, PsiWeaponInZone);
                        PsiWeaponInZone.CurrentCell.Splash("{{M|*}}");
                        PsiWeaponInZone.Destroy();
                        if (WeaponCounter > 0)
                        {
                            --WeaponCounter;
                        }
                    }
                }
                else
                {
                    AddPlayerMessage("There are no weapons to dismiss.");
                }
            }
            else
            {
                AddPlayerMessage("There are no weapons to dismiss.");
            }
        }
        public Psychomateriartis()
        {
            this.DisplayName = "Psychomateriartus";
            this.Type = "Mental";
        }
        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override string GetDescription()
        {
            return "Conjure your thoughtstuff, and materialize weapons as sharp as your mind.\n"
                + "\n"
                + "{{cyan|+50 reputation the Seekers of the Sightless Way.}}";
        }
        public override string GetLevelText(int Level)
        {
            string ConvertedLevelString = Level.ToString();
            if (Level == base.Level)
            {
                if (Level <= 9)
                {

                    return "{{gray|Materialize psionic weaponry, psionic weaponry gains bonus penetration equivocal to your ego modifier and the mutations' ego magnitude. Psionic arms are bonded to its wielder, you may return your first materialized weapon to your hand as long as you are in the same zone and the weapon hasn't been destroyed.}}"

                   + "\n\nEgo Magnitude: {{cyan|0." + Level + "}}\n"
                   + "Weapon Level: {{cyan|" + GetPsychoMatLevel() + "}}\n";
                }
                else
                {
                    return "{{gray|Materialize psionic weaponry, psionic weaponry gains bonus penetration equivocal to your ego modifier and the mutations' ego magnitude. Psionic arms are bonded to its wielder, you may return your first materialized weapon to your hand as long as you are in the same zone and the weapon hasn't been destroyed.}}"

                   + "\n\nEgo Magnitude: {{cyan|" + ConvertedLevelString.Insert(1, ".") + "}}\n"
                   + "Weapon Level: {{cyan|" + GetPsychoMatLevel() + "}}\n";
                }
            }
            else
            {
                if (Level <= 9)
                {
                    return "Increased Ego Magnitude: {{cyan|0." + Level + "}}\n"
                    + "Increased Weapon Level: {{cyan|" + GetPsychoMatLevel() + "}}\n";
                }
                else
                {
                    return "Increased Ego Magnitude: {{cyan|" + ConvertedLevelString.Insert(1, ".") + "}}\n"
                    + "Increased Weapon Level: {{cyan|" + GetPsychoMatLevel() + "}}\n";
                }
            }
        }
        public override bool Mutate(GameObject GO, int Level)
        {
            // string PsychomateriartisinfoSource = "{ \"Psychomateriartis\": [\"*cult*, mind-smiths\", \"Forgemasters *cult*\"] }";
            // SimpleJSON.JSONNode PsychomateriartisInfo = SimpleJSON.JSON.Parse(PsychomateriartisinfoSource);

            // WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", PsychomateriartisInfo["Psychomateriartis"]);

            Mutations GainPSiFocus = GO.GetPart<Mutations>();
            if (!GainPSiFocus.HasMutation("FocusPsi"))
            {
                GainPSiFocus.AddMutation("FocusPsi", 1);
            }

            this.ManifestPsiWeaponActivatedAbilityID = base.AddMyActivatedAbility(Name: "Psi-Forge", Command: "ManifestWeaponCommand", Class: "Mental Mutation", Description: "Manifest or dismiss a psionic weapon.\n\n");
            this.ReturnPsiWeaponActivatedAbilityID = base.AddMyActivatedAbility(Name: "Return Psi-Weapon", Command: "ReturnWeaponCommand", Class: "Mental Mutation", Description: "Rematerialize your last previously crafted psi-arm to your hand so long as you are in the same parasang, this is a turnless action.", Icon: "\u03A9");

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
                var ParentsBodySlots = ParentObject.Body.GetBody();
                var ParentsInventory = ParentObject.Inventory;
                var ParentsHands = ParentsBodySlots.GetPartByName("Hand");

                PlayWorldSound("return");
                PsiWeaponInZone.CurrentCell.Splash("{{M|*}}");
                ParentsInventory.AddObject(PsiWeaponInZone, true, true, true);
                // PsiWeaponInZone.EquipObject(ParentObject, ParentsEquippableSlot, true);

                var WeaponInInvo = ParentsInventory.GetObjects();

                foreach (var O in WeaponInInvo)
                {
                    if (O.id == PsiWeaponsID)
                    {
                        if (ParentObject.Body.HasPrimaryHand())
                        {
                            // AddPlayerMessage("Return getting Obj in Prime for: " + O);
                            XDidY(O, "suddenly appear", "in " + ParentObject.its + " " + ParentsEquippableSlot, "!", null, ParentObject);
                            O.ForceEquipObject(ParentObject, ParentsEquippableSlot, true);
                            Event @event = Event.New("CommandForceEquipObject");
                            @event.SetParameter("Object", PsiWeaponInZone);
                            @event.SetParameter("BodyPart", ParentsHands);
                            @event.SetSilent(Silent: true);
                            ParentObject.FireEvent(@event);
                        }
                        else
                        {
                            // AddPlayerMessage("Dismissing getting Obj in Inv for: " + O);
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


