using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.UI;

namespace XRL.World.Parts.Mutation
{
    [Serializable]


    public class Animancy : BaseMutation
    {
        public const string MOD_PREFIX = "Animancy";

        public Animancy()
        {
            this.DisplayName = "Ancestral Scionasia ({{purple|H}})";
        }
        public override string GetDescription()
        {
            return "Whether it be exponential instinct or an innate lucidity of the mind, you sometimes accrue knowledge from foreign places in your psyche and gain a bonus to experience.";
        }
        public override string GetLevelText(int Level)
        {
            return "Effect: " + (10 + Level - 1) + "% chance to gain bonus experience points, this mutation grants more experience at 10th Level.";
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == AwardingXPEvent.ID;
        }

        private List<Action<Animancy, Event>> VividChoices = new List<Action<Animancy, Event>>()
        {
            (AN, E) =>
            {
                Popup.Show("There’s a rejoicing in the aether, a quickening through encodings far too vast to comprehend. You make out hymns of experiences radiating heartfelt applauds as your journey’s toils bring you strength and prowess. And with it, an ancestral gift from the void of the unconscious.\n" + "[{{blue|The anima favors you ...}}" + "[Bonus Experience Awarded : " + E.GetIntParameter("Amount") + "]");
            },
            (AN, E) =>
            {
                Popup.Show("The sight of your foe drawing their last breath excites the primitive shadows lurking in your mind. Your body adapts, and your instinct hones its killing edge.\n" + "[{{blue|The anima favors you ...}}" + "[Bonus Experience Awarded : " + E.GetIntParameter("Amount") + "]");
            },
        };

        public override bool HandleEvent(AwardingXPEvent E)
        {
            var SeededRandom = ParentObject.GetSeededRandom("Animancy");
            int SavantChance = SeededRandom.Next(1, 100);
            int AnimaMultiplier = Stat.Random(2, 7);
            int currentXPAward = E.Amount;

            int FlavourTextSum = currentXPAward * AnimaMultiplier;

            if (SavantChance <= 10 + this.Level - 1)
            {
                E.Amount = currentXPAward * AnimaMultiplier;
                AddPlayerMessage("[{{blue|You receive a gift from the anima.}}" + "[Bonus Experience Awarded : " + E.Amount + "]");
                if (this.Level >= 10 && Stat.Random(1, 100) <= 10)
                {
                    int BaseBonusExpAward = currentXPAward * SeededRandom.Next(2, 7);
                    E.Amount = BaseBonusExpAward;
                    VividChoices.GetRandomElement();
                }
            }

            return base.HandleEvent(E);
        }
    }
}