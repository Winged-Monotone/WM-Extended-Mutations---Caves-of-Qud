using System;
using XRL.World.Parts.Mutation;
using System.Collections.Generic;
using XRL.Rules;
using XRL.World.ZoneBuilders;
using XRL.World.Capabilities;

namespace XRL.World.Parts
{
    [Serializable]
    public class OvipositorEgg : IPart
    {
        public int IncubationPeriodDuration = 50;
        public GameObject OvipositorHandler;
        public GameObject Mother;
        public override void Register(GameObject ParentObject)
        {
            ParentObject.RegisterPartEvent((IPart)this, "EndTurn");
            base.Register(ParentObject);
        }
        public void HandleEggHatching()
        {
            {
                // string Hatched = "Crack!";
                OvipositorHandler = ParentObject;
                var MothersGenes = Mother.GetPart<Ovipositor>();

                GameObject Droneling = Cloning.GenerateClone(Mother, C: currentCell);
                var CheckOvi = Droneling.GetPart<Mutations>();
                var GetOvi = CheckOvi.GetMutation("Ovipositor");

                CheckOvi.RemoveMutation(GetOvi);

                for (int i = 0; i < 3; i++)
                {
                    var spice = MothersGenes.CollectedGeneSpice.GetRandomElement();
                    if (spice == null) break;

                    // check & add spice mutations here

                    // AddPlayerMessage("Preparing GeneSpicing");

                    if (!CheckOvi.HasMutation(spice))
                    {
                        // AddPlayerMessage("Gene 1");

                        CheckOvi.AddMutation(spice, 1);
                    }

                }


                Droneling.DisplayName = Names.MutantNameMaker.MakeMutantName();
                Droneling.GetPart<Description>().Short = "One of your loyal drones.";
                Droneling.RemoveIntProperty("ProperNoun");
                // ParentObject.ParticleText(Hatched, 1.0f, 1, false);
                ParentObject.ReplaceWith(Droneling);

                int PlaceHolder = 300;
                IncubationPeriodDuration = PlaceHolder;
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade) || ID == GetDisplayNameEvent.ID;
        }

        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            string desc = "[{{pink|hatches in " + IncubationPeriodDuration + "}}]";
            E.AddTag(desc);
            return true;
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "EndTurn")
            {

                if (IncubationPeriodDuration >= 0)
                {
                    --IncubationPeriodDuration;
                }

                if (IncubationPeriodDuration <= 0)
                {
                    OvipositorHandler = ParentObject;
                    HandleEggHatching();
                }

            }

            return base.FireEvent(E);
        }
    }
}