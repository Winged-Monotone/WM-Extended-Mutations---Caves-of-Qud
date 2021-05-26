using System.Collections.Generic;
using XRL.UI;
using XRL.Wish;


namespace MiscellaneousWishes
{
    [HasWishCommand]
    class PsiWeaponList
    {
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

        [WishCommand(Command = "psiweapon")]
        public static void TestOptionList()
        {
            string mainChoice = GetChoice(MainOptions);
            if (!string.IsNullOrEmpty(mainChoice))
            {
                string weaponChoice = GetChoice(WeaponOptions);
                if (!string.IsNullOrEmpty(weaponChoice))
                {
                    string colorChoice = GetChoice(ColorOptions);
                    if (!string.IsNullOrEmpty(colorChoice))
                    {
                        Popup.Show($"You chose to {mainChoice} with a {colorChoice} {weaponChoice}");
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

    }
}