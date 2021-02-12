using System;
using XRL.World;
using XRL.World.Parts.Mutation;
using System.Collections.Generic;
using XRL.Rules;
using System.Linq;
using XRL.World.Effects;
using XRL.Language;
using XRL.World.Capabilities;
using UnityEngine;

namespace XRL.World.Parts
{

    [Serializable]
    public class IntuitLook : IPart
    {
        public override bool WantEvent(int ID, int cascade)
        {

            return ID == GetShortDescriptionEvent.ID;

        }


        public string EntityData()
        {
            AddPlayerMessage("Beginning Entity Data Grab");

            GameObject Target = ParentObject;

            AddPlayerMessage("Entity Data: Scores");

            int TargetsStrengthScore = Target.Statistics["Strength"].Value;
            int TargetsAgilityScore = Target.Statistics["Agility"].Value;
            int TargetsToughnessScore = Target.Statistics["Toughness"].Value;
            int TargetsintelligenceScore = Target.Statistics["Intelligence"].Value;
            int TargetsWillpowerScore = Target.Statistics["Willpower"].Value;
            int TargetsEgoScore = Target.Statistics["Ego"].Value;

            AddPlayerMessage("Entity Data: Modifiers");

            int TargetsStrengthMod = Target.Statistics["Strength"].Modifier;
            int TargetsAgilityMod = Target.Statistics["Agility"].Modifier;
            int TargetsToughnessMod = Target.Statistics["Toughness"].Modifier;
            int TargetsintelligenceMod = Target.Statistics["Intelligence"].Modifier;
            int TargetsWillpowerMod = Target.Statistics["Willpower"].Modifier;
            int TargetsEgoMod = Target.Statistics["Ego"].Modifier;

            AddPlayerMessage("Entity Data: Physical Qualities");

            int TargetsAV = ParentObject.Statistics["AV"].Bonus + ParentObject.Statistics["AV"].Value - 1;
            int TargetsDV = ParentObject.Statistics["DV"].Bonus + ParentObject.Statistics["DV"].Value - 1;
            int TargetsMA = (ParentObject.Statistics["MA"].Modifier * -1) / 2;

            AddPlayerMessage("Entity Data: Resistances");

            int TargetsFireResist = Target.Statistics["HeatResistance"].Value + Target.Statistics["HeatResistance"].Bonus;
            int TargetsColdResist = Target.Statistics["ColdResistance"].Value + Target.Statistics["ColdResistance"].Bonus;
            int TargetsAcidResist = Target.Statistics["AcidResistance"].Value + Target.Statistics["AcidResistance"].Bonus;
            int TargetsElecResist = Target.Statistics["ElectricResistance"].Value + Target.Statistics["ElectricResistance"].Bonus;


            return "{{w|Intuited Information:}}\n\n" +
            "{{Y|Strength: }}" + TargetsStrengthScore + "{{B|(" + TargetsStrengthMod + ")}}\n" +
            "{{Y|Agility: }}" + TargetsAgilityScore + "{{B|(" + TargetsAgilityMod + ")}}\n" +
            "{{Y|Toughness: }}" + TargetsToughnessScore + "{{B|(" + TargetsToughnessMod + ")}}\n" +
            "{{Y|Intelligence: }}" + TargetsintelligenceScore + "{{B|(" + TargetsintelligenceMod + ")}}\n" +
            "{{Y|Willpower: }}" + TargetsWillpowerScore + "{{B|(" + TargetsWillpowerMod + ")}}\n" +
            "{{Y|Ego: }}" + TargetsEgoScore + "{{B|(" + TargetsEgoMod + ")}}\n\n" +
            "{{Y|AV: }}" + TargetsAV + "\n" +
            "{{Y|DV: }}" + TargetsDV + "\n" +
            "{{Y|MA: }}" + TargetsMA + "\n\n" +
            "{{R|Fire Resistance: }}" + TargetsFireResist + "\n" +
            "{{B|Cold Resistance: }}" + TargetsColdResist + "\n" +
            "{{G|Acid Resistance: }}" + TargetsAcidResist + "\n" +
            "{{W|Lightning Resistance: }}" + TargetsElecResist + "\n"
            ;
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            AddPlayerMessage("Adding Intuit Part to creatures and Starting ShortDescript");

            E.Postfix.Append("\n").Append(EntityData());

            return true;
        }
    }
}