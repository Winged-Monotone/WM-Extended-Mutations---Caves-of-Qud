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
    public class Constricted : Effect
    {
        public GameObject ConstrictedBy;
        public bool ChangesWereApplied;
        public int TurnsConstricted;

        public Constricted()
        {
            Duration = 1;
        }
        public Constricted(GameObject ConstrictedBy) : this()
        {
            this.ConstrictedBy = ConstrictedBy;
            base.DisplayName = "&BConstricted by " + ConstrictedBy.a + ConstrictedBy.DisplayNameOnly + "&y";
        }

        public override bool SameAs(Effect e)
        {
            return false;
        }

        public override bool Render(RenderEvent E)
        {
            if (ConstrictedBy != null && ConstrictedBy.IsValid())
            {
                int num = XRLCore.CurrentFrame % 60;
                if (num >= 0 && num <= 30)
                {
                    E.ColorString = ConstrictedBy.pRender.ColorString;
                    E.DetailColor = ConstrictedBy.pRender.DetailColor;
                    E.Tile = ConstrictedBy.pRender.Tile;
                    E.RenderString = ConstrictedBy.pRender.RenderString;
                }
            }
            return base.Render(E);
        }

        public override bool Apply(GameObject Target)
        {
            if (Target.FireEvent(Event.New("ApplyConstricted", 0, 0, 0)))
            {
                if (Target.CurrentCell != null)
                {
                    Target.CurrentCell.FireEvent(Event.New("ObjectBecomingConstricted", "Object", Target));
                }
                Target.MovementModeChanged(null, true);
                Target.BodyPositionChanged(null, true);
                return true;
            }
            return false;
        }

        public bool IsConstrictedByValid()
        {
            return ConstrictedBy != null && !ConstrictedBy.IsInvalid() && ConstrictedBy.pPhysics != null && ConstrictedBy.pPhysics.CurrentCell != null && Object != null && Object.pPhysics != null && Object.pPhysics.CurrentCell != null && ConstrictedBy.pPhysics.CurrentCell == Object.pPhysics.CurrentCell && Object.PhaseMatches(ConstrictedBy);
        }
        public bool CheckConstrictedBy()
        {
            if (!IsConstrictedByValid())
            {
                Duration = 0;
                if (Object != null)
                {
                    Object.RemoveEffect(this);
                }
                ConstrictedBy = null;
                return false;
            }
            if (ConstrictedBy != null)
            {
                base.DisplayName = "&Bconstricted by " + ConstrictedBy.a + ConstrictedBy.DisplayNameOnly + "&y";
            }
            return true;
        }
        public void ApplyChangesCore()
        {
            if (ChangesWereApplied)
            {
                UnapplyChangesCore();
            }
            if (ConstrictedBy != null)
            {
                SerpentineForm part = ConstrictedBy.GetPart<SerpentineForm>();
                if (part != null)
                {
                    foreach (var kv in part.PropertyMap)
                    {
                        // Alternate method of Dictionary, loop over keyvalue pair
                        Object.ModIntProperty(kv.Key, kv.Value, true);
                    }
                }
            }
            ChangesWereApplied = true;
        }
        private void UnapplyChangesCore()
        {
            if (!ChangesWereApplied)
            {
                return;
            }
            if (ConstrictedBy == null)
            {
                return;
            }
            SerpentineForm part = ConstrictedBy.GetPart<SerpentineForm>();
            if (part == null)
            {
                return;
            }
            foreach (var kv in part.PropertyMap)
            {
                // Alternate method of Dictionary, loop over keyvalue pair
                Object.ModIntProperty(kv.Key, -kv.Value, true);
            }
            ChangesWereApplied = false;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == EndTurnEvent.ID || ID == GetDisplayNameEvent.ID;
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            if (CheckConstrictedBy())
            {
                E.AddTag("&y[{{green|constricted by " + ConstrictedBy.a + ConstrictedBy.DisplayNameOnly + "}}]", 0);
            }
            return true;
        }
        public override bool HandleEvent(EndTurnEvent E)
        {
            if (CheckConstrictedBy())
            {
                SerpentineForm part = ConstrictedBy.GetPart<SerpentineForm>();
                part.ProcessTurnConstricted(Object, ++TurnsConstricted);
            }
            return true;
        }
        public override void Register(GameObject Object)
        {
            Object.RegisterEffectEvent(this, "BeginMove");
            Object.RegisterEffectEvent(this, "BeginTakeAction");
            Object.RegisterEffectEvent(this, "BodyPositionChanged");
            Object.RegisterEffectEvent(this, "CanChangeBodyPosition");
            Object.RegisterEffectEvent(this, "CanChangeMovementMode");
            Object.RegisterEffectEvent(this, "LeaveCell");
            Object.RegisterEffectEvent(this, "MovementModeChanged");
            base.Register(Object);
        }
        public override void Unregister(GameObject Object)
        {
            if (Object != null)
            {
                Object.UnregisterEffectEvent(this, "BeginMove");
                Object.UnregisterEffectEvent(this, "BeginTakeAction");
                Object.UnregisterEffectEvent(this, "BodyPositionChanged");
                Object.UnregisterEffectEvent(this, "CanChangeBodyPosition");
                Object.UnregisterEffectEvent(this, "CanChangeMovementMode");
                Object.UnregisterEffectEvent(this, "LeaveCell");
                Object.UnregisterEffectEvent(this, "MovementModeChanged");
            }
            base.Unregister(Object);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                if (Duration > 0)
                {
                    CheckConstrictedBy();
                }
            }
            else if (E.ID == "BeginMove")
            {
                if (Duration > 0 && E.GetIntParameter("Teleporting", 0) <= 0 && CheckConstrictedBy())
                {
                    Object.PerformMeleeAttack(ConstrictedBy);
                    if ((ConstrictedBy == null || !ConstrictedBy.IsValid()) && Object != null)
                    {
                        Object.RemoveEffect(this);
                    }
                    return false;
                }
            }
            else if (E.ID == "LeaveCell" || E.ID == "MovementModeChanged" || E.ID == "BodyPositionChanged")
            {
                Object.RemoveEffect(this);
            }
            else if (E.ID == "CanChangeMovementMode" || E.ID == "CanChangeBodyPosition")
            {
                if (CheckConstrictedBy())
                {
                    if (E.GetIntParameter("ShowMessage", 0) > 0 && Object.IsPlayer())
                    {
                        Popup.Show("You cannot do that while constricted by " + ConstrictedBy.the + ConstrictedBy.ShortDisplayName + "&y.", true);
                    }
                    return false;
                }
            }
            return base.FireEvent(E);
        }
    }
}