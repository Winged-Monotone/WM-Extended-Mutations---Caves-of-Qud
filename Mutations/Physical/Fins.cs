using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.UI;
using XRL.World.Effects;
using System.Linq;

namespace XRL.World.Parts.Mutation
{
    [Serializable]


    public class Fins : BaseMutation
    {
        public Fins()
        {
            this.DisplayName = "Fins";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override string GetDescription()
        {
            return "Your body is lined with elegant blades of cartiligenic ridges.\n\n"

            + "{{cyan|+100}} Reputation with Fish."

            + "\n\nYou have fins that aid in swimming.";
        }

        public override string GetLevelText(int Level)
        {
            return "Swimming penalty reduced by: {{cyan|" + (Level * 10) + "%}} ";
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == GetShortDescriptionEvent.ID
            || ID == GetSwimmingPerformanceEvent.ID;
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Postfix.Append("Glistening blades of cartilage line " + ParentObject.its + " extremities and back.");
            return true;
        }

        public override bool HandleEvent(GetSwimmingPerformanceEvent E)
        {
            E.MoveSpeedPenalty -= (Level * 10);
            return true;
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            return base.Mutate(GO, Level);
        }
    }
}