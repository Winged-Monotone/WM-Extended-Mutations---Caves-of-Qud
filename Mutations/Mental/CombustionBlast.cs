using System;
using System.Collections.Generic;
using System.Threading;
using ConsoleLib.Console;
using UnityEngine;
using XRL.Core;
using XRL.Language;
using XRL.Rules;
using XRL.UI;
using XRL.World.Effects;


namespace XRL.World.Parts.Mutation
{

    [Serializable]
    public class CombustionBlast : BaseMutation
    {
        public Guid CombustionBlastActivatedAbilityID;
        public Guid CombustionBlastVolleyActivatedAbilityID;
        public GameObject CombustionBlastObject;
        public int CombustBlastAbilityCost;
        public int ChargesSpent;
        public string BodyPartType = "Head";
        public GameObject PsiFocusGlyphObj;

        public CombustionBlast()
        {
            this.DisplayName = "Combustion Blast";
            this.Type = "Mental";
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CommandCombustionBlast");
            Object.RegisterPartEvent(this, "CommandQuickFire");
            Object.RegisterPartEvent(this, "AIGetOffensiveMutationList");
            Object.RegisterPartEvent(this, "EndTurn");
            base.Register(Object);
        }

        public override bool CanLevel()
        {
            return true;
        }

        public override int GetMaxLevel()
        {
            return 9999;
        }

        public override string GetDescription()
        {
            return "Focus psionic energy directly into your mind's eye and release a deadly, focused beam of combustible and volatile energy at your foes.";
        }

        public override string GetLevelText(int Level)
        {
            if (Level == base.Level)
                return "Base Damage: {{cyan|" + GetDamage(Level, 1) + "}} Per Charge.\n"
                + "Charges above the first used increase the cooldown of this mutation by {{cyan|25}}.\n"
                + "\n"
                + "Due to the amount of concentration it requires to perform this ability, attempting to project this beam while &Wdazed &Yor &Wconfused &Yhas &Rfatal &Rconsequences&y.\n\n"
                + "The beam projects itself from the forehead, attempting to project this beam while wearing a helmet also has &Rfatal &Rconsequences&y.";
            else
            {
                return "Base Damage: {{cyan|" + GetDamage(Level, 1) + "}} Per Charge.\n";
            }
        }
        public int GetForce(int Level, int Charges)
        {
            return 1000 + (250 * Level) + (250 * Charges);
        }
        public string GetDamage(int Level, int Charges)
        {
            string ChargeDamage = "+" + Charges + "d4";
            return (Level * (2) + ChargeDamage);
        }

        public void CombustionBeamQuickFire()
        {
            ActuallyFire(1);
        }
        public void CombustionBeam()
        {

            FocusPsi PsiMutation = ParentObject.GetPart<FocusPsi>();
            string ChargesSpent = PsiMutation.focusPsiCurrentCharges.ToString();

            var ParentsEgoMod = ParentObject.Statistics["Ego"].Modifier;
            var ParentsLevelMod = ParentObject.Statistics["Level"].BaseValue;

            if (!ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap())
            {
                if (PsiMutation == null)
                {
                    // AddPlayerMessage("You lack the ability to do this.");
                    string verb1 = "lack";
                    string extra1 = "ability to do this";
                    string termiPun1 = "!";
                    XDidY(ParentObject, verb1, extra1, termiPun1);
                    return;
                }
                if (IsPlayer())
                {
                    ChargesSpent = Popup.AskString("Expend how many charges", "1", 3, 1, "0123456789");
                }
                int Charges = Convert.ToInt32(ChargesSpent);
                if (IsPlayer() && Charges <= 0)
                {
                    AddPlayerMessage("That's not a valid amount of charges.");
                    return;
                }
                if (Charges > 1 + ParentsEgoMod + ParentsLevelMod / 3 && !ParentObject.HasEffect("Psiburdening"))
                {
                    int fatigueVar = 25;
                    ParentObject.ApplyEffect(new Psiburdening(fatigueVar * Charges));
                }
                ActuallyFire(1);
            }
        }

