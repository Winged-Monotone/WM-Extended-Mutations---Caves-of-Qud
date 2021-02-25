using System;
using System.Collections.Generic;
using System.Text;
using XRL.Core;
using XRL.UI;
using XRL.World.Parts.Mutation;
using XRL.World.Parts;

namespace XRL.World.Effects
{
    [Serializable]
    public class Temperatura : Effect
    {
        public bool InitialEffect = true;
        public int CurrentEffectBoost;
        public int NewEffectBoost;
        public GameObject Owner;
        public int Charges = 0;
        public string Element = null;
        public GameObject Caster = null;
        public string IncreaseOrDecrease = null;

        public Temperatura() : base()
        {
            base.DisplayName = "{{yellow|Temperatura}}";
        }
        public Temperatura(int Duration, GameObject Owner)
        {
            this.Owner = Owner;
            base.Duration = Duration;
            base.DisplayName = "{{yellow|Temperatura}}";
        }
        public override string GetDetails()
        {
            return "Something is altering " + Object.its + " resistances to the elements.\n\n"
            + "Duration: " + Duration;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == EndTurnEvent.ID;
        }

        public override void Register(GameObject go)
        {
            go.RegisterEffectEvent((Effect)this, "AlteringTemperatureEffectEvent");
            go.RegisterEffectEvent((Effect)this, "TemperatureEffectChangeEvent");
            go.RegisterEffectEvent((Effect)this, "BeginTakeAction");
        }

        public override void Unregister(GameObject go)
        {
            go.UnregisterEffectEvent((Effect)this, "AlteringTemperatureEffectEvent");
            go.UnregisterEffectEvent((Effect)this, "TemperatureEffectChangeEvent");
            go.UnregisterEffectEvent((Effect)this, "BeginTakeAction");
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "AlteringTemperatureEffectEvent")
            {
                // AddPlayerMessage("Event Firing: " + "AlteringTemperatureEffectEvent");

                var eCharges = E.GetIntParameter("ChargesSpent");
                var eElement = E.GetStringParameter("Element");
                var eCaster = E.GetGameObjectParameter("Caster");
                var eIncreaseOrDecrease = E.GetStringParameter("IncreaseOrDecrease");
                var eLevel = E.GetIntParameter("MutationLevel");


                // AddPlayerMessage("Duration: " + Duration);
                // AddPlayerMessage("ResBonus: " + CurrentEffectBoost);
                // AddPlayerMessage("IncreaseOrDecrease: " + eIncreaseOrDecrease);
                // AddPlayerMessage("Charges: " + eCharges);
                // AddPlayerMessage("Element: " + eElement);
                // AddPlayerMessage("Caster: " + eCaster);

                if (InitialEffect == false)
                {
                    Charges = eCharges;
                    Element = eElement;
                    Caster = eCaster;
                    IncreaseOrDecrease = eIncreaseOrDecrease;

                    return base.FireEvent(E);
                }

                int CastersEgo = eCaster.Statistics["Ego"].Modifier;

                // AddPlayerMessage("Event Step: " + "Gathered Parameters / Starting Effect Mechanics");

                // AddPlayerMessage("Event Step: " + "First Application of Mechanics");

                CurrentEffectBoost = (eLevel + CastersEgo) * eCharges;

                if (eIncreaseOrDecrease == "Increase Resistances")
                {
                    // AddPlayerMessage("Event Step: " + "Increase Res to Element: " + eElement);

                    StatShifter.SetStatShift(eElement, CurrentEffectBoost);
                }

                if (eIncreaseOrDecrease == "Decrease Resistances")
                {
                    // AddPlayerMessage("Event Step: " + "Decrease Res to Element: " + eElement);

                    StatShifter.SetStatShift(eElement, CurrentEffectBoost * -1);
                }

                Duration += 10 * (CurrentEffectBoost);
            }
            else if (E.ID == "TemperatureEffectChangeEvent")
            {
                var eCharges = E.GetIntParameter("ChargesSpent");
                var eElement = E.GetStringParameter("Element");
                var eIncreaseOrDecrease = E.GetStringParameter("IncreaseOrDecrease");

                NewEffectBoost = CurrentEffectBoost * Charges;
                if (CurrentEffectBoost < NewEffectBoost)
                {
                    if (eIncreaseOrDecrease == "Increase Resistances")
                    {
                        // AddPlayerMessage("Event Step: " + "Increase Res to Element: " + eElement);

                        StatShifter.SetStatShift(eElement, CurrentEffectBoost);

                    }
                    if (eIncreaseOrDecrease == "Decrease Resistances")
                    {
                        // AddPlayerMessage("Event Step: " + "Decrease Res to Element: " + eElement);

                        StatShifter.SetStatShift(eElement, CurrentEffectBoost * -1);
                    }
                }
                else
                    Duration += Charges * CurrentEffectBoost;
            }
            else if (E.ID == "BeginTakeAction")
            {

                int OwnersEgoMod = Owner.Statistics["Ego"].Modifier;
                var MutationHook = Owner.GetPart<Thermokinesis>();

                int mutationLevelhook = MutationHook.Level;

                if (Object.HasEffect("Temperatura") && IncreaseOrDecrease == "Decrease Resistances")
                {
                    if (Object.MakeSave("Willpower", 10 + OwnersEgoMod + mutationLevelhook, null, null, "Thermokinesis"))
                    {
                        Duration -= Object.Statistics["Willpower"].Modifier;
                    }
                }
                else
                    --Duration;

                // AddPlayerMessage("Duration: " + Duration);
                // AddPlayerMessage("ResBonus: " + CurrentEffectBoost);
                // AddPlayerMessage("newResBonus: " + NewEffectBoost);

            }

            return base.FireEvent(E);
        }

        public override bool Apply(GameObject Object)
        {
            if (Object.HasEffect("Temperatura"))
            {
                InitialEffect = false;
                Event cE = Event.New("TemperatureEffectChangeEvent", "ChargesSpent", Charges, "Element", Element, "IncreaseOrDecrease", IncreaseOrDecrease);
                FireEvent(cE);
            }
            return true;
        }

        public override void Remove(GameObject Object)
        {
            CurrentEffectBoost = 0;
            NewEffectBoost = 0;
            // XDidY(Object, "emerge", "from the depths", "!");
            StatShifter.RemoveStatShifts();
        }
    }
}