//1p Recharge cells
//8p Charge Sense: Sense objects around you that emit an Eletric Signal.
//12p Metal Returns set damage back to enemies who harm you. If wearing metal Armor, the damage effects natural objects as well.
//14p Blocking with a shield deals electric damage to your enemies and a chance to stun/daze them.
//18p Laser damage deals reduced damage to you.
//28p robotic enemies that strike you must make a save or be stunned/take electrac damage amplified by potency/
//28p Laser damage has no effect on you.
//38p Laser Damage is transformed into potency
//42p if an attack with elec damage fails to penetrate you, you transformt he damage into potency
//42p Shimmering Ray attack at cost of potency
//60p EMP Burst on Activating and EMP burst at a whim at the cost of potency.
//60p static burst, random burst of electricity that releases potency into enemies damaging them
//60p Lightning Strike!
//99p+ Self desctruct, warn players this will kill them, they explode with the force of a sun

using System;
using XRL.World.Effects;

using XRL.World.Parts.Mutation;

using XRL.UI;

namespace XRL.World.Parts
{
    [Serializable]
    public class ShimmeringShroudHandler : IPart
    {
        public int Duration = 0;
        public bool EndedOnMessage = false;
        public GameObject Owner = null;

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
            || ID == AttackerDealingDamageEvent.ID
            || ID == GetShieldBlockPreferenceEvent.ID;
        }

        public override bool HandleEvent(GetShieldBlockPreferenceEvent E)
        {
            var Shield = E.Shield.GetPart<Shield>();

            var ElectroHook = ParentObject.GetPart<Electrokinesis>();

            if (ElectroHook.Potency >= 14 && ParentObject.HasEffect("ShimmeringShroud"))
                if (E.Defender == ParentObject)
                {
                    if (E.Shield.HasPart("Metal"))
                    {
                        var Attacker = E.Attacker;
                        var Defender = E.Defender;
                        var PotencyMod = 1 + ElectroHook.Potency;

                        Attacker.TakeDamage((PotencyMod), Attacker.Is + "takes damage from " + Defender.its + " shimmering shroud!", "Electricity", DeathReason: null, ThirdPersonDeathReason: null, Defender);
                        Attacker.ApplyEffect(new Stun(PotencyMod, PotencyMod));
                    }
                }

            return base.HandleEvent(E);
        }