        public void ActuallyFire(int Charges)
        {
            FocusPsi PsiMutation = ParentObject.GetPart<FocusPsi>();

            // Shows the line picker interface for the player.

            TextConsole _TextConsole = UI.Look._TextConsole;
            ScreenBuffer Buffer = TextConsole.ScrapBuffer;
            Core.XRLCore.Core.RenderMapToBuffer(Buffer);

            List<GameObject> hit = new List<GameObject>(1);
            List<Cell> usedCells = new List<Cell>(1);
            var line = PickLine(20, AllowVis.Any, null, false, ForMissileFrom: ParentObject);

            Body body = ParentObject.GetPart<Body>();
            List<BodyPart> ParentsHead = body.GetPart("Head");

            Cell targetCell = line[line.Count - 1];

            if (!PsiMutation.usePsiCharges(Charges))
            {
                AddPlayerMessage("You do not have enough psi-charges!");
                return;
            }
            foreach (var Head in ParentsHead)
            {
                if (Head.Equipped != null)
                {
                    Physics.ApplyExplosion(C: currentCell,
                     Force: GetForce(Level, Charges),
                      UsedCells: usedCells,
                       Hit: hit,
                        Local: true,
                         Show: true,
                          Owner: ParentObject,
                           BonusDamage: GetDamage(Level, Charges),
                            Phase: 1,
                             Neutron: false,
                              Indirect: false,
                               DamageModifier: 2);
                    AddPlayerMessage("Your helmet obstructs the energy of the beam--it explodes in your face!");
                    return;
                }
            }
            if (ParentObject.HasEffect("Dazed") || ParentObject.HasEffect("Confused"))
            {
                Physics.ApplyExplosion(C: currentCell,
                 Force: GetForce(Level, Charges),
                  UsedCells: usedCells,
                   Hit: hit,
                    Local: true,
                     Show: true,
                      Owner: ParentObject,
                       BonusDamage: GetDamage(Level, Charges),
                        Phase: 1,
                         Neutron: false,
                          Indirect: false,
                           DamageModifier: 2);
                AddPlayerMessage("You lack the concentration to hold your focus! The collected energy explodes in your face!");
                return;
            }

            ActivatedAbilities activatedAbilities = ParentObject.GetPart("ActivatedAbilities") as ActivatedAbilities;
            activatedAbilities.GetAbility(CombustionBlastActivatedAbilityID).Cooldown = (Charges - 1) * 100;

            // Loop through each cell of the line in order.

            List<string> SparkySparkyChars = new List<string>() { "\xf8", "*", "." };
            List<Point> Beamline = Zone.Line(line[0].X, line[0].Y, targetCell.X, targetCell.Y);

            base.PlayWorldSound("sparkblast", 0.3f, 0.1f, true, null);

            for (int index = 1; index < line.Count; index++)
            {
                Cell cell = line[index];
                char DisplayBeam = Beamline[index].DisplayChar;
                Buffer.Goto(cell.X, cell.Y);
                Buffer.Write("&Y^r" + DisplayBeam);

                Cell SparkyBeam = cell.GetRandomLocalAdjacentCell();
                Buffer.Goto(SparkyBeam.X, SparkyBeam.Y);
                Buffer.Write("&W" + SparkySparkyChars.GetRandomElement());
                _TextConsole.DrawBuffer(Buffer);
                System.Threading.Thread.Sleep(18);
                // Find a solid object and combat id on obj in line, to hit in this cell.
                GameObject obj = cell.FindObject(o => o.ConsiderSolidFor(ParentObject, ParentObject) || (o.HasPart("Combat") && !o.pPhysics.Solid));


                if (obj != null)
                {
                    targetCell = cell;
                    break;
                }
            }
            if (Stat.Random(1, 100) <= 50)
                base.PlayWorldSound("bang1", 0.3f, 0, true, null);
            else
                base.PlayWorldSound("bang2", 0.3f, 0, true, null);
            ComExplode(GetForce(Level, Charges), targetCell, ParentObject, GetDamage(Level, Charges));
            ParentObject.UseEnergy(10000);

        }


