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
    class MMAWishHandler
    {
        private static void AddMutation(BaseMutation Mutation)
        {
            XRLCore.Core.Game.Player.Body.GetPart<XRL.World.Parts.Mutations>().AddMutation(Mutation, 1);
        }

        [WishCommand(Command = "AllWMMutations")]

        public static void WMAllMutations()
        {

            AddPlayerMessage("All mutations from WM Mutations has been added, enjoy ...");

            AddMutation(new CombustionBlast());
            AddMutation(new Electrokinesis());
            AddMutation(new Hydropartum());
            AddMutation(new Psybrachiomancy());
            AddMutation(new Psychomateriartis());
            AddMutation(new Sciophagia());
            AddMutation(new Thermokinesis());
            AddMutation(new Umbrameum());
            AddMutation(new VitaIntuita());
            AddMutation(new AcidicBlood());
            AddMutation(new ChitinousSkin());
            AddMutation(new Fins());
            AddMutation(new GelatinousFormAcid());
            AddMutation(new GelatinousFormPoison());
            AddMutation(new GelatinousFormSlime());
            AddMutation(new Immunocompromised());
            AddMutation(new Ovipositor());
            AddMutation(new RoughScales());
            AddMutation(new SerpentineForm());
            AddMutation(new ThickTail());
            AddMutation(new wmGills());
            AddMutation(new Animancy());
            AddMutation(new IncorporealForm());
            AddMutation(new NineLivesParadox());
            AddMutation(new PsychoplethoricDeterioration());

        }
    }
}