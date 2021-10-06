using System;
using System.Collections.Generic;
using XRL.Core;
using XRL.World.AI.GoalHandlers;
using XRL.UI;
using XRL.World.Effects;
using System.Linq;


namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class SerpentineForm : BaseMutation
    {
        //Properties/Member Variables / Not Static, Exist on the Instance of this Class
        public string BodyPartType = "Feet";
        public GameObject SerpentileTail;
        public Guid ActivatedAbilitiesID;
        public int DVModifier;
        public int ACModifier;
        public int ArmorDVModifier;
        public int ArmorACModifier;
        public List<string> SynergyMutations = new List<string>()
        {
            "GelatinousFormAcid",
            "Quills",
            "GelatinousFormPoison",
            "RoughScales",
        };

        public GameObject Constricted;
        [NonSerialized]
        public Dictionary<string, int> PropertyMap = new Dictionary<string, int>
        {
            {"DV", -4},
        };
        public int ConstrictorsStrength
        {
            get
            {
                return ParentObject.Statistics["Strength"].Modifier;
            }
        }
        public SerpentineForm()
        {
            this.DisplayName = "Serpentine Form";
            this.Type = "Physical";
            //Intialization Assignment
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "CommandConstrict");
            Object.RegisterPartEvent(this, "BeginTakeAction");
            Object.RegisterPartEvent(this, "ObjectEnteredAdjacentCell");
            Object.RegisterPartEvent(this, "EnteredCell");
            Object.RegisterPartEvent(this, "AIGetOffensiveMutationList");
            Object.RegisterPartEvent(this, "EndTurn");

            base.Register(Object);
        }
        public override bool Mutate(GameObject GO, int Level)
        {
            ACModifier = 0;
            DVModifier = 1;
            StatShifter.SetStatShift("MoveSpeed", 5);
            SerpentileTail = GameObject.create("Serpentine Tail");
            BodyPart firstPart = this.ParentObject.GetPart<Body>().GetFirstPart(this.BodyPartType);
            if (firstPart != null)
            {
                firstPart.Equipped?.UnequipAndRemove();
                firstPart.Equip(SerpentileTail);

                firstPart.DefaultBehaviorBlueprint = "Serpentine Tail";
                firstPart.DefaultBehavior = SerpentileTail;

            }
            else
            {
                return false;
            }
            this.ActivatedAbilitiesID = base.AddMyActivatedAbility(Name: "Constrict", Command: "CommandConstrict", Class: "Physical Mutation", Description: "Coil around and crush your enemies.", Icon: "@");
            this.ChangeLevel(Level);
            return base.Mutate(GO, Level);
        }
        public override bool ChangeLevel(int NewLevel)
        {
            ACModifier = 0 + (NewLevel / 5);
            DVModifier = 1 + (NewLevel / 2);
            ArmorACModifier = ACModifier;
            ArmorDVModifier = DVModifier;

            if (this.SerpentileTail != null)
            {
                Armor part = this.SerpentileTail.GetPart<Armor>();
                part.AV = this.ArmorACModifier;
                part.DV = this.ArmorDVModifier;
            }
            return base.ChangeLevel(NewLevel);
        }

        public override bool Unmutate(GameObject GO)
        {
            base.RemoveMyActivatedAbility(ref this.ActivatedAbilitiesID);
            return base.Unmutate(GO);
        }
        public override bool CanLevel()
        {
            return true;
        }
        public override string GetDescription()
        {
            return "A long, thick prehensile tail has replaced your legs. You now slither instead of walk and can constrict foes and prey alike in a deadly coil.\n"
                    + "\n{{cyan|+100 Reputation with}} {{cyan|unshelled reptiles.}}"
                    + "\n{{cyan|-200 Reputation with}} {{cyan|apes.}}";
        }
        public override string GetLevelText(int Level)
        {
            if (Level == base.Level)
                return "{{gray|Movement Speed:}} {{cyan|-5\n}}"
                + "{{gray|DV Bonus:}} {{cyan|" + Level / 2 + "\n}}"
                + "{{gray|Swimming Speed increased by {{cyan|50%}}}}\n"
                + "\n"
                + "{{cyan|Constrict:}} Constrict enemies, opposing foe must make Toughness Saving Throw, upon failing they are constricted.\n"
                + "Escape Save: {{cyan|" + (10 + Level) + "}} (STR Modifier Included.)\n";

            else
            {
                return "{{gray|DV Bonus:}} {{cyan|" + Level / 2 + "\n}}"
                + "Escape Save increased to: {{cyan|" + (10 + Level) + "}} (STR Modifier Included.)\n";
            }

        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == EnteredCellEvent.ID || ID == GetSwimmingPerformanceEvent.ID;
        }
        public override bool HandleEvent(EnteredCellEvent E)
        {
            if (Constricted != null)
            {
                PerformDamage(Constricted);
            }
            return true;
        }
        public override bool HandleEvent(GetSwimmingPerformanceEvent E)
        {
            E.MoveSpeedPenalty -= 50;
            return true;
        }
        public bool HasPermutation(GameObject ParentObject)
        {
            Mutations HasSynergyMutation = ParentObject.GetPart<Mutations>();
            if (HasSynergyMutation.HasMutation("Quills") == true || HasSynergyMutation.HasMutation("GelatinousFormPoison") == true)
            {
                return true;
            }
            else
                return false;
        }
        public int PermutationDamageQuills()
        {
            try
            {
                int QuillExtraDamage = ParentObject.GetPart<Quills>().Level + Rules.Stat.Roll("1d4");
                return QuillExtraDamage;
            }
            catch
            {
                return 0;
            }
        }
        public int PermutationDamagGFP()
        {
            try
            {
                int GFPoisonExtraDamage = ParentObject.GetPart<GelatinousFormPoison>().Level + Rules.Stat.Roll("1d6");
                return GFPoisonExtraDamage;
            }
            catch
            {
                return 0;
            }
        }
        public int PermutationDamagGFA()
        {
            try
            {
                int GFAcidExtraDamage = ParentObject.GetPart<GelatinousFormPoison>().Level + Rules.Stat.Roll("1d6");
                return GFAcidExtraDamage;
            }
            catch
            {
                return 0;
            }
        }
        public bool ResistanceSave(GameObject Target)
        {
            int EnterSaveTarget = 10 + Level;
            string EnterSaveStat = "Toughness";
            return Target.MakeSave(EnterSaveStat, EnterSaveTarget, ParentObject, "Strength", "Constriction");

        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "BeginTakeAction")
            {
                CheckConstricted();
                if (Constricted != null && !ParentObject.pBrain.HasGoal("FleeLocation"))
                {
                    ParentObject.pBrain.Goals.Clear();
                }

            }
            else if (E.ID == "EndTurn")
            {
                try
                {
                    if (Constricted.HasEffect("Constricted"))
                    {
                        PerformDamage(Constricted);
                    }
                }
                catch
                {

                }
            }
            else if (E.ID == "ObjectEnteredAdjacentCell")
            {
                GameObject gameObjectParameter = E.GetGameObjectParameter("Object");
                if (gameObjectParameter != null && ParentObject.pBrain.IsHostileTowards(gameObjectParameter))
                {
                    CheckConstricted();
                    if (Constricted == null && !gameObjectParameter.MakeSave("Strength", 20, null, null, "Constrictment"))
                    {
                        Constrict(gameObjectParameter, null);
                    }
                }
            }
            else if (E.ID == "EnteredCell")
            {
                if (Constricted != null)
                {
                    if (ParentObject.CurrentCell != null)
                    {
                        if (Constricted.CurrentCell != null && Constricted.CurrentCell != ParentObject.CurrentCell)
                        {
                            Constricted.CurrentCell.RemoveObject(Constricted);
                        }
                        if (!ParentObject.CurrentCell.Objects.Contains(Constricted))
                        {
                            ParentObject.CurrentCell.AddObject(Constricted);
                        }
                    }
                    Constricted.FireEvent(Event.New("ConstrictDragged", "Object", ParentObject));
                    ParentObject.FireEvent(Event.New("ConstricterDragged", "Object", Constricted));
                }
                CheckConstricted();
            }
            else if (E.ID == "BeginBeingTaken")
            {
                EndAllConstriction();
            }
            else if (E.ID == "AIGetOffensiveMutationList")
            {
                CheckConstricted();
                if (Constricted == null)
                {
                    int intParameter = E.GetIntParameter("Distance");
                    ActivatedAbilityEntry ActivatedAbility = MyActivatedAbility(ActivatedAbilitiesID);
                    if (ActivatedAbility != null && ActivatedAbility.Cooldown <= 0 && intParameter <= 1)
                    {
                        GameObject gameObjectParameter2 = E.GetGameObjectParameter("Target");
                        if (gameObjectParameter2.PhaseAndFlightMatches(ParentObject))
                        {
                            List<AICommandList> list = E.GetParameter("List") as List<AICommandList>;
                            list.Add(new AICommandList("CommandConstrict", 1));
                        }
                    }
                }
            }
            // Allows both player and AI to direction a cell to activate the mutation on
            else if (E.ID == "CommandConstrict")
            {
                // AddPlayerMessage("Phase: check frozen");
                if (!ParentObject.CheckFrozen())
                {

                    return true;
                }
                CheckConstricted();
                Cell cell = base.PickDirection(null);
                // AddPlayerMessage("Phase: CooldownSet");
                if (cell != null)
                    CooldownMyActivatedAbility(ActivatedAbilitiesID, 15);
                {
                    GameObject combatTarget = cell.GetCombatTarget(ParentObject, false, false, false, null, true, true, false, null);
                    // AddPlayerMessage("Phase: Combat target grabbed");
                    if (combatTarget != null && combatTarget != ParentObject)
                    {
                        if (Constricted != null)
                        {
                            // AddPlayerMessage("Phase: end constrict");
                            EndConstriction(Constricted, null, null);
                        }
                        // AddPlayerMessage("Phase: start constrict");
                        Constrict(combatTarget, null);
                    }
                }
            }
            return base.FireEvent(E);
        }
        public void ProcessTurnConstricted(GameObject Target, int TurnsConstricted)
        {
            Mutations HasSynergyMutation = ParentObject.GetPart<Mutations>();
            int Synergies = SynergyMutations.Count(HasSynergyMutation.HasMutation);

            if (ResistanceSave(Target))
            {
                EndConstriction(Target);
                XDidY(Target, "escape", "constriction", ".");
            }
            else if (HasPermutation(ParentObject) && HasSynergyMutation.HasMutation("Quills") == true)
            {

                AddPlayerMessage("Your barbs gouge into your foe, dealing extra damage!");
                Target.TakeDamage(PermutationDamageQuills(), Message: "from %t coils", Attributes: null, DeathReason: "Crushed to death by muscled coiling.", Owner: ParentObject, Attacker: ParentObject);
                PerformDamage(Target);
            }

            else if (HasPermutation(ParentObject) == true && HasSynergyMutation.HasMutation("GelatinousFormPoison") == true)
            {

                AddPlayerMessage("You strangle your enemy with miasmic poisons, dealing extra damage!");
                Target.TakeDamage(PermutationDamagGFP(), Message: "from %t coils", Attributes: "Poison", DeathReason: "Crushed to death by muscled coiling.", Owner: ParentObject, Attacker: ParentObject);
                PerformDamage(Target);
            }
            else if (HasPermutation(ParentObject) == true && HasSynergyMutation.HasMutation("GelatinousFormAcid") == true)
            {

                AddPlayerMessage("You strangle your enemy with your acidic form dealing extra damage!");
                Target.TakeDamage(PermutationDamagGFA(), Message: "from %t coils", Attributes: "Acid", DeathReason: "Crushed to death by muscled coiling.", Owner: ParentObject, Attacker: ParentObject);
                PerformDamage(Target);
            }
            else if (Synergies >= 1)
            {
                var MessageRandomizer = Rules.Stat.Random(1, 100);
                if (MessageRandomizer <= 33)
                {
                    AddPlayerMessage("Your barbs gouge into your foe, dealing extra damage!");
                }
                Target.TakeDamage(PermutationDamagGFA(), Message: "from %t coils", Attributes: null, DeathReason: "Crushed to death by muscled coiling.", Owner: ParentObject, Attacker: ParentObject);
                PerformDamage(Target);
            }
        }
        public void CheckConstricted()
        {
            if (Constricted != null && (!Constricted.IsValid() || !Constricted.HasEffect("Constricted")))
            {
                Constricted = null;
            }

        }
        public bool EndConstriction(GameObject Target, Event E = null, Constricted TargetEffect = null)
        {
            if (Target == null)
            {
                Constricted = null;
                return true;
            }
            if (TargetEffect == null)
            {
                TargetEffect = (Target.GetEffect("Constricted") as Constricted);
                if (TargetEffect == null)
                {
                    Constricted = null;
                    return true;
                }
            }
            if (TargetEffect.ConstrictedBy != ParentObject)
            {
                Constricted = null;
                return true;
            }
            Target.RemoveEffect(TargetEffect);
            Target.UseEnergy(1000, "Position");
            Target.FireEvent(Event.New("Exited", "Object", ParentObject));
            Cell TargetsAdjacentPosition = Target.CurrentCell.GetRandomLocalAdjacentCell();
            // Target.DirectMoveTo(TargetsAdjacentPosition, 1000, true);
            ParentObject.FireEvent(Event.New("ObjectExited", "Object", Target));
            if (E != null)
            {
                E.RequestInterfaceExit();
            }
            Constricted = null;
            return true;
        }
        public bool Constrict(GameObject Target, Event E = null)
        {
            // AddPlayerMessage("Phase: initialize constrict");
            CheckConstricted();
            if (Constricted != null)
            {
                EndConstriction(Constricted, null, null);
                if (Constricted != null)
                {
                    // AddPlayerMessage("Phase: end constrict");
                    return false;
                }
            }

            if (!Target.CanChangeBodyPosition("Constricted", false, true, false) && !Target.IsFrozen())
            {
                if (ParentObject.IsPlayer())
                {
                    Popup.Show(string.Concat(new string[]
                    {
                        "You cannot do that while ",
                        Target.the,
                        Target.ShortDisplayName,
                        "&y",
                        Target.Is,
                        " in ",
                        Target.its,
                        " present situation."
                    }), true);
                }
                return false;
            }

            if (!Target.PhaseAndFlightMatches(ParentObject))
            {
                return false;
            }
            // AddPlayerMessage("Phase: 3");
            if (Target.SameAs(ParentObject))
            {
                return false;
            }

            Cell currentCell = ParentObject.pPhysics.CurrentCell;

            if (currentCell == null)
            {
                return false;
            }
            // Save vs constriction
            if (ResistanceSave(Target))
            {
                // AddPlayerMessage("Phase: 6a");
                if (ParentObject.IsPlayer())
                {
                    // AddPlayerMessage("Phase: 6b");
                    Popup.Show("You fail to constrict " + Target.the + Target.ShortDisplayName + ".", true);
                }
                else if (Target.IsPlayer())
                {
                    // AddPlayerMessage("Phase: 7");
                    IPart.AddPlayerMessage(string.Concat(new string[]
                    {
                        ParentObject.The,
                        ParentObject.ShortDisplayName,
                        "&y",
                        ParentObject.GetVerb("try", true, false),
                        " to wrap around and constrict you, but",
                        ParentObject.GetVerb("fail", true, false),
                        "."
                    }));
                }
                else if (IPart.Visible(ParentObject))
                {
                    // AddPlayerMessage("Phase: 8");
                    IPart.AddPlayerMessage(string.Concat(new string[]
                    {
                        ParentObject.The,
                        ParentObject.ShortDisplayName,
                        "&y",
                        ParentObject.GetVerb("try", true, false),
                        " to wrap around and constrict ",
                        Target.the,
                        Target.ShortDisplayName,
                        "&y, but",
                        ParentObject.GetVerb("fail", true, false),
                        "."
                    }));
                }
                // CheckEnterDamage(Target, true);
                ParentObject.UseEnergy(1000, "Position");
                if (E != null)
                {
                    // AddPlayerMessage("Phase: 9");
                    E.RequestInterfaceExit();
                }
                return false;
            }
            if (Target.CurrentCell != currentCell && !ParentObject.DirectMoveTo(Target.CurrentCell, 0, true, true))
            {
                // AddPlayerMessage("Phase: 10");
                if (E != null)
                {
                    // AddPlayerMessage("Phase: 11");
                    E.RequestInterfaceExit();
                }
                return false;
            }

            string verb = "are";
            string preposition = "constricted by";
            GameObject parentObject = ParentObject;
            XDidYToZ(Target, verb, preposition, parentObject, null, null);
            Constricted = Target;
            Target.ApplyEffect(new Constricted(ParentObject), null);
            ParentObject.UseEnergy(1000, "Position");
            ParentObject.FireEvent(Event.New("Constricted", "Object", Target));
            Target.FireEvent(Event.New("ObjectConstricted", "Object", ParentObject));
            if (E != null)
            {
                E.RequestInterfaceExit();
            }
            return true;
        }
        public void EndAllConstriction()
        {
            Cell currentCell = ParentObject.CurrentCell;
            if (currentCell != null)
            {
                currentCell.ForeachObject((GameObject obj) =>
                {
                    Constricted constricted = obj.GetEffect("Constricted") as Constricted;
                    if (constricted != null && constricted.ConstrictedBy == ParentObject)
                    {
                        obj.RemoveEffect(constricted);
                    }
                });
            }
        }
        public bool PerformDamage(GameObject Target)
        {
            if (ParentObject.Statistics["Strength"].Modifier < 0)
            {
                string Damage = "1d4+" + ParentObject.Statistics["Strength"].Modifier + "+" + Level;
                if (string.IsNullOrEmpty(Damage))
                {
                    return false;
                }
                Damage damage = new Damage(Rules.Stat.Roll(Damage));
                // damage.AddAttributes(DamageAttributes);
                Event @event = Event.New("TakeDamage", 0, 0, 0);
                @event.AddParameter("Damage", damage);
                @event.AddParameter("Owner", ParentObject);
                @event.AddParameter("Attacker", ParentObject);
                @event.AddParameter("Message", "from %O!");
                bool flag = Target.FireEvent(@event);
                return flag;
            }
            else
            {
                string Damage = "1d4+" + 1 + "+" + Level;
                if (string.IsNullOrEmpty(Damage))
                {
                    return false;
                }
                Damage damage = new Damage(Rules.Stat.Roll(Damage));
                // damage.AddAttributes(DamageAttributes);
                Event @event = Event.New("TakeDamage", 0, 0, 0);
                @event.AddParameter("Damage", damage);
                @event.AddParameter("Owner", ParentObject);
                @event.AddParameter("Attacker", ParentObject);
                @event.AddParameter("Message", "from %O!");
                bool flag = Target.FireEvent(@event);
                return flag;
            }

        }
        public override bool Render(RenderEvent E)
        {
            if (Constricted != null && Constricted.IsValid())
            {
                int num = XRLCore.CurrentFrame % 60;
                if (num >= 31 && num <= 60)
                {
                    E.ColorString = Constricted.pRender.ColorString;
                    E.DetailColor = Constricted.pRender.DetailColor;
                    E.Tile = Constricted.pRender.Tile;
                    E.RenderString = Constricted.pRender.RenderString;
                }
            }
            return base.Render(E);
        }
    }
}