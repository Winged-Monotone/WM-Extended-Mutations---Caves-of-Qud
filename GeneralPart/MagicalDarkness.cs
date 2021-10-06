using System;
using XRL.World.Effects;
using XRL.World;
using XRL.World.Parts.Mutation;
using System.Linq;
using XRL.Core;
using XRL.Rules;
using XRL.World.AI;
using XRL.World.AI.GoalHandlers;

namespace XRL.World.Parts
{
    [Serializable]
    public class MagicalDarkness : IPart
    {
        public int Duration = 0;
        public GameObject Owner = null;

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == ObjectEnteredCellEvent.ID
            || ID == ObjectLeavingCellEvent.ID
            || ID == ObjectCreatedEvent.ID;
        }
        public override bool HandleEvent(ObjectEnteredCellEvent E)
        {
            if (E.Object != ParentObject && E.Object.HasPart("Combat") && !E.Object.IsPlayer())
            {
                if (Owner != null)
                {

                    int OwnersLevel = Owner.Statistics["Level"].BaseValue;

                    if (!E.Object.MakeSave("Ego", 20 + (OwnersLevel / 2), Owner, null, "Willpower"))
                        E.Object.ApplyEffect(new Terrified(10, ParentObject, true));
                    if (!E.Object.MakeSave("Ego", 15 + (OwnersLevel / 2), Owner, null, "Willpower"))
                    {
                        if (!E.Object.MakeSave("Ego", 15 + (OwnersLevel / 2), Owner, null, "Willpower"))
                        {
                            E.Object.pBrain.Goals.Clear();
                            E.Object.pBrain.MaxKillRadius /= 2;
                            E.Object.pBrain.MinKillRadius /= 2;
                            E.Object.pBrain.PushGoal(new WanderRandomly());
                        }
                        else if (!E.Object.MakeSave("Ego", 10 + (OwnersLevel / 2), Owner, null, "Willpower"))
                        {
                            E.Object.pBrain.Goals.Clear();
                            E.Object.pBrain.MaxKillRadius /= 2;
                            E.Object.pBrain.MinKillRadius /= 2;
                            E.Object.pBrain.PushGoal(new Wait(10));
                        }
                        else if (!E.Object.MakeSave("Ego", 5 + (OwnersLevel / 2), Owner, null, "Willpower"))
                        {
                            E.Object.pBrain.MaxKillRadius /= 2;
                            E.Object.pBrain.MinKillRadius /= 2;
                        }
                        else

                        {
                            E.Object.pBrain.Goals.Clear();
                        }
                    }
                }
                else
                {
                    if (!E.Object.MakeSave("Ego", 20, null, null, "Willpower"))
                        E.Object.ApplyEffect(new Terrified(10, ParentObject, true));
                    if (!E.Object.MakeSave("Ego", 15, null, null, "Willpower"))
                    {
                        if (Stat.Random(1, 100) <= 30)
                        {
                            E.Object.pBrain.Goals.Clear();
                            E.Object.pBrain.MaxKillRadius /= 2;
                            E.Object.pBrain.MinKillRadius /= 2;
                            E.Object.pBrain.PushGoal(new WanderRandomly());
                        }
                        else if (Stat.Random(1, 100) <= 60)
                        {
                            E.Object.pBrain.Goals.Clear();
                            E.Object.pBrain.MaxKillRadius /= 2;
                            E.Object.pBrain.MinKillRadius /= 2;
                            E.Object.pBrain.PushGoal(new Wait(10));
                        }
                        else if (Stat.Random(1, 100) <= 100)
                        {
                            E.Object.pBrain.MaxKillRadius /= 2;
                            E.Object.pBrain.MinKillRadius /= 2;
                        }
                    }
                }
            }

            return base.HandleEvent(E);
        }

        public override bool HandleEvent(ObjectLeavingCellEvent E)
        {
            if (E.Object != ParentObject && E.Object.HasPart("Combat") && !E.Object.IsPlayer())
            {
                if (Owner != null)
                {

                    E.Object.pBrain.MaxKillRadius *= 2;
                    E.Object.pBrain.MinKillRadius *= 2;
                    E.Object.pBrain.PushGoal(new WanderRandomly());

                }
            }

            return base.HandleEvent(E);
        }


        public override bool HandleEvent(ObjectCreatedEvent E)
        {
            ParentObject.AddPart<Blackout>();
            Blackout BlackOutHook = ParentObject.GetPart<Blackout>();

            BlackOutHook.Radius = 0;

            return base.HandleEvent(E);
        }

        public static string[] ColorList = new string[3]
        {
        "&K",
        "&m",
        "&k",
        };

        public string GetDarknesBlips()
        {
            return ColorList.GetRandomElement();
        }

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "EndTurn");
            base.Register(go);
        }


        public override bool FireEvent(Event E)
        {
            if (E.ID == "EndTurn")
            {
                if (Duration > 0)
                    --Duration;
                else
                {
                    XRLCore.ParticleManager.AddSinusoidal(GetDarknesBlips() + "\u0000", currentCell.X, currentCell.Y, 1.5f * (float)Stat.Random(1, 6), 0.1f * (float)Stat.Random(1, 60), 0.1f + 0.025f * (float)Stat.Random(0, 4), 1f, 0f, 0f, -0.1f - 0.05f * (float)Stat.Random(1, 6), 999);
                    ParentObject.Obliterate(null, true);
                }
            }

            return base.FireEvent(E);
        }
    }
}