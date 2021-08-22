using System.Collections.Generic;

using ConsoleLib.Console;
using System.IO;

using SimpleJSON;
using System.Linq;

namespace XRL.World
{
    [HasModSensitiveStaticCache]
    public static class wmHistorySpiceUpdate
    {

        [ModSensitiveCacheInit]
        public static void UpdateJsonHistorySpice()
        {
            ModManager.ForEachFile("wmRecipes.json", (file, mod) =>
            {
                JSONNode ChitinData = JSON.Parse(File.ReadAllText(file)) as JSONClass;

                WMExtendedMutations.History.AddToHistorySpice("spice.cooking.recipeNames.ingredients", ChitinData["Chitin Chips"]);

                // var ingredients = HistoryKit.HistoricSpice.root["cooking"]["recipeNames"]["ingredients"];

                // MetricsManager.LogInfo($"INGREDIENTS: {ingredients}");
            });

            string CombustionBlastinfoSource = "{ \"CombustionBlast\": [\"*cult*, the Fire-Eyed\", \"mind-blast *cult*\"] }";
            SimpleJSON.JSONNode CombustionInfo = SimpleJSON.JSON.Parse(CombustionBlastinfoSource);

            string SciophagiainfoSource = "{ \"Sciophagia\": [\"*cult*, the Soul-Eaters\", \"Thought-Hunters *cult*\"] }";
            SimpleJSON.JSONNode SciophagiaInfo = SimpleJSON.JSON.Parse(SciophagiainfoSource);

            string PsybrachiomancyinfoSource = "{ \"Psybrachiomancy\": [\"*cult*, the Asuran\", \"Many-Armed *cult*\"] }";
            SimpleJSON.JSONNode PsybrachiomancyInfo = SimpleJSON.JSON.Parse(PsybrachiomancyinfoSource);

            string HydropartuminfoSource = "{ \"Hydropartum\": [\"*cult*, torrent-singers\", \"sea-makers *cult*\"] }";
            SimpleJSON.JSONNode HydropartumInfo = SimpleJSON.JSON.Parse(HydropartuminfoSource);

            string PsychomateriartisinfoSource = "{ \"Psychomateriartis\": [\"*cult*, mind-smiths\", \"Forgemasters *cult*\"] }";
            SimpleJSON.JSONNode PsychomateriartisInfo = SimpleJSON.JSON.Parse(PsychomateriartisinfoSource);

            string ThermokinesisinfoSource = "{ \"Thermokinesis\": [\"*cult*, Sun-Moon Myriad\", \"Thermocline *cult*\"] }";
            SimpleJSON.JSONNode ThermokinesisInfo = SimpleJSON.JSON.Parse(ThermokinesisinfoSource);

            string VitaIntuitainfoSource = "{ \"VitaIntuita\": [\"*cult*, God-Eye\", \"Sultan's Sight *cult*\"] }";
            SimpleJSON.JSONNode VitaIntuitaInfo = SimpleJSON.JSON.Parse(VitaIntuitainfoSource);

            string UmbrameuminfoSource = "{ \"Umbrameum\": [\"*cult*, Dark-Stalkers\", \"Shadow-Makers *cult*\"] }";
            SimpleJSON.JSONNode UmbrameumInfo = SimpleJSON.JSON.Parse(UmbrameuminfoSource);

            string FocusPsiinfoSource = "{ \"Umbrameum\": [\"*cult*, Dark-Stalkers\", \"Shadow-Makers *cult*\"] }";
            SimpleJSON.JSONNode FocusPsiInfo = SimpleJSON.JSON.Parse(FocusPsiinfoSource);

            WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", CombustionInfo["CombustionBlast"]);
            WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", SciophagiaInfo["Sciophagia"]);
            WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", PsybrachiomancyInfo["Psybrachiomancy"]);
            WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", HydropartumInfo["Hydropartum"]);
            WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", PsychomateriartisInfo["Psychomateriartis"]);
            WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", ThermokinesisInfo["Thermokinesis"]);
            WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", VitaIntuitaInfo["VitaIntuita"]);
            WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", UmbrameumInfo["Umbrameum"]);
            WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", FocusPsiInfo["FocusPsi"]);

        }
    }
}