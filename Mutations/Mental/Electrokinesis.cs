
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;

using XRL.World.Parts;
using XRL.World;
using XRL.World.Effects;
using XRL.World.AI.GoalHandlers;
using XRL.World.Parts.Mutation;
using XRL.World.Capabilities;

using XRL.Core;
using XRL.Rules;
using XRL.Messages;
using XRL.UI;

using UnityEngine;
using AiUnity.NLog.Core.Targets;
using HarmonyLib;
using ConsoleLib.Console;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class Electrokinesis : BaseMutation
    {
        public Guid PotencyForChargesAbility;
        public Guid ToggleChargeSenseAbility;
        public Guid ShimmeringRayAbility;
        public Guid ThunderStrikeAbility;
        public Guid SelfDestructAbility;
        public int PotencySpentHandler = 0;
        public int Potency = 0;
        public int NewPotency = 0;
        public int EletricCharge = 0;
        public int NewElectricCharge = 0;
        public string Sound = "Electroblast1";
        public GameObject Listener;
        public GameObject Owner;
        public Guid ActivateShimmeringShroudAbilityID;
        public Guid ChargePotentialAbilityID;
        public Guid ElectrokinesisGauge;
        public bool ShowMutationUpdates = true;


        public static readonly List<string> MainOptions = new List<string>()
        {
            "Recharge Cell",
            "Cancel"
        };
        public static readonly List<string> GetInventoryList = new List<string>()
        {

        };
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == GetDisplayNameEvent.ID;
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            try
            {
                UpdateCharges();
            }
            catch
            {
                UpdateCharges();
            }
            return true;
        }
        public void UpdateCharges()
        {
            var ParentsCharges = ParentObject.Statistics["PsiCharges"].BaseValue;

            int MaximumPotency = ParentsCharges * 2;
            var AA = MyActivatedAbility(this.ElectrokinesisGauge);
            if (AA != null)
            {
                AA.DisplayName = "Amprage {{yellow|(" + (Potency) + "/" + MaximumPotency + ".pp)}}";
            }
        }
        public void StartElectroRechargeList()
        {
            string mainChoice = GetChoice(MainOptions);
            if (!string.IsNullOrEmpty(mainChoice))
            {
                ElectrokinesisGetBatteries();
                string inventoryChoice = GetChoice(GetInventoryList);
                GetInventoryList.Clear();
                if (!string.IsNullOrEmpty(inventoryChoice))
                {
                    AddPlayerMessage("Printing Choice: " + inventoryChoice);
                    PotencyCostQuery();
                    ElectrokinesisChargeBattery(inventoryChoice);
                    UpdateCharges();
                }
            }
        }
        public static string GetChoice(List<string> valuesToShow)
        {
            int result = Popup.ShowOptionList(
                Title: "Select an Option",
                Options: valuesToShow.ToArray(),
                AllowEscape: true);
            if (result < 0 || valuesToShow[result].ToUpper() == "CANCEL")
            {
                // The user escaped out of the menu, or chose "Cancel"
                return string.Empty;
            }
            else
            {
                // The user selected a value - return the select value as a string
                return valuesToShow[result];
            }
        }

        public void ElectrokinesisGetBatteries()
        {
            var RechargableItems = ParentObject.Inventory.GetObjects();

            foreach (var I in RechargableItems)
            {
                if (I.HasPart("EnergyCell"))
                {
                    string ObjID = I.DisplayName;
                    GetInventoryList.Add(ObjID);
                }
            }
        }

        public void PotencyCostQuery()
        {
            string PotencyQuery = Popup.AskString("Expend how much potency?", "1", 3, 1, "0123456789");
            int PotencySpent = Convert.ToInt32(PotencyQuery);

            PotencySpentHandler = PotencySpent;
            Potency -= PotencySpent;
        }

        public void ElectrokinesisChargeBattery(string inventoryChoice)
        {
            if (PotencySpentHandler == 0)
            {
                Popup.Show("You must spend amprage in order to charge a battery.");
                return;
            }
            var BatteryList = ParentObject.Inventory.GetObjects();

            AddPlayerMessage("Initiating.");
            foreach (var I in BatteryList)
            {
                if (I.HasPart("LiquidFueledEnergyCell"))
                {
                    Popup.Show("Cannot be a fuel powered energy.");
                    break;
                }
                AddPlayerMessage("Finding Selected battery ...");
                if (I.DisplayName == inventoryChoice)
                {
                    AddPlayerMessage("Battery Found!");
                    AddPlayerMessage("Adding Charge to: " + I.DisplayName);

                    if (I.HasPart("Rechargeable"))
                    {
                        AddPlayerMessage("Found Rechargeable Part.");

                        var Ircg = I.GetPart<IRechargeable>();
                        Ircg.AddCharge(1000 * PotencySpentHandler);

                        AddPlayerMessage("Charge added to: " + I.DisplayName);
                    }
                    else if (I.HasPart("EnergyCell"))
                    {
                        AddPlayerMessage("Found Energy Cell Part.");

                        var ICell = I.GetPart<EnergyCell>();
                        ICell.AddCharge(1000 * PotencySpentHandler);

                        AddPlayerMessage("Charge added to: " + I.DisplayName);
                    }

                    PotencySpentHandler = 0;

                    break;
                }
            }
        }

        public void ChargeVisionAbility()
        {
            Cell currentCell = ParentObject.CurrentCell;
            if (currentCell != null)
            {
                int radius = 7;
                foreach (GameObject item in currentCell.ParentZone.FastSquareSearch(currentCell.X, currentCell.Y, radius, "Combat"))
                {
                    if (ParentObject.DistanceTo(item) <= radius && !item.HasEffect("Electrovoyance") && (ParentObject.HasPart("Robot")
                    || ParentObject.HasPart("BaseRobot")
                    || ParentObject.HasPart("UniversalCharger")
                    || ParentObject.HasPart("ElectricalPowerTransmission")))
                    {
                        item.ApplyEffect(new Electravoyance(base.Level, ParentObject));
                    }
                }
            }
        }

        public string ComputeDamage(int UseLevel)
        {
            string text = UseLevel + "d4";
            text += "+1";

            return text;
        }
        public string ComputeDamage()
        {
            return ComputeDamage(base.Level);
        }
        public void Electra(Cell C, ScreenBuffer Buffer, bool doEffect = true)
        {
            string dice = ComputeDamage();
            if (C != null)
            {
                foreach (GameObject item in C.GetObjectsInCell())
                {
                    if (!item.PhaseMatches(ParentObject))
                    {
                        continue;
                    }
                    item.TemperatureChange(100 + 25 * base.Level, ParentObject);
                    if (doEffect)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            item.ParticleText("&W" + (char)(219 + Stat.Random(0, 4)), 2.9f, 1);
                        }
                        for (int j = 0; j < 5; j++)
                        {
                            item.ParticleText("&B" + (char)(219 + Stat.Random(0, 4)), 2.9f, 1);
                        }
                        for (int k = 0; k < 5; k++)
                        {
                            item.ParticleText("&Y" + (char)(219 + Stat.Random(0, 4)), 2.9f, 1);
                        }
                    }
                }
                int phase = ParentObject.GetPhase();
                DieRoll cachedDieRoll = dice.GetCachedDieRoll();
                foreach (GameObject item2 in C.GetObjectsWithPartReadonly("Combat"))
                {
                    item2.TakeDamage(cachedDieRoll.Resolve(), "from %o shimmering aura!", "Shock", null, null, ParentObject, null, null, Accidental: false, Environmental: false, Indirect: false, ShowUninvolved: false, ShowForInanimate: false, SilentIfNoDamage: false, phase);
                    if (!item2.MakeSave("Toughness", 8 + Level, ParentObject, null, null))
                    {
                        item2.ApplyEffect(new Stun(Level, SaveTarget: 8 + Level, bDontStunIfPlayer: true));
                    }

                }
            }
            Potency -= 2;
            if (doEffect)
            {
                Buffer.Goto(C.X, C.Y);
                string str = "&C";
                int num = Stat.Random(1, 3);
                if (num == 1)
                {
                    str = "&B";
                }
                if (num == 2)
                {
                    str = "&Y";
                }
                if (num == 3)
                {
                    str = "&W";
                }
                int num2 = Stat.Random(1, 3);
                if (num2 == 1)
                {
                    str += "^B";
                }
                if (num2 == 2)
                {
                    str += "^Y";
                }
                if (num2 == 3)
                {
                    str += "^W";
                }
                if (C.ParentZone == XRLCore.Core.Game.ZoneManager.ActiveZone)
                {
                    Stat.Random(1, 3);
                    Buffer.Write(str + (char)(219 + Stat.Random(0, 4)));
                    Popup._TextConsole.DrawBuffer(Buffer);
                    Thread.Sleep(10);
                }
            }
        }
        public static bool Cast(Electrokinesis mutation = null, string level = "5-6")
        {
            ScreenBuffer scrapBuffer = ScreenBuffer.GetScrapBuffer1(bLoadFromCurrent: true);
            XRLCore.Core.RenderMapToBuffer(scrapBuffer);
            List<Cell> list = mutation.PickLine(9, AllowVis.Any, null, IgnoreSolid: false, IgnoreLOS: true, RequireCombat: true, null, null, Snap: true);
            if (list == null || list.Count <= 0)
            {
                return false;
            }
            if (list.Count == 1 && mutation.ParentObject.IsPlayer() && Popup.ShowYesNoCancel("Are you sure you want to target " + mutation.ParentObject.itself + "?") != 0)
            {
                return false;
            }
            mutation.CooldownMyActivatedAbility(mutation.ShimmeringRayAbility, 10);
            mutation.UseEnergy(1000, "Physical Mutation Electrokinesis Hands");
            mutation.PlayWorldSound(mutation.Sound, 0.5f, 0f, combat: true);
            int i = 0;
            for (int num = Math.Min(list.Count, 10); i < num; i++)
            {
                if (list.Count == 1 || list[i] != mutation.ParentObject.CurrentCell)
                {
                    mutation.Electra(list[i], scrapBuffer);
                }
                if (i < num - 1 && list[i].IsSolidFor(null, mutation.ParentObject))
                {
                    break;
                }
            }
            string Aura = "aura";
            IComponent<GameObject>.XDidY(mutation.ParentObject, "emit", "a shimmer ray" + ((Aura != null) ? (" from " + mutation.ParentObject.its + " " + Aura) : ""), "!", null, mutation.ParentObject);

            return true;
        }

        public Electrokinesis()
        {
            this.DisplayName = "Electrokinesis";
            this.Type = "Mental";
            this.Listener = ParentObject;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CommandShimmeringShroud");
            Object.RegisterPartEvent(this, "CommandChargePotencyShroud");

            Object.RegisterPartEvent(this, "ChargeBatteryEvent");
            Object.RegisterPartEvent(this, "TogglingChargeVisionEvent");
            Object.RegisterPartEvent(this, "ShimmeringRayEvent");
            Object.RegisterPartEvent(this, "ThunderingStrikeEvent");
            Object.RegisterPartEvent(this, "SelfDestructPersonalEvent");

            Object.RegisterPartEvent(this, "BeginTakeAction");

            base.Register(Object);
        }
        public override int GetMaxLevel()
        {
            return 9999;
        }
        public override string GetDescription()
        {
            return "You've become a deadly walking battery, take charge of the forces of electricity.";
        }

        public override string GetLevelText(int Level)
        {
            try
            {
                var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;

                return "Create an electric shroud around yourself and manipulate powerful electric forces dependent on a careful balancing of both electric charge and potency.\n\n"
              + "Potency towards Resistance Alteration: " + "{{M|" + ParentsEgo + "}}" + " * " + "{{B|Charges}}\n"
              + "Potency towards Shimmering Shroud: " + "{{W|" + 0.1f + "}}" + " * " + "{{B|Potency}}\n\n"
              + "Save Target Vs' Negative Resistance Changes: " + "{{B|" + (10 + ParentsEgo + Level) + "}}";
            }
            catch
            {
                return "Create an electric shroud around yourself and manipulate powerful electric forces dependent on a careful balancing of both electric charge and potency.\n\n";
            }
        }

        public void ApplyElectricShroud()
        {
            try
            { // AddPlayerMessage("MethodFiring: ChangeTargetResistances");

                FocusPsi PsiMutation = ParentObject.GetPart<FocusPsi>();

                var ParentsEgo = ParentObject.Statistics["Ego"].Modifier;
                var ParentsLevel = ParentObject.Statistics["Level"].BaseValue;
                var ParentsCharges = ParentObject.Statistics["PsiCharges"].BaseValue;

                string ChargesSpent = PsiMutation.focusPsiCurrentCharges.ToString();

                // AddPlayerMessage("MethodStep: Getting Target");

                Cell TargetCell = PickDestinationCell(12 + Level, AllowVis.OnlyVisible, false, true, false, true);
                GameObject Target = TargetCell.GetFirstObjectWithPart("Combat");

                // AddPlayerMessage("MethodStep: Applying Effect");

                if (!Target.HasEffect("ShimmeringShroud"))
                {
                    Owner = ParentObject;
                    Target.ApplyEffect(new ShimmeringShroud(0, Owner: ParentObject));
                    ElectricChargePulse(TargetCell, ParentsCharges);
                    Target.ParticleText("&W\u000f", 0.02f, 10);
                    PlayWorldSound("Electroblast" + (Stat.Random(1, 4)));
                    Event cE = Event.New("SetShimmeringShroudEffectEvent", "MaximumCharges", ParentsCharges, "Potency", Potency, "Caster", ParentObject, "MutationLevel", Level);

                    // AddPlayerMessage("MethodStep: FireEvent CE");

                    Target.FireEvent(cE);
                }
                else
                {
                    Owner = ParentObject;
                    ElectricChargePulse(TargetCell, ParentsCharges);
                    Target.ParticleText("&W\u000f", 0.02f, 10);
                    PlayWorldSound("Electroblast" + (Stat.Random(1, 4)));
                    Event cE = Event.New("AlteringTemperatureEffectEvent", "ChargesSpent", ParentsCharges, "Potency", Potency, "Caster", ParentObject, "MutationLevel", Level);

                    // AddPlayerMessage("MethodStep: FireEvent CE/ No effect");

                    Target.FireEvent(cE);
                }

                // AddPlayerMessage("MethodStep: Setting Flavour Text");


                XDidYToZ(Target, "dawn", null, Target, "shimmering shroud", "!", null, Target, PossessiveObject: true);
                CooldownMyActivatedAbility(ActivateShimmeringShroudAbilityID, ParentsCharges * 3);
            }
            catch
            {

            }
        }

        public void RemoveElectricShroud()
        {
            try
            {
                Cell TargetCell = PickDestinationCell(12 + Level, AllowVis.OnlyVisible, false, true, false, true);
                GameObject Target = TargetCell.GetFirstObjectWithPart("Combat");

                Target.RemoveEffect("ShimmeringShroud");
            }
            catch
            {

            }

        }

        public void ElectricChargePulse(Cell TargetCell, int Charges)
        {
            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 3 + Charges; j++)
                {
                    TargetCell.ParticleText("&W" + (char)Stat.RandomCosmetic(191, 198), 2.9f, 5 + Charges);
                }
                for (int k = 0; k < 2 + Charges; k++)
                {
                    TargetCell.ParticleText("&Y" + (char)Stat.RandomCosmetic(191, 198), 3.9f, 5 + Charges);
                }
                for (int l = 0; l < 4 + Charges; l++)
                {
                    TargetCell.ParticleText("&B" + (char)Stat.RandomCosmetic(191, 198), 4.9f, 5 + Charges);
                }
            }
        }

        public void PotencyChargeEffectPulse(int Potency)
        {
            // AddPlayerMessage("Its Working");
            int ParticlesCount = Potency;

            if (Potency >= 21)
            {
                ParticlesCount = 20;
            }


            for (int i = 0; i < 1; i++)
            {
                for (int j = 0; j < 1 + (ParticlesCount); j++)
                {
                    ReverseParticleText("&W" + (char)Stat.RandomCosmetic(191, 198), 2.9f, (1 + ParticlesCount));
                }
                for (int k = 0; k < 1 + (ParticlesCount); k++)
                {
                    ReverseParticleText("&Y" + (char)Stat.RandomCosmetic(191, 198), 3.9f, (1 + ParticlesCount));
                }
                for (int l = 0; l < 1 + (ParticlesCount); l++)
                {
                    ReverseParticleText("&B" + (char)Stat.RandomCosmetic(191, 198), 4.9f, (1 + ParticlesCount));
                }
            }
        }



        public void CallLightningAnimation()
        {
            TextConsole _TextConsole = UI.Look._TextConsole;
            ScreenBuffer Buffer = TextConsole.ScrapBuffer;
            Core.XRLCore.Core.RenderMapToBuffer(Buffer);

            var TargetCell = PickDestinationCell(9, AllowVis.OnlyVisible);
            var zone = TargetCell.ParentZone;

            var SkyCell = zone.GetCell(TargetCell.X, 0);

            List<Point> Lightningline = Zone.Line(SkyCell.X, SkyCell.Y, TargetCell.X, TargetCell.Y);

            List<string> SparkySparkyChars = new List<string>() { "\xf8", "*", "." };

            for (int index = 0; index < Lightningline.Count; index++)
            {
                Point point = Lightningline[index];
                Cell cell = zone.GetCell(point);

                char DisplayBeam = Lightningline[index].DisplayChar;
                Buffer.Goto(cell.X, cell.Y);
                Buffer.Write("&Y^b" + DisplayBeam);

                Cell SparkyBeam = cell.GetRandomLocalAdjacentCell();
                Buffer.Goto(SparkyBeam.X, SparkyBeam.Y);
                Buffer.Write("&Y" + SparkySparkyChars.GetRandomElement());
                _TextConsole.DrawBuffer(Buffer);
                System.Threading.Thread.Sleep(18);

            }



            // foreach (var C in line)
            // {
            //     GameObject Target = C.GetFirstObjectWithPart("Combat");

            //     AddPlayerMessage("Target: " + Target.DisplayName);

            //     Cell TargetCell = Target.CurrentCell;

            //     AddPlayerMessage("Target Cell Found?: " + TargetCell.DebugName);

            //     Zone TargetsZone = Target.CurrentZone;

            //     AddPlayerMessage("Target Zone Found?: " + TargetsZone.DebugName);



            //     AddPlayerMessage("Sky Cell Found?: " + SkyCell.DebugName);


            //     List<string> ElectricChars = new List<string>() { "\xf8", "*", "." };
            //     // List<Point> Lightningline = Zone.Line(line[0].X, line[0].Y, TargetCell.X, SkyCell.Y);

            //     int num = 1;

            //     int dx = Math.Abs(num - num), sx = num < -num ? 1 : -1;
            //     int dy = Math.Abs(num - num), sy = num < -num ? 1 : -1;

            //     XRLCore.ParticleManager.Add(ElectricChars.GetRandomElement(), TargetCell.X, SkyCell.Y, -dx, -dy, 1, 0f, 0f);

            //     AddPlayerMessage("Lightining Code Fired.");

            //     // for (int index = 1; index < line.Count; index++)
            //     // {
            //     //     Cell cell = line[index];
            //     //     char DisplayLightning = Lightningline[index].DisplayChar;
            //     //     Buffer.Goto(cell.X, cell.Y);
            //     //     Buffer.Write("&B^b" + DisplayLightning);

            //     //     Cell LightningStrike = cell.GetRandomLocalAdjacentCell();
            //     //     Buffer.Goto(LightningStrike.X, LightningStrike.Y);
            //     //     Buffer.Write("&W" + ElectricChars.GetRandomElement());
            //     //     _TextConsole.DrawBuffer(Buffer);
            //     //     System.Threading.Thread.Sleep(18);
            //     //     // Find a solid object and combat id on obj in line, to hit in this cell.
            //     //     GameObject obj = cell.FindObject(o => (o.HasPart("Combat")));


            //     //     if (obj != null)
            //     //     {
            //     //         TargetCell = cell;
            //     //         break;
            //     //     }
            //     // }
            //     break;
            // }
        }
        public void ReverseParticleText(string Text, float Velocity, int Life)
        {
            Zone ParentZone = ParentObject.CurrentZone;
            if (ParentZone == XRLCore.Core.Game.ZoneManager.ActiveZone)
            {
                int X = ParentObject.CurrentCell.X;
                int Y = ParentObject.CurrentCell.Y;
                // The 360 refers to a Circular Pattern associated with map.
                float num = (float)Stat.Random(0, 359) / 58f;
                float YVelocity = (float)Math.Sin(num) / 4f;
                float XVelocity = (float)Math.Cos(num) / 4f;

                YVelocity *= Velocity;
                XVelocity *= Velocity;

                int DeltaX = (int)(XVelocity * Life);
                int DeltaY = (int)(YVelocity * Life);

                X = X + DeltaX;
                Y = Y + DeltaY;

                if (X >= 80 || X < 0 || Y >= 25 || Y < 0)
                {
                    if (X >= 80 && ParentObject.CurrentCell.X != 79)
                    { X = 79; }

                    else if (X < 0 && ParentObject.CurrentCell.X != 0)
                    { X = 0; }

                    if (Y >= 25 && ParentObject.CurrentCell.Y != 24)
                    { Y = 24; }

                    else if (Y < 0 && ParentObject.CurrentCell.Y != 0)
                    { Y = 0; }

                    YVelocity = ParentObject.CurrentCell.Y - Y;
                    XVelocity = ParentObject.CurrentCell.X - X;

                    float Magnitude = (float)Math.Sqrt((XVelocity * XVelocity) + (YVelocity * YVelocity));


                    XVelocity = -(XVelocity / Life);
                    YVelocity = -(YVelocity / Life);

                }

                XRLCore.ParticleManager.Add(Text, X, Y, -XVelocity, -YVelocity, Life, 0f, 0f);

            }
        }

        public static string[] ColorList = new string[3]
        {
        "&W",
        "&Y",
        "&B",
        };

        public string ElectroColors()
        {
            return ColorList.GetRandomElement();
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandShimmeringShroud")
            {
                if (!ParentObject.HasEffect("ShimmeringShroud"))
                {
                    ApplyElectricShroud();
                    if (ToggleMyActivatedAbility(ChargePotentialAbilityID) == true && Potency > 30)
                    {
                        ParentObject.ParticlePulse(ElectroColors() + "\u0000");
                    };
                }
                else
                    RemoveElectricShroud();
            }
            else if (E.ID == "CommandChargePotencyShroud")
            {
                ToggleMyActivatedAbility(ChargePotentialAbilityID);
            }
            else if (E.ID == "TogglingChargeVisionEvent")
            {
                ToggleMyActivatedAbility(ToggleChargeSenseAbility);
            }
            else if (E.ID == "ThunderingStrikeEvent")
            {
                CallLightningAnimation();
            }
            else if (E.ID == "ChargeBatteryEvent")
            {
                StartElectroRechargeList();
            }
            else if (E.ID == "BeginTakeAction")
            {
                if (IsMyActivatedAbilityToggledOn(ChargePotentialAbilityID, ParentObject))
                {
                    var ParentsCharges = ParentObject.Statistics["PsiCharges"].BaseValue;

                    if (Potency < ParentsCharges * 2)
                    {
                        ++Potency;
                        PotencyChargeEffectPulse(Potency);
                    }
                }
                if (IsMyActivatedAbilityToggledOn(ToggleChargeSenseAbility, ParentObject))
                {
                    ChargeVisionAbility();
                }
                if (!ParentObject.MakeSave("Ego", 10 + (Potency / 2), null, null))
                {
                    if (Potency > 0)
                        --Potency;
                }

                AddPlayerMessage("ShowingAttribute - Potency Current Amount: " + Potency);
            }
            else if (E.ID == "ShimmeringRayEvent")
            {
                if (!Cast(this))
                {
                    return false;
                }
            }

            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            if (ToggleChargeSenseAbility != null && NewLevel == 4)
            {
                this.ToggleChargeSenseAbility = base.AddMyActivatedAbility("Toggle Charge Sense", "TogglingChargeVisionEvent", "Electrokinesis", "Sense electromagnetic fields around you.", "p", null, true, false);
            }
            else if (ShimmeringRayAbility != null && NewLevel == 7)
            {
                this.ShimmeringRayAbility = base.AddMyActivatedAbility("Shimmering Ray", "ShimmeringRayEvent", "Electrokinesis", "Direct of beam of electricity at your enemies.", "p", null);
            }
            else if (ThunderStrikeAbility != null && NewLevel == 15)
            {
                this.ThunderStrikeAbility = base.AddMyActivatedAbility("Thunder Strike", "ThunderingStrikeEvent", "Electrokinesis", "Call forth an thundering strike upon your enemies.", "p", null);
            }
            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            Mutations GainPSiFocus = GO.GetPart<Mutations>();
            if (!GainPSiFocus.HasMutation("FocusPsi"))
            {
                GainPSiFocus.AddMutation("FocusPsi", 1);
            }
            if (!ParentObject.HasPart("ShimmeringShroudHandler"))
            {
                ParentObject.AddPart<ShimmeringShroudHandler>();
            }
            ActivateShimmeringShroudAbilityID = base.AddMyActivatedAbility("Activate Shroud", "CommandShimmeringShroud", "Mental Mutation", null, "*", null, false, false, false, false, false);
            ChargePotentialAbilityID = base.AddMyActivatedAbility("Charge Potential", "CommandChargePotencyShroud", "Mental Mutation", null, "*", null, true, false, true, false, false);
            PotencyForChargesAbility = base.AddMyActivatedAbility("Recharge Battery", "ChargeBatteryEvent", "Electrokinesis", null, "~", null, false, false, true, false, false);
            ElectrokinesisGauge = base.AddMyActivatedAbility("Electrokinesis", "ElectroToggleEvent", "Mutation");

            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            RemoveMyActivatedAbility(ref PotencyForChargesAbility, ParentObject);
            RemoveMyActivatedAbility(ref ToggleChargeSenseAbility, ParentObject);
            RemoveMyActivatedAbility(ref ShimmeringRayAbility, ParentObject);
            RemoveMyActivatedAbility(ref ThunderStrikeAbility, ParentObject);
            RemoveMyActivatedAbility(ref ActivateShimmeringShroudAbilityID);
            RemoveMyActivatedAbility(ref ChargePotentialAbilityID);

            return base.Unmutate(GO);
        }

    }
}
