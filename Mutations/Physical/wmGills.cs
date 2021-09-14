using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.UI;
using XRL.World.Effects;
using System.Linq;
using HarmonyLib;
using WingysMod.HarmonyPatches;

namespace XRL.World.Parts.Mutation
{
    [Serializable]


    public class wmGills : BaseMutation
    {
        public Guid DiveActivatedAbility;
        public Guid DeepStrikeActivatedAbility;


        public bool Submerged = false;

        public const string MOD_PREFIX = "Gills";

        public wmGills()
        {
            this.DisplayName = "Gills";
        }
        public override string GetDescription()
        {
            return "Submerge yourself in deep pools of non-visqous liquids for extended periods of time.";
        }
        public override string GetLevelText(int Level)
        {
            return "You can now submerge yourself in deep enough pools of liquid, gaining a pleathora of benefits:"
           + "\n\n"
           + "Enemies cannot reach you unless they have a ranged weapon or are also submerged.\n"
           + "You regenerate HP twice as fast while submerged.\n"
           + "{{cyan|+100}} Reputation with Fish and Frogs.";
        }
        public override bool CanLevel()
        {
            return false;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            this.DeepStrikeActivatedAbility = base.AddMyActivatedAbility(Name: "Submerged-Strike", Command: "DeepStrikeCommand", Class: "Power", Icon: "v");
            this.DiveActivatedAbility = base.AddMyActivatedAbility(Name: "Dive", Command: "DiveCommand", Class: "Physical Mutation", Icon: "v");

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
            ParentObject.RegisterPartEvent((IPart)this, "AIGetOffensiveMutationList");
            ParentObject.RegisterPartEvent((IPart)this, "BeginTakeAction");
            ParentObject.RegisterPartEvent((IPart)this, "BeginTakeAction");


            base.Register(ParentObject);
        }

