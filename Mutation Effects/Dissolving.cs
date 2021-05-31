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
    public class Dissolving : Effect
    {
        public int Damage = 2;
        public GameObject Owner;

        public Dissolving() : base()
        {
            base.DisplayName = "{{red|Dissolving}}";
        }
        public Dissolving(int Duration, GameObject Owner)
        {
            this.Owner = Owner;
            base.Duration = Duration;
            base.DisplayName = "{{red|Dissolving}}";
        }
        public override string GetDetails()
        {
            return "Your acidic gelatin is reacting to concentrated salt! You are dissolving!\n";
        }


    }
}