// XRL.World.Capabilities.Wishing
using ConsoleLib.Console;
using Genkit;
using HistoryKit;
using Qud.API;
using Sheeter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using XRL.Annals;
using XRL.Core;
using XRL.Language;
using XRL.Messages;
using XRL.Rules;
using XRL.UI;
using XRL.Wish;
using XRL.World.AI.GoalHandlers;
using XRL.World.Effects;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Parts.Skill;
using XRL.World.QuestManagers;
using XRL.World.Skills.Cooking;
using XRL.World.Tinkering;
using XRL.World.ZoneBuilders;


using static XRL.World.IComponent<XRL.World.IPart>;

namespace WMEMWishes
{
    [HasWishCommand]
    class WMEMWishesBodyInfo
    {
        private static void ListBodyPart()
        {
            var ListBodyPart = XRLCore.Core.Game.Player.Body.Body.GetParts();

            AddPlayerMessage("Displaying All BodyParts");

            foreach (var p in ListBodyPart)
            {
                AddPlayerMessage("BodyPart: " + p.VariantType);
            }
        }

        [WishCommand(Command = "ShowBodyParts")]

        public static void ShowBodyParts()
        {
            ListBodyPart();
        }
    }
}