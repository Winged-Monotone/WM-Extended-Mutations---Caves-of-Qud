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
    public class WaterHazardous : IPart
    {
        public override bool SameAs(IPart p)
        {
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == ObjectLeavingCellEvent.ID;
        }

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "BeginMove");
            base.Register(go);
        }
        private List<string> WaterThatHurts = new List<string>()
        {
            "SaltyWaterPuddle",
            "SaltyWaterPool",
            "SaltyWaterDeepPool",
            "BrackishWaterPuddle",
            "SaltPool",
            "BrackishPool",
        };

        public override bool FireEvent(Event E)
        {
            //...
            if (E.ID == "BeginMove")
            {
                Cell Cell = E.GetParameter("DestinationCell") as Cell;
                if (Cell.HasObject(X => WaterThatHurts.Contains(X.Blueprint)) && !ParentObject.HasEffect("Dissolving") && ParentObject.IsPlayer())
                {
                    if (Popup.ShowYesNo("This liquid is harmful to you, continue") != 0)
                    {
                        AutoAct.Interrupt();
                        return false;
                    }
                    else
                    {

                    }
                }
            }
            //...
            return base.FireEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }


    }
}
