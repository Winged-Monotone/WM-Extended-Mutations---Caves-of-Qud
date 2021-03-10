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
    public class Darkening : Effect
    {
        public bool InitialEffect = true;
        public int CurrentEffectBoost;
        public int NewEffectBoost;
        public GameObject Owner;
        public int Charges = 0;
        public string Element = null;
        public GameObject Caster = null;
        public string IncreaseOrDecrease = null;

        public Darkening() : base()
        {
            base.DisplayName = "{{dark gray|Darkening}}";
        }
        public Darkening(int Duration, GameObject Owner)
        {
            this.Owner = Owner;
            base.Duration = Duration;
            base.DisplayName = "{{dark gray|Darkening}}";
        }
        public override string GetDetails()
        {
            return "You've casted an aura of darkness around yourself.\n\n"
            + "Duration: " + Duration;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == EndTurnEvent.ID;
        }

        public override void Register(GameObject go)
        {
            go.RegisterEffectEvent((Effect)this, "BeginTakeAction");
        }

        public override void Unregister(GameObject go)
        {
            go.UnregisterEffectEvent((Effect)this, "BeginTakeAction");
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                --Duration;
            }

            return base.FireEvent(E);
        }

        public override bool Apply(GameObject Object)
        {
            if (!Object.HasPart("Blackout"))
            {
                Duration = 100;
                Object.AddPart<Blackout>();
                Umbranimae UmbralHook = Object.GetPart<Umbranimae>();
                Blackout BlackOutHook = Object.GetPart<Blackout>();


                BlackOutHook.Radius = UmbralHook.Level / 2;

            }
            return true;
        }

        public override void Remove(GameObject Object)
        {
            if (Object.HasPart("Blackout"))
            {
                Object.RemovePart<Blackout>();
            }
            StatShifter.RemoveStatShifts();
        }
    }
}