        public bool ComExplode(int Force, Cell TargetCell, GameObject Owner = null, string BonusDamage = null, bool Neutron = false)
        {
            List<Cell> usedCells = new List<Cell>(16);
            List<GameObject> hit = new List<GameObject>(16) { };

            this.ApplyCombustExplosion(TargetCell, usedCells, hit, Force, false, true, Owner, BonusDamage, Phase: false, Neutron: false, DamageModifier: 1f);
            return true;
        }
        public void ApplyCombustExplosion(Cell C, List<Cell> UsedCells, List<GameObject> Hit, int Force, bool bLocal, bool bShow, GameObject Owner, string BonusDamage, bool? Phase, bool Neutron = false, float DamageModifier = 1f)
        {
            TextConsole textConsole = Look._TextConsole;
            ScreenBuffer scrapBuffer = TextConsole.ScrapBuffer;
            if (bShow)
            {
                TextConsole.LoadScrapBuffers();
                XRLCore.Core.RenderMapToBuffer(scrapBuffer);
            }
            CleanQueue<Cell> cleanQueue = new CleanQueue<Cell>();
            CleanQueue<int> cleanQueue2 = new CleanQueue<int>();
            CleanQueue<string> cleanQueue3 = new CleanQueue<string>();
            cleanQueue.Enqueue(C);
            cleanQueue2.Enqueue(Force);
            cleanQueue3.Enqueue(".");
            UsedCells.Add(C);
            while (cleanQueue.Count > 0)
            {
                Cell cell = cleanQueue.Dequeue();
                int num = cleanQueue2.Dequeue();
                string text = cleanQueue3.Dequeue();
                for (int i = 0; i < UsedCells.Count; i++)
                {
                    Cell cell2 = UsedCells[i];
                    if (cell2 == null)
                    {
                        return;
                    }
                    if (cell2.ParentZone == XRLCore.Core.Game.ZoneManager.ActiveZone)
                    {
                        scrapBuffer.Goto(cell2.X, cell2.Y);
                        int num2 = Stat.Random(1, 3);
                        if (num2 == 1)
                        {
                            scrapBuffer.Write((Phase != null) ? ((!(Phase == true)) ? "&Y*" : "&K*") : "&M*");
                        }
                        else if (num2 == 2)
                        {
                            scrapBuffer.Write((Phase != null) ? ((!(Phase == true)) ? "&R*" : "&b*") : "&G*");
                        }
                        else
                        {
                            scrapBuffer.Write((Phase != null) ? ((!(Phase == true)) ? "&W*" : "&c*") : "&m*");
                        }
                    }
                }
                if (bShow && C.ParentZone != null && C.ParentZone.IsActive())
                {
                    textConsole.DrawBuffer(scrapBuffer, null, false);
                    if (Force < 100000)
                    {
                        Thread.Sleep(5);
                    }
                }
                List<Cell> list;
                if (bLocal)
                {
                    list = cell.GetLocalAdjacentCells(1, false);
                }
                else
                {
                    list = cell.GetAdjacentCells(true);
                }
                for (int j = 0; j < UsedCells.Count; j++)
                {
                    Cell item = UsedCells[j];
                    if (list.CleanContains(item))
                    {
                        list.Remove(item);
                    }
                }
                int num3 = 0;
                Damage damage = null;
                Event @event = null;
                foreach (GameObject gameObject in cell.GetObjectsWithPart("Physics"))
                {
                    if (!Hit.Contains(gameObject))
                    {
                        Hit.Add(gameObject);
                        if (gameObject.PhaseMatches(0))
                        {
                            num3 += gameObject.Weight;
                            if (damage == null || !string.IsNullOrEmpty(BonusDamage))
                            {
                                damage = new Damage((int)(DamageModifier * (float)num / 250f));
                                if (!string.IsNullOrEmpty(BonusDamage))
                                {
                                    damage.Amount += BonusDamage.RollCached();
                                }
                                damage.AddAttribute("Explosion");
                                damage.AddAttribute("Heat");
                                damage.AddAttribute("Fire");
                                damage.AddAttribute("Concussion");
                                if (cell != C)
                                {
                                    damage.AddAttribute("Accidental");
                                }
                            }
                            if (@event == null || !string.IsNullOrEmpty(BonusDamage))
                            {
                                @event = Event.New("TakeDamage", 0, 0, 0);
                                @event.SetParameter("Damage", damage);
                                @event.SetParameter("Owner", Owner);
                                @event.SetParameter("Attacker", Owner);
                                if (Neutron)
                                {
                                    if (gameObject.IsPlayer())
                                    {
                                        gameObject.pPhysics.LastDeathReason = "Crushed under the weight of a thousand suns.";
                                    }
                                    @event.SetParameter("Message", "from being crushed under the weight of a thousand suns.");
                                }
                                else
                                {
                                    @event.SetParameter("Message", "from %t explosion!");
                                }
                            }
                            gameObject.FireEvent(@event);
                        }
                    }
                }
                System.Random random = new System.Random();
                for (int k = 0; k < list.Count; k++)
                {
                    int index = random.Next(0, list.Count);
                    Cell value = list[k];
                    list[k] = list[index];
                    list[index] = value;
                }
                Damage damage2 = null;
                Event event2 = null;
                for (; ; )
                {
                IL_44B:
                    int l = 0;
                    while (l < list.Count)
                    {
                        Cell cell3 = list[l];
                        if (!bLocal)
                        {
                            goto IL_4AD;
                        }
                        if (cell3.X != 0)
                        {
                            if (cell3.X != 79)
                            {
                                if (cell3.Y != 0)
                                {
                                    if (cell3.Y != 24)
                                    {
                                        goto IL_4AD;
                                    }
                                }
                            }
                        }
                    IL_68C:
                        l++;
                        continue;
                    IL_4AD:
                        foreach (GameObject gameObject2 in cell3.GetObjectsWithPart("Physics"))
                        {
                            if (!Hit.Contains(gameObject2))
                            {
                                Hit.Add(gameObject2);
                                if (gameObject2.PhaseMatches(0))
                                {
                                    if (damage2 == null || !string.IsNullOrEmpty(BonusDamage))
                                    {
                                        damage2 = new Damage(num / 250);
                                        if (!string.IsNullOrEmpty(BonusDamage))
                                        {
                                            damage2.Amount += BonusDamage.RollCached();
                                        }
                                        damage2.AddAttribute("Explosion");
                                        damage2.AddAttribute("Accidental");
                                        damage2.AddAttribute("Heat");
                                        damage2.AddAttribute("Fire");
                                        damage2.AddAttribute("Concussion");
                                    }
                                    if (event2 == null || !string.IsNullOrEmpty(BonusDamage))
                                    {
                                        event2 = Event.New("TakeDamage", 0, 0, 0);
                                        event2.SetParameter("Damage", damage2);
                                        event2.SetParameter("Owner", Owner);
                                        event2.SetParameter("Attacker", Owner);
                                        if (Neutron)
                                        {
                                            if (gameObject2.IsPlayer())
                                            {
                                                gameObject2.pPhysics.LastDeathReason = "Crushed under the weight of a thousand suns.";
                                            }
                                            event2.SetParameter("Message", "from being crushed under the weight of a thousand suns.");
                                        }
                                        else
                                        {
                                            event2.SetParameter("Message", "from %t explosion!");
                                        }
                                    }
                                    gameObject2.FireEvent(event2);
                                }
                            }
                            if (gameObject2.PhaseMatches(0))
                            {
                                int weight = gameObject2.Weight;
                                if (weight > num)
                                {
                                    list.Remove(cell3);
                                    goto IL_44B;
                                }
                                if (weight > 0)
                                {
                                    gameObject2.Move((!(text == ".")) ? text : Directions.GetRandomDirection(), EnergyCost: 1000);
                                }
                            }
                        }
                        if (cell3.IsSolid())
                        {
                            list.Remove(cell3);
                            goto IL_44B;
                        }
                        goto IL_68C;
                    }
                    break;
                }
                if (list.Count > 0)
                {
                    int num4 = (num - num3) / list.Count;
                    if (num4 > 100)
                    {
                        foreach (Cell cell4 in list)
                        {
                            if (cell4 != null && !UsedCells.Contains(cell4))
                            {
                                UsedCells.Add(cell4);
                                cleanQueue.Enqueue(cell4);
                                cleanQueue2.Enqueue(num4);
                                cleanQueue3.Enqueue(cell.GetDirectionFromCell(cell4));
                            }
                        }
                    }
                }
            }
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "CommandCombustionBlast")
            {
                if (base.IsMyActivatedAbilityUsable(this.CombustionBlastActivatedAbilityID))
                {
                    if (!this.ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap())
                    {
                        CombustionBeam();
                    }
                    return false;
                }
            }

