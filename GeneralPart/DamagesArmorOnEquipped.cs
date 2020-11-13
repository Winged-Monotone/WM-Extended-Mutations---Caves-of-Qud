using System;
using XRL.UI;
using XRL.Rules;
using XRL.World.Effects;
using System.Collections.Generic;
using XRL.World.Capabilities;
using ConsoleLib.Console;
using XRL.Core;

namespace XRL.World.Parts
{
    [Serializable]
    public class DamagesArmorOnEquipped : IPart
    {
        public override bool SameAs(IPart p)
        {
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == EquipperEquippedEvent.ID;
        }

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "BeginMove");
            base.Register(go);
        }

        public override bool HandleEvent(EquipperEquippedEvent E)
        {
            if (E.Actor == ParentObject && E.Actor.IsPlayer() && E.Item.HasPart("Armor"))
            {
                if (E.Item.GetTier() < 5)
                {
                    Popup.Show("This armor cannot be equipped, it'd melt right off you!");
                    AddPlayerMessage("This armor cannot be equipped, it'd melt right off you!");
                    E.Item.ForceUnequip();
                }
            }
            else if (E.Actor == ParentObject && E.Item.HasPart("Armor"))
            {
                if (E.Item.GetTier() < 5 && !E.Item.HasTagOrProperty("NaturalWeapon") && !E.Item.HasTagOrProperty("NaturalEquipment"))
                {
                    E.Item.ForceUnequip();
                }
            }
            if (E.Item.GetTier() <= 5 && E.Item.IsEquippedProperly())
            {
                E.Item.Destroy("Destroyed by " + ParentObject.its + " acidic body.", false, false);
            }
            return base.HandleEvent(E);
        }
    }
}
