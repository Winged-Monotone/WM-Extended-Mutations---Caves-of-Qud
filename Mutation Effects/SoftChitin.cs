using System;
using XRL.UI;

namespace XRL.World.Effects
{
    [Serializable]
    public class SoftChitin : Effect
    {
        public int avDebuff;
        public int movementSpeedBuff;
        public int saveTarget;
        public int saveTargetTurnDivisor;
        public int turns;

        public SoftChitin(int avDebuff)
        {
            Initialize(avDebuff);
        }
        public SoftChitin()
        {
            Initialize(0);
        }
        public void Initialize(int avDebuff)
        {
            base.DisplayName = "Newly-Shed";
            base.Duration = 1;
            this.avDebuff = avDebuff;
            saveTarget = 3600;
            // saveTarget = 10;
            saveTargetTurnDivisor = 1;
            turns = 0;
        }


        public override void Register(GameObject go)
        {
            go.RegisterEffectEvent((Effect)this, "BeginTakeAction");
        }

        public override void Unregister(GameObject go)
        {
            go.UnregisterEffectEvent((Effect)this, "BeginTakeAction");
        }


        public void setAVDebuff(int avDebuff, GameObject ParentObject)
        {
            this.avDebuff = avDebuff;
            StatShifter.SetStatShift(ParentObject, "AV", -avDebuff);
        }

        public override string GetDetails()
        {
            return "Your chitin is currently soft and malleable, until it hardens you lose your natural AV bonus, but now move slightly faster.";
        }

        public override bool Apply(GameObject Object)
        {
            int StrengthDeBuff = (int)((float)Object.Statistics["Strength"].BaseValue / 2);
            movementSpeedBuff = (int)((float)Object.Statistics["MoveSpeed"].BaseValue * 0.25f);
            StatShifter.SetStatShift("MoveSpeed", -movementSpeedBuff);
            StatShifter.SetStatShift("AV", -avDebuff);
            StatShifter.SetStatShift("Strength", -StrengthDeBuff);
            if (Object.IsPlayer())
            {
                Popup.Show("Your chitin is currently soft and malleable, until it hardens you lose your natural AV bonus and half of your strength bonus, but now move slightly faster.", true);
            }
            return true;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                if (saveTarget > 0)
                {
                    int effectiveSaveTarget = 15 + avDebuff;
                    if (Object.MakeSave("Toughness", effectiveSaveTarget, null, null, "SoftChitin"))
                    {
                        saveTarget -= 10;
                    }
                    else
                    {
                        saveTarget--;
                    }
                }
                else
                {
                    Object.RemoveEffect(this);
                }

                this.turns++;
            }
            return true;
        }

        public int GetEffectiveSaveTarget()
        {
            int num = this.saveTarget;
            if (this.saveTargetTurnDivisor != 0)
            {
                num -= this.turns / this.saveTargetTurnDivisor;
            }
            return num;
        }

        public override void Remove(GameObject Object)
        {
            StatShifter.RemoveStatShifts();
            if (Object.IsPlayer())
            {
                Popup.Show("Your chitinous skin has hardened.");
            }

            Object.FireEvent(Event.New("CommandChitinHarden"));
        }
    }
}