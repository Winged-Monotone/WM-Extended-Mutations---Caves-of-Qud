using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.UI;
using XRL.World.Effects;
using System.Linq;

namespace XRL.World.Parts.Mutation
{
    [Serializable]


    public class Gills : BaseMutation
    {
        public Guid DiveActivatedAbility;
        public Guid DeepStrikeActivatedAbility;


        public bool Submerged = false;

        public const string MOD_PREFIX = "Gills";

        public Gills()
        {
            this.DisplayName = "Gills";
        }
        public override string GetDescription()
        {
            return "Submerge yourself in deep pools of non-visqous liquids for extended periods of time.";
        }
        public override string GetLevelText(int Level)
        {
            if (ParentObject != null)
            {
                return "You can now submerge yourself in deep enough pools of liquid, gaining a pleathora of benefits:"
               + "\n\n"
               + "\t Enemies cannot reach you unless they have a ranged weapon or are also submerged.\n"
               + "\t You regenerate HP twice as fast while submerged.\n"
               + "\t {{B|+100}} Reputation with Fish and Frogs."
               + "\n\n"
               + "Submersion Duration: " + (ParentObject.Statistics["Toughness"].Modifier * 10) * Level;
            }
            else
            {
                return "You can now submerge yourself in deep enough pools of liquid, gaining a pleathora of benefits:"
               + "\n\n"
               + "\t Enemies cannot reach you unless they have a ranged weapon or are also submerged.\n"
               + "\t You regenerate HP twice as fast while submerged.\n"
               + "\t {{B|+100}} Reputation with Fish and Frogs."
               + "\n\n"
               + "Submersion Duration: 10";

            }
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("Fish", 100, false);
            XRL.Core.XRLCore.Core.Game.PlayerReputation.modify("Frogs", 100, false);

            this.DeepStrikeActivatedAbility = base.AddMyActivatedAbility("Deep-Strike", "DeepStrikeCommand", "Power", null, "v", null, false, false, false, false, false);
            this.DiveActivatedAbility = base.AddMyActivatedAbility("Dive", "DiveCommand", "Physical Mutation", null, "v", null, false, false, false, false, false);

            return base.Mutate(GO, Level);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == GetShortDescriptionEvent.ID;
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.Append("Gills swell and shutter at " + ParentObject.its + " sides, glistening with " + ParentObject.its + " glandular skin.");
            return true;
        }

        private List<string> SubmergableLiquids = new List<string>()
        {
            "SaltyWaterPool",
            "SaltyWaterDeepPool",
            "SaltPool",
            "BrackishPool",
            "FreshWaterPool",
            "FreshWaterPool300",
            "SaltyWaterExtraDeepPool",
            "ConvalessencePool",
            "OilDeepPool",
            "LavaPool",
        };

        public override void Register(GameObject ParentObject)
        {
            ParentObject.RegisterPartEvent((IPart)this, "BeginMove");
            ParentObject.RegisterPartEvent((IPart)this, "DiveCommand");
            ParentObject.RegisterPartEvent((IPart)this, "DeepStrikeCommand");
            ParentObject.RegisterPartEvent((IPart)this, "EndTurn");

            base.Register(ParentObject);
        }

        public override bool FireEvent(Event E)
        {
            //...
            if (E.ID == "BeginMove" && ParentObject.HasEffect("Submerged"))
            {
                Cell Cell = E.GetParameter("DestinationCell") as Cell;
                if (((!Cell.HasObjectWithPart("LiquidVolume") || (Cell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume < 200) && ParentObject.IsPlayer() && ParentObject.HasEffect("Submerged")))
                {
                    if (Popup.ShowYesNo("Surface and go ashore?") == 0)
                    {
                        ParentObject.Splash("{{b|*}}");
                        ParentObject.RemoveEffect("Submerged");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            if (E.ID == "DiveCommand")
            {
                Cell Cell = ParentObject.GetCurrentCell();

                Mutations ParentsMutations = ParentObject.GetPart<Mutations>();
                if (!Cell.HasObjectWithPart("LiquidVolume"))
                {
                    AddPlayerMessage("You try to dive into the earth, you imagine this would be easier if the ground were, say, just a tad less hard.");
                }
                else if ((Cell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume < 200)
                {
                    AddPlayerMessage("Its too shallow to dive in!");
                }
                else if (ParentObject.HasEffect("Submerged"))
                {
                    // AddPlayerMessage("Your return to the surface.");
                    ParentObject.Splatter("{{B|*}}");
                    ParentObject.RemoveEffect("Submerged");
                }
                else if ((Cell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume >= 200 && ParentsMutations.HasMutation("Amphibious"))
                {
                    AddPlayerMessage("You feel right at home.");
                    ParentObject.Splatter("{{B|*}}");
                    ParentObject.ApplyEffect(new Submerged(Duration: Effect.DURATION_INDEFINITE));
                }
                else if ((Cell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume >= 200 && !ParentsMutations.HasMutation("Amphibious"))
                {
                    ParentObject.Splatter("{{B|*}}");
                    ParentObject.ApplyEffect(new Submerged(Duration: Effect.DURATION_INDEFINITE));
                }
            }

            if (E.ID == "EndTurn")
            {
                if (ParentObject.IsHealingPool() && ParentObject.HasEffect("Submerged"))
                {
                    ParentObject.Heal(ParentObject.Statistics["Toughness"].Modifier);
                }

            }
            //...
            if (E.ID == "DeepStrikeCommand")
            {
                if (!ParentObject.HasEffect("Submerged"))
                {
                    AddPlayerMessage("You must be submerged in deep pools of liquid to use this attack.");
                }
                else
                {
                    string Direction = E.GetStringParameter("Direction");
                    if (Direction == null)
                    {
                        if (ParentObject.IsPlayer())
                        {
                            Direction = PickDirectionS();
                        }
                        if (Direction == null)
                        {
                            return false;
                        }
                    }

                    DidX("rushes from the deep", "to deliver an attack", null, null, ParentObject);

                    try
                    {
                        Event e = Event.New("CommandAttackDirection", "Direction", Direction);
                        bool num11 = FireEvent(e);
                        if (!num11)
                        {
                            return false;
                        }
                        else
                        {
                            ParentObject.UseEnergy(500);
                        }
                    }
                    catch
                    {

                    }
                }
            }

            return base.FireEvent(E);
        }
    }
}