            if (E.ID == "CommandQuickFire")
            {
                if (base.IsMyActivatedAbilityUsable(this.CombustionBlastVolleyActivatedAbilityID))
                {
                    if (!ParentObject.pPhysics.CurrentCell.ParentZone.IsWorldMap())
                    {
                        CombustionBeamQuickFire();
                    }
                    return false;
                }
            }


            if (E.ID == "AIGetOffensiveMutationList")
            {
                FocusPsi PsiMutation = ParentObject.GetPart<FocusPsi>();
                //AddPlayerMessage("I'mma keel yo ass.");
                if (PsiMutation.focusPsiCurrentCharges > 0)
                {
                    E.AddAICommand("CommandCombustionBlast");
                    //AddPlayerMessage("I'mma keel yo ass fo real.");
                }
            }
            return base.FireEvent(E);
        }

        public override bool ChangeLevel(int NewLevel)
        {
            return base.ChangeLevel(NewLevel);
        }
        public override bool Mutate(GameObject GO, int Level)
        {
            // string CombustionBlastinfoSource = "{ \"CombustionBlast\": [\"*cult*, the Fire-Eyed\", \"mind-blast *cult*\"] }";
            // SimpleJSON.JSONNode CombustionInfo = SimpleJSON.JSON.Parse(CombustionBlastinfoSource);

            // WMExtendedMutations.History.AddToHistorySpice("spice.extradimensional", CombustionInfo["CombustionBlast"]);

            this.Unmutate(GO);
            Mutations GainPSiFocus = GO.GetPart<Mutations>();
            if (!GainPSiFocus.HasMutation("FocusPsi"))
            {
                GainPSiFocus.AddMutation("FocusPsi", 1);
                //AddPlayerMessage("Has Focus Psi.");
            }
            this.CombustionBlastActivatedAbilityID = base.AddMyActivatedAbility(Name: "fire volley", Command: "CommandCombustionBlast", Class: "Mental Mutation", Icon: "*");
            this.CombustionBlastVolleyActivatedAbilityID = base.AddMyActivatedAbility(Name: "quick fire", Command: "CommandQuickFire", Class: "Mental Mutation", Icon: "*");
            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }
        public override bool Unmutate(GameObject GO)
        {
            base.RemoveMyActivatedAbility(ref this.CombustionBlastActivatedAbilityID);
            return base.Unmutate(GO);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == GetShortDescriptionEvent.ID;
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {

            string Glyph = (ParentObject.IsPlayer() ? "You" : ParentObject.The + ParentObject.ShortDisplayName) + ParentObject.GetVerb("bear", true, true) + "bear a psionic glyph on " + ParentObject.Poss("forehead");

            if (E.Postfix.Length > 0 && E.Postfix[E.Postfix.Length - 1] != '\n')
            {
                E.Postfix.Append('\n');
            }
            E.Postfix.Append('\n').Append(Glyph);
            return true;
        }

        public void WillPowerDebuffSystem()
        {


        }

    }
}