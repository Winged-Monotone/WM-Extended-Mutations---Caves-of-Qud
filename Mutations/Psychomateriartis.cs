using Qud.API;
using System;
using System.Collections.Generic;
using System.Globalization;
using XRL.Core;
using XRL.Language;
using XRL.UI;



namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Psychomateriartis : BaseMutation
    {
        public Psychomateriartis()
        {
            this.DisplayName = "Psychomateriartis";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override string GetDescription()
        {
            return "Your incredible psionic power comes at the cost of overwhelming the stability of your physical form, you are doomed to hunting down physical husk to maintain your tether to this reality, albeit the magnitude of your psionic abilities are of a realm of its own.\n"
                + "\n"
                + "{{orange|-400 reputation with highly entropic beings.\n}}"
                + "{{light blue|+400 reputation the Seekers of the Sightless Way.\n\n}}";
        }
    }
}


