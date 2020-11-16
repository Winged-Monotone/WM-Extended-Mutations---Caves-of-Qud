using System;
using XRL.World.Parts.Mutation;
using System.Collections.Generic;
using XRL.Rules;

namespace XRL.World.Parts
{
    [Serializable]
    public class NotOriginalEntity : IPart
    {
        public bool IsOriginalEntity = false;


        public override void Register(GameObject ParentObject)
        {
            ParentObject.RegisterPartEvent(this, "EntityHasSwappedBodies");
            base.Register(ParentObject);
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "EntityHasSwappedBodies")
            {
                if (IsOriginalEntity == false)
                {
                    IsOriginalEntity = true;
                }
            }

            return base.FireEvent(E);
        }
    }
}