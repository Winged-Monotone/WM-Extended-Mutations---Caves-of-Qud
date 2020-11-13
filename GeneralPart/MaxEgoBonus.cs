using System;
using System.Text;
using XRL;
using XRL.Rules;
using XRL.UI;
using XRL.World;
using XRL.World.Parts.Skill;

namespace XRL.World.Parts
{
    [Serializable]

    public class MaxEgoBonus : IPart
    {
        public const int BONUS_CAP_UNLIMITED = 9999;

        public int EgoBonus;

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == EquippedEvent.ID;
        }

        public override bool HandleEvent(EquippedEvent E)
        {
            if (E.Item.HasPart("MeleeWeapon") == true)
            {
                EgoBonus = E.Actor.StatMod("Ego", 0);
                MeleeWeapon WeaponStatistics = E.Item.GetPart<MeleeWeapon>();
                WeaponStatistics.MaxStrengthBonus = EgoBonus;
            }
            return base.HandleEvent(E);
        }
    }
}