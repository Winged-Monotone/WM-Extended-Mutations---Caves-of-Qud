using XRL.Wish;
using XRL.World;
using static XRL.World.IComponent<XRL.World.IPart>;

[HasWishCommand]
public class MyWishHandler
{
    [WishCommand]
    public static bool SwapBodiesTest()
    {
        var OriginalBody = game.Player.Body;

        Cell cell = game.Player.Body.pBrain.PickDirection();
        var TargetHusk = cell.GetFirstObjectWithPart("Brain");

        game.Player.Body = TargetHusk;
        // TargetHusk.FireEvent(Event.New("Result", "OriginalBody", ParentObject));


        var CreatureTier = OriginalBody.GetTier();
        var PrimaryFaction = OriginalBody.GetPrimaryFaction();
        var FactionVar = Factions.get(PrimaryFaction);

        if (FactionVar.Visible)
        {
            XRL.Core.XRLCore.Core.Game.PlayerReputation.modify(PrimaryFaction, -CreatureTier * 50, true);
        }

        



        return true;
    }
}