        public override bool HandleEvent(AttackerDealingDamageEvent E)
        {
            try
            {
                var Attacker = E.Object;
                var Defender = E.Actor;
                var ElectroHook = ParentObject.GetPart<Electrokinesis>();

                var WeaponMeleeHook = E.Weapon.GetPart<MeleeWeapon>();
                var ProjectileHook = E.Projectile.GetPart<Projectile>();
                var DefenderArmor = Defender.Equipped.GetEquippedObjects();

                var PotencyMod = 1 + ElectroHook.Potency;

                if (ElectroHook.Potency >= 12 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    if (Defender == ParentObject && E.Weapon.HasPart("Metal") && !E.Weapon.HasPart("NaturalEquipment"))
                    {
                        Attacker.TakeDamage((PotencyMod), Attacker.Is + "takes damage from " + Defender.its + " shimmering shroud!", "Electricity", DeathReason: null, ThirdPersonDeathReason: null, Defender);
                    }
                    else if (Defender == ParentObject && E.Weapon.HasPart("Metal") || E.Weapon.HasPart("NaturalEquipment"))
                    {
                        foreach (var wornObj in DefenderArmor)
                        {
                            if (wornObj.IsEquippedOnLimbType("Body") && wornObj.HasPart("Metal"))
                            {
                                Attacker.TakeDamage((PotencyMod), Attacker.Is + "takes damage from " + Defender.its + " shimmering shroud!", "Electricity", DeathReason: null, ThirdPersonDeathReason: null, Defender);
                            }
                            else
                            {

                            }
                        }
                    }
                }

                if (ElectroHook.Potency >= 28 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    if (Defender == ParentObject
                   && Attacker.HasPart("Combat")
                   || Attacker.HasPart("Robot")
                   || Attacker.HasTag("Robot"))
                    {
                        if (!Attacker.MakeSave("Toughness", 8 + ElectroHook.Potency, Defender, null, null))
                        {

                            Attacker.TakeDamage((PotencyMod), Attacker.Is + "takes damage from " + Defender.its + " shimmering shroud!", "Electricity", DeathReason: null, ThirdPersonDeathReason: null, Defender);
                            Attacker.ApplyEffect(new Stun(PotencyMod, PotencyMod));
                        }
                    }
                }
                else if (ElectroHook.Potency >= 38 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    if (Defender == ParentObject && E.Damage.IsElectricDamage())
                    {
                        var DamageConversion = E.Damage.Amount;
                        ElectroHook.Potency += DamageConversion / 2;
                        E.Damage.Amount /= 2;
                    }
                }
                else if (ElectroHook.Potency >= 42 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    if (Defender == ParentObject
                    && E.Weapon.HasPart("DischargeOnHit")
                    || WeaponMeleeHook.Attributes == "Electric"
                    || ProjectileHook.Attributes == "Electric")
                    {
                        ElectroHook.Potency += E.Damage.Amount / 3;
                    }
                }

                if (Defender == ParentObject
            && E.Damage.HasAttribute("Light Laser")
            || E.Projectile.HasTagOrStringProperty("EnergyAmmoLoader", "ProjectileLaserRifle")
            || E.Projectile.HasTagOrStringProperty("EnergyAmmoLoader", "ProjectileEigenrifle")
            || E.Projectile.HasTagOrStringProperty("EnergyAmmoLoader", "ProjectileLaserPistol")
            || E.Projectile.HasTagOrStringProperty("EnergyAmmoLoader", "ProjectileOverloadedLaserPistol")
            || E.Projectile.HasTagOrStringProperty("EnergyAmmoLoader", "ProjectileElectrobow"))
                {
                    if (ElectroHook.Potency >= 18 && ParentObject.HasEffect("ShimmeringShroud"))
                    {

                        E.Damage.Amount /= 2;

                    }
                    else if (ElectroHook.Potency >= 28 && ParentObject.HasEffect("ShimmeringShroud"))
                    {

                        E.Damage.Amount = 0;

                    }
                    else if (ElectroHook.Potency >= 38 && ParentObject.HasEffect("ShimmeringShroud"))
                    {
                        var DamageConversion = E.Damage.Amount;
                        ElectroHook.Potency += DamageConversion / 2;
                        E.Damage.Amount = 0;
                    }
                    else if (ElectroHook.Potency >= 42 && ParentObject.HasEffect("ShimmeringShroud"))
                    {
                        if (E.Damage.IsElectricDamage())
                        {
                            var DamageConversion = E.Damage.Amount;
                            ElectroHook.Potency += DamageConversion;
                            E.Damage.Amount = 0;
                        }
                    }
                }
            }
            catch { }

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

        public override void Register(GameObject go)
        {
            go.RegisterPartEvent((IPart)this, "SetShimmeringShroudEffectEvent");
            go.RegisterPartEvent((IPart)this, "AlterShimmeringEffectChangeEvent");

            go.RegisterPartEvent((IPart)this, "ChargeBatteryEvent");
            go.RegisterPartEvent((IPart)this, "BeginTakeAction");

            base.Register(go);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {

                //______________________________________________________________________________
                var ElectroHook = ParentObject.GetPart<Electrokinesis>();

                Owner = ElectroHook.Owner;

                if (ElectroHook.Potency == 1 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {

                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        Popup.Show("You can now recharge batteries by expending Potency.\n\n\n [To deactivate these messages, toggle the Electrokinesis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                        // AddPlayerMessage("Step 1a");
                    }
                }
                else if (ElectroHook.Potency > 1 && ElectroHook.Potency < 8 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                    // AddPlayerMessage("Step 1c");
                }
                else if (ElectroHook.Potency == 8 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    //______________________________________________________________________________

                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        Popup.Show("You can now sense electromagnetic fields around you such as robots and machinery.\n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                    }
                    // AddPlayerMessage("Step 2a");
                }
                else if (ElectroHook.Potency > 8 && ElectroHook.Potency < 12 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                    // AddPlayerMessage("Step 2c");
                }
                else if (ElectroHook.Potency == 12 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    //______________________________________________________________________________

                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        Popup.Show("\n\nCreatures that strike you while wielding any form of conductive weapon, now take damage upon successeful penetrations. If you are wearing armor, this extends to creatures' natural equipment as well.\n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                    }
                    // AddPlayerMessage("Step 3a");
                }
                else if (ElectroHook.Potency > 12 && ElectroHook.Potency < 14 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                    // AddPlayerMessage("Step 3b");
                }
                else if (ElectroHook.Potency == 14 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        Popup.Show("\n\nSuccessfully blocking an attack will deal damage to an enemy as long as you are using a conductive shield. \n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                        // AddPlayerMessage("Step 4a");
                    }
                }
                else if (ElectroHook.Potency > 14 && ElectroHook.Potency < 18 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                    // AddPlayerMessage("Step 4b");
                }
                else if (ElectroHook.Potency == 18 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    //______________________________________________________________________________

                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        Popup.Show("Laser Weaponry reduced damage to you. \n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                    }
                    // AddPlayerMessage("Step 5a");
                }
                else if (ElectroHook.Potency > 18 && ElectroHook.Potency < 28 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                    // AddPlayerMessage("Step 5b");
                }
                else if (ElectroHook.Potency == 28 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    //______________________________________________________________________________

                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        //blocking things hurt them
                        Popup.Show("Robotic and mechanical enemies must make a save or be stunned upon dealing damage to you. \n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                        // AddPlayerMessage("Step 6a");
                    }
                }
                else if (ElectroHook.Potency > 28 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    // AddPlayerMessage("Step 6b");
                    EndedOnMessage = false;
                }
                else if (ElectroHook.Potency == 28 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    //______________________________________________________________________________

                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        //blocking things hurt them
                        Popup.Show("Laser Weaponry deals no damage to you. \n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                        // AddPlayerMessage("Step 7a");
                    }
                }
                else if (ElectroHook.Potency > 28 && ElectroHook.Potency < 38 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                    // AddPlayerMessage("Step 7b");
                }
                else if (ElectroHook.Potency == 38 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    //______________________________________________________________________________

                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        //blocking things hurt them
                        Popup.Show("Laser weapons now transfers damage into potency. \n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                        // AddPlayerMessage("Step 8a");
                    }
                }
                else if (ElectroHook.Potency > 38 && ElectroHook.Potency < 42 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                    // AddPlayerMessage("Step 8b");
                }
                else if (ElectroHook.Potency == 42 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    //______________________________________________________________________________

                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        Popup.Show("Electricity damage is now converted into potency. \n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                        // AddPlayerMessage("Step 9a");
                    }
                }
                else if (ElectroHook.Potency > 42 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                    // AddPlayerMessage("Step 9b");
                }
                else if (ElectroHook.Potency == 42 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    //______________________________________________________________________________



                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        Popup.Show("You can now direct a shimmering beam of electric energy at your foes in the form of Shimmering Ray at the cost of potency. You can also release a blast of electric energy that emp's nearby equipment. \n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                        // AddPlayerMessage("Step 10b");
                    }
                }
                else if (ElectroHook.Potency > 42 && ElectroHook.Potency < 60 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                    // AddPlayerMessage("Step 10e");
                }
                else if (ElectroHook.Potency == 60 && ParentObject.HasEffect("ShimmeringShroud") && EndedOnMessage == false)
                {
                    //______________________________________________________________________________

                    if (ElectroHook.ShowMutationUpdates == true)
                    {
                        Popup.Show("You can now call a great lightning strike on your enemies at the cost of Potency. \n\n\n [To deactivate these messages, toggle the Electrokenisis Prompts in the Command menu to OFF.]");
                        EndedOnMessage = true;
                    }
                }
                else if (ElectroHook.Potency > 60 && ParentObject.HasEffect("ShimmeringShroud"))
                {
                    EndedOnMessage = false;
                }

            }

            return base.FireEvent(E);
        }
    }
}