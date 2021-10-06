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

using Genkit;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using XRL.Core;
using XRL.Messages;
using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class IntuitLook : IPart
    {
        public List<string> MutationInformation;
        private string[] ListOfMutations;

        public override bool WantEvent(int ID, int cascade)
        {

            return ID == GetShortDescriptionEvent.ID;

        }


        public string EntityData()
        {
            // AddPlayerMessage("Beginning Entity Data Grab");


            GameObject Target = ParentObject;


            // AddPlayerMessage("Entity Data: Scores");

            string TargetsStrengthScore = Target.Statistics.TryGetValue("Strength", out var str) ? str.Value.ToString() : "N/A";
            string TargetsAgilityScore = Target.Statistics.TryGetValue("Agility", out var agi) ? agi.Value.ToString() : "N/A";
            string TargetsToughnessScore = Target.Statistics.TryGetValue("Toughness", out var tou) ? tou.Value.ToString() : "N/A";
            string TargetsintelligenceScore = Target.Statistics.TryGetValue("Toughness", out var inte) ? inte.Value.ToString() : "N/A";
            string TargetsWillpowerScore = Target.Statistics.TryGetValue("Willpower", out var wil) ? wil.Value.ToString() : "N/A";
            string TargetsEgoScore = Target.Statistics.TryGetValue("Ego", out var ego) ? ego.Value.ToString() : "N/A";

            // AddPlayerMessage("Entity Data: Modifiers");

            string TargetsStrengthMod = Target.Statistics.TryGetValue("Strength", out var str2) ? str2.Modifier.ToString() : " ";
            string TargetsAgilityMod = Target.Statistics.TryGetValue("Agility", out var agi2) ? agi2.Modifier.ToString() : " ";
            string TargetsToughnessMod = Target.Statistics.TryGetValue("Toughness", out var tou2) ? tou2.Modifier.ToString() : " ";
            string TargetsintelligenceMod = Target.Statistics.TryGetValue("Toughness", out var inte2) ? inte2.Modifier.ToString() : " ";
            string TargetsWillpowerMod = Target.Statistics.TryGetValue("Willpower", out var wil2) ? wil2.Modifier.ToString() : " ";
            string TargetsEgoMod = Target.Statistics.TryGetValue("Ego", out var ego2) ? ego2.Modifier.ToString() : " ";

            // AddPlayerMessage("Entity Data: Physical Qualities");

            string TargetsAV = Target.Statistics.TryGetValue("AV", out var av) ? (av.Bonus + av.Value).ToString() : "N/A";
            string TargetsDV = Target.Statistics.TryGetValue("DV", out var dv) ? (dv.Bonus + dv.Value).ToString() : "N/A";
            string TargetsMA = Target.Statistics.TryGetValue("MA", out var ma) ? ((ma.Modifier * -1) / 2).ToString() : "N/A";

            // AddPlayerMessage("Entity Data: Resistances");

            string TargetsFireResist = Target.Statistics.TryGetValue("HeatResistance", out var hres) ? (hres.Bonus + hres.Value).ToString() : "N/A";
            string TargetsColdResist = Target.Statistics.TryGetValue("ColdResistance", out var cres) ? (cres.Bonus + cres.Value).ToString() : "N/A";
            string TargetsAcidResist = Target.Statistics.TryGetValue("AcidResistance", out var ares) ? (ares.Bonus + ares.Value).ToString() : "N/A";
            string TargetsElecResist = Target.Statistics.TryGetValue("ElectricResistance", out var eres) ? (eres.Bonus + eres.Value).ToString() : "N/A";

            // var GettingMutations = Target.GetMutationNames();

            // foreach (var M in GettingMutations)
            // {
            //     MutationInformation.Add("{{M|" + M + "}}\n");
            //     ListOfMutations = MutationInformation.ToArray();
            // }

            // if (GettingMutations != null)
            //     return "{{w|Intuited Information:}}\n\n" +
            //     "{{Y|Strength: }}" + TargetsStrengthScore + "{{B|(" + TargetsStrengthMod + ")}}\n" +
            //     "{{Y|Agility: }}" + TargetsAgilityScore + "{{B|(" + TargetsAgilityMod + ")}}\n" +
            //     "{{Y|Toughness: }}" + TargetsToughnessScore + "{{B|(" + TargetsToughnessMod + ")}}\n" +
            //     "{{Y|Intelligence: }}" + TargetsintelligenceScore + "{{B|(" + TargetsintelligenceMod + ")}}\n" +
            //     "{{Y|Willpower: }}" + TargetsWillpowerScore + "{{B|(" + TargetsWillpowerMod + ")}}\n" +
            //     "{{Y|Ego: }}" + TargetsEgoScore + "{{B|(" + TargetsEgoMod + ")}}\n\n" +
            //     "{{Y|AV: }}" + TargetsAV + "\n" +
            //     "{{Y|DV: }}" + TargetsDV + "\n" +
            //     "{{Y|MA: }}" + TargetsMA + "\n\n" +
            //     "{{R|Fire Resistance: }}" + TargetsFireResist + "\n" +
            //     "{{B|Cold Resistance: }}" + TargetsColdResist + "\n" +
            //     "{{G|Acid Resistance: }}" + TargetsAcidResist + "\n" +
            //     "{{W|Lightning Resistance: }}" + TargetsElecResist + "\n" +
            //     "\n\n" +
            //     "{{M|Mutations: }}" + "\n\n" +

            //     ;
            // else
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
            "{{W|Lightning Resistance: }}" + TargetsElecResist;
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            // AddPlayerMessage("Adding Intuit Part to creatures and Starting ShortDescript");

            E.Postfix.Append("\n").Append(EntityData());

            return true;
        }
    }
}