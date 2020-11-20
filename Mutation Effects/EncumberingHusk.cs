using System;
using XRL.UI;

namespace XRL.World.Effects
{

    [Serializable]

    public class EncumberingHusk : Effect
    {
        public int agilityDebuff;
        public int movementSpeedDebuff;

        public EncumberingHusk() : base()
        {
            base.DisplayName = "Encumbering Husk";
        }

        public EncumberingHusk(int Duration)
        {
            base.DisplayName = "Encumbering Husk";
            base.Duration = 1;
        }

        public override string GetDetails()
        {
            return "Your husk encumbers you, you move 20% percent slower and your Agility is reduced by 25%.";
        }

        public override bool Apply(GameObject Object)
        {
            movementSpeedDebuff = (int)((float)Object.Statistics["MoveSpeed"].BaseValue * 0.20f);
            StatShifter.SetStatShift("MoveSpeed", -movementSpeedDebuff);
            agilityDebuff = (int)((float)Object.Statistics["Agility"].BaseValue * 0.25f);
            StatShifter.SetStatShift("Agility", -agilityDebuff);
            if (Object.IsPlayer())
            {
                Popup.Show("Your husk encumbers you, you move 20% percent slower and your Agility is reduced by 25%.", true);
            }
            return true;
        }

        public override bool FireEvent(Event E)
        {
            return true;
        }

        public override void Remove(GameObject Object)
        {
            StatShifter.RemoveStatShifts();
        }
    }

}

