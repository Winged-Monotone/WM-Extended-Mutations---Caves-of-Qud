// XRL.World.Effects.HeightenedHearingEffect
using System;
using XRL.UI;
using XRL.World;
using XRL.World.Capabilities;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;

namespace XRL.World.Effects
{
    [Serializable]
    public class Electravoyance : Effect
    {
        public bool bIdentified;

        public int Level = 1;

        public GameObject Listener;

        public Electravoyance()
        {
            base.Duration = 1;
        }

        public Electravoyance(int Level, GameObject Listener)
            : this()
        {
            this.Level = Level;
            this.Listener = Listener;
        }

        public override int GetEffectType()
        {
            return 2048;
        }

        public override bool SameAs(Effect e)
        {
            return false;
        }

        public override string GetDescription()
        {
            return null;
        }

        private bool BadListener()
        {
            Listener = null;
            base.Object.RemoveEffect(this);
            return true;
        }

        public bool CheckListen()
        {
            if (!GameObject.validate(ref Listener) || !Listener.IsPlayer())
            {
                return BadListener();
            }
            Electrokinesis Electrokinesis = Listener.GetPart("HeightenedHearing") as Electrokinesis;
            if (Electrokinesis == null || Electrokinesis.Level <= 0)
            {
                return BadListener();
            }
            int num = base.Object.DistanceTo(Listener);
            if (num > 7)
            {
                return BadListener();
            }
            if (bIdentified)
            {
                CheckInterruptAutoExplore();
                return true;
            }
            if (base.Object.CurrentCell == null)
            {
                return true;
            }
            if (((int)((double)(100 + 10 * Level) / Math.Pow(num + 9, 2.0) * 100.0)).in100())
            {
                bIdentified = true;
                CheckInterruptAutoExplore();
            }
            return false;
        }

        public void CheckInterruptAutoExplore()
        {
            if (bIdentified && GameObject.validate(ref Listener) && Listener.IsPlayer() && AutoAct.IsInterruptable() && !AutoAct.IsGathering() && Listener.IsRelevantHostile(base.Object))
            {
                IComponent<GameObject>.AddPlayerMessage(Listener.GenerateSpotMessage(base.Object, null, "sense"));
                AutoAct.Interrupt();
            }
        }

        public override bool Apply(GameObject Object)
        {
            Brain pBrain = Object.pBrain;
            if (pBrain != null)
            {
                pBrain.Hibernating = false;
            }
            CheckListen();
            return true;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterEffectEvent(this, "EndTurn");
            base.Register(Object);
        }

        public override void Unregister(GameObject Object)
        {
            Object.UnregisterEffectEvent(this, "EndTurn");
            base.Unregister(Object);
        }

        public bool HeardAndNotSeen(GameObject obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (!obj.IsVisible())
            {
                return true;
            }
            Cell currentCell = obj.CurrentCell;
            if (currentCell != null && (!currentCell.IsLit() || !currentCell.IsExplored()))
            {
                return true;
            }
            return false;
        }

        public override bool FinalRender(RenderEvent E, bool bAlt)
        {
            if (HeardAndNotSeen(base.Object) && base.Object.FireEvent("CanHypersensesDetect"))
            {
                if (bIdentified)
                {
                    E.HighestLayer = 0;
                    base.Object.Render(E);
                    E.RenderString = base.Object.pRender.RenderString;
                    if (Options.UseTiles)
                    {
                        E.Tile = base.Object.pRender.Tile;
                    }
                    else
                    {
                        E.Tile = null;
                    }
                    E.CustomDraw = true;
                }
                else
                {
                    E.RenderString = "&K?";
                    E.Tile = null;
                    E.CustomDraw = true;
                }
                return false;
            }
            return true;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "EndTurn" && base.Object != null)
            {
                CheckListen();
            }
            return base.FireEvent(E);
        }
    }
}