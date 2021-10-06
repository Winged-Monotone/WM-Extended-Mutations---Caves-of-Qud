using System;
using System.Collections.Generic;
using System.Text;
using XRL.Core;
using XRL.UI;
using XRL.World.Parts.Mutation;
using XRL.World.Parts;
using XRL.Rules;


namespace XRL.World.Effects
{
    [Serializable]
    public class Darkening : Effect
    {
        public GameObject Owner;

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
            return "You've wrought an aura of darkness around yourself.\n\n"
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

        public void CreateMagicalDarknessEffect()
        {
            try
            {
                if (!Object.HasPart("Blackout"))
                {


                    // AddPlayerMessage("Status Check: Apply Effect");


                    int ParentsEgoModifer = Object.Statistics["Ego"].Modifier;

                    Object.AddPart<Blackout>();


                    // AddPlayerMessage("Status Check: Applied Blackout");


                    Umbrameum UmbralHook = Object.GetPart<Umbrameum>();
                    Blackout BlackOutHook = Object.GetPart<Blackout>();

                    // AddPlayerMessage("Status Check: Getting Hooks");

                    List<Cell> AdjacentCells;

                    int MaximumRadius = Math.Min(UmbralHook.Level / 2, 4);

                    if (UmbralHook.Level > 3)
                        AdjacentCells = Object.CurrentCell.GetAdjacentCells(MaximumRadius);
                    else
                        AdjacentCells = Object.CurrentCell.GetAdjacentCells();


                    // AddPlayerMessage("Status Check: Getting Adjacent Cells");


                    foreach (var Cell in AdjacentCells)
                    {
                        // AddPlayerMessage("Status Check: For Each Loop");


                        GameObject gameObject = Cell.AddObject("MagicalDarkness");
                        MagicalDarkness Darkness = gameObject.GetPart<MagicalDarkness>();

                        // AddPlayerMessage("Status Check: Setting up Objects");
                        int Ran = Stat.Random(0, 3);
                        Darkness.Duration = ((UmbralHook.Level * (10 + ParentsEgoModifer) / 2) + Ran);
                        Darkness.Owner = Object;

                        // AddPlayerMessage("Status Check: Setting Darkness Duration: " + Darkness.Duration);

                    }

                    // AddPlayerMessage("Status Check: Ending Foreach Loop");

                    BlackOutHook.Radius = UmbralHook.Level / 2;
                    Duration = UmbralHook.Level * (10 + ParentsEgoModifer);

                    // AddPlayerMessage("Status Check: Setting Efect Duration: " + Duration);

                }
            }
            catch
            {

            }
        }
        public void ConstantDarknessEffect()
        {
            try
            {
                if (Duration > 0)
                    --Duration;

                int ParentsEgoModifer = Object.Statistics["Ego"].Modifier;

                List<Cell> AdjacentCells;

                Umbrameum UmbralHook = Object.GetPart<Umbrameum>();
                Blackout BlackOutHook = Object.GetPart<Blackout>();

                int MaximumRadius = Math.Min(UmbralHook.Level / 2, 8);

                if (UmbralHook.Level > 3)
                    AdjacentCells = Object.CurrentCell.GetAdjacentCells(MaximumRadius);
                else
                    AdjacentCells = Object.CurrentCell.GetAdjacentCells();

                foreach (var Cell in AdjacentCells)
                {
                    if (!Cell.HasObject("MagicalDarkness"))
                    {
                        // AddPlayerMessage("Status Check: For Each Loop");

                        GameObject gameObject = Cell.AddObject("MagicalDarkness");
                        MagicalDarkness Darkness = gameObject.GetPart<MagicalDarkness>();

                        // AddPlayerMessage("Status Check: Setting up Objects");

                        int Ran = Stat.Random(0, 3);
                        Darkness.Duration = ((UmbralHook.Level * (10 + ParentsEgoModifer) / 2) + Ran);

                        // AddPlayerMessage("Status Check: Setting Darkness Duration: " + Darkness.Duration);
                    }
                }
            }
            catch
            {

            }
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                ConstantDarknessEffect();
            }
            return base.FireEvent(E);
        }

        public override bool Apply(GameObject Object)
        {
            base.PlayWorldSound("DarknessShroud", 75f, 0, true, null);
            CreateMagicalDarknessEffect();
            return true;
        }

        public override void Remove(GameObject Object)
        {
            AddPlayerMessage("{{gray|The shroud of darkness about you disperses.}}");

            if (Object.HasPart("Blackout"))
            {
                Object.RemovePart<Blackout>();
            }
            StatShifter.RemoveStatShifts();
        }
    }
}