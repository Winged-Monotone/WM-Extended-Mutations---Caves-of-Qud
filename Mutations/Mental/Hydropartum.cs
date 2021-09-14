using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.UI;
using XRL.World.Effects;
using System.Linq;
using ConsoleLib.Console;
using System.Threading;
using XRL;
using XRL.Core;
using XRL.World;
using XRL.World.Parts.Mutation;


namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Hydropartum : BaseMutation
    {

        public Guid FloodActivatedAbilityID;

        public Hydropartum()
        {
            this.DisplayName = "Hydropartuma";
            this.Type = "Mental";
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            this.FloodActivatedAbilityID = base.AddMyActivatedAbility(Name: "Torrent", Command: "TorrentBlastCommand", Class: "Mental Mutation", Icon: "*", Cooldown: 50);
            return base.Mutate(GO, Level);
        }

        public override string GetDescription()
        {
            return "Through sheer will, you shake the moistened air and conjure a deluge of streaming water.";
        }

        public override string GetLevelText(int Level)
        {
            if (((byte)Level) == base.Level)
                return "Create a cone of surging water in front of you, enemies in the torrent are pushed back and take damage, higher levels summon larger piles of water at a larger distance.\n\n"

                + "Torrent Range {{cyan|+" + Level + "}}"
                + "\n Damage Multiplier {{cyan|x" + 2 * Level + ".0}}";
            else
                return "Increased Torrent Range {{cyan|+" + Level + "}}"
                + "\n Increased Damage Multiplier {{cyan|x" + 2 * Level + ".0}}";
        }

        public static string[] ColorList = new string[5]
    {
        "&C",
        "&b",
        "&B",
        "&c",
        "&Y"
    };

        public string GetRandomBlueFlashes()
        {
            return ColorList.GetRandomElement();
        }

        public void Hydrofabricate()
        {
            List<Cell> CellList = PickCone(4 + Level, 30 + Level, AllowVis.OnlyVisible);
            CellList = CellList.OrderBy(cell => cell.ManhattanDistanceTo(ParentObject.CurrentCell)).ToList();

            if (!this.ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap())
            {
                base.PlayWorldSound("splashcast", 1.5f, 0, true, null);
                XDidY(ParentObject, "materialize", "a torrential flood", "!", "C", null);
                for (int index = 0; index < CellList.Count; index++)
                {
                    Cell C = CellList[index];

                    if (C.ParentZone.IsActive() && C.IsVisible())
                    {

                        TextConsole.LoadScrapBuffers();
                        ScreenBuffer scrapBuffer = TextConsole.ScrapBuffer;
                        XRLCore.Core.RenderMapToBuffer(TextConsole.ScrapBuffer);
                        scrapBuffer.Goto(C.X, C.Y);
                        scrapBuffer.Write(GetRandomBlueFlashes() + "\a");
                        Popup._TextConsole.DrawBuffer(scrapBuffer);
                        Thread.Sleep(10);
                        XRLCore.Core.RenderMapToBuffer(scrapBuffer);
                        scrapBuffer.Goto(C.X, C.Y);
                        scrapBuffer.Write(GetRandomBlueFlashes() + "\u0489");
                        Popup._TextConsole.DrawBuffer(scrapBuffer);
                        Thread.Sleep(10);

                        var Roll = Stat.Random(1, 100);

                        if (Roll <= 50)
                        {
                            if (Roll >= 90)
                                base.PlayWorldSound("flood1", 1.5f, 0, true, null);
                            else if (Roll >= 60)
                                base.PlayWorldSound("flood2", 1.5f, 0, true, null);
                            else if (Roll >= 30)
                                base.PlayWorldSound("flood3", 1.5f, 0, true, null);
                            else
                            {

                            }
                        }

                        if (C.HasObjectWithPart("Combat") || C.HasObjectWithPart("Brain"))
                        {
                            var cObj = C.GetFirstObjectWithPart("Combat");
                            if (cObj != ParentObject)
                            {
                                cObj.Push(ParentObject.CurrentCell.GetDirectionFromCell(cObj.CurrentCell), 1000, 4);
                                cObj.TakeDamage(2 * Level, "Drowned in a torrent of liquid.", null, DeathReason: "from %t torrent-blast.", ThirdPersonDeathReason: null, ParentObject, ParentObject, ParentObject);
                                cObj.GetAngryAt(ParentObject, -50);
                            }
                        }

                        C.Splash("{{B|~}}");
                        C.AddObject("SaltyWaterPuddle");
                    }
                }
            }
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "TorrentBlastCommand");

            base.Register(Object);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "TorrentBlastCommand")
            {
                if (base.IsMyActivatedAbilityUsable(this.FloodActivatedAbilityID))
                {
                    Hydrofabricate();
                    CooldownMyActivatedAbility(FloodActivatedAbilityID, 50, ParentObject);

                    return false;
                }
            }

            return base.FireEvent(E);
        }
    }
}