using System;
using XRL.UI;
using XRL.Rules;
using XRL.World.Effects;
using System.Collections.Generic;
using System.Linq;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Immunocompromised : BaseMutation
    {
        public bool StatsShifted = false;
        public int DailyCycleCount = 1200;
        public Immunocompromised()
        {
            this.DisplayName = "Immunocompromised";
        }

        public override bool CanLevel()
        {
            return false;
        }

        public override string GetDescription()
        {
            return "Your cellular structure is particularly vulnerable to pathogenic and fungal predators.";
        }

        private List<string> DiseaseEffects = new List<string>()
        {
            "Ironshank",
            "Glotrot",
            "FungalSporeInfection",
            "PaxInfection",
            "PuffInfection",
            "LuminousInfection",
        };

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "ModifyDefendingSave");
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "ModifyDefendingSave" && E.GetStringParameter("Vs", (string)null).Contains("Disease"))
            {
                int roll = E.GetIntParameter("Roll", 0) - (6);
                E.SetParameter("Roll", roll);
            }
            return base.FireEvent(E);
        }

        public override bool WantTurnTick()
        {
            if (!ParentObject.HasEffect(x => DiseaseEffects.Contains(x.ClassName)))
            {
                return true;
            }
            else if (ParentObject.HasEffect(x => DiseaseEffects.Contains(x.ClassName) && ParentObject.GetStatValue("Hitpoints") <= 1))
            {
                StatsShifted = true;
                --DailyCycleCount;
                if (DailyCycleCount <= 0)
                {
                    StatShifter.SetStatShift(ParentObject, "Hitpoints", -1, true);
                    DailyCycleCount += 2400;
                    return true;
                }
            }
            else if (!ParentObject.HasEffect(x => DiseaseEffects.Contains(x.ClassName)))
            {
                StatShifter.RemoveStatShifts();
                DailyCycleCount = 2400;
                return true;
            }
            else if (ParentObject.HasEffect(x => DiseaseEffects.Contains(x.ClassName)) && ParentObject.GetStatValue("Hitpoints") <= 1)
            {
                ParentObject.Die(null, null, "Your form withers away to your pathogenic guest.");
            }
            return base.WantTurnTick();
        }


        public override string GetLevelText(int Level)
        {
            return "You are {{cyan|30%}} more likely to catch diseases and it is {{cyan|25%}} harder for you to fight off diseases."
            + "\n"
            + "\nIf ever diseased, your maximum hitpoints are reduced by {{cyan|1}} per every {{bcyanlue|2400}} turns until the disease is cured. If your maximum hitpoints reach {{red|1}}, you die.";
        }
    }
}