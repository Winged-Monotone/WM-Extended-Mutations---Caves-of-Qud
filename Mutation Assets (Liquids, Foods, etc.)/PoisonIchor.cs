using Qud.API;
using System;
using System.Collections.Generic;
using System.Text;
using XRL;
using XRL.Core;
using XRL.Liquids;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Effects;

[Serializable]
[IsLiquid]
public class PoisonIchor : BaseLiquid
{
    public PoisonIchor() : base("poisonichor")
    {
        FlameTemperature = 0;
        VaporTemperature = 0;
        Temperature = 0;
        Fluidity = 50;
        Cleansing = 1;
        Weight = 0.1;
        InterruptAutowalk = false;
        ConsiderDangerousToContact = false;
        ConsiderDangerousToDrink = true;
    }

    [NonSerialized]
    public static List<string> Colors = new List<string>(3)
    {
        "M",
        "m",
        "g"
    };
    public override List<string> GetColors()
    {
        return Colors;
    }
    public override string GetColor()
    {
        return "M";
    }
    public override string GetName(LiquidVolume Liquid)
    {
        return "{{poisonous|poison ichor}}";
    }
    public override string GetAdjective(LiquidVolume Liquid)
    {
        return "{{poisonous|poisonous}}";
    }
    public override string GetWaterRitualName()
    {
        return "poisonous ichor";
    }
    public override string GetSmearedAdjective(LiquidVolume Liquid)
    {
        return "{{poisonous|poisonous}}";
    }
    public override string GetSmearedName(LiquidVolume Liquid)
    {
        return "{{poisonous|poisoned}}";
    }
    public override string GetStainedName(LiquidVolume Liquid)
    {
        return "{{poisonous|ichor}}-stained";
    }
    public override float GetValuePerDram()
    {
        return 10f;
    }
    public override bool Vaporized(LiquidVolume Liquid)
    {
        return false;
    }
    public override bool Drank(LiquidVolume Liquid, int Volume, GameObject Target, StringBuilder Message, ref bool ExitInterface)
    {
        Message.Compound("{{poisonous| It tastes like bitter omens ...}}");
        Damage value = new Damage((Liquid.Proportion("PoisonIchor") / 100 + 1 + "d100").Roll());
        Event @event = Event.New("TakeDamage");
        @event.SetParameter("Attribute", "Poison");
        @event.SetParameter("Damage", value);
        @event.SetParameter("Owner", Liquid.ParentObject);
        @event.SetParameter("Attacker", Liquid.ParentObject);
        @event.SetParameter("Message", "from {{poisonous|drinking poison ichor}}!");
        @event.SetParameter("Phase", Target.GetPhase());
        Target.FireEvent(@event);
        ExitInterface = true;
        return true;
    }
    public override void RenderBackground(LiquidVolume Liquid, RenderEvent eRender)
    {
        eRender.ColorString = "^M" + eRender.ColorString;
    }
    public override void BaseRenderPrimary(LiquidVolume Liquid)
    {
        if (Liquid.IsWadingDepth())
        {
            Liquid.ParentObject.pRender.RenderString = "~";
        }
        Liquid.ParentObject.pRender.ColorString = "&M^g";
    }
    public override void RenderPrimary(LiquidVolume Liquid, RenderEvent eRender)
    {
        if (!Liquid.IsWadingDepth())
        {
            return;
        }
        if (Liquid.ParentObject.IsFrozen())
        {
            eRender.RenderString = "~";
            eRender.ColorString = "&M^g";
            return;
        }
        Render pRender = Liquid.ParentObject.pRender;
        int num = (XRLCore.CurrentFrame + Liquid.nFrameOffset) % 60;
        if (Stat.RandomCosmetic(1, 600) == 1)
        {
            eRender.RenderString = "\u000f";
            eRender.ColorString = "&M^g";
        }
        if (Stat.RandomCosmetic(1, 60) == 1)
        {
            if (num < 15)
            {
                pRender.RenderString = "÷";
                pRender.ColorString = "&M^g";
            }
            else if (num < 30)
            {
                pRender.RenderString = "~";
                pRender.ColorString = "&M^g";
            }
            else if (num < 45)
            {
                pRender.RenderString = "\t";
                pRender.ColorString = "&M^g";
            }
            else
            {
                pRender.RenderString = "~";
                pRender.ColorString = "&M^g";
            }
        }
    }
    public override void ObjectGoingProne(LiquidVolume Liquid, GameObject GO)
    {
        if (Liquid.IsWadingDepth())
        {
            if (GO.IsPlayer())
            {
                BaseLiquid.AddPlayerMessage("{{R|Poisonous ichor splashes into your mouth. You feel your nervious system shutters.}}");
            }
            GO.Splatter("&w.");
            if (!GO.MakeSave("Toughness", 30, null, null, "Goo Poison"))
            {
                GO.ApplyEffect(new Poisoned(Stat.Roll("1d4+4"), Stat.Roll("1d2+2") + "d2", 10));
            }
        }
    }
}