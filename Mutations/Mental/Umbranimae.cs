using System;
using XRL.Rules;
using XRL.UI;
using Qud.API;
using XRL.Language;
using XRL.World.Effects;

namespace XRL.World.Parts.Mutation
{

    [Serializable]
    public class Umbranimae : BaseMutation
    {
        //Properties/Member Variables / Not Static, Exist on the Instance of this Class
        public Guid ActivatedAbilitiesID;
        public Guid ShadeSelfActivatedAbilityID;
        public Guid ShadowAreaEffectActivatedAbilityID;

        public Umbranimae()
        {
            this.DisplayName = "Umbranimae";
            this.Type = "Mental";
        }



        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CastShadowCommand");
            Object.RegisterPartEvent(this, "ShadeSelfCommand");

            base.Register(Object);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            this.ShadowAreaEffectActivatedAbilityID = base.AddMyActivatedAbility("Cast Shadow", "CastShadowCommand", "Mental Mutation", null, "\u03A9");
            this.ShadeSelfActivatedAbilityID = base.AddMyActivatedAbility("Shade self", "ShadeSelfCommand", "Mental Mutation", null, "\u03A9");

            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            StatShifter.RemoveStatShifts();
            base.RemoveMyActivatedAbility(ref this.ActivatedAbilitiesID);
            return base.Unmutate(GO);
        }
        public override bool CanLevel()
        {
            return false;
        }
        public override string GetDescription()
        {
            return "An aura of darkness is about you, stealing light from the world. You hide your light--along with your glimmer.\n"
            + "\n{{white|-200 Reputation with}} {{blue|Seekers of the Sightless Way.}}";
        }
        public override string GetLevelText(int Level)
        {
            return "{{white|Cast a shadow on an area or emit a constant fume of darkness around yourself. You are innately more difficult to sense through the glimmer, abilities like clairvoyance, and sense psychic are less effective on you.\n"
            + "\n";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == KilledEvent.ID;
        }

        public override bool HandleEvent(KilledEvent E)
        {
            var DyingEgo = E.Dying.StatMod("Ego");
            var KillerEgo = ParentObject.StatMod("Ego");

            var OwnersLevel = ParentObject.Stat("Level");
            var TargetsLevel = E.Dying.Stat("Level");

            var LevelDifference = OwnersLevel - TargetsLevel;
            var DevourChance = Stat.Random(1, 100) <= 10 + (LevelDifference * 0.10);

            var Killed = E.Dying;

            return base.HandleEvent(E);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CastShadowCommand")
            {

            }
            else if (E.ID == "ShadeSelfCommand")
            {
                if (ParentObject.HasEffect("Darkening"))
                {
                    return base.FireEvent(E);
                }
                else
                {
                    ParentObject.ApplyEffect(new Darkening());
                }
            }

            return base.FireEvent(E);

        }
    }
}