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
        public Guid ManifestPsiWeaponActivatedAbilityID;
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
                string weaponChoice = GetChoice(WeaponOptions);
                if (!string.IsNullOrEmpty(weaponChoice))
                {
                    PlayWorldSound("ManifestWeapon");
                    string colorChoice = GetChoice(ColorOptions);
                    if (!string.IsNullOrEmpty(colorChoice))
                    {
                        Popup.Show($"You chose to {mainChoice} with a {colorChoice} {weaponChoice}");
                        ManifestPsiWeapon(weaponChoice);
                    }
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

        public void ManifestPsiWeapon(string weaponOptions)
        {
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

                        WeaponBPToBeManifested = "Long Sword1";
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
                    else if ((ParentsEgo + ParentsLevel) <= 27)
                    {
                        AddPlayerMessage("Return Tier 8");

                        WeaponBPToBeManifested = "Long Sword8";
                    }

                    if (WeaponObjToBeManifested == null)
                    {
                        AddPlayerMessage("Begin Manifest Weapon");

                        WeaponObjToBeManifested = GameObject.create(WeaponBPToBeManifested);
                        // WeaponObjToBeManifested.AddPart<PsionicWeapon>();
                        // WeaponObjToBeManifested.RemovePart("TinkerItem");

                        AddPlayerMessage("Weapon Ready for Parameter Shifts");


                        // var WeaponProps = WeaponObjToBeManifested.GetPart<MeleeWeapon>();

                        // Create some kind of algorythm that takes the creatures ego and increases it based on the level, think of it as the more you level the mutation, the more you get access to your ego score.

                        // WeaponProps.Ego = (int)Math.Floor(ParentsEgo * (Level * 0.1));

                        AddPlayerMessage("Weapon Ego Set");


                        AddPlayerMessage("Weapon Fabricated: equipping ...");

                        if (ParentObject.GetFirstBodyPart("Hand").Equipped == null)
                        {
                            AddPlayerMessage("Weapon equipped.");
                            ParentObject.GetFirstBodyPart("Hand").Equip(WeaponObjToBeManifested);
                        }
                        else
                        {
                            AddPlayerMessage("Weapon is added to Inventory.");
                            ParentObject.Inventory.AddObject(WeaponObjToBeManifested);
                        }

                    }

                }
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

            return base.Mutate(GO, Level);
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent((IPart)this, "EndTurn");
            Object.RegisterPartEvent(this, "Dismember");
            Object.RegisterPartEvent(this, "CommandManifestLimb");
            Object.RegisterPartEvent(this, "ManifestWeaponCommand");
            base.Register(Object);
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
            return base.FireEvent(E);
        }
    }
}


