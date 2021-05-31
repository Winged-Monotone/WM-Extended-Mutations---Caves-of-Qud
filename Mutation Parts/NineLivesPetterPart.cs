using System;
using XRL.World.Parts.Mutation;
using System.Collections.Generic;
using XRL.Rules;

namespace XRL.World.Parts
{
    [Serializable]
    public class NineLivesPetter : IPart
    {

        public override bool AllowStaticRegistration()
        {
            return true;
        }
        public override void Register(GameObject ParentObject)
        {
            ParentObject.RegisterPartEvent(this, "ObjectPetted");
            base.Register(ParentObject);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "ObjectPetted")
            {
                GameObject Petter = E.GetGameObjectParameter("Petter");
                if (Petter == null)
                {
                    return true;
                }
                if (Petter.HasPart("NineLivesParadox"))
                {
                    Petter.GetPart<NineLivesParadox>().CatPetted(ParentObject);
                    return true;
                }
            }

            return base.FireEvent(E);

        }

    }
}