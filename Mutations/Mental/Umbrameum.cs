using System;
using System.Collections.Generic;
using XRL.Rules;
using XRL.UI;
using XRL.World.Effects;
using System.Linq;
using ConsoleLib.Console;
using System.Threading;
using XRL.Core;
using XRL.World.Encounters;
using XRL.World.Capabilities;
using XRL.World;
using Qud.API;


namespace XRL.World.Parts.Mutation
{

    [Serializable]
    public class Umbrameum : BaseMutation
    {
        //Properties/Member Variables / Not Static, Exist on the Instance of this Class
        public Guid ActivatedAbilitiesID;
        public Guid ShadeSelfActivatedAbilityID;
        public Guid ShadowAreaEffectActivatedAbilityID;

        public Umbrameum()
        {
            this.DisplayName = "Umbrameum";
            this.Type = "Mental";
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CastShadowCommand");
            Object.RegisterPartEvent(this, "ShadeSelfCommand");

            base.Register(Object);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            this.ShadowAreaEffectActivatedAbilityID = base.AddMyActivatedAbility("invoke darkness", "CastShadowCommand", "Mental Mutation", null, "\u03A9");
            this.ShadeSelfActivatedAbilityID = base.AddMyActivatedAbility("shade self", "ShadeSelfCommand", "Mental Mutation", null, "\u03A9");

            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            StatShifter.RemoveStatShifts();
            base.RemoveMyActivatedAbility(ref this.ActivatedAbilitiesID);
            return base.Unmutate(GO);
        }

        public override bool CanLevel()
        {
            return true;
        }

        public override string GetDescription()
        {
            return "An aura of darkness is about you, stealing light from the world. You hide your light--along with your glimmer.\n"
            + "\n{{cyan|-200 Reputation with}} {{cyan|Seekers of the Sightless Way.}}";
        }

        public override string GetLevelText(int Level)
        {
            if (Level == base.Level)
                return "{{white|Cast a deep shadow on an area or emit a constant fume of darkness around yourself, hampering the visibility of your enemies and yourself. You are innately more difficult to sense through the glimmer, abilities like clairvoyance, and sense psychic are less effective on you.\n"
                + "\n"
                + "Glimmer Reduced by {{cyan|50%}}\n"
                + "Radius: {{cyan|" + (8 + Level) + "}}\n"
                + "Self-cast Duration {{cyan|}}: " + ((Level * (10) / 2)) + "}}";
            else
                return "Radius: {{cyan|" + (8 + Level) + "}}\n"
                + "Self-cast Duration {{cyan|}}: " + ((Level * (10) / 2)) + "}}";

        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
             || ID == AwardingXPEvent.ID;
        }

        public override bool HandleEvent(AwardingXPEvent E)
        {
            try
            {
                int CurrentGlimmer = ParentObject.GetPsychicGlimmer();


                ParentObject.SetIntProperty("GlimmerModifier", CurrentGlimmer / 2);
                ParentObject.SetIntProperty("Glimmer", CurrentGlimmer / 2);
            }
            catch
            {

            }
            return base.HandleEvent(E);
        }

        public static string[] ColorList = new string[3]
{
        "&K",
        "&m",
        "&k",
};

        public string GetDarknesBlips()
        {
            return ColorList.GetRandomElement();
        }

        public void CastDarkness()
        {
            try
            {
                int ParentsEgoModifer = ParentObject.Statistics["Ego"].Modifier;

                int MaximumRadius = Math.Min(1 + (Level / 2), 4);
                List<Cell> Cells = PickBurst(MaximumRadius, 8 + Level, false, AllowVis.OnlyExplored);

                int Ran1 = Stat.Random(1, 100);

                if (Cells != null)
                    if (!this.ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap())
                    {
                        if (Stat.Random(1, 100) < Ran1)
                            base.PlayWorldSound("darknesscast", 15f, 0, true, null);
                        else
                            base.PlayWorldSound("darknesscastdeep", 15f, 0, true, null);

                        for (int index = 0; index < Cells.Count; index++)
                        {
                            Cell C = Cells[index];

                            if (C.ParentZone.IsActive() && C.IsVisible())
                            {
                                TextConsole.LoadScrapBuffers();
                                ScreenBuffer scrapBuffer = TextConsole.ScrapBuffer;
                                XRLCore.Core.RenderMapToBuffer(TextConsole.ScrapBuffer);
                                scrapBuffer.Goto(C.X, C.Y);
                                scrapBuffer.Write(GetDarknesBlips() + "\a");
                                Popup._TextConsole.DrawBuffer(scrapBuffer);
                                Thread.Sleep(10);
                                XRLCore.Core.RenderMapToBuffer(scrapBuffer);
                                scrapBuffer.Goto(C.X, C.Y);
                                scrapBuffer.Write(GetDarknesBlips() + "\u0489");
                                Popup._TextConsole.DrawBuffer(scrapBuffer);
                                Thread.Sleep(5);

                                var Roll = Stat.Random(1, 100);
                                if (Roll <= 10)
                                {
                                    base.PlayWorldSound("darkness", 15f, 0, true, null);
                                }

                                if (C.HasObjectWithPart("Combat") || C.HasObjectWithPart("Brain"))
                                {
                                    var cObj = C.GetFirstObjectWithPart("Combat");
                                    if (cObj != ParentObject)
                                    {
                                        if (cObj.MakeSave(Stat: "Ego", Difficulty: 5 + Level, Attacker: ParentObject, AttackerStat: "Ego"))
                                            cObj.ApplyEffect(new Confused(ParentsEgoModifer * 10, Level, (ParentsEgoModifer + Level) / 2));
                                    }
                                }

                                GameObject gameObject = C.AddObject("MagicalDarkness");

                                MagicalDarkness Darkness = gameObject.GetPart<MagicalDarkness>();

                                // AddPlayerMessage("Status Check: Setting up Objects");

                                int Ran2 = Stat.Random(0, 3);
                                Darkness.Duration = ((Level * (10 + ParentsEgoModifer) / 2) + Ran2);
                            }
                        }
                    }
                CooldownMyActivatedAbility(ShadowAreaEffectActivatedAbilityID, 60);

            }
            catch
            {

            }
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CastShadowCommand")
            {
                CastDarkness();
            }
            else if (E.ID == "ShadeSelfCommand")
            {
                CooldownMyActivatedAbility(ShadeSelfActivatedAbilityID, 30);
                if (ParentObject.HasEffect("Darkening"))
                {
                    AddPlayerMessage("{{gray|The shroud of darkness about you disperses.}}");
                    ParentObject.RemoveEffect(new Darkening());
                    return base.FireEvent(E);
                }
                else
                {
                    AddPlayerMessage("{{dark gray|You shroud yourself in darkness.}}");
                    ParentObject.ApplyEffect(new Darkening());
                }
            }

            return base.FireEvent(E);

        }
    }
}