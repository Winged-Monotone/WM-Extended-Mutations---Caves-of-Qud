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
using XRL.World;
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
    class WMEMWishHandler
    {
        private static void AddSkill(BaseSkill Skill)
        {
            XRLCore.Core.Game.Player.Body.GetPart<XRL.World.Parts.Skills>().AddSkill(Skill);
        }

        private static void AddEffect(Effect Effect)
        {
            XRLCore.Core.Game.Player.Body.ApplyEffect(Effect);
        }

        [WishCommand(Command = "InspiredCooking")]

        public static void InspireMe()
        {

            AddPlayerMessage("Cook Something cool.");

            AddEffect(new Inspired());



        }
    }
}