        public override bool FireEvent(Event E)
        {
            //...
            if (E.ID == "Regenerating" && ParentObject.HasEffect("wmSubmerged"))
            {
                int RegenerationAmountParameter = E.GetIntParameter("Amount");
                RegenerationAmountParameter += (int)Math.Ceiling((float)RegenerationAmountParameter);
                E.SetParameter("Amount", RegenerationAmountParameter);
            }
            else if (E.ID == "BeginMove" && ParentObject.HasEffect("wmSubmerged"))
            {
                Cell Cell = E.GetParameter("DestinationCell") as Cell;
                if (((!Cell.HasObjectWithPart("LiquidVolume") || (Cell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume < 200) && ParentObject.IsPlayer() && ParentObject.HasEffect("wmSubmerged")))
                {
                    if (Popup.ShowYesNo("Surface and go ashore?") == (int)DialogResult.Yes)
                    {
                        ParentObject.Splash("{{b|*}}");
                        ParentObject.RemoveEffect("wmSubmerged");
                    }
                    else
                    {

                        return false;
                    }
                }
            }
            else if (E.ID == "DiveCommand")
            {
                Cell Cell = ParentObject.GetCurrentCell();

                Mutations ParentsMutations = ParentObject.GetPart<Mutations>();
                if (ParentObject.HasEffect("Flying"))
                {
                    if (IsPlayer())
                        AddPlayerMessage("You cannot do this while flying");
                    return false;
                }
                else if (!Cell.HasObjectWithPart("LiquidVolume"))
                {
                    AddPlayerMessage("You try to dive into the earth, you imagine this would be easier if the ground were, say, just a tad less hard.");
                    return false;
                }
                else if ((Cell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume < 200)
                {
                    AddPlayerMessage("Its too shallow to dive in!");
                    return false;
                }
                else if (ParentObject.HasEffect("wmSubmerged"))
                {
                    ParentObject.Splatter("{{B|*}}");
                    ParentObject.RemoveEffect("wmSubmerged");
                }
                else if ((Cell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume >= 200 && ParentsMutations.HasMutation("Amphibious") && !ParentObject.HasEffect("wmSubmerged"))
                {
                    AddPlayerMessage("You feel right at home.");
                    ParentObject.Splatter("{{B|*}}");
                    ParentObject.ApplyEffect(new wmSubmerged(Duration: Effect.DURATION_INDEFINITE));
                }
                else if ((Cell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume >= 200 && !ParentsMutations.HasMutation("Amphibious") && !!ParentObject.HasEffect("wmSubmerged"))
                {
                    ParentObject.Splatter("{{B|*}}");
                    ParentObject.ApplyEffect(new wmSubmerged(Duration: Effect.DURATION_INDEFINITE));
                }
            }
            else if (E.ID == "EndTurn")
            {
                Cell Cell = ParentObject.GetCurrentCell();

                if (ParentObject.HasEffect("Flying") && (ParentObject.HasEffect("wmSubmerged")))
                {
                    ParentObject.RemoveEffect(new Flying());
                    AddPlayerMessage("Removing Paradox Incident.");
                }
                else if (ParentObject.IsHealingPool() && ParentObject.HasEffect("wmSubmerged"))
                {
                    ParentObject.Heal(+ParentObject.Statistics["Toughness"].Modifier);
                }
                else if (((!Cell.HasObjectWithPart("LiquidVolume") || (Cell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume < 200) && ParentObject.HasEffect("wmSubmerged")))
                {
                    ParentObject.Splash("{{b|*}}");
                    ParentObject.RemoveEffect("wmSubmerged");
                    return false;
                }
            }
            //...---------------------------------------------------------------------------------------------
            else if (E.ID == "DeepStrikeCommand")
            {
                if (!ParentObject.HasEffect("wmSubmerged") && ParentObject.IsPlayer())
                {
                    AddPlayerMessage("You must be submerged in deep pools of liquid to use this attack.");
                }
                else if (!ParentObject.HasEffect("wmSubmerged") && !ParentObject.IsPlayer())
                {

                }
                else if (ParentObject.HasEffect("wmSubmerged"))
                {
                    string Direction = E.GetStringParameter("Direction");

                    if (Direction == null)
                    {
                        if (ParentObject != null)
                        {
                            Direction = PickDirectionS();
                            try
                            {
                                Patch_PhaseAndFlightMatches.TemporarilyDisabled = true;
                                Event e = Event.New("CommandAttackDirection", "Direction", Direction);
                                bool num11 = FireEvent(e);
                                ParentObject.FireEvent(e);
                                XDidY(ParentObject, "rush", "from the depths to strike!", "!", "C", ParentObject);
                                Patch_PhaseAndFlightMatches.TemporarilyDisabled = false;
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            else if (E.ID == "AIGetOffensiveMutationList")
            {
                if (ParentObject.CurrentCell.HasSwimmingDepthLiquid())
                {
                    if (IsMyActivatedAbilityAIUsable(DiveActivatedAbility))
                    {
                        if (!ParentObject.HasEffect("wmSubmerged") && (ParentObject.CurrentCell.GetFirstObjectWithPart("LiquidVolume") as GameObject).LiquidVolume.Volume >= 200)
                        {
                            E.AddAICommand("DiveCommand");
                        }
                    }
                    int intParameter = E.GetIntParameter("Distance");
                    if (E.GetGameObjectParameter("Target") != null && intParameter <= 1 && !ParentObject.IsFrozen() && IsMyActivatedAbilityAIUsable(DeepStrikeActivatedAbility))
                    {
                        E.AddAICommand("DeepStrikeCommand");
                    }
                }

            }
            else if (E.ID == "BeginTakeAction")
            {
                if (ParentObject.HasEffect("Flying") && (ParentObject.HasEffect("wmSubmerged")))
                {
                    ParentObject.RemoveEffect(new Flying());
                    AddPlayerMessage("Removing Paradox Incident.");
                }
            }

            return base.FireEvent(E);
        }
    }
}